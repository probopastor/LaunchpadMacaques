using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using UnityEditorInternal;
using UnityEngine;

public class WizardInteraction : MonoBehaviour
{
    [SerializeField]
    private string interactionName = "Default";

    [SerializeField]
    private Animator wizardAnimator;
    private Transform wizardTransform;

    [SerializeField]
    private float walkingTime = 5f;

    private bool wizardDoneWalking = false;

    private WizardInteractionsManager wizIntroManager;

    private void Awake()
    {
        wizIntroManager = FindObjectOfType<WizardInteractionsManager>();
        wizardAnimator = GetComponent<Animator>();
        wizardTransform = GetComponent<Transform>();
    }

    public IEnumerator MoveWizardForward()
    {
        WizardDoneWalking = false;
        float timeElapsed = 0f;

        while (!WizardDoneWalking)
        {

            //wizardTransform.localRotation = Quaternion.identity;

            timeElapsed += Time.deltaTime;

            if (timeElapsed < walkingTime)
            {
                WizardAnimator.SetBool("isWalking", true);
            }
            else if (timeElapsed >= walkingTime)
            {
                WizardDoneWalking = true;
                WizardAnimator.SetBool("isWalking", false);
            }

            yield return null;

        }

        Debug.Log("Coroutine is done...");

        StopCoroutine(MoveWizardForward());

    }

    public IEnumerator MoveWizardBackwards()
    {

        wizardAnimator.SetBool("turnAround", true);

        //Debug.Log("turn around is: " + WizardAnimator.GetBool("isTurning"));

        while (wizardAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
        {
            Debug.Log("turn around is: " + WizardAnimator.GetBool("isTurning"));
            yield return null;
        }

        wizardAnimator.SetBool("turnAround", false);

        Debug.Log("turn around is: " + WizardAnimator.GetBool("isTurning"));

        StartCoroutine(MoveWizardForward());

        //Debug.Log("Make Walk backward Coroutine do more stuffs......");

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
