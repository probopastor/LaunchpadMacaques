/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* ExampleInputType.cs
* Example of how to implement controller detection for context sensitive things
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExampleInputType : A_InputType
{
    [SerializeField] TextMeshProUGUI exampleText;
    /// <summary>
    /// Method called when controller status changes
    /// </summary>
    public override void ChangeUI()
    {
        // If a controller was connected
        if (controllerDetected)
        {
            exampleText.text = "Controller Connected";
        }

        // If a Controller was Disconected
        else
        {
            exampleText.text = "Controller Not Connected";
        }
    }
}
