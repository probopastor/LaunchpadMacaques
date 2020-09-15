using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigJoint : MonoBehaviour
{
    [SerializeField] private GameObject hitObject;
    private GameObject hitObjectClone;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    private Vector3 currentGrapplePosition;
    public Transform gunTip, camera, player;
    private float maxPullDistance = 100f;
    private float maxPushDistance = 20f;
    private ConfigurableJoint joint;
    private RaycastHit grappleRayHit;


    private enum GrappleType { Pull, Push }
    //If the player is Pulling, this is the object they're pulling toward
    private Transform currentGrappleTarget = null;
    //Offset to account for the grapple target moving while pulling to it
    private Vector3 currentGrappleTargetOffset = Vector3.zero;
    //Whether the player is currently Grappling
    private bool isGrappling = false;

    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionPower = 10.0f;

    //this is the value that is updated every frame to set the grapples max distance equal to the player distance from the desired point by this value

    //values that affect how the spring joint grapple behaves

    SoftJointLimit jointLimit;
    SoftJointLimitSpring springLimit;
    JointDrive jointDrive;

    [Header("Pull - Spring Settings")]
    [SerializeField] float pullSpringForce = 350;
    [SerializeField] float pullSpringDamper = 300;

    [Header("Push - Spring Settings")]
    [SerializeField] float pushSpringForce = 350;
    [SerializeField] float pushSpringDamper = 300;

    [Header("Grapple Settings")]
    [Tooltip("The Min amount of time a joint has to connected before it can be discontented")]
    [SerializeField] float minJointTime = .08f;
    [SerializeField] bool grappleHasMax = false;

    [Tooltip("The Max amount of time a joint will be connected for")]
    [SerializeField] float maxJointTime = 2;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Adjust 
    /// </summary>
    /// <param name="grappleType"></param>
    private void SetSpringSettings(GrappleType grappleType)
    {
        jointLimit = new SoftJointLimit();
        springLimit = new SoftJointLimitSpring();
        jointDrive = new JointDrive();
        jointDrive.maximumForce = 3.402823e+38f;

        if (grappleType == GrappleType.Pull)
        {
            jointDrive.positionSpring = pullSpringForce;
            jointDrive.positionDamper = pullSpringDamper;
        }
        else
        {
            jointDrive.positionSpring = pushSpringForce;
            jointDrive.positionDamper = pushSpringDamper;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isGrappling)
        {

            StartGrapple(GrappleType.Pull);
        }
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    StopGrapple();
        //}

        if (Input.GetMouseButton(1) && !isGrappling)
        {
            StartGrapple(GrappleType.Push);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    //adds explosion force to raycast point when called
    void Explode()
    {
        print("Explode");
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxPullDistance, whatIsGrappleable))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider rbHit in colliders)
            {
                Rigidbody rb = rbHit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius, 0.0f, ForceMode.Impulse);
                }
            }
        }
    }


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple(GrappleType grappleType)
    {

        RaycastHit hit;
        //If pushing and there is a surface in front of the player for them to push off of
        if (grappleType == GrappleType.Push && Physics.Raycast(camera.position, camera.forward, out hit, maxPushDistance, ~LayerMask.GetMask("CantPush")))
        {
            isGrappling = true;

            lr.positionCount = 0;
        }
        //If pulling and there is a surface in front of the player in which they can grapple to
        else if (grappleType == GrappleType.Pull && Physics.Raycast(camera.position, camera.forward, out hit, maxPullDistance, whatIsGrappleable))
        {
            isGrappling = true;

            grappleRayHit = hit;

            //Set Grapple target and mark point to pull to
            currentGrappleTarget = hit.collider.transform;

            hitObjectClone = Instantiate(hitObject);
            hitObjectClone.transform.position = hit.point;
            hitObjectClone.transform.parent = hit.transform;
            grapplePoint = hitObjectClone.transform.position;

            currentGrappleTargetOffset = grapplePoint - currentGrappleTarget.position;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            StartCoroutine(JointDestroyDelay());
        }

        //Not able to pull or push
        if (!isGrappling)
            return;

        //Joint setup based on push or pull
        joint = player.gameObject.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;

        SetSpringSettings(grappleType);
        joint.xDrive = jointDrive;
        joint.yDrive = jointDrive;
        joint.zDrive = jointDrive;

        StartCoroutine(UpdateGrapplePosition(grappleType));
    }

    /// <summary>
    /// Coroutine that updates the position of the grapple anchor to the appropriate point and then updates the joint
    /// </summary>
    /// <param name="grappleType">Push or Pull</param>
    /// <returns></returns>
    IEnumerator UpdateGrapplePosition(GrappleType grappleType)
    {
        RaycastHit hit;

        while (isGrappling)
        {
            //Pushing
            if (grappleType == GrappleType.Push && Physics.Raycast(camera.position, camera.forward, out hit, maxPushDistance, ~LayerMask.GetMask("CantPush")))
            {
                //Update grapple point to arbitrary point behind player
                grapplePoint = hit.point + -(camera.forward * maxPushDistance);
            }
            else if (grappleType == GrappleType.Push)
            {
                StopGrapple();
            }
            //Pulling
            else if (grappleType == GrappleType.Pull)
            {
                //Update grapple point to the target + original offset
                grapplePoint = currentGrappleTarget.position + currentGrappleTargetOffset;
                //StartCoroutine(JointDestroyDelay());
            }


            //Update Joint
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            jointLimit.limit = distanceFromPoint;
            joint.targetPosition = Vector3.zero;

            currentGrapplePosition = gunTip.position;

            yield return null;
        }
    }

    /// <summary>
    /// Will Handle Forcing the player to have the joint enabled for a min set of time
    /// </summary>
    /// <returns></returns>
    IEnumerator JointDestroyDelay()
    {
        float heldDownTime = 0;
        while (Input.GetMouseButton(0))
        {
            if (grappleHasMax && maxJointTime <= heldDownTime)
            {
                StopGrapple();
                StopCoroutine(JointDestroyDelay());
            }
            heldDownTime += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        if (heldDownTime < minJointTime)
        {
            yield return new WaitForSeconds(minJointTime - heldDownTime);
        }

        StopGrapple();
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        Destroy(hitObjectClone.gameObject);
        isGrappling = false;
        currentGrappleTarget = null;

        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        if (lr.positionCount == 0) return;

        currentGrapplePosition = grapplePoint;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    /// <summary>
    /// Returns the max grapple distance possible. 
    /// </summary>
    /// <returns></returns>
    public float GetMaxGrappleDistance()
    {
        return maxPullDistance;
    }

    /// <summary>
    /// Returns the Rayhit from the grappling Raycast. 
    /// </summary>
    /// <returns></returns>
    public RaycastHit GetGrappleRayhit()
    {
        return grappleRayHit;
    }
}
