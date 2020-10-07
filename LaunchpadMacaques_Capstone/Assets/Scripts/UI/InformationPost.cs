using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationPost : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI informationText;

    [SerializeField, TextArea] private string information;

    private bool isActive;
    private bool toggleFunctionality;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        toggleFunctionality = true;
        SetInformation();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            if(toggleFunctionality)
            {
                toggleFunctionality = false;
            }
            else if(!toggleFunctionality)
            {
                toggleFunctionality = true;
            }

            SetInformation();
        }
    }

    private void SetInformation()
    {
        if(informationText != null)
        {
            if (isActive && toggleFunctionality)
            {
                informationText.text = information;
            }
            else if (!isActive || !toggleFunctionality)
            {
                informationText.text = " ";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isActive = true;
            SetInformation();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            isActive = true;
            SetInformation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isActive = false;
            SetInformation();
        }
    }
}
