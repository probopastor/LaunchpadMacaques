using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuKeyRebinding : MonoBehaviour
{
    public enum InputTypes { startGrapple, stopGrapple, batmanGrapple ,dropCube, up, down, left, right, jump ,look, interact, back, dash, pause}

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
            case InputTypes.up:
                action = controlls.GamePlay.Move;
                currentPlayerPref = "Up";
                break;
            case InputTypes.down:
                action = controlls.GamePlay.Move;
                currentPlayerPref = "Down";
                break;
            case InputTypes.left:
                action = controlls.GamePlay.Move;
                currentPlayerPref = "Left";
                break;
            case InputTypes.right:
                action = controlls.GamePlay.Move;
                currentPlayerPref = "Right";
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

    #region Rebind Movement
    public void RebindKeyboardUp()
    {
        RemapKeyboard(ReturnInputActionType(InputTypes.up), 1);
    }

    public void RebindKeyboardDown()
    {
        RemapKeyboard(ReturnInputActionType(InputTypes.down), 2);
    }

    public void RebindKeyboardLeft()
    {
        RemapKeyboard(ReturnInputActionType(InputTypes.left), 3);
    }

    public void RebindKeyboardRight()
    {
        RemapKeyboard(ReturnInputActionType(InputTypes.right), 4);
    }


    public void RebindControllerUp()
    {
        RemapController(ReturnInputActionType(InputTypes.up), 6);
    }

    public void RebindControllerDown()
    {
        RemapController(ReturnInputActionType(InputTypes.down), 7);
    }

    public void RebindControllerLeft()
    {
        RemapController(ReturnInputActionType(InputTypes.left), 8);
    }

    public void RebindControllerRight()
    {
        RemapController(ReturnInputActionType(InputTypes.right), 9);
    }
    # endregion


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

    void RemapKeyboard(InputAction actionToRebind, int binding = 0)
    {
        controlls.GamePlay.Disable();
        StopAllCoroutines();

        var rebindOperation = actionToRebind
            .PerformInteractiveRebinding(binding).WithTargetBinding(binding).WithControlsExcluding("<Gamepad>").Start();

        currentPlayerPref += "K";
        StartCoroutine(CloseBinding(rebindOperation, actionToRebind, binding));

    }

    void RemapController(InputAction actionToRebind, int binding = 1)
    {
        controlls.GamePlay.Disable();
        var rebindOperation = actionToRebind
            .PerformInteractiveRebinding(binding).WithTargetBinding(binding).WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>").Start();

        currentPlayerPref += "Controller";
        StartCoroutine(CloseBinding(rebindOperation, actionToRebind, binding));


    }

    IEnumerator CloseBinding(InputActionRebindingExtensions.RebindingOperation op, InputAction action, int bindingNum)
    {
        yield return new WaitForSeconds(.5f);

        Debug.Log(action.bindings[bindingNum].effectivePath);
        op.Dispose();
        PlayerPrefs.SetString(currentPlayerPref, action.bindings[bindingNum].effectivePath);

        controlls.GamePlay.Enable();
    }


    public void ResetBindings()
    {
        ResetBindingHelper("StartGrapple");
        ResetBindingHelper("StopGrapple");
        ResetBindingHelper("StartBatman");
        ResetBindingHelper("DropCube");
        ResetBindingHelper("Up");
        ResetBindingHelper("Down");
        ResetBindingHelper("Left");
        ResetBindingHelper("Right");
        ResetBindingHelper("Jump");
        ResetBindingHelper("Interact");
        ResetBindingHelper("Pause");
        ResetBindingHelper("Dash");
        ResetBindingHelper("Look");
    }

    private void ResetBindingHelper(string pref)
    {
        PlayerPrefs.DeleteKey(pref += "K");
        PlayerPrefs.DeleteKey(pref += "Controller");
    }
}
