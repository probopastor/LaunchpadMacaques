/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* A_InputType.cs
* An abstract class that can be implanted to receive updates on status of controllers
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_InputType : MonoBehaviour
{
    protected bool controllerDetected;
    private DetectController controllerDetection;

    public void Start()
    {
        controllerDetection = DetectController.instance;
        if (controllerDetection)
        {
            controllerDetected = controllerDetection.ControllerEnabled();
        }

        else
        {
            StartCoroutine(SetController());
        }
    }


    /// <summary>
    /// Method that is called to tell the object to check the current status of the controller
    /// </summary>
    public void UpdateUI()
    {
        if (controllerDetection)
        {
            controllerDetected = controllerDetection.ControllerEnabled();
        }

        ChangeUI();
    }

    /// <summary>
    /// Checks current status of controller
    /// </summary>
    public void CheckController()
    {
        if (controllerDetection)
        {
            controllerDetected = controllerDetection.ControllerEnabled();
        }
    }

    /// <summary>
    /// The Abstract method that will be called when controller status changes
    /// </summary>
    public abstract void ChangeUI();

    private void OnEnable()
    {
        controllerDetection = FindObjectOfType<DetectController>();
        controllerDetected = controllerDetection.ControllerEnabled();

        CheckController();

        UpdateUI();
    }

    /// <summary>
    /// Finds/Sets this objects reference to DetectController.CS
    /// </summary>
    /// <returns></returns>
    IEnumerator SetController()
    {
        while (controllerDetection == null)
        {
            controllerDetection = DetectController.instance;
            yield return new WaitForSeconds(0);
        }

        controllerDetected = controllerDetection.ControllerEnabled();
        yield return new WaitForSeconds(0);
    }
}
