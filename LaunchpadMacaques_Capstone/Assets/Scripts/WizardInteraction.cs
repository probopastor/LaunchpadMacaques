using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class WizardInteraction : MonoBehaviour
{
    [SerializeField]
    private string interactionName = "Default";

    private Animator wizardAnimator;
    private Transform wizardTransform;

    [SerializeField]
    private float walkSpeed = 5.0f;

    float walkingTime = 0f;

    private bool wizardDoneWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        wizardAnimator = GetComponent<Animator>();
        wizardTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(walkingTime);
    }

    public void MoveWizard()
    {
        walkingTime = 0f;
        WizardDoneWalking = false;

        while(!WizardDoneWalking)
        {

            walkingTime += Time.deltaTime;

            if (walkingTime < 2f)
            {
                wizardTransform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            }
            else if(walkingTime >= 2f)
            {
                WizardDoneWalking = true;
            }
        }

        Debug.Log(walkingTime);

    }

    #region Getters and Setters
    public string InteractionName
    {
        get
        {
            return interactionName;
        }

        set
        {
            interactionName = value;
        }
    }

    public bool WizardDoneWalking
    {
        get
        {
            return wizardDoneWalking;
        }

        set
        {
            wizardDoneWalking = value;
        }
    }

    public Animator WizardAnimator
    {
        get
        {
            return wizardAnimator;
        }
    }

    #endregion

}
