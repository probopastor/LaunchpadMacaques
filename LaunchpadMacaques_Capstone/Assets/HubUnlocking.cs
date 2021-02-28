using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubUnlocking : MonoBehaviour
{
    [SerializeField] ActivationDoor area5Door;
    private void Start()
    {
        if (HandleSaving.instance.IsLevelComplete("Slingshot_2"))
        {
            Debug.Log("Turn Off Door");
            area5Door.gameObject.SetActive(false);
        }
    }
}
