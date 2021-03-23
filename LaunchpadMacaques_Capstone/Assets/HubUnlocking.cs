using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubUnlocking : MonoBehaviour
{
    [SerializeField] ActivationDoor area5Door;

    public ActivationDoor Area5Door { get => area5Door; set => area5Door = value; }

    private void Start()
    {
        if (HandleSaving.instance.IsLevelComplete("SlingShot_2"))
        {
            Debug.Log("Turn Off Door");
            area5Door.gameObject.SetActive(false);
        }
    }
}
