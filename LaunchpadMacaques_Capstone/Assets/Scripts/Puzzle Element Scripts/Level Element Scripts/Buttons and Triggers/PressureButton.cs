/* 
* Launchpad Macaques - Neon Oblivion
* Jamey Colleen, Jake Buri, William Nomikos
* PressureButton.cs
* Handles button activation when objects are on a button. Button disables set game objects when the amount of objects
* required by the button are placed onto it. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [SerializeField, Tooltip("The objects linked to the button. Objects in this array will be disabled when the button is active. ")] private GameObject[] objectsLinkedToButton;
    [SerializeField, Tooltip("The triggers that the button should read. Other triggers are ignored. ")] private string[] triggerTags;
    [SerializeField, Tooltip("The amount of objects on a pressure button required for it to be activated. ")] private int triggerEnableGoal = 2;

    [SerializeField, Tooltip("Material of active object. ")] private Material activeButtonMaterial;
    [SerializeField, Tooltip("Material of inactive object. ")] private Material inactiveButtonMaterial;

    private int objectsOnButton = 0;
    private bool activeStatus;
    private Material buttonMaterial;

    // Start is called before the first frame update
    void Start()
    {
        objectsOnButton = 0;
        activeStatus = false;
        buttonMaterial = GetComponent<MeshRenderer>().material;
        ActivateDeactivateButton(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        else if ((objectsOnButton < triggerEnableGoal) && activeStatus)
        {
            activeStatus = false;
            ActivateDeactivateButton(false);
        }
    }

    private void ActivateDeactivateButton(bool isActive)
    {
        if(isActive)
        {
            buttonMaterial = activeButtonMaterial;

            for (int i = 0; i < objectsLinkedToButton.Length; i++)
            {
                objectsLinkedToButton[i].SetActive(false);
            }
        }
        else if(!isActive)
        {
            buttonMaterial = inactiveButtonMaterial;

            for (int i = 0; i < objectsLinkedToButton.Length; i++)
            {
                objectsLinkedToButton[i].SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < triggerTags.Length; i++)
        {
            if(other.CompareTag(triggerTags[i]))
            {
                objectsOnButton++;
                CheckButtonActivity();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < triggerTags.Length; i++)
        {
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
}
