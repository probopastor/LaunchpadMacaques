/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* DetectController.cs
* Script handles Detecting Controller Connected/Tells objects
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DetectController : MonoBehaviour
{

    string[] joySticks;
    bool controller;
    A_InputType[] inputSensitive;
    public static DetectController instance = null;
    private bool foundController = false;

    public GameObject selectedGameObject;



    private void Start()
    {

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        StopCoroutine(DetectJoySticks());
        StartCoroutine(DetectJoySticks());
    }

    private void Update()
    {

        if (FindObjectOfType<Button>() & controller)
        {
            EventSystem even = FindObjectOfType<EventSystem>();
            if (even.currentSelectedGameObject == null)
            {
                ControllerConnected();
            }
        }
    }


    /// <summary>
    /// Loops through Joysticks to check if it can find Xbox Controller
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectJoySticks()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(.5f);
            foundController = false;

            //Get Joystick Names
            joySticks = Input.GetJoystickNames();


            // Loops through all the joysticks
            if (joySticks.Length > 0)
            {
                for (int i = 0; i < joySticks.Length; ++i)
                {
                    // Will check to see if the Joystick is empty
                    if (!string.IsNullOrEmpty(joySticks[i]))
                    {
                        // Will check to see if the joystick is an xbox controller
                        if (joySticks[i] == "Controller (Xbox One For Windows)")
                        {
                            foundController = true;
                            if (!controller)
                            {
                                ControllerConnected();

                            }
                        }

                    }

                }
            }

            if (!foundController)
            {
                ControllerDisconnected();
            }
        }
    }


    /// <summary>
    /// Informs all inputType objects that controller is connected
    /// </summary>
    public void ControllerConnected()
    {
        inputSensitive = FindObjectsOfType<A_InputType>();
        controller = true;
        foreach (A_InputType sens in inputSensitive)
        {
            sens.UpdateUI();
        }

        if (FindObjectOfType<EventSystem>())
        {
            EventSystem even = FindObjectOfType<EventSystem>();
            if (selectedGameObject)
            {
                even.SetSelectedGameObject(selectedGameObject);
            }

        }
    }

    /// <summary>
    /// Informs all inputType objects that controller is not connected
    /// </summary>
    private void ControllerDisconnected()
    {
        inputSensitive = FindObjectsOfType<A_InputType>();
        controller = false;
        foreach (A_InputType sens in inputSensitive)
        {
            sens.UpdateUI();
        }
    }

    public bool ControllerEnabled()
    {
        return controller;
    }


}
