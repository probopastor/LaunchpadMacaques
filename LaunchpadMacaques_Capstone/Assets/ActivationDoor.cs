using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationDoor : MonoBehaviour
{
    private PressureButton[] buttonsInScene;

    private void Awake()
    {
        buttonsInScene = FindObjectsOfType<PressureButton>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
