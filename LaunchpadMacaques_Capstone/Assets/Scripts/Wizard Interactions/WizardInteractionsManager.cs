using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class WizardInteractionsManager : MonoBehaviour
{

    private Vector3 wizardPosition = new Vector3(0, 0, 0);
    private bool isWizardTalking = false;

    private bool wizardIntroIsPlaying = false;
    private bool wizardOutroIsPlaying = false;

    [SerializeField]
    private List<string> interactionNames = new List<string>();

    private GameObject wizardGameObject;
    private WizardInteraction wizardInteraction;
    private Portal wizardPortal;

    private NarrativeTriggerHandler narrativeHandler;
    private Matt_PlayerMovement movementScript;

    private void Awake()
    {
        wizardGameObject = GameObject.FindGameObjectWithTag("Wizard");
        wizardPortal = FindObjectOfType<Portal>();
        narrativeHandler = FindObjectOfType<NarrativeTriggerHandler>();
        wizardInteraction = FindObjectOfType<WizardInteraction>();
        movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Matt_PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Hub")
        {
            Debug.Log("We are not in the hub");

            if (wizardInteraction.InteractionName == interactionNames[0]) // UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Hub" && 
            {

                IsWizardIntroPlaying = true;
                StartCoroutine(WizardIntro());

            }
            else if (wizardInteraction.InteractionName == interactionNames[2])
            {
                IsWizardIntroPlaying = true;
                StartCoroutine(WizardIntro());
            }

            //IsWizardIntroPlaying = true;
            //StartCoroutine(WizardIntro());
            //StartCoroutine(wizardInteraction.MoveWizardBackwards());
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Hub")
        {
            Debug.Log("We are in the Hub");
        }
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(wizardPortal.WizardCollisions);

        LimitMovement();

        //if (wizardInteraction.InteractionName == interactionNames[0] || wizardInteraction.InteractionName == interactionNames[2])
        //{
        //    if (narrativeHandler.CurrentDialogueLine != null)
        //    {

        //        if (narrativeHandler.CurrentDialogueLine.GetLineType() == Dialogue.Line.Type.NarrationLine)
        //        {

        //            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.text);
        //            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.GetLineType());

        //            if (IsWizardTalking)
        //            {
        //                wizardInteraction.WizardAnimator.SetBool("isTalking", false);
        //            }

        //            if (!IsWizardIntroPlaying)
        //            {
        //                Debug.Log("Start Wizard Outro Coroutine");
        //                IsWizardOutroPlaying = true;
        //                StartCoroutine(WizardOutro());
        //            }
        //        }

        //    }
        //}

    }

    public IEnumerator WizardIntro()
    {

        if (IsWizardIntroPlaying)
        {
            // Opens the portal and then wait two seconds.
            wizardPortal.PortalStatesReference = Portal.PortalStates.OPEN;

            yield return new WaitForSeconds(2f);

            // Start Moving the Wizard
            StartCoroutine(wizardInteraction.MoveWizardForward());

            yield return new WaitForSeconds(1.5f);

            IsWizardIntroPlaying = false;

        }

        StopCoroutine(WizardIntro());
        Debug.Log("Wizard Intro has stopped!");

    }

    public IEnumerator WizardOutro()
    {

        Debug.Log("Wizard Outro Coroutine started...");

        //if (IsWizardOutroPlaying)
        //{

        StartCoroutine(wizardInteraction.MoveWizardBackwards());


        if (WizardPortalReference.WizardCollisions == 2)
        {
            wizardGameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            wizardPortal.PortalStatesReference = Portal.PortalStates.CLOSE;
            IsWizardOutroPlaying = false;
            wizardInteraction.StopAllCoroutines();

        }


        //}

        StopCoroutine(WizardOutro());
    }

    private void LimitMovement()
    {

        if (IsWizardIntroPlaying)
        {
            movementScript.SetPlayerCanMove(false);
        }

    }

    #region Getters and Setters
    public bool IsWizardTalking
    {
        get
        {
            return isWizardTalking;
        }
        set
        {
            isWizardTalking = value;
        }
    }

    public bool IsWizardIntroPlaying
    {
        get
        {
            return wizardIntroIsPlaying;
        }
        set
        {
            wizardIntroIsPlaying = value;
        }
    }

    public bool IsWizardOutroPlaying
    {
        get
        {
            return wizardOutroIsPlaying;
        }
        set
        {
            wizardOutroIsPlaying = value;
        }
    }

    public Portal WizardPortalReference
    {
        get
        {
            return wizardPortal;
        }
    }

    public WizardInteraction WizardInteractionReference
    {
        get
        {
            return wizardInteraction;
        }
    }

    #endregion 

}
