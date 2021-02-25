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
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class DetectController : MonoBehaviour
{

    string[] joySticks;
    bool controller;
    A_InputType[] inputSensitive;
    public static DetectController instance = null;
    private bool foundController = false;

    public GameObject selectedGameObject;

    EventSystem even;

    private void Awake()
    {
        InputSystem.onDeviceChange += ControllerStatusChanged;
    }

    private void Start()
    {

        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
        }

        even = FindObjectOfType<EventSystem>();

        InitialDetection();

        Invoke("InitialDetection", .1f);
    }

    private void InitialDetection()
    {
        var test = InputSystem.FindControls("<gamepad>");

        var count = test.Count;

        test.Dispose();

        if (count >= 1)
        {
            ControllerConnected();
        }
    }

    private void Update()
    {

        if (even == null)
        {
            even = FindObjectOfType<EventSystem>();
        }


        if (FindObjectOfType<Button>() & controller)
        {
            if (FindObjectOfType<Button>().isActiveAndEnabled)
            {

                if (even.currentSelectedGameObject == null)
                {
                    ControllerConnected();
                }

                else if (even.currentSelectedGameObject.GetComponent<Button>())
                {
                    if (!even.currentSelectedGameObject.GetComponent<Button>().isActiveAndEnabled)
                    {
                        ControllerConnected();
                    }
                }

                else if (even.currentSelectedGameObject.GetComponent<TMP_Dropdown>())
                {
                    if (!even.currentSelectedGameObject.GetComponent<TMP_Dropdown>().isActiveAndEnabled)
                    {
                        ControllerConnected();
                    }
                }

                else if (even.currentSelectedGameObject.GetComponent<Slider>())
                {
                    if (!even.currentSelectedGameObject.GetComponent<Slider>().isActiveAndEnabled)
                    {
                        ControllerConnected();
                    }
                }

                else if (even.currentSelectedGameObject.GetComponent<Toggle>())
                {
                    if (!even.currentSelectedGameObject.GetComponent<Toggle>().isActiveAndEnabled)
                    {
                        ControllerConnected();
                    }
                }

                else if (even.currentSelectedGameObject.GetComponent<Scrollbar>())
                {
                    if (!even.currentSelectedGameObject.GetComponent<Scrollbar>().isActiveAndEnabled)
                    {
                        ControllerConnected();
                    }
                }
            }

        }
    }


    private void ControllerStatusChanged(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log("Controller Added");
                ControllerConnected();
                break;
            case InputDeviceChange.Disconnected:
                Debug.Log("Controller Removed");
                ControllerDisconnected();
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log("Controller Added");
                ControllerConnected();
                break;
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

            if (selectedGameObject && selectedGameObject.activeSelf)
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
