/*
* Launchpad Macaques - Neon Oblivion
* Jake Buri, William Nomikos
* GrapplePoint.cs
* Handles behavior for Grapple Points that break after a period of time when grappled to.
*/

using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplePoint : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("The amount of time it takes for a grapple point to vanish after grappled to (In Seconds). ")] private float breakTime = 3f;
    [Tooltip("The remaining break time left in the breaking coroutine. ")] private float remainingBreakTime = 0f;
    [Tooltip("Stores the default break time. ")] private float breakTimeRef = 0f;

    [Tooltip("Bool is true if the object is in the process of breaking. False otherwise. ")] private bool breaking;
    [Tooltip("Bool is true if breaking has not been starting. Used to determine whether or not breaking should be continued on enable. ")] private bool breakingNotStarted;
    [Tooltip("Determines if On Enable enabled condition has already been checked. ")] private bool checkEnableConditionsOnce;
    [Tooltip("Grappling gun reference. ")] private GrapplingGun grapplingGun;
    [Tooltip("MakeSpotNotGrappleable reference. Used to uncorrupt grapple points on player respawn. ")] private MakeSpotNotGrappleable notGrappleableReference;
    #endregion 

    private void Awake()
    {
        breakingNotStarted = true;
        checkEnableConditionsOnce = false;
        grapplingGun = FindObjectOfType<GrapplingGun>();
        notGrappleableReference = FindObjectOfType<MakeSpotNotGrappleable>();
        breaking = false;
        breakTimeRef = breakTime;
    }

    private void OnEnable()
    {
        // Determines if the StartBreak() coroutine had been cancelled before it finished. 
        if (!breakingNotStarted && !checkEnableConditionsOnce)
        {
            checkEnableConditionsOnce = true;

            // If StartBreak() was cancelled before finishing, restart the coroutine at the remaining break time.
            breakTime = remainingBreakTime;
            Break();
        }
    }

    private void OnDisable()
    {
        checkEnableConditionsOnce = false;
    }

    /// <summary>
    /// Enables the breaking grapple point.  
    /// </summary>
    public void EnablePoint()
    {
        breaking = false;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
        this.gameObject.GetComponent<Renderer>().enabled = true;

        notGrappleableReference.UncorruptSingleObject(gameObject);
    }

    /// <summary>
    /// Breaks the breakable grapple point. 
    /// </summary>
    public void Break()
    {
        StartCoroutine(StartBreak());
    }

    public void StopBreaking()
    {
        breaking = false;
        StopCoroutine(StartBreak());
    }

    /// <summary>
    /// Coroutine breaks the breakable grapple point after a set period of time. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBreak()
    {
        // Breaking has now been started. This bool determines whether the coroutine finishes before being cancelled. 
        breakingNotStarted = false;

        // Breaking is now true. This bool prevents StartBreak() from being called multiple times before finishing.
        breaking = true;

        // If the breaktime is not finished, update the remainingBreakTime and return null.
        for (float timeLeft = breakTime; timeLeft > 0; timeLeft -= Time.deltaTime)
        {
            remainingBreakTime = timeLeft;
            yield return null;
        }

        // If the player is grappling to this object when its disabled, break their grapple. 
        if (gameObject == grapplingGun.GetCurrentGrappledObject())
        {
            grapplingGun.StopGrapple();
        }

        // Disable this object. 
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        this.gameObject.GetComponent<Renderer>().enabled = false;

        // Reset breakTime back to its default value in the scenario that breakTime was changed. 
        breakTime = breakTimeRef;

        // breakingNotStarted set to true, as the coroutine process is complete. 
        breakingNotStarted = true;
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Returns true if the breakable grapple point is breaking.
    /// </summary>
    /// <returns></returns>
    public bool isBreaking()
    {
        return breaking;
    }
}

