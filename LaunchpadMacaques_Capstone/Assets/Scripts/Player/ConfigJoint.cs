using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigJoint : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    private Vector3 currentGrapplePosition;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private ConfigurableJoint joint;


    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionPower = 10.0f;

    //this is the value that is updated every frame to set the grapples max distance equal to the player distance from the desired point by this value

    //values that affect how the spring joint grapple behaves

    SoftJointLimit jointLimit;
    SoftJointLimitSpring springLimit;
    JointDrive jointDrive;

    [Header("Spring Settings")]
    [SerializeField] float positionSpringForce = 350;
    [SerializeField] float positionSpringDamper = 300;

    private GameObject objectFixedTo;
    private RaycastHit grappleRayHit;

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

    private void Start()
    {
        SetSpringSettings();
    }

    private void SetSpringSettings()
    {
        jointLimit = new SoftJointLimit();
        springLimit = new SoftJointLimitSpring();
        jointDrive = new JointDrive();
        jointDrive.maximumForce = 3.402823e+38f;
        jointDrive.positionSpring = positionSpringForce;
        jointDrive.positionDamper = positionSpringDamper;
        lr.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && joint == null)
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Explode();
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
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
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
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grappleRayHit = hit;

            objectFixedTo = hit.collider.gameObject;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            jointLimit.limit = distanceFromPoint;
            joint.targetPosition = Vector3.zero;


            joint.xDrive = jointDrive;
            joint.yDrive = jointDrive;
            joint.zDrive = jointDrive;
           
          
            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            StartCoroutine(JointDestroyDelay());

           
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
            if(grappleHasMax && maxJointTime <= heldDownTime)
            {
                StopGrapple();
                StopCoroutine(JointDestroyDelay());
            }
            heldDownTime += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        if(heldDownTime < minJointTime)
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
        lr.positionCount = 0;
        Destroy(joint);
        objectFixedTo = null;
    }


    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        if(lr.positionCount > 0)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, currentGrapplePosition);
        }
    }

    /// <summary>
    /// Returns true if the player is grappling, false if they are not.
    /// </summary>
    /// <returns></returns>
    public bool IsGrappling()
    {
        return joint != null;
    }

    /// <summary>
    /// Returns the world space point that the player grapples to. 
    /// </summary>
    /// <returns></returns>
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
        return maxDistance;
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
