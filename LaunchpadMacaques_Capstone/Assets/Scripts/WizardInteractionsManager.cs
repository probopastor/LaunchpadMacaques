using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class WizardInteractionsManager : MonoBehaviour
{

    private string levelName = "Default";

    private Vector3 wizardPosition = new Vector3(0, 0, 0);
    private bool isWizardTalking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public string LevelName
    {
        get
        {
            return levelName;
        }

        set
        {
            levelName = value;
        }
    }

    #endregion 

}
