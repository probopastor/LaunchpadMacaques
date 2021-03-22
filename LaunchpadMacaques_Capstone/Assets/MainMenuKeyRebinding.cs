using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuKeyRebinding : MonoBehaviour
{
    public enum InputTypes { startGrapple, stopGrapple, batmanGrapple ,dropCube, move, jump ,look, interact, back, dash, pause}

    PlayerControlls controlls;

    string currentPlayerPref;

    private void Awake()
    {
        controlls = new PlayerControlls();
        controlls.Enable();
    }


    private InputAction ReturnInputActionType(InputTypes input)
    {
        InputAction action = controlls.GamePlay.Crouch;
        switch (input)
        {
            case InputTypes.startGrapple:
                action = controlls.GamePlay.StartGrapple;
                currentPlayerPref = "StartGrapple";
                break;
            case InputTypes.stopGrapple:
                action = controlls.GamePlay.StopGrapple;
                currentPlayerPref = "StopGrapple";
                break;
            case InputTypes.batmanGrapple:
                action = controlls.GamePlay.StartBatmanGrapple;
                currentPlayerPref = "StartBatman";
                break;
            case InputTypes.dropCube:
                action = controlls.GamePlay.DropCube;
                currentPlayerPref = "DropCube";
                break;
            case InputTypes.move:
                action = controlls.GamePlay.Move;
                currentPlayerPref = "Move";
                break;
            case InputTypes.jump:
                action = controlls.GamePlay.Jump;
                currentPlayerPref = "Jump";
                break;
            case InputTypes.look:
                action = controlls.GamePlay.Look;
                currentPlayerPref = "Look";
                break;
            case InputTypes.interact:
                action = controlls.GamePlay.Interact;
                currentPlayerPref = "Interact";
                break;
            case InputTypes.back:
                action = controlls.GamePlay.Back;
                currentPlayerPref = "Back";
                break;
            case InputTypes.dash:
                action = controlls.GamePlay.Dash;
                currentPlayerPref = "Dash";
                break;
            case InputTypes.pause:
                action = controlls.GamePlay.Pause;
                currentPlayerPref = "Pause";
                break;

        }

        return action;
    }

    public void RebindController(string input)
    {
        var enumState = (InputTypes) System.Enum.Parse(typeof(InputTypes), input);
        RemapController(ReturnInputActionType(enumState));
    }
    
    public void RebindKeyboard(string input)
    {
        var enumState = (InputTypes)System.Enum.Parse(typeof(InputTypes), input);
        RemapKeyboard(ReturnInputActionType(enumState));
    }

    void RemapKeyboard(InputAction actionToRebind)
    {
        controlls.GamePlay.Disable();
        StopAllCoroutines();

        var rebindOperation = actionToRebind
            .PerformInteractiveRebinding().WithTargetBinding(0).WithControlsExcluding("<Gamepad>").Start();

        currentPlayerPref += "K";
        StartCoroutine(CloseBinding(rebindOperation, actionToRebind));

    }

    void RemapController(InputAction actionToRebind)
    {
        controlls.GamePlay.Disable();
        var rebindOperation = actionToRebind
            .PerformInteractiveRebinding().WithTargetBinding(0).WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>").Start();

        currentPlayerPref += "Controller";
        StartCoroutine(CloseBinding(rebindOperation, actionToRebind));


    }

    IEnumerator CloseBinding(InputActionRebindingExtensions.RebindingOperation op, InputAction action)
    {
        yield return new WaitForSeconds(.5f);

        Debug.Log(action.bindings[0].effectivePath);
        op.Dispose();
        PlayerPrefs.SetString(currentPlayerPref, action.bindings[0].effectivePath);

        controlls.GamePlay.Enable();
    }
}
