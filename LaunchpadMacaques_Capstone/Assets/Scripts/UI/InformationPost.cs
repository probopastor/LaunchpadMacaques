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

    [Header("Note Details")]
    [SerializeField, Tooltip("Mark true if this post is a note object and should function like a note. Otherwise keep false.")] private bool isNote = false;
    [Tooltip("The note panel. Only applicable for notes. ")] private GameObject notePanel;
    [Tooltip("The note TMP. Only applicable for notes. ")] private TextMeshProUGUI noteText;

    [Tooltip("The note panel tag. Only applicable for notes. ")] private string notePanelTag;
    [Tooltip("The note TMP tag. Only applicable for notes. ")] private string noteTMPTag;

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

        informationText = GameObject.Find("New Standard Scene Group/Canvas Group/StandardCanvas/Info Text/Information Post Text").GetComponent<TextMeshProUGUI>();

        notePanel = GameObject.FindGameObjectWithTag("NotePanel");
        noteText = GameObject.FindGameObjectWithTag("NoteTMP").GetComponent<TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        isActive = false;
        SetInformation();
        narrative = FindObjectOfType<NarrativeTriggerHandler>();
    }

    private void Update()
    {
        Debug.Log("Note Panel: " + notePanel.gameObject.name);
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
            noteText.text = " ";
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
                if (!isNote)
                {
                    narrative.CancelDialouge();
                    informationText.text = information;
                }
                else if (isNote)
                {
                    notePanel.SetActive(true);
                    noteText.text = information;
                }
            }
            else if (!isActive)
            {
                if (!isNote)
                {
                    informationText.text = " ";
                }
                else if (isNote)
                {
                    notePanel.SetActive(false);
                    noteText.text = " ";
                }
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

                if (!isNote)
                {
                    informationText.gameObject.transform.parent.gameObject.SetActive(true);
                    informationText.text = information;
                    narrative.CancelDialouge();
                }
                else if (isNote)
                {
                    notePanel.SetActive(true);
                    noteText.gameObject.SetActive(true);
                    noteText.text = information;
                }

                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isNote)
            {
                informationText.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else if (isNote)
            {
                notePanel.SetActive(false);
                noteText.gameObject.SetActive(false);
            }

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
