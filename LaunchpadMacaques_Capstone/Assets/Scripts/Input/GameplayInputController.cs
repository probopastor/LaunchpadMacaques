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
    private PlayerControlls controls;
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


    [SerializeField, Tooltip("The Event that is Called When player Presses pause Input ")] PauseEvent pauseEvent;

    private void Awake()
    {
        controls = new PlayerControlls();
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
    }

    #region SetInputs

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
    }

    private void SetBatmanGrapple()
    {
        controls.GamePlay.StartBatmanGrapple.performed += OnBatmanGrapple;
    }

    private void SetStopGrapple()
    {
        controls.GamePlay.StopGrapple.performed += OnStopGrapple;
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


