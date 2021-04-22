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
using UnityEngine.InputSystem;

public class InformationPost : A_InputType
{
    #region Variables

    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField, TextArea, Tooltip("The information that will be displayed when player is in the information post's radius. ")] private string information;


    private bool isActive;

    private bool playerInRange;

    private NarrativeTriggerHandler narrative;

    private string interactText = "Press E";

    private PlayerControlls controls;

    #endregion

    private void Awake()
    {
        controls = new PlayerControlls();

        controls.GamePlay.Interact.performed += InteractInput;
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        isActive = false;
        SetInformation();

        narrative = FindObjectOfType<NarrativeTriggerHandler>();
    }

    /// <summary>
    /// When the controller is connected/disconnected will update the Press XXX To Read Text
    /// </summary>
    public override void ChangeUI()
    {

        if (controllerDetected)
        {
            interactText = "Press<sprite=18>To Read";
        }

        else
        {
            interactText = "Press E To Read";
        }
    }

    private void InteractInput(InputAction.CallbackContext cxt)
    {
        //if (playerInRange)
        //{
        //    if (informationText.text.Equals(information))
        //    {
        //        playerInRange = false;
        //    }

        //    else
        //    {
        //        informationText.text = information;
        //    }

        //    UpdateUI();
        //}
    }

    private new void OnEnable()
    {
        base.OnEnable();
        controls.Enable();
        SetInformation();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }

        if (informationText != null)
        {
            informationText.text = " ";
        }
    }

    /// <summary>
    /// Sets the text of the information post when the player enters or leaves its radius.
    /// </summary>
    private void SetInformation()
    {
        if (informationText != null)
        {
            if (isActive)
            {
                narrative.CancelDialouge();
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
                ChangeUI();
                informationText.gameObject.transform.parent.gameObject.SetActive(true);
                informationText.text = information;

                playerInRange = true;
                narrative.CancelDialouge();
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

    #region Getters/Setters
    public bool GetTutorialCanvas()
    {
        return informationText.gameObject.transform.parent.gameObject.activeSelf;
    }

    public void TurnOffTutorialCanvas()
    {
        informationText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void SetInformationPost(string newString)
    {
        information = newString;
    }


    public string GetInformationPostTest()
    {
        return information;
    }

    public bool GetPlayerInRange()
    {
        return playerInRange;
    }

    public TextMeshProUGUI GetTextBox()
    {
        return informationText;
    }
    #endregion

}
