using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ConfigJoint : MonoBehaviour
{
    [SerializeField] private GameObject hitObject;
    private GameObject hitObjectClone;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    [SerializeField] LayerMask whatIsNotGrappleable;
    private Vector3 currentGrapplePosition;
    public Transform gunTip, camera1, player;
    private float maxPullDistance = 100f;
    private ConfigurableJoint joint;
    private RaycastHit grappleRayHit;

    [SerializeField] private Transform orientation;


    private enum GrappleType { Pull, Push }
    //If the player is Pulling, this is the object they're pulling toward
    private Transform currentGrappleTarget = null;
    //Offset to account for the grapple target moving while pulling to it
    private Vector3 currentGrappleTargetOffset = Vector3.zero;
    //Whether the player is currently Grappling
    private bool isGrappling = false;




    //this is the value that is updated every frame to set the grapples max distance equal to the player distance from the desired point by this value

    //values that affect how the spring joint grapple behaves

    SoftJointLimit jointLimit;
    SoftJointLimitSpring springLimit;
    JointDrive jointDrive;

    [Header("Pull - Spring Settings")]
    [SerializeField] float pullSpringForce = 350;
    [SerializeField] float pullSpringDamper = 300;
    [SerializeField] float launchSpeed = 30000;
    [SerializeField] float minDistanceFromObjForLaunch = 10f;
    [SerializeField] float distanceFromObjForLaunch;
    [SerializeField] float minTimeForLaunch = 1f;
    [SerializeField] float maxLaunchMultiplier = 5f;
    [SerializeField] float startTime = 0f;
    [SerializeField] float endTime = 0f;
    [SerializeField] float ghostTime = 1f;
    

[Header("Push Settings")]
    [SerializeField, Tooltip("The maximum distance that the player is allowed to push from")]
    private float maxPushDistance = 20f;
    [SerializeField, Tooltip("The maximum time (in seconds) that the player can constantly push before being forced to stop")]
    private float maxPushTime = 2f;
    //Current time player has pushed out of max
    private float currentPushTime = 0;
    [SerializeField, Tooltip("The delay (in seconds) between pushes ")]
    private float pushDelay = 1.25f;
    private float currentPushDelay = 0;
    [SerializeField] private GameObject pushParticle;

    [Space(10)]

    [SerializeField] float pushSpringForce = 350;
    [SerializeField] float pushSpringDamper = 300;

    [Header("Grapple Settings")]
    [Tooltip("The Min amount of time a joint has to connected before it can be discontented")]
    [SerializeField] float minJointTime = .08f;
    [SerializeField] bool grappleHasMax = false;

    [Tooltip("The Max amount of time a joint will be connected for")]
    [SerializeField] float maxJointTime = 2;
    private bool inGrappleRoutine = false;



    private MakeSpotNotGrappleable grappleSpotChanger;
    private float[] currentCooldowns;

    private PushPullObjects pushPull;
    [Header("Audio Variables")]
    private GrappleGun_Audio m_audio;


    GameObject obj;

    private bool canApplyForce;

    JointDrive ydrive;
    void Awake()
    {

        lr = GetComponent<LineRenderer>();
        pushPull = this.GetComponent<PushPullObjects>();
        m_audio = GetComponent<GrappleGun_Audio>();


        if (joint)
        {
            Debug.Log("Joint was created on awake");
            Destroy(joint);
        }
    }

    private void Start()
    {
        grappleSpotChanger = FindObjectOfType<MakeSpotNotGrappleable>();

        if (grappleSpotChanger)
        {
            Debug.Log("Found it");
        }

        else
        {
            Debug.Log("Did Not Find it");
        }
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
        ydrive.maximumForce = 3.402823e+38f;
        jointDrive.maximumForce = 3.402823e+38f;

        if (grappleType == GrappleType.Pull)
        {
            jointDrive.positionSpring = pullSpringForce;
            jointDrive.positionDamper = pullSpringDamper;

            ydrive.positionSpring =  pullSpringForce;
            ydrive.positionDamper = 5;
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
        else if (Input.GetMouseButtonDown(0) && isGrappling)
        {
            inGrappleRoutine = false;
            StopGrapple();
            StartGrapple(GrappleType.Pull);
        }

        if (Input.GetMouseButton(1) && !isGrappling)
        {
            StartGrapple(GrappleType.Push);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }

        if(lr == null && joint)
        {
            Debug.Log("Line Render Was Dead but Joint was still there");
            StopGrapple();
        }


        if (isGrappling)
        {
            if (obj != null)
            {
                Vector3 objectDirection = (obj.transform.position - player.transform.position).normalized;
                Vector3 groundDirection = Vector3.down;

                float angle = Vector3.Angle(objectDirection, groundDirection);

                Debug.Log("angle " + angle);

                if (angle < 90 || angle > -90)
                {
                    canApplyForce = true;
                }
                else
                {
                    canApplyForce = false;
                }
            }
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple(GrappleType grappleType)
    {
        RaycastHit hit;
        startTime = Time.time;

        //If pushing and there is a surface in front of the player for them to push off of
        if (grappleType == GrappleType.Push && Physics.Raycast(camera1.position, camera1.forward, out hit, maxPushDistance, ~LayerMask.GetMask("CantPush"))
            && CanPush())
        {
            isGrappling = true;
            Instantiate(pushParticle, hit.point, Quaternion.LookRotation((camera1.position - hit.point).normalized));
            lr.positionCount = 0;
            //GetComponent<FMODUnity.StudioEventEmitter>().Play();
            m_audio.m_push.Play();


        }
        //If pulling and there is a surface in front of the player in which they can grapple to
        else if (grappleType == GrappleType.Pull && Physics.Raycast(camera1.position, camera1.forward, out hit, maxPullDistance, whatIsGrappleable) && !pushPull.IsGrabbing())
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            Vector3 dir = (hit.point - transform.position).normalized;

            if (!Physics.Raycast(transform.position, dir, distance, whatIsNotGrappleable))
            {
                obj = hit.collider.gameObject;
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


                inGrappleRoutine = true;
                //GetComponent<FMODUnity.StudioEventEmitter>().Play();
               // m_audio.m_grapple.Play();
               // m_audio.m_beam.Play();
                StartCoroutine(JointDestroyDelay());

                //grappleSpotChanger.MakeSpotNotGrappable(hit, hit.collider.gameObject);

                if(grappleSpotChanger != null)
                {
                    grappleSpotChanger.MakeSpotNotGrappable(hit, hit.collider.gameObject);
                }

                //Pinwheel
                Pinwheel pinwheel = null;
                if (pinwheel = hitObjectClone.GetComponentInParent<Pinwheel>())
                {
                    pinwheel.TriggerRotation(hitObjectClone.transform, camera1.forward);
                }
            }
        }

        //Not able to pull or push
        if (!isGrappling)
            return;

        //Joint setup based on push or pull
        joint = player.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = obj.gameObject.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;

        SetSpringSettings(grappleType);
        joint.xDrive = jointDrive;
        joint.yDrive = ydrive;
        //joint.yDrive = jointDrive;
        joint.zDrive = jointDrive;



        joint.linearLimitSpring = springLimit;

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
            if (grappleType == GrappleType.Push && Physics.Raycast(camera1.position, camera1.forward, out hit, maxPushDistance, ~LayerMask.GetMask("CantPush"))
                && CanPush())
            {
                //Update grapple point to arbitrary point behind player
                grapplePoint = hit.point + -(camera1.forward * maxPushDistance);
                //Spawn a particle every 10th frame
                if(Time.frameCount % 10 == 0)
                    Instantiate(pushParticle, hit.point, Quaternion.LookRotation((camera1.position - hit.point).normalized));
                currentPushTime += Time.deltaTime;
            }
            else if (grappleType == GrappleType.Push)
            {
                StopGrapple();
            }
            //Pulling
            else if (grappleType == GrappleType.Pull && currentGrappleTarget != null)
            {
                //Update grapple point to the target + original offset
                grapplePoint = currentGrappleTarget.position + currentGrappleTargetOffset;
                //StartCoroutine(JointDestroyDelay());
            }

            //Update Joint
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            // jointLimit.limit = distanceFromPoint;
            joint.enableCollision = false;
            joint.targetPosition = (Vector3.zero);

            currentGrapplePosition = gunTip.position;

            yield return null;
        }

        if (grappleType == GrappleType.Push)
            StartCoroutine(PushDelay());

    }

    /// <summary>
    /// Will Handle Forcing the player to have the joint enabled for a min set of time
    /// </summary>
    /// <returns></returns>
    IEnumerator JointDestroyDelay()
    {
        float heldDownTime = 0;
        while (inGrappleRoutine)
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
    /// Handles the short delay cooldown between pushes
    /// </summary>
    /// <returns></returns>
    IEnumerator PushDelay()
    {
        //Set push delay to max and count down
        currentPushDelay = pushDelay;

        while(currentPushDelay != 0)
        {
            currentPushDelay = Mathf.Clamp(currentPushDelay - Time.deltaTime, 0, pushDelay);
            yield return null;
        }

        //Reset Push Time for next Push
        currentPushTime = 0;
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        endTime = Time.time;
        Vector3 pullDirection = player.transform.position - grapplePoint;

        distanceFromObjForLaunch = (grapplePoint - player.transform.position).magnitude;

        float deltaLaunchTime = endTime - startTime;
        float launchMultiplier = Mathf.Min(endTime - startTime + 2f, maxLaunchMultiplier);

        //launch player in direction if distance allows it
        //if ((distanceFromObjForLaunch > minDistanceFromObjForLaunch) && (deltaLaunchTime > minTimeForLaunch))
        //{
        //    player.GetComponent<CapsuleCollider>().enabled = false;
        //    StartCoroutine(GhostMode(ghostTime, pullDirection, launchMultiplier));
        //}



        if (hitObjectClone)
        {
            Destroy(hitObjectClone.gameObject);
        }

        isGrappling = false;
        currentGrappleTarget = null;

        lr.positionCount = 0;
        Destroy(joint);

       // m_audio.m_beam.Stop();
    }

    IEnumerator GhostMode(float time, Vector3 pullDirection, float launchMultiplier)
    {
        float currentTime = 0;
        while (currentTime < time)
        {
           player.GetComponent<Rigidbody>().AddForce((-pullDirection.normalized) * (launchSpeed * launchMultiplier) * Time.deltaTime, ForceMode.Impulse);
           currentTime += Time.deltaTime;
           yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(time / 2);

        player.GetComponent<CapsuleCollider>().enabled = true;

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

    public bool CanPush()
    {
        return (currentPushTime < maxPushTime) && (currentPushDelay == 0f);
    }

    public bool IsGrappling()
    {
        return joint != null && isGrappling;
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

    public bool GetCanApplyForce()
    {
        return canApplyForce;
    }
}
