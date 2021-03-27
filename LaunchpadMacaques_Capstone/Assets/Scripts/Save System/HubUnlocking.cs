using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubUnlocking : MonoBehaviour
{
    [SerializeField] ActivationDoor area5Door;
    [SerializeField] 
    public GameObject[] closedPathNarrativeTriggerObjects;
    private void Start()
    {
        //Level 2 Unlock
        if(HandleSaving.instance.IsLevelComplete("Movement_2"))
        {
            closedPathNarrativeTriggerObjects[2 - 2].SetActive(false);
        }

        //Level 3 Unlock
        if(HandleSaving.instance.IsLevelComplete("Boxes_2"))
        {
            closedPathNarrativeTriggerObjects[3 - 2].SetActive(false);
        }

        //Level 4 Unlock
        if(HandleSaving.instance.IsLevelComplete("Dash_2"))
        {
            closedPathNarrativeTriggerObjects[4 - 2].SetActive(false);
        }

        //Level 5 Unlock
        if (HandleSaving.instance.IsLevelComplete("SlingShot_2"))
        {
            Debug.Log("Turn Off Door");
            area5Door.gameObject.SetActive(false);
            closedPathNarrativeTriggerObjects[5 - 2].SetActive(false);
        }
    }
}
