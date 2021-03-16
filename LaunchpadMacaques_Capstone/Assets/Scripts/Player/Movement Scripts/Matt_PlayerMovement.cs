/*
* Launchpad Macaques - Neon Oblivion
* Matt Kirchoff, Levi Schoof, William Nomikos, Jamey Colleen
* Matt_PlayerMovement.cs
* Script handles player movement, player gravity, and dashing.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Matt_PlayerMovement : MonoBehaviour
{
    #region References
    [SerializeField, Tooltip("The text element that displays the current gravity. ")] TextMeshProUGUI currentGravityText;
    private GrapplingGun grappleGunReference;
    private CollectibleController collectibleController;
    private NarrativeTriggerHandler narrativeTriggerReference;
    #endregion

    #region Player Camera Variables
    [Header("Player Transform Assignables")]
    [SerializeField, Tooltip("The transform of the player's camera. ")] private Transform playerCam;
    [SerializeField, Tooltip("The transform of the player's orientation ")] private Transform orientation;
    float m_fieldOfView = 60.0f;
    #endregion

    //Other
    private Rigidbody rb;


    #region Player Sensitivity
    [Header("Player Rotation and Look")]
    private float xRotation;
    [Tooltip("The player's look sensitivity. Higher value lets the player look around quicker. ")] private float sensitivity = 50f;
    [SerializeField] float mouseAccerlationAmmount = .5f;
    [SerializeField] float normalSpeedAmmount = 1;
    private float lastFrameMouseInput;
    private bool inMouseAccerlation = false;
    #endregion

    #region Player Movement Variables
    [Header("Player Movement Variables")]
    private float moveSpeed = 4500;
    [Range(0f, 1f), SerializeField, Tooltip("The movement speed multiplier while the player is airborn. ")] private float airMoveSpeedMultiplier = .75f;
    [SerializeField, Tooltip("The player's max speed. when walking, doesnt affect swing speed ")] private float maxSpeed = 20;

    private bool killForce = false;

    #endregion

    [HideInInspector] public bool grounded;
    [SerializeField, Tooltip("The layer for the ground. Anything on this layer will be considered ground. ")] private LayerMask whatIsGround;
    [SerializeField, Tooltip("The physics material that platforms should obtain if they are collided with from the side")] private PhysicMaterial frictionlessMat;
    private PhysicMaterial originalMaterial;
    private bool applyPhysicsMaterial;

    [SerializeField, Tooltip("This is the max speed that the player can achieve when swinging. ")]
    private float maxVelocity = 50f;

    #region Movement Stabilization Variables
    [Header("Counter Movement")]
    [Tooltip("This is the variable that affects the speed of counter movement applied to the player, which slows the player down to a stop")] public float counterMovement = 0.175f;
    [SerializeField] private float threshold = 0.01f;
    [SerializeField, Tooltip("The angle that the player starts to be unable to walk up ")] private float maxSlopeAngle = 35f;
    #endregion

    #region Movement FOV Variables
    [Header("Movement FOV Variables")]
    [SerializeField] private float xFOVActivationVel = 40f;
    [SerializeField] private float zFOVActivationVel = 40f;
    [SerializeField] private float yFOVActicationVel = 15f;
    [SerializeField] private float fovChangeRate = 0.75f;
    [SerializeField] private float maxFOV = 120.75f;
    [SerializeField] private float maxFOVSpeedScale = .05f;
    #endregion

    #region Grappling Velocity Reset Variables
    [Header("Velocity Reset Variables")]
    [SerializeField, Tooltip("The X velocity range (between -x and x) that is checked to determine if Velocity and Rope Length need to be reset. ")] private float xVelocityResetRange = 1;
    [SerializeField, Tooltip("The Y velocity range (between -y and y) that is checked to determine if Velocity and Rope Length need to be reset. ")] private float yVelocityResetRange = 1;
    [SerializeField, Tooltip("The Z velocity range (between -z and z) that is checked to determine if Velocity and Rope Length need to be reset. ")] private float zVelocityResetRange = 1;
    [SerializeField, Tooltip("The time the player must have low velocity for in order to have Velocity and Rope Length reset. ")] private float lowVelocityDuration = 0.01f;
    #endregion

    #region Crouch and Slide Variables
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    private float slideForce = 400;
    private float slideCounterMovement = 0.2f;
    #endregion

    #region Sliding Variables
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    #endregion

    #region Jumping Variables
    [Header("Jumping")]
    [SerializeField]
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    private bool readyToJump = true;
    #endregion

    #region Gravity Variables
    [Header("Gravity Settings")]
    [SerializeField, Tooltip("The Gravity that will be applied to the player when they are on the ground")] float gravity = -9.81f;
    [SerializeField, Tooltip("The Gravtiy that will be applied to the player when they are in the air")] float inAirGravity = -12f;
    [SerializeField, Tooltip("The Gravity that will be applied to the player when they are swinging")] float grapplingGravity = -6;
    [SerializeField, Tooltip("The Gravity that will be applied when resetting rope velocity and rope length. The greater this value, " +
        "the faster velocity and rope length are reset. ")]
    float grapplingResetGravity = -78.48f;

    // grapplingGravityReference stores the grapplingGravity at Start, so that grapplingGravity may be reverted to its default easily.
    private float grapplingGravityReference = 0;
    private float defaultGravity;
    private Vector3 gravityVector;
    #endregion

    #region Sprinting Variables
    [Header("Sprinting")]
    [SerializeField] private float sprintMultiplier = 1.75f;
    private bool readyToSprint = true;
    private float speedStorage;
    [SerializeField] ParticleSystem sprintParticles;
    #endregion

    #region Dash Settings
    [Header("Dash Settings")]

    [SerializeField, Tooltip("The dash ammount that will be applied to the player , when using the CourtineDash" +
        "Will be scaled with time.delta time and will be applied for a set ammount of time")]
    private float courtineDashAmmount = 100;
    [SerializeField, Tooltip("How long the courtineDash will apply force to the player")] [Range(0, 1)] private float dashLength = .5f;
    [SerializeField, Tooltip("The cooldown before the player can dash again in seconds (While the player is still in the air). ")] private float dashCooldown = 1.5f;

    [Tooltip("The dash ammount that will be applied to the player, when using the addForceDah" +
    "This ammount is only applied to the player for one frame")]
    private float impulseDashAmmount = 4000;
    private bool useAddForceDash = false;
    private bool useCourtineDash = true;

    [SerializeField] GameObject dashUI;
    #endregion

    #region Platform Landing Settings
    [Header("Platform Landing Settings")]
    [SerializeField, Tooltip("The tags that should be checked to cancel out player velocity when landing.")] private string[] tagsToCancelVelocity;
    private bool notLandedAfterAirTime = false;

    [SerializeField, Tooltip("If enabled, player will be able to walk off of platforms for a short period of time without falling. ")] private bool enableCoyoteTime = false;
    [SerializeField, Tooltip("The duration that Coyote Time should last.")] private float coyoteTimeDuration = 1f;

    // The game object currently on.
    private GameObject gameObjectStoodOn;
    [SerializeField, Tooltip("The Coyote Time game objects. ")] private List<GameObject> coyoteTimeObjs = new List<GameObject>(4);
    [SerializeField] List<string> coyoteTimeTags;

    // Determines if the Coyote Time coroutine is running.
    private bool coyoteTimeStarted = false;

    // Determines if the Coyote Time objects are active.
    private bool coyoteTimeEnabled = false;
    #endregion


    [Header("Screen Shake Settings")]
    [SerializeField] float minVelocityForScreenShake = 30;
    [SerializeField] float startingScreenShakeAmmount = 20;
    [SerializeField] float shakeAmmountVelocityScaling = .1f;
    [SerializeField] float maxScreenShakeAmmount = 50;
    [SerializeField] float screenShakeLength = .1f;



    [Header("Art Settings")]
    [SerializeField]
    Animator anim;

    private float minFOV = 60;

    #region Getters/Setters
    public float GetMaxVelocity()
    {
        return maxVelocity;
    }

    public bool GetKillForce()
    {
        return killForce;
    }

    #endregion


    [Header("Player Input")]

    private float x, y;
    private bool jumping = false, sprinting = false, crouching = false, canDash = false;

    private PauseManager pauseManager;

    ConfigJoint config;

    private Vector3 latestOrientation;

    private float deafultVelocity;

    private float currentMaxFOV;

    private float lastVelocity = 0;

    private int lastMaxFOV;

    private float mouseX;
    private float mouseY;

    private MoveCamera cam;

    private bool resetVelocity = false;

    private float timeOffGround;

    private bool dashUnlocked = false;

    private bool canMove = true;

    [Header("Edge Detection")]
    [SerializeField] private float distanceXZ = 5;
    [SerializeField] private float distanceToCheckDown = 5;
    private float stepIncreaseAmmount = .1f;

    [Header("Vignette Settings")]
    [SerializeField] private Color vigneteColor;
    [SerializeField] float startingVigneteIntensity = .4f;
    [SerializeField] float maxVigneteIntensity = .6f;
    [SerializeField] float vigneteIntensityScaleAmmount = .1f;

    SetPostProcessing postProcessing;

    Vector3 previousPlayerPositon;

    bool foundGround = true;

    ParticleSystem system
    {
        get
        {
            if (_CachedSystem == null)
                _CachedSystem = FindObjectOfType<ParticleSystem>();
            return _CachedSystem;
        }
    }

    [SerializeField, Tooltip("Particle system that is used while dashing.")] private ParticleSystem _CachedSystem;
    [SerializeField, Tooltip("Amount of speedlines emitted after each dash. ")] private int emitParticles = 20;

    void Awake()
    {
        defaultGravity = gravity;
        deafultVelocity = maxVelocity;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        pauseManager = FindObjectOfType<PauseManager>();
        collectibleController = FindObjectOfType<CollectibleController>();
        grappleGunReference = FindObjectOfType<GrapplingGun>();
        narrativeTriggerReference = FindObjectOfType<NarrativeTriggerHandler>();

        config = FindObjectOfType<ConfigJoint>();

        //Cannot dash while on the ground.
        canDash = false;
        DashFeedback(false);

        applyPhysicsMaterial = false;

        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("FovValue"))
        {
            maxFOV += PlayerPrefs.GetInt("FovValue") - m_fieldOfView;
            m_fieldOfView = PlayerPrefs.GetInt("FovValue");
            minFOV = PlayerPrefs.GetInt("FovValue");
        }


        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speedStorage = maxSpeed;
        grapplingGravityReference = grapplingGravity;
        cam = FindObjectOfType<MoveCamera>();
        // Makes xVelocityResetRange, yVelocityResetRange, and zVelocityResetRange positive if they are negative.
        if (xVelocityResetRange < 0)
        {
            xVelocityResetRange *= -1;
        }
        if (yVelocityResetRange < 0)
        {
            yVelocityResetRange *= -1;
        }
        if (zVelocityResetRange < 0)
        {
            zVelocityResetRange *= -1;
        }

        currentMaxFOV = maxFOV;

        dashUnlocked = HandleSaving.instance.UnlockedAbility(Ability.AbilityType.Dash);

        postProcessing = FindObjectOfType<SetPostProcessing>();

    }

    private void FixedUpdate()
    {
        Movement();
        LimitVelocity();
        SetGravityModifier();
        CheckForCoyoteObjects();
    }

    private void Update()
    {
        if ((!pauseManager.GetPaused() && !pauseManager.GetGameWon()) && Time.timeScale > 0)
        {
            if (canMove)
            {
                Look();
                grappleGunReference.UpdateHandRotation(rb.velocity);
            }

        }
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }


        if ((rb.velocity.x < xVelocityResetRange && rb.velocity.x > -xVelocityResetRange) &&
            (rb.velocity.y < yVelocityResetRange && rb.velocity.y > -yVelocityResetRange) &&
            (rb.velocity.z < zVelocityResetRange && rb.velocity.z > -zVelocityResetRange) && !killForce)
        {
            if (grappleGunReference.IsGrappling())
            {
                StartCoroutine(KillForces());
            }
        }

        changeFOV();

        //Particles with speed
        float speed = rb.velocity.magnitude;
        var ps = _CachedSystem.main;
        var rot = _CachedSystem.emission.rateOverTime;
        ps.startSpeed = speed * 2;
        rot = speed;

        //Set Particles to change direction with the rigidbody
        var localVel = transform.InverseTransformDirection(rb.velocity);
        var psRotation = _CachedSystem.shape.rotation;
        psRotation = localVel;

        if (!grounded)
        {
            timeOffGround += Time.deltaTime;

            if (!notLandedAfterAirTime && grappleGunReference.IsGrappling())
            {
                notLandedAfterAirTime = true;
            }
        }

        EdgeDetection();

        SprintFeedBack();
    }

    private void SprintFeedBack()
    {
        if (!readyToSprint && grounded)
        {
            if ((x != 0 || y != 0) && !sprintParticles.isPlaying)
            {
                sprintParticles.Play();
            }

            else if (x == 0 && y == 0)
            {
                sprintParticles.Stop();
            }
        }

        else if (sprintParticles.isPlaying)
        {
            sprintParticles.Stop();
        }


    }

    private void EdgeDetection()
    {
        if (grounded)
        {
            if ((this.x == 0 && this.y == 0))
            {
                return;
            }
            if (Physics.Raycast(this.transform.position, (-transform.up), out RaycastHit hit,distanceToCheckDown, whatIsGround))
            {
                if (!coyoteTimeTags.Contains(hit.collider.gameObject.tag) || coyoteTimeObjs.Contains(hit.collider.gameObject))
                {
                    //postProcessing.SetVignete(false, vigneteColor);
                    return;
                }
                float x = 0;
                foundGround = true;
                Vector3 checkPos = this.transform.position;
                for (x = 0; x < distanceXZ; x += stepIncreaseAmmount)
                {
                    checkPos = this.transform.position;
                    checkPos.x += x;

                    if (EdgeDectionRayCast(checkPos))
                    {
                        foundGround = false;
                        break;
                    }

                    checkPos = this.transform.position;
                    checkPos.x -= x;

                    if (EdgeDectionRayCast(checkPos))
                    {
                        foundGround = false;
                        break;
                    }


                    checkPos = this.transform.position;
                    checkPos.z += x;
                    if (EdgeDectionRayCast(checkPos))
                    {
                        foundGround = false;
                        break;
                    }

                    checkPos = this.transform.position;
                    checkPos.z -= x;
                    if (EdgeDectionRayCast(checkPos))
                    {
                        foundGround = false;
                        break;
                    }


                }

                if (!foundGround)
                {

                    postProcessing.SetVignete(true, vigneteColor, Mathf.Clamp(((distanceXZ - x) * vigneteIntensityScaleAmmount) 
                        + startingVigneteIntensity, 0, 1));
                }

                else
                {
                    postProcessing.SetVignete(false, vigneteColor);
                }
            }

            else
            {
                postProcessing.SetVignete(false, vigneteColor);
            }
        }

        else
        {
            postProcessing.SetVignete(false, vigneteColor);
        }
    }

    private bool EdgeDectionRayCast(Vector3 checkPos)
    {

        if (!Physics.Raycast(checkPos, (-transform.up), out RaycastHit hit, distanceToCheckDown, whatIsGround))
        {
            return true;
        }

        else
        {
            if (coyoteTimeObjs.Contains(hit.collider.gameObject))
            {
                return true;
            }
            return false;
        }
    }



    #region Dash Stuff

    /// <summary>
    /// The method that is called to start the player dash
    /// </summary>
    public void Dash()
    {

        if (!grounded && dashUnlocked)
        {
            if (grappleGunReference.IsGrappling())
            {
                grappleGunReference.StopGrapple();
            }

            if (canDash)
            {
                anim.SetTrigger("Dash");

                DashFeedback(false);
                canDash = false;
                StartCoroutine(DashCooldown());

                if (useAddForceDash)
                {
                    _CachedSystem.Emit(emitParticles);
                    AddForceDash();
                }
                else if (useCourtineDash)
                {
                    StartCoroutine(DashCourtine());
                }

                else
                {
                    ChangeDirectionDash();
                }
            }
        }

    }


    private void DashFeedback(bool onOff)
    {
        dashUI.SetActive(onOff);
    }
    /// <summary>
    /// The dash that will only change the player's direction does not change their speed
    /// </summary>
    private void ChangeDirectionDash()
    {
        //dashing = true;
        _CachedSystem.Emit(emitParticles);
        float currentMag = rb.velocity.magnitude;
        rb.velocity += playerCam.forward * currentMag;
        rb.velocity = grappleGunReference.CustomClampMagnitude(rb.velocity, currentMag, currentMag);
        //dashing = false;
    }

    /// <summary>
    /// Will apply a force dash for one frame (Causes player to teleport)
    /// </summary>
    private void AddForceDash()
    {
        //dashing = true;
        _CachedSystem.Emit(emitParticles);
        GetComponent<Rigidbody>().AddForce((playerCam.forward) * impulseDashAmmount, ForceMode.Impulse);
        //dashing = false;
    }

    /// <summary>
    /// Will apply a dash force over time of dash length
    /// </summary>
    /// <returns></returns>
    IEnumerator DashCourtine()
    {
        float currentTime = 0;
        //dashing = true;
        _CachedSystem.Emit(emitParticles);

        yield return new WaitForEndOfFrame();

        float startingSpeed = rb.velocity.magnitude;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.velocity = playerCam.forward * startingSpeed;

        while (currentTime < dashLength)
        {
            if (grounded || grappleGunReference.IsGrappling())
            {
                break;
            }
            GetComponent<Rigidbody>().AddForce((playerCam.forward) * courtineDashAmmount * Time.deltaTime, ForceMode.Impulse);
            currentTime += Time.deltaTime;
            //dashing = false;
            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator DashCooldown()
    {
        //Test if cooldown started
        Debug.Log("Start Cooldown");

        //Set Max CD
        float timeLeft = dashCooldown;

        //CD Timer
        float totalTime = 0;

        while (totalTime <= timeLeft)
        {
            totalTime += Time.deltaTime;
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Velocity and Momentum Reset
    /// <summary>
    /// Coroutine that stops forces from being applied to the player and resets the rope length. (Needs better description, i think)
    /// </summary>
    /// <returns></returns>
    IEnumerator KillForces()
    {
        killForce = true;

        if (lowVelocityDuration != 0)
        {
            yield return new WaitForSeconds(lowVelocityDuration);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        if ((rb.velocity.x < xVelocityResetRange && rb.velocity.x > -xVelocityResetRange) &&
           (rb.velocity.y < yVelocityResetRange && rb.velocity.y > -yVelocityResetRange) &&
           (rb.velocity.z < zVelocityResetRange && rb.velocity.z > -zVelocityResetRange))
        {
            grapplingGravity = grapplingResetGravity;
            //grappleGunReference.SetRopeLength(grappleGunReference.GetStartingRopeLength());

            rb.velocity = -rb.velocity / 2;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            yield return new WaitForSecondsRealtime(1.5f);
            grapplingGravity = grapplingGravityReference;
        }

        killForce = false;
    }

    #endregion

    #region Input
    public void SetLook(float x, float y)
    {
        mouseX = x;
        mouseY = y;
    }
    public void CrouchInput()
    {
        //if (!grappleGunReference.IsGrappling())
        //{
        //    crouching = !crouching;

        //    if (crouching)
        //    {
        //        StartCrouch();
        //    }

        //    else
        //    {
        //        StopCrouch();
        //    }
        //}

    }

    public LayerMask GetGround()
    {
        return whatIsGround;
    }

    public void OnMoveInput(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void OnJumpInput()
    {
        jumping = !jumping;
    }

    /// <summary>
    /// Handles the player sprinting.
    /// </summary>
    public void StartSprint()
    {
        if (/*grounded && */readyToSprint)
        {
            readyToSprint = false;
            //Apply sprint to player
            maxSpeed = speedStorage * sprintMultiplier;
        }
    }

    /// <summary>
    /// Stops the player from sprinting.
    /// </summary>
    public void StopSprint()
    {
        maxSpeed = speedStorage;
        readyToSprint = true;
    }

    #endregion

    #region Crouching Stuff

    /// <summary>
    /// Starts player crouching.
    /// </summary>
    public void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    /// <summary>
    /// Stops the player from crouching.
    /// </summary>
    public void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    /// <summary>
    /// Returns true if the player is crouching.
    /// </summary>
    /// <returns></returns>
    public bool GetCrouchStatus()
    {
        return crouching;
    }

    #endregion

    /// <summary>
    /// This function limits the velocity of the player so they can't just increase speed into oblivion.
    /// </summary>
    private void LimitVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity && !grappleGunReference.IsGrappling())
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }

    #region Movement

    /// <summary>
    /// Handles the player gravity when the player is moving normally and grappling.
    /// </summary>
    private void SetGravityModifier()
    {
        if (collectibleController.GetIsActive())
        {
            gravity = collectibleController.GetNewPlayerGravity();
        }
        else if (collectibleController.GetIsActive() == false)
        {
            gravity = defaultGravity;
        }

        if ((!grappleGunReference.IsGrappling() && !grounded) && !collectibleController.GetIsActive()) // If in the air // (gameObject.transform.position.y > 20)
        {
            gravityVector = new Vector3(0, inAirGravity, 0);
        }
        else if ((grappleGunReference.IsGrappling()) && !collectibleController.GetIsActive())
        {
            gravityVector = new Vector3(0, grapplingGravity, 0);
        }

        else
        {
            gravityVector = new Vector3(0, gravity, 0);
        }

        // rb.AddForce(gravityVector * Time.fixedDeltaTime, ForceMode.Acceleration);

        rb.velocity += gravityVector * Time.fixedDeltaTime;

        if (currentGravityText)
        {
            currentGravityText.text = "Gravity In M/S: " + gravityVector.y;
        }
    }

    /// <summary>
    /// Handles player movement.
    /// </summary>
    private void Movement()
    {
        if (canMove)
        {
            // Find actual velocity relative to where player is looking
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            // Counteract sliding and sloppy movement
            CounterMovement(x, y, mag);

            // If holding jump && ready to jump, then jump
            if (readyToJump && jumping) Jump();

            // If holding sprint && ready to sprint, then sprint
            if (readyToSprint && sprinting) StartSprint();

            // Set max speed
            float maxSpeed = this.maxSpeed;

            // If sliding down a ramp, add force down so player stays grounded and also builds speed
            if (crouching && grounded && readyToJump)
            {
                rb.AddForce(Vector3.down * Time.deltaTime * 3000);
                return;
            }

            float tempX = x;
            float tempY = y;
            // If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (tempX > 0 && xMag > maxSpeed) tempX = 0;
            if (tempX < 0 && xMag < -maxSpeed) tempX = 0;
            if (tempY > 0 && yMag > maxSpeed) tempY = 0;
            if (tempY < 0 && yMag < -maxSpeed) tempY = 0;

            //// If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            //if (x > 0 && xMag > maxSpeed) x = 0;
            //if (x < 0 && xMag < -maxSpeed) x = 0;
            //if (y > 0 && yMag > maxSpeed) y = 0;
            //if (y < 0 && yMag < -maxSpeed) y = 0;

            // Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Movement in air
            if (!grounded)
            {
                multiplier = airMoveSpeedMultiplier;
                multiplierV = airMoveSpeedMultiplier;

                //PlayerHitGround Event for Dialogue/Narrative Trigger System
                if (fallCheckRunning == false)
                    StartCoroutine(FallCheck());
            }

            // Movement while sliding
            if (grounded && crouching) multiplierV = 0f;

            // If the player is grounded, they cannot dash.
            if (grounded)
            {
                canDash = false;
                DashFeedback(false);
            }
            // Dash cooldown is reset if the player grapples again.
            else if (grappleGunReference.IsGrappling() && !canDash)
            {
                DashFeedback(true);
                canDash = true;
            }

            // If the player is not grappling, add a force in the direction they are moving in.
            if (!grappleGunReference.IsGrappling())
            {
                rb.AddForce(orientation.transform.forward * tempY * moveSpeed * Time.deltaTime * multiplier * multiplierV);
                rb.AddForce(orientation.transform.right * tempX * moveSpeed * Time.deltaTime * multiplier);


            }
            // If Swing Lock is not active, and the player is grappling, add a force in the player's orientation
            else if (!grappleGunReference.GetSwingLockToggle() && grappleGunReference.IsGrappling())
            {
                // If the force can be applied, add a force in the direction of the player's orientation.
                if (grappleGunReference.GetCanApplyForce())
                {
                    rb.AddForce(orientation.transform.forward * grappleGunReference.GetSwingSpeed() * 2 * Time.deltaTime);
                    latestOrientation = orientation.transform.forward;
                }
            }
            // If the swing lock is enabled and the player is grappling, apply force to the player in the most recent orientation they were facing.
            else if (grappleGunReference.GetSwingLockToggle() && grappleGunReference.IsGrappling())
            {
                if (grappleGunReference.GetCanApplyForce())
                {
                    if (latestOrientation != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddForce(latestOrientation * grappleGunReference.GetSwingSpeed() * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    #region Coyote Time Methods
    /// <summary>
    /// Determines whether or not the Coyote Time objects should be enabled. 
    /// </summary>
    private void CheckForCoyoteObjects()
    {
        // If the player is on the ground and not grappling, shoot a raycast downward to determine the object they are on.
        if (grounded && readyToJump && !grappleGunReference.IsGrappling())
        {
            Ray downRay = new Ray(gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(downRay, out hit, 5f, whatIsGround))
            {
                if (coyoteTimeTags.Contains(hit.collider.gameObject.tag))
                {
                    Debug.Log("Yeah");
                    bool onCoyoteTimeObj = false;

                    // Update the game object stood on if when player stands on a new object.
                    if (gameObjectStoodOn != hit.collider.gameObject)
                    {
                        gameObjectStoodOn = hit.collider.gameObject;

                        if (!onCoyoteTimeObj)
                        {
                            //DisableCoyoteTime();
                            EnableCoyoteTime(gameObjectStoodOn);
                        }
                    }

                    // Check to see if the Coyote Time object is stepped on. 
                    foreach (GameObject objs in coyoteTimeObjs)
                    {
                        if (hit.collider.gameObject.name == objs.name)
                        {
                            onCoyoteTimeObj = true;
                        }
                    }

                    // If on coyote time object, begin the coyote time countdown.
                    if (onCoyoteTimeObj)
                    {
                        if (!coyoteTimeStarted)
                        {
                            coyoteTimeStarted = true;
                            StartCoroutine(BeginCoyoteTimeCount());
                        }
                    }
                    else if (!coyoteTimeEnabled && !coyoteTimeStarted)
                    {
                        //DisableCoyoteTime();
                        EnableCoyoteTime(gameObjectStoodOn);
                    }
                }

                else
                {
                    Debug.Log(hit.collider.gameObject.tag);
                }
            }
            else
            {
                gameObjectStoodOn = null;
            }
        }
        else if (coyoteTimeEnabled && ((!readyToJump && !grounded) || grappleGunReference.IsGrappling()))
        {
            StopCoroutine(BeginCoyoteTimeCount());
            DisableCoyoteTime();
        }
    }

    /// <summary>
    /// Enables the Coyote Time object.
    /// </summary>
    private void EnableCoyoteTime(GameObject objStoodOn)
    {
        if (gameObjectStoodOn != null)
        {
            coyoteTimeEnabled = true;

            Vector3 objectStoodOnCollider = objStoodOn.GetComponent<BoxCollider>().size;
            Vector3 objectStoodOnPos = objStoodOn.transform.position;

            // Set the coyote time obj scale to be the same as the object stepped on's collider.
            for (int i = 0; i < coyoteTimeObjs.Count; i++)
            {
                coyoteTimeObjs[i].SetActive(true);
                //coyoteTimeObjs[i].transform.localScale = new Vector3(objectStoodOnCollider.x + 30, objectStoodOnCollider.y, objectStoodOnCollider.z + 30);

                //Vector3 coyoteCollider = coyoteTimeObjs[i].GetComponent<BoxCollider>().size;

                //coyoteTimeObjs[i].GetComponent<BoxCollider>().size = new Vector3(coyoteCollider.x, objectStoodOnCollider.y, coyoteCollider.z);

                BoxCollider coyoteCollider = coyoteTimeObjs[i].GetComponent<BoxCollider>();
                coyoteCollider.size = new Vector3(coyoteCollider.size.x, gameObjectStoodOn.GetComponent<BoxCollider>().bounds.size.y - 0.02f, coyoteCollider.size.z);
            }

            float objectColliderDiffX = objectStoodOnCollider.x / 2;
            float objectColliderDiffZ = objectStoodOnCollider.z / 2;

            // Aling the coyote time objects to one side of each platform being stepped on.
            coyoteTimeObjs[0].transform.position = new Vector3(objectStoodOnPos.x, objectStoodOnPos.y, objectStoodOnPos.z + objectColliderDiffZ + (coyoteTimeObjs[0].transform.localScale.z / 2));
            coyoteTimeObjs[1].transform.position = new Vector3(objectStoodOnPos.x + objectColliderDiffX + (coyoteTimeObjs[1].transform.localScale.x / 2), objectStoodOnPos.y, objectStoodOnPos.z);
            coyoteTimeObjs[2].transform.position = new Vector3(objectStoodOnPos.x, objectStoodOnPos.y, objectStoodOnPos.z - objectColliderDiffZ - (coyoteTimeObjs[2].transform.localScale.z / 2));
            coyoteTimeObjs[3].transform.position = new Vector3(objectStoodOnPos.x - objectColliderDiffX - (coyoteTimeObjs[3].transform.localScale.x / 2), objectStoodOnPos.y, objectStoodOnPos.z);
        }
    }

    /// <summary>
    /// Disables Coyote Time objects.
    /// </summary>
    private void DisableCoyoteTime()
    {
        // Cycles through Coyote Time objects to set them to false.
        for (int i = 0; i < coyoteTimeObjs.Count; i++)
        {
            coyoteTimeObjs[i].SetActive(false);
        }

        // States that the Coyote Time objects are no longer enabled.
        coyoteTimeEnabled = false;
    }

    /// <summary>
    /// Coroutine that starts the Coyote Time object countdown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginCoyoteTimeCount()
    {
        yield return new WaitForSeconds(coyoteTimeDuration);
        DisableCoyoteTime();

        // States that the BeginCoyoteTimeCount is no longer running.
        coyoteTimeStarted = false;
    }
    #endregion

    #endregion

    #region Jumping Stuff

    /// <summary>
    /// Handles the player jumping.
    /// </summary>
    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    /// <summary>
    /// Resets the jump parameter to true. When true, the player may jump again.
    /// </summary>
    private void ResetJump()
    {
        readyToJump = true;
    }

    #region Narrative/Dialogue Trigger - OnPlayerHitGroundEvent
    bool fallCheckRunning = false;
    /// <summary>
    /// Keeps track of how long the player falls for the Dialogue/Narrative System's onPlayerHitGroundCheck
    /// </summary>
    /// <returns></returns>
    IEnumerator FallCheck()
    {
        if (narrativeTriggerReference == null)
            yield break;

        fallCheckRunning = true;

        float airTime = 0;
        while (!grounded)
        {
            if (grappleGunReference.IsGrappling())
                airTime = 0;
            airTime += Time.deltaTime;

            yield return null;
        }

        if (airTime > narrativeTriggerReference.GetFallTime())
        {
            GameEventManager.TriggerEvent("onPlayerHitGround");
        }

        fallCheckRunning = false;
    }
    #endregion
    #endregion

    private float desiredX;




    /// <summary>
    /// Rotates the player in the direction they are looking in.
    /// </summary>
    private void Look()
    {
        float tempX = mouseX * sensitivity * Time.fixedDeltaTime;
        float tempY = mouseY * sensitivity * Time.fixedDeltaTime;

        float mouseInput = Mathf.Abs(tempX) + Mathf.Abs(tempY);
        if (mouseInput > normalSpeedAmmount)
        {
            tempX *= 1 + (mouseAccerlationAmmount * (mouseInput - normalSpeedAmmount));

            tempY *= 1 + (mouseAccerlationAmmount * (mouseInput - normalSpeedAmmount));

        }

        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + tempX;

        //Rotate, and also make sure we dont over- or under-rotate.
        if (PlayerPrefs.HasKey("InvertY"))
        {
            if (PlayerPrefs.GetInt("InvertY") == 1)
            {
                xRotation += tempY;
            }

            else
            {
                xRotation -= tempY;
            }
        }

        else
        {
            xRotation -= tempY;
        }

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

        //Narrative/Dialogue Trigger LookAtObject Event

        if (GetGameObjectInLineOfSight() != null)
        {
            narrativeTriggerReference.ObjectInSightCheck(GetGameObjectInLineOfSight());
        }

    }

    /// <summary>
    /// Helper function for use in the Narrative/Dialogue Trigger LookAtObject Event
    /// </summary>
    /// <returns>The first Gameobject found in the player's line of sight</returns>
    GameObject GetGameObjectInLineOfSight()
    {
        RaycastHit hit;
        Physics.Raycast(playerCam.position, playerCam.transform.forward, out hit, Mathf.Infinity, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        else
        {
            return null;
        }

    }


    /// <summary>
    /// Handles movement counter measures to maintain smooth movement.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mag"></param>
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement (this makes it so that the player slows down and slides to a stop) counterMovement to affect speed of this
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf
            .Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    /// <summary>
    /// Returns true if the angle between the player and a vector is less than the max slope angle.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                timeOffGround = 0;
                grounded = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }

        //// Update the game object stood on if when player stands on a new object. Occurs here as a percausion in case not set in OnCollisionEnter.
        //if (other.collider.tag == "Platform" || other.gameObject.layer.Equals("Ground"))
        //{
        //    if (gameObjectStoodOn != other.collider.gameObject)
        //    {
        //        gameObjectStoodOn = other.collider.gameObject;
        //    }
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player is exiting a surface, change its physics material back to its original one.
        if (collision.collider.tag == "GrapplePoint" || collision.collider.tag == "Platform")
        {
            if (applyPhysicsMaterial)
            {
                applyPhysicsMaterial = false;

                collision.collider.material = originalMaterial;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ScreenShake(other);
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;

        }

        // Determines whether or not the point collided with is a surface.
        if (other.collider.tag == "GrapplePoint" || other.collider.tag == "Platform")
        {
            RaycastHit hit;

            // If it's a surface, determine whether the player is on top or on the side of the surface.
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 1f))
            {
                if (hit.collider.gameObject != other.collider.gameObject)
                {
                    applyPhysicsMaterial = true;
                }
            }
            else
            {
                applyPhysicsMaterial = true;
            }

            // If the player is on the side of the surface, set the surface's physics material to the frictionless material to prevent sticking.
            if (applyPhysicsMaterial)
            {
                if (other.collider.material != null)
                {
                    originalMaterial = other.collider.material;
                }

                other.collider.material = frictionlessMat;
            }
        }

        bool cancelVelocity = false;

        // Sets velocity to 0 when grounded after grappling to prevent sliding.
        foreach (string tag in tagsToCancelVelocity)
        {
            if (other.collider.tag == tag)
            {
                if (notLandedAfterAirTime && !grappleGunReference.IsGrappling())
                {
                    Debug.Log("Landed - Velocity Cancelled. ");
                    cancelVelocity = true;
                }
            }
        }

        if(cancelVelocity)
        {
            notLandedAfterAirTime = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ScreenShake(Collision other)
    {
        if (!grounded && rb.velocity.magnitude > minVelocityForScreenShake && PlayerPrefs.GetInt("ScreenShake") == 1 && timeOffGround > .5f)
        {
            Debug.Log("Screen Shaked");
            float tempShakeAmmount = startingScreenShakeAmmount + ((rb.velocity.magnitude - minVelocityForScreenShake) * shakeAmmountVelocityScaling);
            cam.ScreenShake(screenShakeLength, tempShakeAmmount);
        }

    }

    /// <summary>
    /// Sets ground state to false.
    /// </summary>
    private void StopGrounded()
    {
        grounded = false;
    }

    public bool GetGrounded()
    {
        return grounded;
    }

    public void SetPlayerCanMove(bool move)
    {
        canMove = move;
    }

    void changeFOV()
    {

        if (PlayerPrefs.GetInt("FOV") == 1)
        {
            Camera.main.fieldOfView = m_fieldOfView;

            Cinemachine.CinemachineVirtualCamera[] camArray = FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>();
            foreach (Cinemachine.CinemachineVirtualCamera cam in camArray)
            {
                cam.m_Lens.FieldOfView = Camera.main.fieldOfView;
            }


            var targetMaxFOV = (int)(maxFOV * (1 + (rb.velocity.magnitude * maxFOVSpeedScale)));

            if (lastMaxFOV != 0 && Mathf.Abs(targetMaxFOV - lastMaxFOV) <= 2)
            {
                targetMaxFOV = lastMaxFOV;
            }

            if (currentMaxFOV < (targetMaxFOV - 2))
            {
                currentMaxFOV += fovChangeRate * Time.deltaTime;
            }

            else if (currentMaxFOV > (targetMaxFOV + 2))
            {
                currentMaxFOV -= fovChangeRate * Time.deltaTime;
            }

            m_fieldOfView = Mathf.Clamp(m_fieldOfView, minFOV, currentMaxFOV);

            if (rb.velocity.x >= xFOVActivationVel ||
                rb.velocity.z >= zFOVActivationVel ||
                rb.velocity.y >= yFOVActicationVel ||
                rb.velocity.y <= -yFOVActicationVel ||
                rb.velocity.x <= -xFOVActivationVel ||
                rb.velocity.z <= -zFOVActivationVel)
            {

                m_fieldOfView += (fovChangeRate * Time.deltaTime);
                //  m_fieldOfView = (int)m_fieldOfView;
            }
            else
            {
                m_fieldOfView -= (fovChangeRate * Time.deltaTime);
                //   m_fieldOfView = (int)m_fieldOfView;
            }


            lastVelocity = rb.velocity.magnitude;


            lastMaxFOV = targetMaxFOV;
        }
    }

    public bool CanPlayerMove()
    {
        return canMove;
    }


}


