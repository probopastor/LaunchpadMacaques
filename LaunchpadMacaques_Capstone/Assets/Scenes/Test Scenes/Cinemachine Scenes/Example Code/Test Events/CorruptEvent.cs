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

public class CorruptEvent : MonoBehaviour
{

    private UnityAction listener;
    private string eventName = "CorruptEvent";

    [SerializeField]
    private CorruptableObject[] corruptibleObjects;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(true);

        listener = new UnityAction(CorruptObjects);

        corruptibleObjects = FindObjectsOfType<CorruptableObject>();

    }

    private void OnEnable()
    {
        GameEventManager.StartListening(eventName, listener);
    }

    private void OnDisable()
    {
        GameEventManager.StopListening(eventName, listener);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameEventManager.TriggerEvent("CorruptEvent");
        }
    }

    private void CorruptObjects()
    {
        foreach(CorruptableObject corruptibleObject in corruptibleObjects)
        {

            if(corruptibleObject.corruptionState == CorruptableObject.CorruptionState.Uncorrupted)
            {
                //corruptibleObject.corruptionState = CorruptableObject.CorruptionState.Corrupting;

                corruptibleObject.StartCorrupting(corruptibleObject.transform.position);

                gameObject.SetActive(false);

            }

        }
    }

}
