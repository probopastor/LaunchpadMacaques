using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private GameObject hitObject;
    private GameObject hitObjectClone;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    [SerializeField] private LayerMask whatIsNotGrappleable;

    [Header("Object References")]
    [SerializeField] private GameObject grappleToggleEnabledText;
    [SerializeField] private GameObject grappleToggleDisabledText;
    [SerializeField] private TextMeshProUGUI ropeLengthText;


    private float explosionRadius = 5f;
    private float explosionPower = 10.0f;

    [Header("Grapple Settings")]

    public Transform gunTip;
    public Transform camera;
    public Transform player;



    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField, Tooltip("The amount of length subtraced from grapple length on each subsequent grapple. ")] private float grappleLengthModifier = 10;
    [SerializeField] private float wheelSensitivity = 2;
    [SerializeField] private float maxGrappleDistance = 100f;
    [SerializeField] private float groundCheckDistance = 5f;

    private SpringJoint joint;
    private float distanceFromPoint;

    //this is the value that is updated every frame to set the grapples max distance equal to the player distance from the desired point by this value
    public float grappleSpeed = 50f;

    //values that affect how the spring joint grapple behaves
    public float springValue = 10f;
    public float springDamp = 10f;
    public float springMass = 5f;

    [SerializeField] private float minSwingAngle = -90f;
    [SerializeField] private float maxSwingAngle = 90f;

    private RaycastHit grappleRayHit;

    private bool swingLockToggle;
    private bool canApplyForce;

    private GameObject grappledObj;

    private MakeSpotNotGrappleable corruptObject;

    private PushPullObjects pushPull;

    [Header("Dash / Launch Settings")]
    [SerializeField] public float launchSpeed = 30000;
    [SerializeField] public float maxLaunchMultiplier = 5f;
    [HideInInspector] public float startTime = 0f;
    [HideInInspector] public float endTime = 0f;
    [HideInInspector] public float launchMultiplier;


    [Header("Auto Aim Settiings")]
    [SerializeField] float sphereRadius = 2;
    private bool canHoldDownToGrapple;
    [SerializeField] private float neededVelocityForAutoAim = 20;

    private float dist;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        if (joint)
        {
            Debug.Log("Joint was created on awake");
            Destroy(joint);
        }

        swingLockToggle = false;

        if (grappleToggleEnabledText != null)
        {
            grappleToggleEnabledText.SetActive(false);
        }

        if (grappleToggleDisabledText != null)
        {
            grappleToggleDisabledText.SetActive(false);
        }

        if (ropeLengthText != null)
        {
            ropeLengthText.text = " ";
        }

        corruptObject = FindObjectOfType<MakeSpotNotGrappleable>();

        pushPull = this.gameObject.GetComponent<PushPullObjects>();
    }

    void Update()
    {
        if (IsGrappling())
        {
            if (ropeLengthText != null)
            {
                ropeLengthText.text = "Rope Length: " + (int)distance;
            }

            if (joint.maxDistance <= 0)
            {
                joint.maxDistance = 0;
            }
            else
            {
                joint.maxDistance = distanceFromPoint - grappleSpeed * Time.deltaTime;
            }

            grapplePoint = hitObjectClone.transform.position;

            if (grappledObj != null)
            {
                Vector3 objectDirection = (grappledObj.transform.position - player.transform.position).normalized;
                Vector3 groundDirection = Vector3.down;

                float angle = Vector3.Angle(objectDirection, groundDirection);

                if (angle < minSwingAngle || angle > maxSwingAngle)
                {
                    canApplyForce = true;
                }
                else
                {
                    canApplyForce = false;
                }
            }
        }
        else if (!IsGrappling())
        {
            if (ropeLengthText != null)
            {
                ropeLengthText.text = " ";
            }
        }

        if (Input.GetMouseButtonUp(0) && IsGrappling())
        {
            canHoldDownToGrapple = true;
        }
        if (Input.GetMouseButton(0) && IsGrappling() && canHoldDownToGrapple == true)
        {
            // StopGrapple();
            StartGrapple();
        }

        else if (Input.GetMouseButton(0) && !IsGrappling() && !pushPull.IsGrabbing())
        {
            StartGrapple();
        }

        else if (Input.GetMouseButtonDown(1) && IsGrappling())
        {
            StopGrapple();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrappling())
        {
            if (!swingLockToggle)
            {
                swingLockToggle = true;
                grappleToggleEnabledText.SetActive(true);
                grappleToggleDisabledText.SetActive(false);
            }
            else
            {
                swingLockToggle = false;
                grappleToggleEnabledText.SetActive(false);
                grappleToggleDisabledText.SetActive(true);
            }
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Explode();
        //}

        if (lr == null && joint)
        {
            Debug.Log("Line Render Was Dead but Joint was still there");
            StopGrapple();
        }


        ChangeDistance();
    }

    private void ChangeDistance()
    {
        var wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput < 0)
        {
            distance += wheelSensitivity;
            if (distance > maxDistance)
            {
                distance = maxDistance;
            }
        }

        else if (wheelInput > 0)
        {
            distance -= wheelSensitivity;

            if (distance < minDistance)
            {
                distance = minDistance;
            }
        }

        if (joint)
        {
            joint.minDistance = distance;
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
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
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

    #region Look For Grapple Location
    /// <summary>
    /// Will return a bool for if a grapple location be be found
    /// </summary>
    /// <returns></returns>
    public bool CanFindGrappleLocation()
    {
        RaycastHit hit = new RaycastHit();

        // Cheks if the Normal Raycast returns a RayCastHit with a collider
        if(CheckRayCast().collider != null)
        {
            hit = CheckRayCast();
        }

        // Cheks if the SphereCast returns a RayCastHit with a collider
        else if (CheckSphereCast().collider != null)
        {
            hit = CheckSphereCast();
        }


        if(hit.collider != null)
        {
            dist = Vector3.Distance(camera.position, hit.point);

            if (!(Physics.Raycast(camera.position, camera.forward, dist, whatIsNotGrappleable)))
            {

                grappleRayHit = hit;
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Will run a Ray Cast from player to max grapple distance
    /// Returns a Ray Cast Hit
    /// Retuns a Null Collider one if a grappleable object is not found
    /// Returns a RayCastHit with a collider if a grappleable object is found
    /// </summary>
    /// <returns></returns>
    private RaycastHit CheckRayCast()
    {
        RaycastHit returnHit = new RaycastHit();
        Physics.Raycast(camera.position, camera.forward, out returnHit, maxGrappleDistance, whatIsGrappleable);
        return returnHit;
    }

    /// <summary>
    /// Will run a sphere cast from player to max grapple distance
    /// Returns a Ray Cast Hit
    /// Retuns a Null Collider one if a grappleable object is not found
    /// Returns a RayCastHit with a collider if a grappleable object is found
    /// </summary>
    /// <returns></returns>
    private RaycastHit CheckSphereCast()
    {
        RaycastHit returnHit = new RaycastHit();
        if(player.GetComponent<Rigidbody>().velocity.magnitude >= neededVelocityForAutoAim)
        {
            RaycastHit grappleObject;
            if(Physics.SphereCast(camera.position, sphereRadius, camera.forward, out grappleObject, maxGrappleDistance, whatIsGrappleable))
            {
                RaycastHit checkDownHit;
                if (Physics.Raycast(camera.position, -camera.up, out checkDownHit, groundCheckDistance, whatIsGrappleable))
                {
                    if (checkDownHit.collider.gameObject != grappleObject.collider.gameObject)
                    {
                        returnHit = grappleObject;
                    }
                }
            }
        }
        return returnHit;
    }
    #endregion

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    { 
        if (CanFindGrappleLocation())
        {
            canHoldDownToGrapple = false;

            if (IsGrappling())
            {
                StopGrapple();
            }

            startTime = Time.time;

            hitObjectClone = Instantiate(hitObject);
            hitObjectClone.transform.position = grappleRayHit.point;
            hitObjectClone.transform.parent = grappleRayHit.transform;
            grapplePoint = hitObjectClone.transform.position;

            if (grappleRayHit.collider != null)
            {
                grappledObj = grappleRayHit.transform.gameObject;
            }
            else
            {
                return;
            }

            corruptObject.MakeSpotNotGrappable(grappleRayHit, grappledObj);

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint;
            joint.minDistance = dist;

            distance = dist - grappleLengthModifier;

            if (distance > maxDistance)
            {
                distance = maxDistance;
            }

            if (distance < minDistance)
            {
                distance = minDistance;
            }

            joint.enableCollision = false;

            joint.spring = springValue;
            joint.damper = springDamp;
            joint.massScale = springMass;


            lr.positionCount = 2;
            currentGrapplePosition = hitObjectClone.transform.position;
            GetComponent<FMODUnity.StudioEventEmitter>().Play();

            //Pinwheel
            Pinwheel pinwheel = null;
            if (pinwheel = grappleRayHit.collider.GetComponentInParent<Pinwheel>())
            {
                pinwheel.TriggerRotation(grappleRayHit.collider.transform, camera.forward);
            }

            //Temporary lock UI disabled after completing a grapple
            if (grappleToggleEnabledText != null)
            {
                grappleToggleEnabledText.SetActive(false);
            }
            if (grappleToggleDisabledText != null)
            {
                grappleToggleDisabledText.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        //managing variables for dash
        endTime = Time.time;
        launchMultiplier = Mathf.Min(endTime - startTime + 2f, maxLaunchMultiplier);


        swingLockToggle = false;

        //Temporary lock UI disabled after completing a grapple
        if (grappleToggleEnabledText != null)
        {
            grappleToggleEnabledText.SetActive(false);
        }
        if (grappleToggleDisabledText != null)
        {
            grappleToggleDisabledText.SetActive(false);
        }

        if (hitObjectClone)
        {
            Destroy(hitObjectClone.gameObject);
        }

        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    /// <summary>
    /// Draws the line from the grapple gun to the current grapple point.
    /// </summary>
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        if (lr.positionCount == 0) return;

        currentGrapplePosition = grapplePoint;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    /// <summary>
    /// Booleon function that determines if the player is grappleing or not.
    /// </summary>
    /// <returns></returns>
    public bool IsGrappling()
    {
        return joint != null;
    }

    /// <summary>
    /// Vector function that returns the grapple point
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
        return maxGrappleDistance;
    }

    /// <summary>
    /// Returns the Rayhit from the grappling Raycast.
    /// </summary>
    /// <returns></returns>
    public RaycastHit GetGrappleRayhit()
    {
        return grappleRayHit;
    }

    public bool GetSwingLockToggle()
    {
        return swingLockToggle;
    }

    public bool GetCanApplyForce()
    {
        return canApplyForce;
    }


    public float GetSphereSphereRadius()
    {
        return sphereRadius;
    }

    public Transform GetCamera()
    {
        return camera;
    }

    public LayerMask GetGrappleLayer()
    {
        return whatIsGrappleable;
    }

    public LayerMask GetUnGrappleLayer()
    {
        return whatIsNotGrappleable;
    }

    public float GetAutoAimVelocity()
    {
        return neededVelocityForAutoAim;
    }

    public float GetDistance()
    {
        return dist;
    }

    //public RaycastHit GetSecondHit()
    //{
    //    //return secondhit
    //}
}
