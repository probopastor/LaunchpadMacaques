using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrapplingGun : MonoBehaviour
{
    #region InspectorVariables
    [Header("Object References")]
    [SerializeField] private GameObject grappleToggleEnabledText;
    [SerializeField] private GameObject grappleToggleDisabledText;
    [SerializeField] private TextMeshProUGUI ropeLengthText;
    [SerializeField] [Tooltip("The point where the grapple is created on the playe")] Transform ejectPoint;
    [SerializeField] [Tooltip("The Main Camera")] Transform cam;
    [SerializeField] Transform player;
    [SerializeField] private GameObject hitObject;

    [Header("Layer Settings")]
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] private LayerMask whatIsNotGrappleable;

    [Header("Grapple Settings")]
    [SerializeField] [Tooltip("The Max distance the player can grapple form")] private float maxGrappleDistance = 100f;
    [SerializeField] [Tooltip("The Speed At Which the Grapple will move the player")] float grappleSpeed = 10f;

    [Header("Rope Settings")]

    [SerializeField] [Tooltip("The Min Rope Distance")] private float minDistance = 5f;
    [SerializeField] [Tooltip("The Max Rope Distance")] private float maxDistance = 50f;
    [SerializeField, Tooltip("The amount of length subtraced from grapple length on each subsequent grapple. ")] private float grappleLengthModifier = 10;
    [SerializeField] private float wheelSensitivity = 2;

    [Header("Swing Settings")]
    [SerializeField] [Tooltip("The Force The Joint will apply to the player")] float springValue = 5f;
    [SerializeField] [Tooltip("The amount the Joint will slow down over time")] float springDamp = 10f;
    [SerializeField] float springMass = 5f;
    [SerializeField] private float minSwingAngle = -90f;
    [SerializeField] private float maxSwingAngle = 90f;

    [Header("Dash / Launch Settings")]
    [SerializeField] public float launchSpeed = 30000;
    [SerializeField] public float maxLaunchMultiplier = 5f;
   private float startTime = 0f;
    private float endTime = 0f;
    float launchMultiplier;

    [Header("Auto Aim Settiings")]
    [SerializeField] [Tooltip("The Radius of the Sphere that will be created to handle Auto Aim")] float sphereRadius = 2;
    [SerializeField] private float neededVelocityForAutoAim = 20;
    [SerializeField]
    [Tooltip("The Distance the A Ray will be shot down to, to fix thse issue of auto aiming onto the platforming your standint on")] private float groundCheckDistance = 5f;
    #endregion

    #region PrivateVariables
    // The current length of the rope
    private float ropeLength = 5f;
    private GameObject hitObjectClone;

    // The Line Renderer that creates the grapple robe
    private LineRenderer lr;
    private Vector3 grapplePoint;

    // Explosion Settings
    private float explosionRadius = 5f;
    private float explosionPower = 10.0f;

    // The private instance of the joint
    private SpringJoint joint;
    private float distanceFromPoint;




    // The Raycast that will be set to what the player is looking at
    private RaycastHit grappleRayHit;

    private bool swingLockToggle;
    private bool canApplyForce;

    // Two private instances of the objec that the player is grappling to (Both used for different things)
    private GameObject grappledObj;
    private GameObject currentGrappledObj;

    // The position that the Joint is connected to
    private Vector3 currentGrapplePosition;

    // The object that will be called to make objects corrupted
    private MakeSpotNotGrappleable corruptObject;

    // The private instance of the push pull objects
    private PushPullObjects pushPull;
    //A bool that when true will allow the player to hold down the mouse button to grapple
    private bool canHoldDownToGrapple;



    private float dist;


    #endregion

    #region StartFunctions
    void Awake()
    {
        SetObject();
        SetText();
    }

    private void SetObject()
    {
        lr = GetComponent<LineRenderer>();

        corruptObject = FindObjectOfType<MakeSpotNotGrappleable>();

        pushPull = this.gameObject.GetComponent<PushPullObjects>();

        swingLockToggle = false;
    }

    private void SetText()
    {
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
    }
    #endregion

    #region UpdateFunctions
    void Update()
    {
        GrappleUpdateChanges();
        GrapplingInput();
        GrapplingLockInput();
        ChangeDistance();
    }

    private void GrappleUpdateChanges()
    {
        if (IsGrappling())
        {
            if (ropeLengthText != null)
            {
                ropeLengthText.text = "Rope Length: " + (int)ropeLength;
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
    }
    #endregion

    #region UserInput
    /// <summary>
    /// Will Get input for things relevant to grappling
    /// </summary>
    private void GrapplingInput()
    {
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



    }

    /// <summary>
    /// Will Get input from the player for enabling/disabling Grapple Lock
    /// </summary>
    private void GrapplingLockInput()
    {
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
    }
    
    /// <summary>
    /// Will change the rope length while grappling, based on mouse wheel input from player
    /// </summary>
    private void ChangeDistance()
    {
        var wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput < 0)
        {
            ropeLength += wheelSensitivity;
            if (ropeLength > maxDistance)
            {
                ropeLength = maxDistance;
            }
        }

        else if (wheelInput > 0)
        {
            ropeLength -= wheelSensitivity;

            if (ropeLength < minDistance)
            {
                ropeLength = minDistance;
            }
        }

        if (joint)
        {
            joint.minDistance = ropeLength;
        }
    }
    #endregion

    #region LateUpdateFunctions
    void LateUpdate()
    {
        DrawRope();

        /// <summary>
        /// Draws the line from the grapple gun to the current grapple point.
        /// </summary>
        void DrawRope()
        {
            //If not grappling, don't draw rope
            if (!joint) return;

            if (lr.positionCount == 0) return;

            currentGrapplePosition = grapplePoint;

            lr.SetPosition(0, ejectPoint.position);
            lr.SetPosition(1, currentGrapplePosition);
        }
    }

    #endregion

    #region Explosion Settings
    //adds explosion force to raycast point when called
    void Explode()
    {
        print("Explode");
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
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
    #endregion

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
            dist = Vector3.Distance(cam.position, hit.point);

            if (!(Physics.Raycast(cam.position, cam.forward, dist, whatIsNotGrappleable)))
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
        Physics.Raycast(cam.position, cam.forward, out returnHit, maxGrappleDistance, whatIsGrappleable);
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
            if(Physics.SphereCast(cam.position, sphereRadius, cam.forward, out grappleObject, maxGrappleDistance, whatIsGrappleable))
            {
                RaycastHit checkDownHit;
                if (Physics.Raycast(cam.position, -cam.up, out checkDownHit, groundCheckDistance, whatIsGrappleable))
                {
                    if (checkDownHit.collider.gameObject != grappleObject.collider.gameObject)
                    {
                        returnHit = grappleObject;
                        return returnHit;
                    }
                }

                else
                {
                    returnHit = grappleObject;
                }
            }
        }
        return returnHit;
    }
    #endregion

    #region Start/Stop Grapple
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

            currentGrappledObj = grappleRayHit.collider.gameObject;

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

            ropeLength = dist - grappleLengthModifier;

            if (ropeLength > maxDistance)
            {
                ropeLength = maxDistance;
            }

            if (ropeLength < minDistance)
            {
                ropeLength = minDistance;
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
                pinwheel.TriggerRotation(grappleRayHit.collider.transform, cam.forward);
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
        currentGrappledObj = null;
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

    #endregion
  
    #region Getters/Setters
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

    public GameObject GetCurrentGrappledObject()
    {
        return currentGrappledObj;
    }

    public float GetLaunchMultipler()
    {
        return launchMultiplier;
    }
    #endregion
}
