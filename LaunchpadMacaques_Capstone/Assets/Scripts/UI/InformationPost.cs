/* 
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

    private bool playerInRange;

    private NarrativeTriggerHandler narrative;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        toggleFunctionality = true;
        SetInformation();

        narrative = FindObjectOfType<NarrativeTriggerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (informationText.text.Equals(information))
            {
                playerInRange = false;
            }

            else
            {
                informationText.text = information;
            }
    
        }
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
                narrative.TurnOffDialouge();
                informationText.text = information;
            }
            else if (!isActive)
            {
                informationText.text = " ";
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !playerInRange)
        {
            
            if (informationText)
            {
                informationText.gameObject.transform.parent.gameObject.SetActive(true);
                informationText.text = "press E to read";
         
                playerInRange = true;
                narrative.TurnOffDialouge();
            }
  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            informationText.gameObject.transform.parent.gameObject.SetActive(false);
            playerInRange = false;
        }
    }

    public bool GetTutorialCanvas()
    {
        return informationText.gameObject.transform.parent.gameObject.activeSelf;
    }
}
