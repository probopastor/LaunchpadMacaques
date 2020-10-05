﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matt_PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GrapplingGun grappleGunReference;

    private CollectibleController collectibleController;

    [Header("Player Transform Assignables")]
    //Assingables
    public Transform playerCam;
    public Transform orientation;

    [Header("Player Rigidbody")]
    //Other
    [SerializeField]
    private Rigidbody rb;

    [Header("Player Rotation and Look")]
    //Rotation and look
    [SerializeField]
    private float xRotation;
    [SerializeField]
    private float sensitivity = 50f;
    [SerializeField]
    private float sensMultiplier = 1f;

    [Header("PLayer Movement Variables")]
    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float swingSpeed = 4500;
    public bool grounded;
    public LayerMask whatIsGround;

    [Header("Max Player Velocity")]
    // Max velocity for the character
    [SerializeField]
    private float maxVelocity = 50f;

    [Header("Counter Movement")]
    public float counterMovement = 0.175f;
    [SerializeField]
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    [Header("Crouch & Slide")]
    //Crouch & Slide
    [SerializeField]
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField]
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Sliding
    [SerializeField]
    private Vector3 normalVector = Vector3.up;
    [SerializeField]
    private Vector3 wallNormalVector;

    [Header("Jumping")]
    //Jumping
    [SerializeField]
    private bool readyToJump = true;
    [SerializeField]
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    public float gravity = 1500f;
    public float defaultGravity = 1500f;

    [Header("Sprinting")]
    //Sprinting
    [SerializeField]
    private bool readyToSprint = true;
    private float speedStorage;
    [SerializeField]
    private float sprintMultiplier = 1.75f;


    [Header("Player Input")]
    //Input
    [SerializeField]
    private float x, y;
    [SerializeField]
    private bool jumping, sprinting, crouching;

    private PauseManager pauseManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pauseManager = FindObjectOfType<PauseManager>();
        collectibleController = FindObjectOfType<CollectibleController>();
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
    }

    private void Update()
    {
        if(!pauseManager.GetPaused() && !pauseManager.GetGameWon())
        {
            MyInput();
            Look();
        }
    }

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
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift))
            Sprint();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            StopSprint();
    }

    #endregion

    #region Crouching Stuff

    private void StartCrouch()
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

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    #endregion

    /// <summary>
    /// This function limits the velocity of the player so they can't just increase speed into oblivion.
    /// </summary>
    private void LimitVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }

    #region Movement

    private void Movement()
    {
        if ((!grappleGunReference.IsGrappling() && !grounded) && !collectibleController.GetIsActive()) // If in the air // (gameObject.transform.position.y > 20)
        {
            //Add gravity
            gravity = 3000;
            rb.AddForce(Vector3.down * Time.deltaTime * gravity);
        }
        else if((grappleGunReference.IsGrappling() || grounded) && !collectibleController.GetIsActive())
        {
            //Add gravity
            gravity = defaultGravity;
            rb.AddForce(Vector3.down * Time.deltaTime * gravity);
        }

        if (collectibleController.GetIsActive())
        {
                gravity = 200f;
        }
        else if (collectibleController.GetIsActive() == false)
        {
                gravity = defaultGravity;
        }

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //If holding sprint && ready to sprint, then sprint
        if (readyToSprint && sprinting) Sprint();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        if(!grappleGunReference.IsGrappling())
        {
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }
        else if(grappleGunReference.GetSwingToggle() && grappleGunReference.IsGrappling())
        {
            if(grappleGunReference.GetCanApplyForce())
            {
                rb.AddForce(orientation.transform.forward * swingSpeed * Time.deltaTime);
            }
        }
    }

    #endregion

    #region Sprinting Stuff
    //sprinting
    private void Sprint()
    {
        if (grounded && readyToSprint)
        {
            readyToSprint = false;
            //Apply sprint to player
            maxSpeed = speedStorage * sprintMultiplier;
        }
    }
    private void StopSprint()
    {
        maxSpeed = speedStorage;
        readyToSprint = true;
    }

    #endregion

    #region Jumping Stuff

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

    private void ResetJump()
    {
        readyToJump = true;
    }

    #endregion

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

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

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
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

    private void StopGrounded()
    {
        grounded = false;
    }

}