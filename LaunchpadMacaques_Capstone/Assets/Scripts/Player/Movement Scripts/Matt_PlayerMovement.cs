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

public class Matt_PlayerMovement : MonoBehaviour
{
    #region References
    [SerializeField, Tooltip("The text element that displays the current gravity. ")] TextMeshProUGUI currentGravityText;
    private GrapplingGun grappleGunReference;
    private CollectibleController collectibleController;
    #endregion

    #region Player Camera Variables
    [Header("Player Transform Assignables")]
    [SerializeField, Tooltip("The transform of the player's camera. ")] private Transform playerCam;
    [SerializeField, Tooltip("The transform of the player's orientation ")] private Transform orientation;
    //public float initialFieldofView = 90f;
    //public float desiredFieldofView = 60f;
    //public float fieldofViewTime = .5f;
    #endregion

    //Other
    private Rigidbody rb;

    #region Player Sensitivity 
    [Header("Player Rotation and Look")]
    private float xRotation;
    [SerializeField, Tooltip("The player's look sensitivity. Higher value lets the player look around quicker. ")] private float sensitivity = 50f;
    #endregion

    #region Player Movement Variables
    [Header("Player Movement Variables")]
    [SerializeField, Tooltip("The player's movement speed. ")] private float moveSpeed = 4500;
    [Range(0f, 1f), SerializeField, Tooltip("The movement speed multiplier while the player is airborn. ")] private float airMoveSpeedMultiplier = .75f;
    [SerializeField, Tooltip("The player's max speed. when walking, doesnt affect swing speed ")] private float maxSpeed = 20;
    #endregion 

    [HideInInspector] public bool grounded;
    [SerializeField, Tooltip("The layer for the ground. Anything on this layer will be considered ground. ")] private LayerMask whatIsGround;

    [SerializeField, Tooltip("This is the max speed that the player can achieve when swinging. ")]
    private float maxVelocity = 50f;

    #region Movement Stabilization Variables
    [Header("Counter Movement")]
    [Tooltip("This is the variable that affects the speed of counter movement applied to the player, which slows the player down to a stop")] public float counterMovement = 0.175f;
    [SerializeField] private float threshold = 0.01f;
    [SerializeField, Tooltip("The angle that the player starts to be unable to walk up ")] private float maxSlopeAngle = 35f;
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
    private float defaultGravity;
    private Vector3 gravityVector;
    #endregion

    #region Sprinting Variables
    [Header("Sprinting")]
    [SerializeField] private float sprintMultiplier = 1.75f;
    private bool readyToSprint = true;
    private float speedStorage;
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
    #endregion

    #region Getters/Setters
    public float GetMaxVelocity()
    {
        return maxVelocity;
    }
    #endregion


    [Header("Player Input")]

    private float x, y;
    private bool jumping, sprinting, crouching, canDash;

    private PauseManager pauseManager;

    ConfigJoint config;

    private Vector3 latestOrientation;

    private float deafultVelocity;

    [Header("Animation")]
    [SerializeField]
    Animator anim;

    void Awake()
    {
        defaultGravity = gravity;
        deafultVelocity = maxVelocity;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        pauseManager = FindObjectOfType<PauseManager>();
        collectibleController = FindObjectOfType<CollectibleController>();
        grappleGunReference = FindObjectOfType<GrapplingGun>();

        config = FindObjectOfType<ConfigJoint>();

        //Cannot dash while on the ground. 
        canDash = false;
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speedStorage = maxSpeed;
    }

    private void FixedUpdate()
    {
        Movement();
        LimitVelocity();
        SetGravityModifier();
    }

    private void Update()
    {
        if ((!pauseManager.GetPaused() && !pauseManager.GetGameWon()) || Time.timeScale > 0)
        {
            MyInput();
            Look();
            grappleGunReference.UpdateHandRotation(rb.velocity);
        }
    }


    #region Dash Stuff

    /// <summary>
    /// The method that is called to start the player dash
    /// </summary>
    private void Dash()
    {
        if (grappleGunReference.IsGrappling())
        {
            grappleGunReference.StopGrapple();
        }

        if (canDash)
        {
            anim.ResetTrigger("GrappleStart");
            anim.SetTrigger("Dash");
            canDash = false;
            StartCoroutine(DashCooldown());

            if (useAddForceDash)
            {
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
    /// <summary>
    /// The dash that will only change the player's direction does not change their speed
    /// </summary>
    private void ChangeDirectionDash()
    {
        float currentMag = rb.velocity.magnitude;
        rb.velocity += playerCam.forward * currentMag;
        rb.velocity = grappleGunReference.CustomClampMagnitude(rb.velocity, currentMag, currentMag);
    }

    /// <summary>
    /// Will apply a force dash for one frame (Causes player to teleport)
    /// </summary>
    private void AddForceDash()
    {
        GetComponent<Rigidbody>().AddForce((playerCam.forward) * impulseDashAmmount, ForceMode.Impulse);
    }

    /// <summary>
    /// Will apply a dash force over time of dash length
    /// </summary>
    /// <returns></returns>
    IEnumerator DashCourtine()
    {
        float currentTime = 0;

        yield return new WaitForEndOfFrame();

        while (currentTime < dashLength)
        {
            if (grounded || grappleGunReference.IsGrappling())
            {
                break;
            }
            GetComponent<Rigidbody>().AddForce((playerCam.forward) * courtineDashAmmount * Time.deltaTime, ForceMode.Impulse);
            currentTime += Time.deltaTime;
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

    #region Input

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy.
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && !grappleGunReference.IsGrappling())
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl) && !grappleGunReference.IsGrappling())
            StopCrouch();

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift))
            Sprint();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            StopSprint();

        //dash, when grappling
        if (Input.GetKeyDown(KeyCode.LeftShift) && !grounded)
        {
            Dash();
        }
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
        // Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        // Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        // If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        // If holding sprint && ready to sprint, then sprint
        if (readyToSprint && sprinting) Sprint();

        // Set max speed
        float maxSpeed = this.maxSpeed;

        // If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        // If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        // Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = airMoveSpeedMultiplier;
            multiplierV = airMoveSpeedMultiplier;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        // If the player is grounded, they cannot dash.
        if(grounded)
        {
            canDash = false;
        }
        // Dash cooldown is reset if the player grapples again. 
        else if (grappleGunReference.IsGrappling() && !canDash)
        {
            canDash = true;
        }

        //if (config.isActiveAndEnabled)
        //{
        //    Debug.Log("Config Joint");
        //    if (!config.IsGrappling())
        //    {
        //        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        //        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        //    }
        //    else if (config.IsGrappling())
        //    {
        //        if (/*config.GetCanApplyForce())*/true)
        //        {
        //            rb.AddForce(orientation.transform.forward * swingSpeed * Time.deltaTime);
        //        }
        //    }
        //}

        // If the player is not grappling, add a force in the direction they are moving in.
        if (!grappleGunReference.IsGrappling())
        {
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }
        // If Swing Lock is not active, and the player is grappling, add a force in the player's orientation
        else if (!grappleGunReference.GetSwingLockToggle() && grappleGunReference.IsGrappling())
        {
            // If the force can be applied, add a force in the direction of the player's orientation. 
            if (grappleGunReference.GetCanApplyForce())
            {
                rb.AddForce(orientation.transform.forward * grappleGunReference.GetSwingSpeed() * Time.deltaTime);
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
                    rb.AddForce(latestOrientation * grappleGunReference.GetSwingSpeed() * Time.deltaTime);
                }
            }
        }

    }

    #endregion

    #region Sprinting Stuff
    
    /// <summary>
    /// Handles the player sprinting.
    /// </summary>
    private void Sprint()
    {
        if (grounded && readyToSprint)
        {
            readyToSprint = false;
            //Apply sprint to player
            maxSpeed = speedStorage * sprintMultiplier;
        }
    }

    /// <summary>
    /// Stops the player from sprinting. 
    /// </summary>
    private void StopSprint()
    {
        maxSpeed = speedStorage;
        readyToSprint = true;
    }

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

    #endregion

    private float desiredX;

    /// <summary>
    /// Rotates the player in the direction they are looking in. 
    /// </summary>
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
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
                grounded = true;
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
    }

    /// <summary>
    /// Sets ground state to false. 
    /// </summary>
    private void StopGrounded()
    {
        grounded = false;
    }

}
