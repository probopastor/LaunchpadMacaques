/* 
* Launchpad Macaques - Neon Oblivion
* William Nomikos, Jamey Colleen
* ActivationDoor.cs
* Script handles enabling or disabling objects activation doors (and their objects) upon the buttons 
* being activated. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ActivationDoor : MonoBehaviour
{
    [SerializeField, Tooltip("All of the doors handled by this activation door. ")] private GameObject[] doors;

    [SerializeField, Tooltip("The amount of buttons that need to be triggered to activate these doors. ")] private int buttonsToActivate = 0;
    [SerializeField, Tooltip("The amount of time waited after doors are activated / deactivated before their activity status is actually set. ")] private float activationBuffer = 0f;

    [SerializeField, Tooltip("If true, items handled by this button will be enabled (from a disabled state) when the button is pressed. Otherwise, " +
     "items handled by this button will be disabled when the button is activated. ")]
    private bool enableOnActivation;

    [SerializeField, Tooltip("If set to true, this activated object will never be deactivated once it is activated. ")] private bool stayDeactivated;

    private int currentButtonsPressed = 0;
    private bool doorsDeactivated;
    private GrapplingGun grapplingGunReference;
    private DoorAudio doorAudio;

    private bool startInProgress;

    private void Awake()
    {
        currentButtonsPressed = 0;
        doorsDeactivated = false;
        grapplingGunReference = FindObjectOfType<GrapplingGun>();
    }

    private void Start()
    {
        startInProgress = true;
        if (enableOnActivation)
        {
            DisableAllDoors();
        }
        else
        {
            EnableAllDoors();
        }
        startInProgress = false;
    }

    /// <summary>
    /// Waits a period of time before activating / deactivating doors. 
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    private IEnumerator BufferDoorActivity(bool enable)
    {
        // Cycle through all doors to determine which door enabling / disabling system should be used for each. 
        for (int i = 0; i < doors.Length; i++)
        {
            // If doors have a door movement script attached, move the door
            if (doors[i].GetComponentInChildren<DoorMovement>() != null)
            {
                if (!enable)
                {
                    MoveDoor(doors[i], false);
                }
                else if (enable)
                {
                    MoveDoor(doors[i], true);
                }
            }
            // If doors do not have a door movement script attached, simply enable / disable the proper door. 
            else
            {
                if (activationBuffer != 0)
                {
                    yield return new WaitForSeconds(activationBuffer);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }

                if (!enable)
                {
                    DisableAllDoors();
                }
                else if (enable)
                {
                    EnableAllDoors();
                }
            }
        }

        yield return new WaitForEndOfFrame();
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
        if ((currentButtonsPressed >= buttonsToActivate) && !doorsDeactivated)
        {
            doorsDeactivated = true;

            if (enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(true));
            }
            else if (!enableOnActivation)
            {
                StartCoroutine(BufferDoorActivity(false));
            }
        }
        else if ((currentButtonsPressed < buttonsToActivate) && doorsDeactivated && !stayDeactivated)
        {
            doorsDeactivated = false;

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
    /// Disables all doors connected to this activation door without moving them.
    /// </summary>
    private void DisableAllDoors()
    {
        // Cycles through door array and disables all door game objects.
        for (int i = 0; i < doors.Length; i++)
        {
            // If the object is a cube, respawn the cube
            if (doors[i].GetComponent<CubeRespawn>() != null)
            {
                doors[i].GetComponent<CubeRespawn>().RespawnCube();
            }

            // If the player is grappling to the object that was disabled, break the grapple. 
            if (grapplingGunReference.IsGrappling() && grapplingGunReference.GetCurrentGrappledObject() == doors[i])
            {
                grapplingGunReference.StopGrapple();
            }

            Collider[] colliders = doors[i].GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            doors[i].GetComponent<Renderer>().enabled = false;

            // Play proper door audio. 
            if (doors[i].GetComponent<DoorAudio>() != null)
            {
                doorAudio = doors[i].GetComponent<DoorAudio>();
                doorAudio.PlayDoorSound(false);
            }
        }
    }

    /// <summary>
    /// Enables all doors connected to this activation door without moving them.
    /// </summary>
    private void EnableAllDoors()
    {
        // Cycles through door array and enables all door game objects.
        for (int i = 0; i < doors.Length; i++)
        {

            Collider[] colliders = doors[i].GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            doors[i].GetComponent<Renderer>().enabled = true;

            // Play proper door audio. 
            if (doors[i].GetComponent<DoorAudio>() != null)
            {
                doorAudio = doors[i].GetComponent<DoorAudio>();
                doorAudio.PlayDoorSound(false);
            }
        }
    }

    /// <summary>
    /// Disables a single door object.
    /// </summary>
    /// <param name="thisDoor"></param>
    public void DisableDoor(GameObject thisDoor)
    {
        // If the object is a cube, respawn the cube
        if (thisDoor.GetComponent<CubeRespawn>() != null)
        {
            thisDoor.GetComponent<CubeRespawn>().RespawnCube();
        }


        // If the player is grappling to the object that was disabled, break the grapple. 
        if (grapplingGunReference.IsGrappling() && grapplingGunReference.GetCurrentGrappledObject() == thisDoor)
        {
            grapplingGunReference.StopGrapple();
        }

        Collider[] colliders = thisDoor.GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            //collider.enabled = false;
        }

        //thisDoor.GetComponent<Renderer>().enabled = false;

        // Play proper door audio. 
        if (thisDoor.GetComponent<DoorAudio>() != null)
        {
            doorAudio = thisDoor.GetComponent<DoorAudio>();
            doorAudio.PlayDoorSound(false);
        }
    }

    /// <summary>
    /// Enables a single door object.
    /// </summary>
    /// <param name="thisDoor"></param>
    public void EnableDoor(GameObject thisDoor)
    {
        Collider[] colliders = thisDoor.GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            //collider.enabled = true;
        }

        //thisDoor.GetComponent<Renderer>().enabled = true;

        // Play proper door audio. 
        if (thisDoor.GetComponent<DoorAudio>() != null)
        {
            doorAudio = thisDoor.GetComponent<DoorAudio>();
            doorAudio.PlayDoorSound(false);
        }
    }


    /// <summary>
    /// Moves the door upon activation and deactivation. 
    /// </summary>
    /// <param name="objectToMove"></param>
    /// <param name="activate"></param>
    private void MoveDoor(GameObject objectToMove, bool activate)
    {
        // Ignore the following if this method is called from the start setup.
        if (!startInProgress)
        {
            if (!activate)
            {
                objectToMove.GetComponentInChildren<DoorMovement>().MoveDoorOnDeactivation(activationBuffer);
            }
            else if (activate)
            {
                objectToMove.GetComponentInChildren<DoorMovement>().MoveDoorOnActivation(activationBuffer);
            }
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
