﻿/* 
* Launchpad Macaques - Neon Oblivion
* Matt Kirchoff, Levi Schoof, William Nomikos
* GrapplingGun.cs
* Script handles grappling and ungrappling from objects, and swinging mechanics. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using FMOD.Studio;

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
    [SerializeField] private TextMeshProUGUI currentSwingSpeedText;
    [SerializeField] TextMeshProUGUI actualVelocityText;
    [SerializeField] Slider ropeLengthSlider;
    [SerializeField] GameObject postText;

    [Header("Layer Settings")]
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] private LayerMask whatIsNotGrappleable;

    [Header("Grapple Settings")]
    [SerializeField] [Tooltip("The Max distance the player can grapple form")] private float maxGrappleDistance = 100f;
    [SerializeField] [Tooltip("The Speed At Which the Grapple will move the player")] float grappleSpeed = 10f;

    [Header("Rope Settings")]

    [SerializeField] [Tooltip("The Min Rope Distance")] private float minDistance = 5f;
    [SerializeField] [Tooltip("The Max Rope Distance")] private float maxDistance = 50f;
    [SerializeField, Tooltip("The grapple length on regrapple. ")] private float newSwingGrappleLength = 10;
    [SerializeField] private float wheelSensitivity = 2;

    [Header("Swing Settings")]
    [SerializeField] [Tooltip("The Force The Joint will apply to the player")] float springValue = 5f;
    [SerializeField] [Tooltip("The amount the Joint will slow down over time")] float springDamp = 10f;
    [SerializeField] float springMass = 5f;
    [SerializeField] private float minSwingAngle = -90f;
    [SerializeField] private float maxSwingAngle = 90f;
    [SerializeField, Tooltip("The force added at the bottom of a swing to keep the loop going")]
    public float swingSpeed = 4500;
    [SerializeField] private float maxSwingVelocity = 18;
    [SerializeField] private bool useConstantVelocity = false;
    private float currentSwingSpeed;

    [Header("Auto Aim Settiings")]
    [SerializeField] [Tooltip("The Radius of the Sphere that will be created to handle Auto Aim")] float sphereRadius = 2;
    [SerializeField] private float neededVelocityForAutoAim = 20;
    [SerializeField]
    [Tooltip("The Distance the A Ray will be shot down to, to fix thse issue of auto aiming onto the platforming your standint on")] private float groundCheckDistance = 5f;

    [Header("Hand Movement Settings")]
    [SerializeField, Tooltip("The max amount the hand will tilt up and down based on movement")]
    private float horizontalRotationMax = 15.0f;
    [SerializeField, Tooltip("The max amount the hand will tilt left and right based on movement")]
    private float verticalRotationMax = 15.0f;
    [SerializeField, Tooltip("When True, the hand will tilt left and right while grappling")]
    private bool useHorizontalMovementWhileGrappling = true;
    [SerializeField, Tooltip("When True, the hand will tilt up and down while grpapling")]
    private bool useVerticalMovementWhileGrappling = true;
    [SerializeField, Range(0, 1), Tooltip("When the Cosine calculations are preformed and the result is within this value of 0 or 1, it" +
    "will be rounded either down or up respecitvely. Increasing this value can decrease twitching-like movements of the hand, but will decrease" +
    "the fluidity of the movement")]
    private float roundingRange = 0.2f;

    [SerializeField]
    Animator anim;

    #endregion

    #region PrivateVariables
    // The current length of the rope
    private float ropeLength = 5f;
    private float startingRopeLength = 0;
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

    private Matt_PlayerMovement playerMovementReference;

    // The private instance of the push pull objects
    private PushPullObjects pushPull;
    //A bool that when true will allow the player to hold down the mouse button to grapple
    private bool canHoldDownToGrapple;

    

    private float dist;

    #endregion

    #region StartFunctions
    void Awake()
    {
        if (postText)
        {
            postText.SetActive(false);
        }
     
        SetObject();
        SetText();

        currentSwingSpeed = swingSpeed;

        if (ropeLengthSlider)
        {
            ropeLengthSlider.gameObject.SetActive(false);
        }
    }

    private void SetObject()
    {
        cam = Camera.main.transform;

        lr = GetComponent<LineRenderer>();
        playerMovementReference = FindObjectOfType<Matt_PlayerMovement>();

        corruptObject = FindObjectOfType<MakeSpotNotGrappleable>();

        pushPull = this.gameObject.GetComponent<PushPullObjects>();

        swingLockToggle = false;
        startingRopeLength = ropeLength;
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
        if (actualVelocityText)
        {
            actualVelocityText.text = "Velocity: (" + (int)player.GetComponent<Rigidbody>().velocity.x + ", " + 
                (int)player.GetComponent<Rigidbody>().velocity.y + ", " + (int)player.GetComponent<Rigidbody>().velocity.z +")";
        }


        if (currentSwingSpeedText != null)
        {
            currentSwingSpeedText.text = "Magnitude: " + (int)player.GetComponent<Rigidbody>().velocity.magnitude;
        }
        GrappleUpdateChanges();
        GrapplingInput();
        GrapplingLockInput();
        ChangeDistance();
    }

    private void GrappleUpdateChanges()
    {
        if (IsGrappling())
        {
            joint.damper = springDamp * Mathf.Clamp(ropeLength * 100, 1, Mathf.Infinity);
            joint.spring = joint.damper * 0.5f;

            currentSwingSpeed = swingSpeed * (1 + ropeLength * 0.05f);
            //joint.spring = normalSpring * (1 + ropeLength * 1);
            //joint.damper = springDamp * (1 + ropeLength * 0.5f);

            //if (ropeLength > 30)
            //{
            //    Debug.Log("THING");
            //    //joint.spring = higherSpringValue;
            //    joint.damper = 1000;
            //   // currentSwingSpeed = higherSwing;
            //}

            Debug.Log("Damper: " + joint.damper + " | Spring: " + joint.spring + " | SwingSpeed: " + currentSwingSpeed);

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
                joint.maxDistance = Vector3.Distance(grapplePoint, player.transform.position);
                joint.maxDistance = distanceFromPoint - grappleSpeed * Time.deltaTime;
            }

            grapplePoint = hitObjectClone.transform.position;

            if (grappledObj != null)
            {
                Vector3 objectDirection = (grappledObj.transform.position - player.transform.position).normalized;
                Vector3 groundDirection = Vector3.down;

                float angle = Vector3.Angle(objectDirection, groundDirection);

                if ((angle < minSwingAngle || angle > maxSwingAngle) && !playerMovementReference.GetKillForce())
                {
                    canApplyForce = true;
                }
                else
                {
                    canApplyForce = false;
                }
            }

            if (player.GetComponent<Rigidbody>().velocity.magnitude > maxSwingVelocity && !useConstantVelocity)
            {
                player.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(player.GetComponent<Rigidbody>().velocity, maxSwingVelocity);
            }

            if (useConstantVelocity)
            {
                player.GetComponent<Rigidbody>().velocity = CustomClampMagnitude(player.GetComponent<Rigidbody>().velocity, maxSwingVelocity, maxSwingVelocity);
            }


            if (ropeLengthSlider)
            {
                ropeLengthSlider.value = ropeLength;
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
            if (Input.GetMouseButtonDown(0) && !IsGrappling() && !pushPull.IsGrabbing())
            {
                StartGrapple();
            }
            else if (Input.GetMouseButtonDown(0) && IsGrappling())
            {
                StopGrapple();
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

            if (!(Physics.Raycast(cam.position, cam.forward, dist, whatIsNotGrappleable)) && !pushPull.IsGrabbing())
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

            anim.ResetTrigger("Dash");
            anim.ResetTrigger("GrappleEnd");
            anim.SetTrigger("GrappleStart");
            

            if (IsGrappling())
            {
                StopGrapple();
            }

            if (ropeLengthSlider)
            {
                ropeLengthSlider.gameObject.SetActive(true);
                ropeLengthSlider.maxValue = maxDistance;
                ropeLengthSlider.minValue = minDistance;
            }

            currentGrappledObj = grappleRayHit.collider.gameObject;

            if(currentGrappledObj.GetComponent<GrapplePoint>() != null)
            {
                GrapplePoint point = currentGrappledObj.GetComponent<GrapplePoint>();

                if (!point.isBreaking())
                {
                    point.Break();
                }
            }

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

            //ropeLength = distanceFromPoint / 2; /*dist - grappleLengthModifier;*/

            ropeLength = newSwingGrappleLength;

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
        anim.ResetTrigger("GrappleStart");
        anim.SetTrigger("GrappleEnd");

        if (ropeLengthSlider)
        {
            ropeLengthSlider.gameObject.SetActive(false);
        }
        currentGrappledObj = null;
        //managing variables for dash

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

    #region Hand Movement
    /// <summary>
    /// Called Every Frame by Matt_PlayerMovement.cs, takes the players current velocity and uses it to determine how the hand should move
    /// </summary>
    /// <param name="currentVelocity">The vector current velocity of the player</param>
    public void UpdateHandRotation(Vector3 currentVelocity)
    {
        //Find where hand should move horizontally
        Vector3 camReferenceX = Vector3.ProjectOnPlane(cam.right, Vector3.up); //Project the camera's right onto a horizontal plane
        Vector3 velocityX = Vector3.ProjectOnPlane(currentVelocity, Vector3.up); //Project the player's velocity onto the same horizontal plane
        float angleX = Vector3.Angle(camReferenceX, velocityX); //Get the angle between the two vectors (Since they're on the same horizontal plane, this will be the direction the velocity is relative to the player
        float horizontalCos = Mathf.Cos(angleX * Mathf.Deg2Rad);  //Get the cosine of the angle (1 when to the left or right of the player, 0 when to the front or back
        //Prevent precision issues, if the number is small or big enough just set it to 0 or 1
        if (Mathf.Abs(horizontalCos) < roundingRange)
            horizontalCos = 0;
        else if (Mathf.Abs(horizontalCos) > 1 - roundingRange)
            horizontalCos = 1 * Mathf.Sign(horizontalCos);
        //Lerp between no movement and the xRotationMax variable based on how close the players velocity is to the max velocity
        float horizontalRotationEulerAngle = Mathf.Lerp(0, horizontalRotationMax, (currentVelocity.magnitude / playerMovementReference.GetMaxVelocity())) * horizontalCos;

        //If option is selected, don't use horizontal movement while grappling (can have slightly weird effects)
        if (!useHorizontalMovementWhileGrappling && IsGrappling())
            horizontalRotationEulerAngle = 0;

        //Find where hand should move vertically
        Vector3 velocityY = currentVelocity;
        float angleY = Vector3.Angle(Vector3.down, velocityY); //Get the angle between the global down and the player's velocity
        float verticalCos = Mathf.Cos(angleY * Mathf.Deg2Rad); //Get the cos of the angle such that down and up directions are one and everything to the side is 0
       //Prevent precision issues, if the cos is close to 1 or 0, round it to one or 0
        if (Mathf.Abs(verticalCos) < roundingRange)
            verticalCos = 0;
        else if (Mathf.Abs(verticalCos) > 1 - roundingRange)
            verticalCos = 1 * Mathf.Sign(verticalCos);
        //Get the rotation amount based on a lerp between 0 and the maximum rotation amount
        float verticalRotationEulerAngle = Mathf.Lerp(0, verticalRotationMax, (currentVelocity.magnitude / playerMovementReference.GetMaxVelocity())) * verticalCos;

        //If option is selected, don't use vertical movement while grappling
        if (!useVerticalMovementWhileGrappling && IsGrappling())
            horizontalRotationEulerAngle = 0;

        //Set the rotation
        transform.localRotation = Quaternion.Euler(verticalRotationEulerAngle, horizontalRotationEulerAngle, 0);
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

    public float GetSwingSpeed()
    {
        return currentSwingSpeed;
    }

    public float GetRopeLength()
    {
        return ropeLength;
    }

    public float GetStartingRopeLength()
    {
        return startingRopeLength;
    }

    public float SetRopeLength(float value)
    {
        return ropeLength = value;
    }

    #endregion

    /// <summary>
    /// Will clamp the Velocity between a min and a max
    /// </summary>
    /// <param name="v"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    public Vector3 CustomClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (max * max)) return v.normalized * max;
        else if (sm < min * min) return v.normalized * min;
        return v;
    }
}
