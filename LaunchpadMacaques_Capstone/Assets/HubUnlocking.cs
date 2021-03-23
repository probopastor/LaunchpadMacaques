using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubUnlocking : MonoBehaviour
{
    [SerializeField] ActivationDoor area5Door = null;
    private void Start()
    {
        if (HandleSaving.instance.IsLevelComplete("SlingShot_2"))
        {
            Debug.Log("Turn Off Door");
            area5Door.gameObject.SetActive(false);
        }
    }
}
