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
            //IsWizardIntroPlaying = true;
            //StartCoroutine(WizardIntro());
            StartCoroutine(wizardInteraction.MoveWizardBackwards());
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Hub")
        {
            Debug.Log("We are in the Hub");
        }
        //StartCoroutine(wizardInteraction.MoveWizard());
        //wizardInteraction.MoveWizard();
    }

    // Update is called once per frame
    void Update()
    {

        LimitMovement();

        if (narrativeHandler.CurrentDialogueLine != null)
        {
            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.text);
            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.GetLineType());
        }
        else
        {
            Debug.Log("There is no dialogue running at the moment...");
        }

        //if(!IsWizardIntroPlaying)
        //{
        //    Debug.Log("Wizard Intro is done, Wizard outro is starting...");
        //    IsWizardOutroPlaying = true;
        //    StartCoroutine(WizardOutro());
        //}

    }

    private IEnumerator WizardIntro()
    {

        if (IsWizardIntroPlaying)
        {
            // Opens the portal and then wait two seconds.
            wizardPortal.PortalStatesReference = Portal.PortalStates.OPEN;

            yield return new WaitForSeconds(5f);

            //wizardInteraction.MoveWizard();

            // Start Moving the Wizard
            StartCoroutine(wizardInteraction.MoveWizardForward());

            IsWizardIntroPlaying = false;

        }

        StopCoroutine(WizardIntro());
        Debug.Log("Wizard Intro has stopped!");

    }

    private IEnumerator WizardOutro()
    {

        if(IsWizardOutroPlaying)
        {

            StartCoroutine(wizardInteraction.MoveWizardBackwards());


            if (WizardPortalReference.WizardCollisions == 2)
            {
                wizardGameObject.SetActive(false);
                wizardPortal.PortalStatesReference = Portal.PortalStates.CLOSE;
                wizardInteraction.StopAllCoroutines();
                yield return null;
            }


        }

        StopCoroutine(WizardOutro());

        //Turn Wizard Around and walk back through the portal.

        // Make the wizard disappear.

        //Turn off the protal and then start Narrative Doalogue.
    }

    private void LimitMovement()
    {
        
        if(IsWizardIntroPlaying)
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

    #endregion 

}
