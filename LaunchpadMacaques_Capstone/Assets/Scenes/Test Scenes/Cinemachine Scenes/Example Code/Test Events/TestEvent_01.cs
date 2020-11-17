/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestEvent_01 : MonoBehaviour
{

    private UnityAction someListener;
    private string evenName = "Test";

    private void Awake()
    {
        someListener = new UnityAction(SomeFunction);
    }

    private void OnEnable()
    {
        GameEventManager.StartListening(evenName, someListener);
    }

    private void OnDisable()
    {
        GameEventManager.StopListening(evenName, someListener);
    }

    private void SomeFunction()
    {
        Debug.Log("Some Function was called!");
    }

}
