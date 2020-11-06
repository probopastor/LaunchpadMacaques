using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationDoor : MonoBehaviour
{
    [SerializeField, Tooltip("All of the doors handled by this activation door. ")] private GameObject[] doors;

    [SerializeField, Tooltip("The amount of buttons that need to be triggered to activate these doors. ")] private int buttonsToActivate = 0;
    [SerializeField, Tooltip("The amount of time waited after doors are activated / deactivated before their activity status is actually set. ")] private float activationBuffer;

    [SerializeField, Tooltip("If true, items handled by this button will be enabled (from a disabled state) when the button is pressed. Otherwise, " +
    "items handled by this button will be disabled when the button is activated. ")] private bool enableOnActivation;

    private int currentButtonsPressed = 0;
    private bool doorsDectivated;

    private void Awake()
    {
        currentButtonsPressed = 0;
        doorsDectivated = false;

        if (enableOnActivation)
        {
            DisableDoor();
        }
        else
        {
            EnableDoor();
        }
    }


    /// <summary>
    /// Waits a period of time before activating / deactivating doors. 
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    private IEnumerator BufferDoorActivity(bool enable)
    {
        if(activationBuffer != 0)
        {
            yield return new WaitForSeconds(activationBuffer);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        if(enable)
        {
            EnableDoor();
        }
        else if(!enable)
        {
            DisableDoor();
        }
    }

    /// <summary>
    /// Increases or decreases the amount of active buttons. 
    /// </summary>
    /// <param name="activeGain"></param>
    public void SetActiveButtons(int activeGain)
    {
        currentButtonsPressed += activeGain;

        if (currentButtonsPressed < 0)
        {
            currentButtonsPressed = 0;
        }

        CheckButtonActivation();
    }

    /// <summary>
    /// Checks to see if doors should be disabled or enabled based on the current amount of pressed buttons.
    /// </summary>
    private void CheckButtonActivation()
    {
        if((currentButtonsPressed >= buttonsToActivate) && !doorsDectivated)
        {
            doorsDectivated = true;

            if (enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(true));
            }
            else if (!enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(false));
            }
        }
        else if ((currentButtonsPressed < buttonsToActivate) && doorsDectivated)
        {
            doorsDectivated = false;

            if (enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(false));
            }
            else if (!enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(true));
            }
        }
    }

    /// <summary>
    /// Disables all doors connected to this activation door.
    /// </summary>
    private void DisableDoor()
    {
        // Cycles through door array and disables all door game objects.
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }
    }

    /// <summary>
    /// Enables all doors connected to this activation door.
    /// </summary>
    private void EnableDoor()
    {
        // Cycles through door array and enables all door game objects.
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(true);
        }
    }

    /// <summary>
    /// Directly sets the current number of buttons pressed to the passed in amount.
    /// </summary>
    /// <param name="newButtonPressedAmount"></param>
    public void SetCurrentButtonsPressed(int newButtonPressedAmount)
    {
        currentButtonsPressed = newButtonPressedAmount;

        // Make sure buttons pressed does not fall below 0.
        if (currentButtonsPressed < 0)
        {
            currentButtonsPressed = 0;
        }

        // Recheck the door's activation status after the buttons pressed number changed. 
        CheckButtonActivation();
    }

}
