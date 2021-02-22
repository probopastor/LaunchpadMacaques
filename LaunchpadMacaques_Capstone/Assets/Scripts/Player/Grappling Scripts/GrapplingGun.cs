/*
* Launchpad Macaques - Neon Oblivion
* Matt Kirchoff, Levi Schoof, William Nomikos, Connor Wolf
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
using FMODUnity;

public class GrapplingGun : MonoBehaviour
{
    #region InspectorVariables
    [Header("Object References")]
    [SerializeField] [Tooltip("The point where the grapple is created on the player")] Transform ejectPoint;
    [SerializeField] [Tooltip("The Main Camera")] Transform cam;
    [SerializeField] Transform player;
    [SerializeField] private GameObject hitObject;
    [SerializeField] GameObject postText;
    [SerializeField] TextMeshProUGUI grapplesLeftTextBox;

    [Header("Layer Settings")]
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] private LayerMask whatIsNotGrappleable;

    [Header("Grapple Settings")]
    [SerializeField] [Tooltip("The Max distance the player can grapple form")] private float maxGrappleDistance = 100f;
    [SerializeField] [Tooltip("The Speed At Which the Grapple will move the player")] float grappleSpeed = 10f;

    [Header("Which Type Of Grapple")]
    [SerializeField] GrappleTypes typeOfGrapple;
    [SerializeField]
    enum GrappleTypes
    {
        PlayerControlled_Constant_Velocity,
        PlayerControlled_Non_Constant_Velocity, IncreasingVelocity_Constant_Velocity, IncreasingVelocity_Non_Constant_Velocity, JustConstantVelocit, Just_Non_Constant_Velocity
    }



    [Header("Rope Settings")]
    [SerializeField, Tooltip("The grapple length on regrapple. ")] private float ropeLength = 10;


    [Header("Rope Attach Settings")]
    [SerializeField, Tooltip("The Speed at which the rope attaches to the Grapple Point")] float startingAttachSpeed = 20;
    [SerializeField, Tooltip("The amount the line attach speed will increase over time")] float attachSpeedIncrease = 10;

    [Header("Swing Settings")]
    [SerializeField] [Tooltip("The Force The Joint will apply to the player")] float springValue = 5f;
    [SerializeField] [Tooltip("The amount the Joint will slow down over time")] float springDamp = 10f;
    [SerializeField] float springMass = 5f;
    [SerializeField] private float minSwingAngle = -90f;
    [SerializeField] private float maxSwingAngle = 90f;
    [SerializeField, Tooltip("The force added at the bottom of a swing to keep the loop going")]
    public float swingSpeed = 4500;
    [SerializeField] private float maxSwingVelocity = 18;
    private bool useConstantVelocity = false;
    private float currentSwingSpeed;


    [Header("Pull Settings")]
    [SerializeField, Tooltip("How Fast the player will be pulled toward grapple point")] float pullSpeed = 50;
    [SerializeField, Tooltip("The Max Speed the player can be moving while being pulled")] float maxPullVelocity = 100;
    [SerializeField, Tooltip("The max length the pull can last")] float pullLength = .5f;
    [SerializeField, Tooltip("The Min Distance the player can be from the object and still get pulled towards it")] float minDistanceFromObject = 5;

    [Header("Increasing Velocity Settings")]
    [SerializeField, Tooltip("How fast the players velocity will increase while swinging")] float increaseAmmount = 10;
    [SerializeField, Tooltip("The Minimum velocity the player will have when they start swinging")] float minStartingVelocity = 5;
    [Tooltip("If the Players velocity should increase while they swing")] bool useIncreasingVelocity = true;


    [Header("Player Control Velocity Settings")]
    bool playerCanControlVelocity;
    [SerializeField] float mouseWheelSensitivity = 1;

    [Header("Max Grapple Settings")]
    [SerializeField, Tooltip("The Max Amount of times the player can grapple before hitting ground")] int maxGrapples = 4;
    [SerializeField, Tooltip("If the amount of graples should be limited")] bool useMaxGrapples = true;

    [Header("Auto Aim Settings")]
    [SerializeField] [Tooltip("The Radius of the Sphere that will be created to handle Auto Aim")] float sphereRadius = 2;
    [SerializeField] private float neededVelocityForAutoAim = 20;
    [SerializeField]
    [Tooltip("The Distance the A Ray will be shot down to, to fix these issue of auto aiming onto the platforming your standing on")] private float groundCheckDistance = 5f;

    [Header("Hand Movement Settings")]
    [SerializeField, Tooltip("The max amount the hand will tilt up and down based on movement")]
    private float horizontalRotationMax = 15.0f;
    [SerializeField, Tooltip("The max amount the hand will tilt left and right based on movement")]
    private float verticalRotationMax = 15.0f;
    [SerializeField, Tooltip("When True, the hand will tilt left and right while grappling")]
    private bool useHorizontalMovementWhileGrappling = true;
    [SerializeField, Tooltip("When True, the hand will tilt up and down while grappling")]
    private bool useVerticalMovementWhileGrappling = true;
    [SerializeField, Range(0, 1), Tooltip("When the Cosine calculations are preformed and the result is within this value of 0 or 1, it" +
    "will be rounded either down or up respectively. Increasing this value can decrease twitching-like movements of the hand, but will decrease" +
    "the fluidity of the movement")]
    private float roundingRange = 0.2f;

    private bool pulling = false;

    [SerializeField]
    Animator anim;

    [Header("Audio Clips")]
    [EventRef, SerializeField, Tooltip("Audio clip that plays when grapple is initiated.")]
    private string grappleStart;
    [EventRef, SerializeField, Tooltip("Audio clip that plays when grapple is active.")]
    private string grappleActive;
    [EventRef, SerializeField, Tooltip("Audio clip that plays when grapple is ended.")]
    private string grappleEnd;

    private EventInstance grapplingInstance;


    #endregion

    #region Particle Effects
    [Header("Particle Effects Played While Grappling")]
    [SerializeField, Tooltip(" An array of particle systems to be played while grappling. All will be played at once.")] private ParticleSystem[] grappleParticles;
    [SerializeField, Tooltip(" The positions of each grappling particle system from the player. A value of 0 is at the player hand.")] private Vector3[] particlePositions;
    [SerializeField, Tooltip(" The rotation  of each grappling particle system. 0 is default particle rotation.")] private Quaternion[] particleRotations;

    private List<GameObject> particleObjs = new List<GameObject>();
    private GameObject grappleParticlesObj;
    private bool particlesStarted;
    #endregion

    #region PrivateVariables
    // The current length of the rope
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

    private bool drawlingLine = false;


    private float dist;

    private float maxStuckTime = 5;
    private float stuckStatusTime;


    private float timeGrappling;
    private bool actualMaxVelocity;
    private float currentMaxVelocity = 0;

    private int currentGrapplesLeft;

    private bool holdingDownGrapple;
    private bool holdingDownStopGrapple;

    Rigidbody playerRB;

    float wheelInput;

    private bool canBatman;

    private bool passedGrapplePoint = false;

    [SerializeField] [Tooltip("The Decal that will appear on the ground while the player is grappling. ")] GameObject groundDecal;
    private GameObject thisDecal;
    private bool displayShadow = false;
    #endregion

    #region StartFunctions
    void Awake()
    {
        SetTypeOfGrapple();
        if (postText)
        {
            postText.SetActive(false);
        }

        SetObject();


        currentSwingSpeed = swingSpeed;

        currentGrapplesLeft = maxGrapples;
        SetGrapplesLeft();

        playerRB = player.GetComponent<Rigidbody>();

        grapplingInstance = RuntimeManager.CreateInstance(grappleActive);
        particlesStarted = false;

        // Set up swinging decal objects
        thisDecal = Instantiate(groundDecal);
        thisDecal.SetActive(false);
        displayShadow = false;
    }

    private void SetTypeOfGrapple()
    {
        switch (typeOfGrapple)
        {
            case GrappleTypes.IncreasingVelocity_Constant_Velocity:
                useConstantVelocity = true;
                useIncreasingVelocity = true;
                playerCanControlVelocity = false;
                break;
            case GrappleTypes.IncreasingVelocity_Non_Constant_Velocity:
                useConstantVelocity = false;
                useIncreasingVelocity = true;
                playerCanControlVelocity = false;
                break;
            case GrappleTypes.JustConstantVelocit:
                useConstantVelocity = true;
                useIncreasingVelocity = false;
                playerCanControlVelocity = false;
                break;
            case GrappleTypes.PlayerControlled_Constant_Velocity:
                useConstantVelocity = true;
                useIncreasingVelocity = false;
                playerCanControlVelocity = true;
                break;
            case GrappleTypes.PlayerControlled_Non_Constant_Velocity:
                useConstantVelocity = false;
                useIncreasingVelocity = false;
                playerCanControlVelocity = true;
                break;
            case GrappleTypes.Just_Non_Constant_Velocity:
                useConstantVelocity = false;
                useIncreasingVelocity = false;
                playerCanControlVelocity = false;
                break;

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
    }


    private void Start()
    {
        CheckBatman();
    }

    private void CheckBatman()
    {
        canBatman = HandleSaving.instance.UnlockedAbility(Ability.AbilityType.Batman);
    }
    #endregion

    #region UpdateFunctions
    void Update()
    {
        GrappleUpdateChanges();
        GrapplingInput();
        CheckForGrapplingThroughWall();

        CheckToSeeIfTriggersHeldDown();

        HoverShadow();
    }

    private void CheckToSeeIfTriggersHeldDown()
    {

        if (Input.GetAxis("Start Grapple") < 1 && holdingDownGrapple)
        {
            holdingDownGrapple = false;
        }

        if (Input.GetAxis("Start Grapple") > -1 && holdingDownStopGrapple)
        {
            holdingDownStopGrapple = false;
        }
    }

    private void GrappleUpdateChanges()
    {
        grapplingInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));

        if (IsGrappling() && joint)
        {
            timeGrappling += Time.deltaTime;
            JointChanges();
            grapplePoint = hitObjectClone.transform.position;

            SwingDirectionChanging();


        }

        else
        {
            if (grappledObj)
            {
                grappledObj = null;
            }
        }
    }

    private void JointChanges()
    {
        joint.damper = springDamp * Mathf.Clamp(ropeLength * 100, 1, Mathf.Infinity);
        joint.spring = joint.damper * 0.5f;

        currentSwingSpeed = swingSpeed * (1 + ropeLength * 0.05f);



        if (joint.maxDistance <= 0)
        {
            joint.maxDistance = 0;
        }
        else
        {
            joint.maxDistance = Vector3.Distance(grapplePoint, player.transform.position);
            joint.maxDistance = distanceFromPoint - grappleSpeed * Time.deltaTime;
        }
    }
    private void SwingDirectionChanging()
    {

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
    }
    private void VelocityChanging()
    {
        if (useConstantVelocity && timeGrappling > 2)
        {
            actualMaxVelocity = true;
        }

        if (useIncreasingVelocity)
        {
            currentMaxVelocity += increaseAmmount * Time.deltaTime;

            currentMaxVelocity = Mathf.Clamp(currentMaxVelocity, 0, maxSwingVelocity);
        }

        SetScroll();

        if (pulling)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(player.GetComponent<Rigidbody>().velocity, maxPullVelocity);
        }

        else if (/*player.GetComponent<Rigidbody>().velocity.magnitude > maxSwingVelocity &&*/ !actualMaxVelocity)
        {
            if (!canApplyForce)
            {
                if (!passedGrapplePoint)
                {
                    player.GetComponent<Rigidbody>().velocity = CustomClampMagnitude(player.GetComponent<Rigidbody>().velocity, currentMaxVelocity, 20);
                }

            }

            else
            {
                //passedGrapplePoint = true;
                player.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(player.GetComponent<Rigidbody>().velocity, currentMaxVelocity);
            }

        }

        else if (actualMaxVelocity)
        {
            player.GetComponent<Rigidbody>().velocity = CustomClampMagnitude(player.GetComponent<Rigidbody>().velocity, currentMaxVelocity, currentMaxVelocity);
        }
    }

    public void ChangeVelocity(float x, float y)
    {
        wheelInput = y;

    }

    private void SetScroll()
    {
        if (playerCanControlVelocity && IsGrappling())
        {
            if (wheelInput > 0)
            {
                currentMaxVelocity += mouseWheelSensitivity;
            }

            if (wheelInput < 0)
            {
                currentMaxVelocity -= mouseWheelSensitivity;

                if (currentMaxVelocity < minStartingVelocity)
                {
                    currentMaxVelocity = minStartingVelocity;
                }
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
        //if ((Input.GetButtonUp("Start Grapple") || !holdingDownGrapple) && IsGrappling())
        //{
        //    canHoldDownToGrapple = true;
        //}

        //if ((Input.GetButton("Start Grapple") || GetGrappleTrigger()) && IsGrappling() && canHoldDownToGrapple == true /*&& !holdingDownGrapple*/)
        //{
        //    StartGrapple("Normal");
        //}

        //else if ((Input.GetButton("Start Grapple") || GetGrappleTrigger()) && !IsGrappling() && !pushPull.IsGrabbing())
        //{
        //    StartGrapple("Normal");
        //}

        //else if ((Input.GetButtonDown("Stop Grapple") || GetStopGrappleTriggerDown()) && IsGrappling())
        //{
        //    StopGrapple();
        //}

        //else if((Input.GetButtonDown("Stop Grapple") || GetStopGrappleTrigger()) && !IsGrappling())
        //{
        //    StartGrapple("Batman");
        //}



    }

    #region Handle Trigger Input

    #endregion

    #endregion

    #region LateUpdateFunctions
    void LateUpdate()
    {
        if (IsGrappling())
        {
            VelocityChanging();
        }
        DrawRope();

        /// <summary>
        /// Draws the line from the grapple gun to the current grapple point.
        /// </summary>
        void DrawRope()
        {
            //If not grappling, don't draw rope
            if (!joint) return;

            if (lr.positionCount == 0 || drawlingLine) return;

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
        if (Time.timeScale == 0)
        {
            return false;
        }

        if (currentGrapplesLeft <= 0 && useMaxGrapples)
        {
            return false;
        }
        RaycastHit hit = new RaycastHit();

        // Cheks if the Normal Raycast returns a RayCastHit with a collider
        if (CheckRayCast().collider != null)
        {
            hit = CheckRayCast();
        }

        // Cheks if the SphereCast returns a RayCastHit with a collider
        else if (CheckSphereCast().collider != null)
        {
            hit = CheckSphereCast();
        }


        if (hit.collider != null)
        {
            dist = Vector3.Distance(cam.position, hit.point);

            if (!(Physics.Raycast(cam.position, cam.forward, dist, whatIsNotGrappleable)) && !pushPull.IsGrabbing())
            {
                if (grappledObj != hit.collider.gameObject)
                {
                    grappleRayHit = hit;
                    return true;
                }

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
        if (player.GetComponent<Rigidbody>().velocity.magnitude >= neededVelocityForAutoAim)
        {
            RaycastHit grappleObject;
            if (Physics.SphereCast(cam.position, sphereRadius, cam.forward, out grappleObject, maxGrappleDistance, whatIsGrappleable, QueryTriggerInteraction.Collide))
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
    /// <summary>
    /// Will make sure player does not get stuck behind wall
    /// </summary>
    private void CheckForGrapplingThroughWall()
    {
        if (joint && player.GetComponent<Rigidbody>().velocity.magnitude < 5)
        {
            stuckStatusTime += Time.deltaTime;
            Vector3 fromPosition = ejectPoint.transform.position;
            Vector3 toPosition = grappleRayHit.transform.position;
            Vector3 direction = toPosition - fromPosition;
            RaycastHit hit;
            float dist = Vector3.Distance(ejectPoint.transform.position, grappleRayHit.transform.position);

            if (Physics.Raycast(ejectPoint.transform.position, direction, out hit, dist, whatIsNotGrappleable))
            {
                if (!hit.collider.isTrigger && stuckStatusTime > maxStuckTime)
                {

                    StopGrapple();
                }
            }

            else if (stuckStatusTime > maxStuckTime)
            {
                StopGrapple();
            }

        }

        else
        {
            stuckStatusTime = 0;
        }

        stuckStatusTime = 0;

    }

    #endregion

    #region Start/Stop Grapple
    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    public void StartGrapple()
    {
        if (CanFindGrappleLocation())
        {
            StartGrapplingSettings();
            CreateGrapplePoint();
            passedGrapplePoint = false;
            EventInstance beginGrappleInstance = RuntimeManager.CreateInstance(grappleStart);
            beginGrappleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            beginGrappleInstance.start();
            grapplingInstance.start();
        }

    }

    public void StartBatManGrapple()
    {
        if (CanFindGrappleLocation() && canBatman)
        {
            StartGrapplingSettings();
            BatmanGrapple();

            EventInstance beginGrappleInstance = RuntimeManager.CreateInstance(grappleStart);
            beginGrappleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            beginGrappleInstance.start();
            grapplingInstance.start();
        }
    }

    private void StartGrapplingSettings()
    {

        if (IsGrappling())
        {
            StopGrapple();
        }

        SetDifferentGrappleTypeSettings();

        canHoldDownToGrapple = false;
        holdingDownGrapple = true;


        StopAllCoroutines();

        anim.ResetTrigger("Dash");
        anim.ResetTrigger("GrappleEnd");
        anim.SetTrigger("GrappleStart");

        // Instantiate grappling particles and play them
        if (grappleParticles != null && !particlesStarted)
        {
            particlesStarted = true;

            for (int i = 0; i < grappleParticles.Length; i++)
            {
                Vector3 particlePos = new Vector3(transform.position.x + particlePositions[i].x, transform.position.y + particlePositions[i].y, transform.position.z + particlePositions[i].z);
                GameObject grappleParticlesObj = Instantiate(grappleParticles[i].gameObject, particlePos, particleRotations[i]);
                particleObjs.Add(grappleParticlesObj);
                grappleParticlesObj.transform.parent = gameObject.transform;

                grappleParticles[i].Play();
            }
        }

        StartCoroutine(DrawLine());
    }
    /// <summary>
    /// Set settings based on what grapple type is chosen
    /// </summary>
    private void SetDifferentGrappleTypeSettings()
    {
        currentGrapplesLeft--;
        SetGrapplesLeft();

        timeGrappling = 0;

        if (useIncreasingVelocity)
        {
            actualMaxVelocity = true;

            currentMaxVelocity = Mathf.Clamp(player.GetComponent<Rigidbody>().velocity.magnitude, minStartingVelocity, maxSwingVelocity);
        }

        else
        {
            currentMaxVelocity = maxSwingVelocity;
        }

        if (useConstantVelocity && !useConstantVelocity)
        {
            actualMaxVelocity = false;
        }
    }

    /// <summary>
    /// Sets how many grapples the player has left
    /// </summary>
    private void SetGrapplesLeft()
    {
        if (grapplesLeftTextBox && useMaxGrapples)
        {
            if (useMaxGrapples)
            {
                grapplesLeftTextBox.text = "Grapples: " + currentGrapplesLeft;
            }

            else
            {
                grapplesLeftTextBox.text = "";
            }
        }
    }

    /// <summary>
    /// Will Pull player to point until they are within a certin distant from the point
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    IEnumerator PullCourtine(RaycastHit hit)
    {
        pulling = true;
        currentGrappledObj = hit.collider.gameObject;
        hit.collider.isTrigger = true;
        yield return new WaitForEndOfFrame();
        float currentTime = 0;

        float startingSpeed = playerRB.velocity.magnitude;

        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
        Vector3 target = hit.point;

        Vector3 dir = (target - this.transform.position).normalized;
        playerRB.velocity = dir * startingSpeed;

        while (Vector3.Distance(currentGrappledObj.transform.position, player.position) > 10 && currentTime < pullLength)
        {
            playerRB.AddForce(dir * pullSpeed * Time.deltaTime, ForceMode.Impulse);
            dir = (target - this.transform.position).normalized;
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(.5f);

        StopGrapple();
    }

    /// <summary>
    /// The Corutine that draws the Grappling Rope
    /// </summary>
    /// <returns></returns>
    IEnumerator DrawLine()
    {
        drawlingLine = true;
        lr.positionCount = 2;
        //Vector3 grappled = grappleRayHit.point;
        Vector3 grappled = grappleRayHit.transform.position;
        dist = Vector3.Distance(ejectPoint.position, grappled);

        float counter = 0;

        lr.SetPosition(0, ejectPoint.position);

        float tempAttachSpeed = startingAttachSpeed;
        while (counter < dist)
        {

            Vector3 point1 = ejectPoint.position;
            Vector3 point2 = grappled;

            lr.SetPosition(0, ejectPoint.position);
            counter += tempAttachSpeed * Time.deltaTime;

            Vector3 pointAlongLine = (counter) * Vector3.Normalize(point2 - point1) + point1;

            lr.SetPosition(1, pointAlongLine);

            tempAttachSpeed += attachSpeedIncrease * Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        drawlingLine = false;
    }


    /// <summary>
    /// The method that will actually create the joint
    /// </summary>
    private void CreateGrapplePoint()
    {

        currentGrappledObj = grappleRayHit.collider.gameObject;

        if (currentGrappledObj.GetComponent<GrapplePoint>() != null)
        {
            GrapplePoint point = currentGrappledObj.GetComponent<GrapplePoint>();

            if (!point.isBreaking())
            {
                point.Break();
            }
        }

        hitObjectClone = Instantiate(hitObject);
        //hitObjectClone.transform.position = grappleRayHit.point;
        hitObjectClone.transform.position = grappleRayHit.transform.position;
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
        joint.minDistance = ropeLength;


        joint.enableCollision = false;

        joint.spring = springValue;
        joint.damper = springDamp;
        joint.massScale = springMass;

        currentGrapplePosition = hitObjectClone.transform.position;



        //Pinwheel
        Pinwheel pinwheel = null;
        if (pinwheel = grappleRayHit.collider.GetComponentInParent<Pinwheel>())
        {
            pinwheel.TriggerRotation(grappleRayHit.collider.transform, cam.forward);
        }
    }

    private void BatmanGrapple()
    {
        StopCoroutine(PullCourtine(grappleRayHit));
        StartCoroutine(PullCourtine(grappleRayHit));
    }

    public void StopGrappleInput()
    {
        if (CanFindGrappleLocation())
        {
            StartBatManGrapple();
        }

        else
        {
            StopGrapple();
        }
    }
    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        if (IsGrappling())
        {
            anim.ResetTrigger("GrappleStart");
            anim.SetTrigger("GrappleEnd");

            if (grappleRayHit.collider != null)
            {
                grappleRayHit.collider.isTrigger = false;
            }


            pulling = false;
            StopCoroutine(PullCourtine(grappleRayHit));
            currentGrappledObj = null;

            if (grappleParticles != null && particlesStarted)
            {
                // Stop all grapple particles playing
                for (int i = 0; i < grappleParticles.Length; i++)
                {
                    grappleParticles[i].Stop();
                    Destroy(particleObjs[i]);
                }
                // Clear the particle object list so it can be reused.
                particleObjs.Clear();

                particlesStarted = false;
            }

            swingLockToggle = false;


            if (hitObjectClone)
            {
                Destroy(hitObjectClone.gameObject);
            }

            lr.positionCount = 0;

            if (joint)
            {
                Destroy(joint);
            }


            StopAllCoroutines();

            EventInstance endGrappleInstance = RuntimeManager.CreateInstance(grappleEnd);
            endGrappleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            endGrappleInstance.start();
            grapplingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

    }

    /// <summary>
    /// Handles the decal and the line renderer that appear under the player while they are grappling.
    /// </summary>
    private void HoverShadow()
    {
        // Set display shadow status if the player is swinging. Keep display shadow on until the player hits the ground again.
        if (IsGrappling())
        {
            displayShadow = true;
        }
        else if (!IsGrappling() && playerMovementReference.GetGrounded())
        {
            displayShadow = false;
        }

        // Do not change the active status of the decal if it is being changed to the same state. (e.g. Don't set it to true if it's already true).
        if (!(thisDecal.activeSelf && displayShadow) || !(!thisDecal.activeSelf && !displayShadow))
        {
            thisDecal.SetActive(displayShadow);
        }

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            MoveDecal(hit);
        }
    }

    /// <summary>
    ///  Places a decal at a given location.
    /// </summary>
    /// <param name="info"></param>
    private void MoveDecal(RaycastHit info)
    {
        thisDecal.transform.position = info.point;
        thisDecal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);

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
        if (joint != null || pulling == true)
        {
            return true;
        }

        else
        {
            return false;
        }
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

    public void ResetGrapples()
    {
        if (!IsGrappling())
        {
            currentGrapplesLeft = maxGrapples;
            SetGrapplesLeft();
        }

    }

    public float SetRopeLength(float value)
    {
        return ropeLength = value;
    }

    #endregion

    #region Helper Functions
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
    #endregion
}
