using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        wizardAnimator = GetComponent<Animator>();
        wizardTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator MoveWizard()
    {
        WizardDoneWalking = false;
        float timeElapsed = 0f;

        while (!WizardDoneWalking)
        {

            wizardTransform.localRotation = Quaternion.identity;

            timeElapsed += Time.deltaTime;

            if (timeElapsed < walkingTime)
            {
                //wizardTransform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
                WizardAnimator.SetBool("isWalking", true);
            }
            else if(timeElapsed >= walkingTime)
            {
                WizardDoneWalking = true;
                WizardAnimator.SetBool("isWalking", false);
            }

            Debug.Log(WizardAnimator.GetBool("isWalking"));
            yield return null;

        }

        Debug.Log("Corotuine is done...");
        StopCoroutine(MoveWizard());

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
