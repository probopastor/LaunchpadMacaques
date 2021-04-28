using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class WizardInteractionsManager : MonoBehaviour
{

    private Vector3 wizardPosition = new Vector3(0, 0, 0);
    private bool isWizardTalking = false;

    [SerializeField]
    private List<string> interactionNames = new List<string>();

    private WizardInteraction wizardInteraction;

    private NarrativeTriggerHandler narrativeHandler;

    // Start is called before the first frame update
    void Start()
    {
        narrativeHandler = FindObjectOfType<NarrativeTriggerHandler>();
        wizardInteraction = FindObjectOfType<WizardInteraction>();

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Hub")
        {
            Debug.Log("We are not in the hub");
            StartCoroutine(WizardIntro());
        }
        else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Hub")
        {
            Debug.Log("We are in the Hub");
        }
        //StartCoroutine(wizardInteraction.MoveWizard());
        //wizardInteraction.MoveWizard();
    }

    // Update is called once per frame
    void Update()
    {

        if(narrativeHandler.CurrentDialogueLine != null)
        {
            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.text);
            Debug.Log("The current line of dialogue is: " + narrativeHandler.CurrentDialogueLine.GetLineType());
        }
        else
        {
            Debug.Log("There is no dialogue running at the moment...");
        }

        if(wizardInteraction.WizardDoneWalking == true)
        {
            Debug.Log("The wizard is now done walking");
        }
        else
        {
            Debug.Log("The wizard isn't done walking...");
        }
    }

    private IEnumerator WizardIntro()
    {

        //switch(wizardInteraction.InteractionName)
        //{

        //}


        // Open Portal...
        // Portal.Open() or something like that and then wait a couple seconds before the wizard walks through.

        yield return new WaitForSeconds(2f);

        //wizardInteraction.MoveWizard();
        StartCoroutine(wizardInteraction.MoveWizard());

        //yield return null;
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

    #endregion 

}
