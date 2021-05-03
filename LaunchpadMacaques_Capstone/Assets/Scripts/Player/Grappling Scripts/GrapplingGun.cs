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
        JustConstantVelocit, Just_Non_Constant_Velocity
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
    [EventRef, SerializeField, Tooltip("Audio clips for VA effort")]
    private string[] effortGrunts;

    public StudioEventEmitter grapplingEmitter;
    private PauseManager pauseManager;


    [SerializeField] LayerMask groundDecalLayer;

    [SerializeField] Camera handCam;

    private Vector3 handStartingPos;

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
    private float distanceFromPoint = 0;

    // The Raycast that will be set to what the player is looking at
    private RaycastHit grappleRayHit;

    private bool swingLockToggle;
    private bool canApplyForce;

    private bool canGrapple = true;

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


    private float dist = 0f;
    private float distRef = 0f;

    private float maxStuckTime = 5;
    private float stuckStatusTime;


    private float timeGrappling;
    private bool actualMaxVelocity;
    private float currentMaxVelocity = 0;


    private bool holdingDownGrapple;
    private bool holdingDownStopGrapple;

    Rigidbody playerRB;

    float wheelInput;

    private bool canBatman;

    private bool batmanInProgress = false;

    private bool passedGrapplePoint = false;

    private RespawnSystem respawnSystem;

    private SwingHelper swingHelper;

    #endregion

    #region Shadow & Downwards Line Renferer
    [Header("Grappling Shadow Settings")]

    [SerializeField] [Tooltip("The Decal that will appear on the ground while the player is grappling. ")] GameObject groundDecal;
    public GameObject thisDecal;
    public bool displayShadow = false;

    [SerializeField, Tooltip("The object with the grappling shadow line renderer. ")] public GameObject grapplingLrObj;
    public LineRenderer grapplingLr;

    public GameObject GroundDecal { get => groundDecal; set => groundDecal = value; }
    public Transform EjectPoint { get => ejectPoint; set => ejectPoint = value; }
    public Quaternion[] ParticleRotations { get => particleRotations; set => particleRotations = value; }
    public string GrappleStart { get => grappleStart; set => grappleStart = value; }
    private GrappleTypes TypeOfGrapple { get => typeOfGrapple; set => typeOfGrapple = value; }
    public Vector3[] ParticlePositions { get => particlePositions; set => particlePositions = value; }
    public LayerMask WhatIsGrappleable { get => whatIsGrappleable; set => whatIsGrappleable = value; }
    public Animator Anim { get => anim; set => anim = value; }
    public ParticleSystem[] GrappleParticles { get => grappleParticles; set => grappleParticles = value; }
    public GameObject HitObject { get => hitObject; set => hitObject = value; }
    public GameObject PostText { get => postText; set => postText = value; }
    public string GrappleEnd { get => grappleEnd; set => grappleEnd = value; }
    public LayerMask WhatIsGrappleable1 { get => whatIsGrappleable; set => whatIsGrappleable = value; }
    public LayerMask GroundDecalLayer { get => groundDecalLayer; set => groundDecalLayer = value; }
    public Transform Player { get => player; set => player = value; }
    public GameObject GrapplingLrObj { get => grapplingLrObj; set => grapplingLrObj = value; }
    public bool HoldingDownGrapple { get => holdingDownGrapple; set => holdingDownGrapple = value; }
    public bool CanHoldDownToGrapple { get => canHoldDownToGrapple; set => canHoldDownToGrapple = value; }
    public float ExplosionPower { get => explosionPower; set => explosionPower = value; }
    public float MinDistanceFromObject { get => minDistanceFromObject; set => minDistanceFromObject = value; }
    public float ExplosionRadius { get => explosionRadius; set => explosionRadius = value; }
    #endregion

    [Header("Grapple Lock Variables")]
    #region Grapple Lock Variables
    [SerializeField, Tooltip("The distance object reference for grappling lock. ")] private GameObject grappleLockDistanceReference = null;
    [SerializeField, Tooltip("The distance away from a grappling point the player will remain locked to it. ")] private float grappleLockDistance = 10f;
    private GameObject currentGrappleLockedObject = null;
    private bool grappleLocked = false;
    private RaycastHit grappleLockRaycastHitRef;
    #endregion

    #region StartFunctions
    void Awake()
    {

        swingHelper = FindObjectOfType<SwingHelper>();
        SetTypeOfGrapple();
        if (postText)
        {
            postText.SetActive(false);
        }

        SetObject();

        respawnSystem = player.GetComponent<RespawnSystem>();

        currentSwingSpeed = swingSpeed;


        playerRB = player.GetComponent<Rigidbody>();

        particlesStarted = false;

        // Set up swinging decal objects
        thisDecal = Instantiate(groundDecal);
        thisDecal.SetActive(false);
        displayShadow = false;

        grapplingLr = grapplingLrObj.GetComponent<LineRenderer>();
        grapplingLr.enabled = false;
    }

   

    private void SetTypeOfGrapple()
    {
        switch (typeOfGrapple)
        {
            case GrappleTypes.JustConstantVelocit:
                useConstantVelocity = true;
                break;
            case GrappleTypes.Just_Non_Constant_Velocity:
                useConstantVelocity = false;
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

        handStartingPos = ejectPoint.transform.position;

        pauseManager = FindObjectOfType<PauseManager>();
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
        CheckForGrapplingThroughWall();
        UpdateGrappleLockPos(currentGrappleLockedObject, distRef);

        if (PlayerPrefs.GetInt("HoverLine") == 1)
        {
            HoverShadow();
        }

        //Debug.Log(Camera.main.ViewportToWorldPoint(this.transform.position));

        handStartingPos = ejectPoint.transform.position;

        if (pauseManager.GetPaused() || !IsGrappling()) grapplingEmitter.Stop();
        else if (!grapplingEmitter.IsPlaying()) grapplingEmitter.Play();
    }
    private void GrappleUpdateChanges()
    {

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

        if (pulling)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(player.GetComponent<Rigidbody>().velocity, maxPullVelocity);
        }

        else if (!actualMaxVelocity)
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

    #endregion

    #region UserInput



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



            var temp = handCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(hitObjectClone.transform.position));
            //var temp = Vector3.zero;
            currentGrapplePosition = temp;

            lr.SetPosition(0, ejectPoint.position);
            lr.SetPosition(1, currentGrapplePosition);
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

        // If the hit.collider is not null
        if (hit.collider != null)
        {
            // Find the distance between the camera and the point that was hit.
            dist = Vector3.Distance(cam.position, hit.point);

            // Check to make sure that there are no walls between the player and the grappling point. 
            if (!(Physics.Raycast(cam.position, cam.forward, dist, whatIsNotGrappleable)) && !pushPull.IsGrabbing())
            {
                // If the player is not aiming at the object they are currently grappled to
                if (grappledObj != hit.collider.gameObject)
                {
                    // If the current grapple locked object is different from the grappling point aimed at, update the current grapple locked object
                    if (currentGrappleLockedObject != hit.collider.gameObject)
                    {
                        currentGrappleLockedObject = hit.collider.gameObject;
                        grappleLocked = false;
                    }

                    // If the grapple locked status is false, set the grapple locked distance and raycast hit reference
                    if (!grappleLocked)
                    {
                        grappleLocked = true;
                        distRef = dist;
                        grappleLockRaycastHitRef = hit;
                    }
                }
            }
        }

        // If grapple locked, set the grappleRayHit (which determines which object will be grappled to) to the proper object
        if (grappleLocked)
        {
            grappleRayHit = grappleLockRaycastHitRef;
            return true;
        }
        else
        {
            currentGrappleLockedObject = null;
        }

        return false;
    }

    /// <summary>
    /// Updates the grappling lock position.
    /// </summary>
    /// <param name="grappledPoint">The Game Object that the lock position should be set to. </param>
    /// <param name="distance">The distance away the grappling point will remain locked at. </param>
    private void UpdateGrappleLockPos(GameObject grappledPoint, float distance)
    {
        // Check if the grapple should actually lock
        if (grappleLocked)
        {
            // Sets the grapple lock distance reference object to the position the grappling point was detected at. 
            Ray ray = new Ray(cam.position, cam.forward);
            grappleLockDistanceReference.transform.position = ray.GetPoint(distance);

            // If the current grapple locked object isn't null
            if (currentGrappleLockedObject != null)
            {
                // Check the distance between the grapple lock object and the locked grapple point.
                if (Vector3.Distance(grappleLockDistanceReference.transform.position, currentGrappleLockedObject.transform.position) > grappleLockDistance)
                {
                    // If the distance is greater than the minimum grapple lock distance, set the grapple lock to false. 
                    grappleLocked = false;
                }
            }
        }
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
        if (CanFindGrappleLocation() && !batmanInProgress && !pulling && canGrapple)
        {
            swingHelper.ResetVariables();
            StartGrapplingSettings();
            CreateGrapplePoint();
            playerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            passedGrapplePoint = false;
            EventInstance beginGrappleInstance = RuntimeManager.CreateInstance(grappleStart);
            beginGrappleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            beginGrappleInstance.start();
            beginGrappleInstance.release();
            PlayRandom(effortGrunts);
            playerMovementReference.HitAngle(true);
            playerMovementReference.TurnOffReset();
        }

    }

    public void StartBatManGrapple()
    {
        if (CanFindGrappleLocation() && canBatman && !batmanInProgress && !pulling && canGrapple)
        {
            batmanInProgress = true;
            StartGrapplingSettings();
            BatmanGrapple();

            playerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            EventInstance beginGrappleInstance = RuntimeManager.CreateInstance(grappleStart);
            beginGrappleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            beginGrappleInstance.start();
            beginGrappleInstance.release();
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


        timeGrappling = 0;



        currentMaxVelocity = maxSwingVelocity;


        if (useConstantVelocity && !useConstantVelocity)
        {
            actualMaxVelocity = false;
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
        hit.collider.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        yield return new WaitForEndOfFrame();
        float currentTime = 0;

        float startingSpeed = playerRB.velocity.magnitude;

        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
        Vector3 target = hit.point;

        Vector3 dir = (target - this.transform.position).normalized;
        playerRB.velocity = dir * startingSpeed;

        while (currentTime < pullLength)
        {
            playerRB.AddForce(dir * pullSpeed * Time.deltaTime, ForceMode.Impulse);
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
      //  Vector3 grappled = grappleRayHit.point;

      var temp = handCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(currentGrapplePosition));
        Vector3 grappled = temp;
        dist = Vector3.Distance(ejectPoint.position, grappled);

        float counter = 0;

        lr.SetPosition(0, ejectPoint.position);

        float tempAttachSpeed = startingAttachSpeed;
        while (counter < dist)
        {

            temp = handCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(currentGrapplePosition));
            Vector3 point1 = ejectPoint.position;
            Vector3 point2 = temp;


            lr.SetPosition(0, ejectPoint.position);
            counter += tempAttachSpeed * Time.deltaTime;

            Vector3 pointAlongLine = (counter) * Vector3.Normalize(point2 - point1) + point1;

            var temp2 = handCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pointAlongLine));

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

            respawnSystem.SetCurrentGrapplePoint(point);

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


        var temp =  Camera.main.ViewportToWorldPoint(handCam.WorldToViewportPoint(hitObjectClone.transform.position));
        //var temp = Vector3.zero;
        currentGrapplePosition = temp;
        //currentGrapplePosition = hitObjectClone.transform.position;

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
        StopGrapple();
    }

    public void BatmanGrappleInput()
    {
        if (CanFindGrappleLocation())
        {
            StartBatManGrapple();
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
                grappleRayHit.collider.gameObject.GetComponent<BoxCollider>().isTrigger = false;
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
            endGrappleInstance.release();

            if (batmanInProgress)
            {
                StartCoroutine(BatmanInputDelay(0.25f));
            }
        }
    }

    /// <summary>
    /// Sets batmanInProgress to be false after a period of time, to prevent player from cancelling batman with
    /// regular swinging input. 
    /// </summary>
    /// <param name="delay">The time before batmanInProgress should be set to false.</param>
    /// <returns></returns>
    private IEnumerator BatmanInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        batmanInProgress = false;
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

        // Do not change the active status of the decal and line renderer if it is being changed to the same state. (e.g. Don't set it to true if it's already true).
        if (!(thisDecal.activeSelf && displayShadow) || !(!thisDecal.activeSelf && !displayShadow))
        {
            thisDecal.SetActive(displayShadow);
            grapplingLr.enabled = displayShadow;
        }

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity, groundDecalLayer))
        {
            MoveDecal(hit);
            MoveGrapplingShadowLineRenderer(grapplingLr, hit, grapplingLrObj.transform.position);
        }
    }

    /// <summary>
    /// Moves the passed in line renderer to the point a raycast hits from the passed in position.
    /// </summary>
    /// <param name="lineRenderer">The line renderer to move. </param>
    /// <param name="info">The RaycastHit of the raycast to move the line renderer to. </param>
    /// <param name="shootPos">The position to shoot the line renderer from. </param>
    private void MoveGrapplingShadowLineRenderer(LineRenderer lineRenderer, RaycastHit info, Vector3 shootPos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, shootPos);
        lineRenderer.SetPosition(1, info.point);
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


    public float SetRopeLength(float value)
    {
        return ropeLength = value;
    }

    public bool GetCanGrapple()
    {
        return canGrapple;
    }

    public void SetCanGrapple(bool newCanGrapple)
    {
        canGrapple = newCanGrapple;
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
    /// <summary>
    /// Plays a random FMOD event from an array.
    /// </summary>
    /// <param name="vs"></param>
    public void PlayRandom(string[] vs)
    {
        string randEvent = vs[Random.Range(0, vs.Length)];
        EventInstance randInstance = RuntimeManager.CreateInstance(randEvent);
        randInstance.start();
        randInstance.release();
    }
    #endregion

    #region Unity Callbacks
    #endregion
}
