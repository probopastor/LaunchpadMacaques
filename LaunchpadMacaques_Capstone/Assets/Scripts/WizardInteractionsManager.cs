using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class WizardInteractionsManager : MonoBehaviour
{

    private Vector3 wizardPosition = new Vector3(0, 0, 0);
    private bool isWizardTalking = false;

    private WizardInteraction wizardInteraction;

    // Start is called before the first frame update
    void Start()
    {
        wizardInteraction = FindObjectOfType<WizardInteraction>();
        //StartCoroutine(WizardIntro());
        wizardInteraction.MoveWizard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WizardIntro()
    {
        // Open Portal...
        // Portal.Open() or something like that.

        yield return new WaitForSeconds(2f);

        wizardInteraction.MoveWizard();

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
