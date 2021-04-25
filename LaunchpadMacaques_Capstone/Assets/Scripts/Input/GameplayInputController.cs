/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* GameplayInputController.cs
* Handles the bulk of the players input
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class GameplayInputController : MonoBehaviour
{
    [SerializeField] private bool holdDownToSwing;


    private PlayerControlls controls;
    [Header("Events")]
    [SerializeField, Tooltip("The Event that is Called when the player attempts to move (Vector 2)")] MoveEvent moveEvent;
    [SerializeField, Tooltip("The Event that is called when player uses mouse scroll wheel or the DPAD (Vector 2 Use Y)")] ScrollEvent scrollEvent;
    [SerializeField, Tooltip("The Event that is Called When player presses Start Grapple Input")] StartGrappleEvent startGrappleEvent;
    [SerializeField, Tooltip("The Event that is Called When player Presses Stop Grapple Input ")] StopGrappleEvent stopGrappleEvent;
    [SerializeField, Tooltip("The Event that is Called When player uses the Jump Input")] JumpEvent jumpEvent;
    [SerializeField, Tooltip("The Event that is Called When player presses the dash/sprint input")] DashStart dashStartEvent;
    [SerializeField, Tooltip("The Event that is Called When player lets go the dash/sprint input")] DashCancel dashCancelEvent;
    [SerializeField, Tooltip("The Event That is Called when player rotates camera (Vector 2)")] LookEvent lookEvent;
    [SerializeField, Tooltip("The Event that Is Called when Player Lets Go Of/Presses Crouch")] CrouchEvent crouchEvent;
    [SerializeField, Tooltip("The Event that is Called when Player Presses Batman Grapple Input")] BatmanEvent batmanEvent;
    [SerializeField, Tooltip("The Event that is Called when Player Presses Drop Cube Input")] DropCubeEvent dropCubeEvent;


    [SerializeField, Tooltip("The Event that is Called When player Presses pause Input ")] PauseEvent pauseEvent;

    [SerializeField, Tooltip("The Event that is Called when player Presses Reset Grapple Input")] ResetGrappleEvent resetGrappleEvent;

    [SerializeField, Tooltip("The event that is called to continue dialouge")] DialougeEvent dialouge;
    [SerializeField, Tooltip("The Event that is called to open/close Log")] LogEvent logEvent;

    #region Jamies Stuff
    public GameplayInputController(LookEvent lookEvent)
    {
        this.lookEvent = lookEvent;
    }

    public GameplayInputController(PauseEvent pauseEvent)
    {
        this.pauseEvent = pauseEvent;
    }

    public GameplayInputController(StartGrappleEvent startGrappleEvent)
    {
        this.startGrappleEvent = startGrappleEvent;
    }

    public GameplayInputController(DropCubeEvent dropCubeEvent)
    {
        this.dropCubeEvent = dropCubeEvent;
    }

    public GameplayInputController(MoveEvent moveEvent)
    {
        this.moveEvent = moveEvent;
    }

    public GameplayInputController(JumpEvent jumpEvent)
    {
        this.jumpEvent = jumpEvent;
    }

    public GameplayInputController(ScrollEvent scrollEvent)
    {
        this.scrollEvent = scrollEvent;
    }

    public GameplayInputController(BatmanEvent batmanEvent)
    {
        this.batmanEvent = batmanEvent;
    }

    public GameplayInputController(DashCancel dashCancelEvent)
    {
        this.dashCancelEvent = dashCancelEvent;
    }

    public GameplayInputController(StopGrappleEvent stopGrappleEvent)
    {
        this.stopGrappleEvent = stopGrappleEvent;
    }

    public GameplayInputController(CrouchEvent crouchEvent)
    {
        this.crouchEvent = crouchEvent;
    }

    public GameplayInputController(DashStart dashStartEvent)
    {
        this.dashStartEvent = dashStartEvent;
    }
    #endregion

    private void Awake()
    {
        controls = new PlayerControlls();
        SetInputs();
    }

    private void SetInputs()
    {

        ApplyOveride("StartGrappleK", controls.GamePlay.StartGrapple, 0);
        ApplyOveride("StartGrappleController", controls.GamePlay.StartGrapple, 1);



        ApplyOveride("JumpK", controls.GamePlay.Jump, 0);
        ApplyOveride("JumpController", controls.GamePlay.Jump, 1);


        ApplyOveride("DropCubeK", controls.GamePlay.DropCube, 0);
        ApplyOveride("DropCubeController", controls.GamePlay.DropCube, 1);

        ApplyOveride("InteractK", controls.GamePlay.Interact, 0);
        ApplyOveride("InteractController", controls.GamePlay.Interact, 1);

        ApplyOveride("LookK", controls.GamePlay.Look, 0);
        ApplyOveride("LookController", controls.GamePlay.Look, 1);

        ApplyOveride("MoveK", controls.GamePlay.Move, 0);
        ApplyOveride("MoveController", controls.GamePlay.Move, 1);

        ApplyOveride("PauseK", controls.GamePlay.Pause, 0);
        ApplyOveride("PauseController", controls.GamePlay.Pause, 1);

        ApplyOveride("StartBatmanK", controls.GamePlay.StartBatmanGrapple, 0);
        ApplyOveride("StartBatmanController", controls.GamePlay.StartBatmanGrapple, 1);

        ApplyOveride("StopGrappleK", controls.GamePlay.StopGrapple, 0);
        ApplyOveride("StopGrappleController", controls.GamePlay.StopGrapple, 1);

        ApplyOveride("BackK", controls.GamePlay.Back, 0);
        ApplyOveride("BackController", controls.GamePlay.Back, 1);

        ApplyOveride("BackController", controls.GamePlay.ControllerBack, 0);

        ApplyOveride("DashK", controls.GamePlay.Dash, 0);
        ApplyOveride("DashController", controls.GamePlay.Dash, 1);

        ApplyOveride("UpK", controls.GamePlay.Move, 1);
        ApplyOveride("UpController", controls.GamePlay.Move, 6);


        ApplyOveride("DownK", controls.GamePlay.Move, 2);
        ApplyOveride("DownController", controls.GamePlay.Move, 7);


        ApplyOveride("LeftK", controls.GamePlay.Move, 3);
        ApplyOveride("LeftController", controls.GamePlay.Move, 8);


        ApplyOveride("RightK", controls.GamePlay.Move, 4);
        ApplyOveride("RightController", controls.GamePlay.Move, 9);



    }

    private void ApplyOveride(string pref, InputAction action, int binding)
    {
        if (PlayerPrefs.HasKey(pref))
        {
            action.ApplyBindingOverride(binding, PlayerPrefs.GetString(pref));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        controls.GamePlay.Enable();

        SetMovement();
        SetScroll();
        SetStartGrapple();
        SetStopGrapple();
        SetJump();
        SetDash();
        SetLook();
        SetPauseEvent();
        SetCrouchEvent();
        SetBatmanGrapple();
        SetDropCube();
        SetResetGrapple();
        SetDialouge();
        SetLog();
    }

    #region SetInputs
    
    private void SetLog()
    {
        controls.GamePlay.OpenLog.performed += OnLogEvent;
    }

    private void SetDialouge()
    {
        controls.GamePlay.Dialouge.performed += OnDialougeEvent;
    }

    private void SetCrouchEvent()
    {
        controls.GamePlay.Crouch.performed += OnCrouchEvent;
        controls.GamePlay.Crouch.canceled += OnCrouchEvent;
    }
    private void SetPauseEvent()
    {
        controls.GamePlay.Pause.performed += OnPauseEvent;
    }
    private void SetMovement()
    {
        controls.GamePlay.Move.performed += OnMovePerformed;
        controls.GamePlay.Move.canceled += OnMovePerformed;
    }

    private void SetStartGrapple()
    {
        controls.GamePlay.StartGrapple.performed += OnStartGrapple;

        if (holdDownToSwing)
        {
            controls.GamePlay.StartGrapple.canceled += OnStopGrapple;
        }

    }

    private void SetResetGrapple()
    {
        controls.GamePlay.ResetGrapple.performed += OnResetGrapple;
    }

    private void SetBatmanGrapple()
    {
        controls.GamePlay.StartBatmanGrapple.performed += OnBatmanGrapple;
    }

    private void SetStopGrapple()
    {
        if (!holdDownToSwing)
        {
            controls.GamePlay.StopGrapple.performed += OnStopGrapple;
        }

    }

    private void SetDropCube()
    {
        controls.GamePlay.DropCube.performed += OnDropCube;
    }

    private void SetScroll()
    {
        controls.GamePlay.Scroll.performed += OnScrollPerformed;
        controls.GamePlay.Scroll.canceled += OnScrollPerformed;
    }

    private void SetJump()
    {
        controls.GamePlay.Jump.performed += OnJump;
        controls.GamePlay.Jump.canceled += OnJump;
    }

    private void SetDash()
    {
        controls.GamePlay.Dash.performed += OnDashStart;
        controls.GamePlay.Dash.canceled += OnDashCancel;
    }

    private void SetLook()
    {
        controls.GamePlay.Look.performed += OnLookPerformed;
        controls.GamePlay.Look.canceled += OnLookPerformed;
    }
    #endregion

    #region Detect Inputs


    private void OnCrouchEvent(InputAction.CallbackContext cxt)
    {
        crouchEvent.Invoke(cxt.ReadValue<float>());
    }
    

    private void OnLogEvent(InputAction.CallbackContext cxt)
    {
        logEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnDialougeEvent(InputAction.CallbackContext cxt)
    {
        dialouge.Invoke(cxt.ReadValue<float>());
    }
    private void OnJump(InputAction.CallbackContext cxt)
    {
        jumpEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnPauseEvent(InputAction.CallbackContext cxt)
    {
        pauseEvent.Invoke(cxt.ReadValue<float>());
    }
    private void OnStartGrapple(InputAction.CallbackContext cxt)
    {
        startGrappleEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnBatmanGrapple(InputAction.CallbackContext cxt)
    {
        batmanEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnStopGrapple(InputAction.CallbackContext cxt)
    {
        stopGrappleEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnResetGrapple(InputAction.CallbackContext cxt)
    {
        resetGrappleEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnDropCube(InputAction.CallbackContext cxt)
    {
        dropCubeEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnMovePerformed(InputAction.CallbackContext cxt)
    {
        Vector2 input = cxt.ReadValue<Vector2>();
        moveEvent.Invoke(input.x, input.y);

    }

    private void OnLookPerformed(InputAction.CallbackContext cxt)
    {
        Vector2 input = cxt.ReadValue<Vector2>();
        lookEvent.Invoke(input.x, input.y);


    }

    private void OnScrollPerformed(InputAction.CallbackContext cxt)
    {
        Vector2 input = cxt.ReadValue<Vector2>();
        scrollEvent.Invoke(input.x, input.y);
    }

    private void OnDashStart(InputAction.CallbackContext cxt)
    {
        dashStartEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnDashCancel(InputAction.CallbackContext cxt)
    {
        dashCancelEvent.Invoke(cxt.ReadValue<float>());
    }

    #endregion

}
[Serializable] public class MoveEvent : UnityEvent<float, float> { }

[Serializable] public class LookEvent : UnityEvent<float, float> { }
[Serializable] public class ScrollEvent : UnityEvent<float, float> { }
[Serializable] public class StartGrappleEvent : UnityEvent<float> { }

[Serializable] public class StopGrappleEvent : UnityEvent<float> { }

[Serializable] public class JumpEvent : UnityEvent<float> { }

[Serializable] public class DashStart : UnityEvent<float> { }

[Serializable] public class DashCancel : UnityEvent<float> { }

[Serializable] public class PauseEvent : UnityEvent<float> { }

[Serializable] public class CrouchEvent : UnityEvent<float> { }

[Serializable] public class BatmanEvent : UnityEvent<float> { }

[Serializable] public class DropCubeEvent : UnityEvent<float> { }

[Serializable] public class ResetGrappleEvent : UnityEvent<float> { }

[Serializable] public class DialougeEvent: UnityEvent<float> { }

[Serializable] public class LogEvent: UnityEvent<float> { }


