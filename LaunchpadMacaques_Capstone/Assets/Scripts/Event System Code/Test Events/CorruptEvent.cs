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
using UnityEngine.UI;

public class CorruptEvent : MonoBehaviour
{

    private UnityAction listener;
    private string eventName = "CorruptEvent";

    [SerializeField]
    private CorruptableObject[] corruptibleObjects;

    private float timeElapsed;

    public Text textDisplay;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(true);

        listener = new UnityAction(CorruptObjects);

        corruptibleObjects = FindObjectsOfType<CorruptableObject>();

        textDisplay.gameObject.SetActive(false);

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

            textDisplay.gameObject.SetActive(true);

            GameEventManager.TriggerEvent("CorruptEvent");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            timeElapsed += 1 * Time.deltaTime;

            textDisplay.text = "You have been standing in the trigger for " + Mathf.Round(timeElapsed) + " seconds.";

        }
    }

    private void OnTriggerExit(Collider other)
    {
        timeElapsed = 0;

        textDisplay.text = "";

        textDisplay.gameObject.SetActive(false);

    }

    private void CorruptObjects()
    {
        foreach(CorruptableObject corruptibleObject in corruptibleObjects)
        {

            if(corruptibleObject.corruptionState == CorruptableObject.CorruptionState.Uncorrupted)
            {
                //corruptibleObject.corruptionState = CorruptableObject.CorruptionState.Corrupting;

                corruptibleObject.StartCorrupting(corruptibleObject.transform.position);

                //gameObject.SetActive(false);

            }

        }
    }

}
