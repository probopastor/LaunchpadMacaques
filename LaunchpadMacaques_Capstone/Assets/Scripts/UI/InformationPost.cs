﻿/* 
* Launchpad Macaques - Trial & Error
* William Nomikos
* InformationPosts.cs
* Displays information when the player triggers an information post. 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationPost : MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField, TextArea, Tooltip("The information that will be displayed when player is in the information post's radius. ")] private string information;

    private bool isActive;
    private bool toggleFunctionality;

    #endregion

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
        //if(Input.GetKeyDown(KeyCode.O))
        //{
        //    if(toggleFunctionality)
        //    {
        //        toggleFunctionality = false;
        //    }
        //    else if(!toggleFunctionality)
        //    {
        //        toggleFunctionality = true;
        //    }

        //    SetInformation();
        //}
    }

    private void OnEnable()
    {
        SetInformation();
    }

    private void OnDisable()
    {
        informationText.text = " ";
    }

    /// <summary>
    /// Sets the text of the information post when the player enters or leaves its radius.
    /// </summary>
    private void SetInformation()
    {
        if(informationText != null)
        {
            if (isActive)
            {
                informationText.text = information;
            }
            else if (!isActive)
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

    //public void SetActivityStatus(bool active)
    //{
    //    isActive = active;
    //    SetInformation();
    //}
}
