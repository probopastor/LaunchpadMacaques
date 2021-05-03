﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
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
    private float backwardsAnimationEndTime = 37f;
    private float currentBackwardsAnimationTime = 0f;

    private bool backwardsAnimationInProgress = false;
    private bool endBackwardsAnimation = false;

    private void Awake()
    {
        wizIntroManager = FindObjectOfType<WizardInteractionsManager>();
        wizardAnimator = GetComponent<Animator>();
        wizardTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if(backwardsAnimationInProgress)
        {
            //if(wizardAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > backwardsAnimationEndTime)
            //{
            //    endBackwardsAnimation = true;
            //}

            if((currentBackwardsAnimationTime < backwardsAnimationEndTime) && !endBackwardsAnimation)
            {
                currentBackwardsAnimationTime += 0.02f;
            }
            else
            {
                endBackwardsAnimation = true;
            }
        }
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
        backwardsAnimationInProgress = true;

       // WizardAnimator.SetBool("turnAround", true);

        //Debug.Log("turn around is: " + WizardAnimator.GetBool("isTurning"));

        //while (!endBackwardsAnimation)
        //{
        //    yield return null;
        //}

        transform.Rotate(new Vector3(0, 180, 0));
        //wizardAnimator.SetBool("turnAround", false);

        //yield return new WaitForSeconds(WizardAnimator.GetCurrentAnimatorStateInfo(0).length + WizardAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        //WizardAnimator.SetBool("turnAround", false);

        StartCoroutine(MoveWizardForward());

        //Debug.Log("Make Walk backward Coroutine do more stuffs......");

        backwardsAnimationInProgress = false;

        yield return null;
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
