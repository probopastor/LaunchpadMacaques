/* 
* Launchpad Macaques - Neon Oblivion
* Jamey Colleen, Jake Buri, William Nomikos, Connor Wolf
* PressureButton.cs
* Handles button activation when objects are on a button. Button disables set game objects when the amount of objects
* required by the button are placed onto it. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PressureButton : MonoBehaviour
{
    [SerializeField, Tooltip("The objects linked to the button. Objects in this array will be disabled when the button is active. ")] private GameObject[] objectsLinkedToButton;
    [SerializeField, Tooltip("The triggers that the button should read. Other triggers are ignored. ")] private string[] triggerTags;
    [SerializeField, Tooltip("The amount of objects on a pressure button required for it to be activated. ")] private int triggerEnableGoal = 2;

    [SerializeField, Tooltip("If active, objects linked to the button will be deactivated after a period of time from when activated. ")] private bool useTimer;
    [SerializeField, Tooltip("Used only if useTimer is true. The amount of seconds waited before deactive objects are reactivated. ")] private float timeUntilReactivation = 1f;

    [SerializeField, Tooltip("Material of active object. ")] private Material activeButtonMaterial;
    [SerializeField, Tooltip("Material of inactive object. ")] private Material inactiveButtonMaterial;

    [SerializeField, Tooltip("If true, button can be activated in a proximity. ")] private bool proximityTrigger;
    [SerializeField, Tooltip("The area around this button that will trigger it. Only active if proximityTrigger is true. ")] private Vector3 proximityTriggerArea;

    private int objectsOnButton = 0;
    private bool activeStatus;
    private Renderer buttonRend;

    private StudioEventEmitter soundEmitter;

    // Start is called before the first frame update
    void Awake()
    {
        SetValues();
        soundEmitter = GetComponent<StudioEventEmitter>();
    }

    /// <summary>
    /// Sets start variables and values.
    /// </summary>
    private void SetValues()
    {
        objectsOnButton = 0;
        activeStatus = false;
        buttonRend = GetComponent<MeshRenderer>();
        CheckIfProximityTrigger();
        ActivateDeactivateButton(false);
    }

    /// <summary>
    /// If the button is a proximity trigger, adjust its trigger collider to set length. 
    /// </summary>
    private void CheckIfProximityTrigger()
    {
        if (proximityTrigger)
        {
            BoxCollider[] buttonColliders = gameObject.GetComponents<BoxCollider>();

            for (int i = 0; i < buttonColliders.Length; i++)
            {
                if (buttonColliders[i].isTrigger)
                {
                    buttonColliders[i].size = proximityTriggerArea;
                }
            }
        }
    }

    /// <summary>
    /// Checks whether or not the button should be activated or deactivated. 
    /// </summary>
    private void CheckButtonActivity()
    {
        if ((objectsOnButton >= triggerEnableGoal) && !activeStatus)
        {
            activeStatus = true;
            ActivateDeactivateButton(true);

            if (soundEmitter.IsPlaying()) soundEmitter.Stop();
            soundEmitter.Play();
        }
        else if ((objectsOnButton < triggerEnableGoal) && activeStatus)
        {
            activeStatus = false;
            ActivateDeactivateButton(false);
            soundEmitter.EventInstance.triggerCue();
        }
    }

    /// <summary>
    /// Handles activating and deactiving button. Pass in true for if the button should be activated,
    /// false if it should be deactivated. 
    /// </summary>
    /// <param name="isActive"></param>
    private void ActivateDeactivateButton(bool isActive)
    {
        if (isActive)
        {
            buttonRend.material = activeButtonMaterial;

            // Deactivates all the objectes in the objectsLinkedToButton array.
            for (int i = 0; i < objectsLinkedToButton.Length; i++)
            {
                if (objectsLinkedToButton[i].GetComponent<ActivationDoor>() != null)
                {
                    objectsLinkedToButton[i].GetComponent<ActivationDoor>().SetActiveButtons(1);
                }
                else
                {
                    objectsLinkedToButton[i].SetActive(false);
                }
            }

            // If a timer should be used

            //if (useTimer)
            //{
            //    StartCoroutine(DisableAfterTime());
            //}
        }
        else if (!isActive)
        {
            buttonRend.material = inactiveButtonMaterial;

            // Activates all objects in the objectsLinkedToButton array. 
            for (int i = 0; i < objectsLinkedToButton.Length; i++)
            {
                if (objectsLinkedToButton[i].GetComponent<ActivationDoor>() != null)
                {
                    objectsLinkedToButton[i].GetComponent<ActivationDoor>().SetActiveButtons(-1);
                }
                else
                {
                    objectsLinkedToButton[i].SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Disables objects after a period of time from when they are activated. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSecondsRealtime(timeUntilReactivation);

        for (int i = 0; i < objectsLinkedToButton.Length; i++)
        {
            if (objectsLinkedToButton[i].GetComponent<ActivationDoor>() != null)
            {
                objectsLinkedToButton[i].GetComponent<ActivationDoor>().SetCurrentButtonsPressed(0);
            }
        }

        activeStatus = false;
        ActivateDeactivateButton(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks the triggerTags array to determine if the object should be checked. 
        for (int i = 0; i < triggerTags.Length; i++)
        {
            // If the object should be checked, increase the amount of objects on the button and check if it should be made active. 
            if (other.CompareTag(triggerTags[i]))
            {


                objectsOnButton++;
                CheckButtonActivity();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Checks the triggerTags array to determine if the object should be checked. 
        for (int i = 0; i < triggerTags.Length; i++)
        {
            // If the object should be checked, decrease the amount of objects on the button and check if it should be made inactive. 
            if (other.CompareTag(triggerTags[i]))
            {
                objectsOnButton--;

                // Check to make sure there can never be negative objects on the button. 
                if (objectsOnButton < 0)
                {
                    objectsOnButton = 0;
                }

                CheckButtonActivity();
            }
        }
    }

    /// <summary>
    /// Returns the current button status. If button is active, return true. 
    /// </summary>
    /// <returns></returns>
    public bool GetButtonActivity()
    {
        return activeStatus;
    }

    /// <summary>
    /// Returns all objects linked to this button. 
    /// </summary>
    /// <returns></returns>
    public GameObject[] GetObjectsLinkedToButton()
    {
        return objectsLinkedToButton;
    }
}
