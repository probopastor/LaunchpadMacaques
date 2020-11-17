/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            GameEventManager.TriggerEvent("Test");
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            GameEventManager.TriggerEvent("CameraPanToObject");
        }
    }
}
