/*
* Jake Buri, William Nomikos
* GrapplePoint.cs
* Removes a grapple point if used
*/
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplePoint : MonoBehaviour
{
    //Variables
    [SerializeField, Tooltip("The amount of time it takes for a grapple point to vanish after grappled to (In Seconds). ")] private float breakTime = 3f;
    private float remainingBreakTime = 0f;
    private float breakTimeRef = 0f;

    private bool breaking;
    private bool breakingNotStarted;
    private bool checkEnableConditionsOnce;
    private GrapplingGun grapplingGun;
    private MakeSpotNotGrappleable notGrappleableReference;

    private void Awake()
    {
        breakingNotStarted = true;
        checkEnableConditionsOnce = false;
        grapplingGun = FindObjectOfType<GrapplingGun>();
        notGrappleableReference = FindObjectOfType<MakeSpotNotGrappleable>();
        breaking = false;
        breakTimeRef = breakTime;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if(!breakingNotStarted && !checkEnableConditionsOnce)
        {
            checkEnableConditionsOnce = true;

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
        this.gameObject.GetComponent<Collider>().enabled = true;
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

    /// <summary>
    /// Coroutine breaks the breakable grapple point after a set period of time. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBreak()
    {
        breakingNotStarted = false;
        breaking = true;
        //yield return new WaitForSeconds(breakTime);

        for (float timeLeft = breakTime; timeLeft > 0; timeLeft -= Time.deltaTime)
        {
            remainingBreakTime = timeLeft;
            yield return null;
        }

        if (gameObject == grapplingGun.GetCurrentGrappledObject())
        {
            //Force the Grapple to Stop
            //For protection, add this check in GrappleGun.cs
            grapplingGun.StopGrapple();
        }

        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Renderer>().enabled = false;
        breakingNotStarted = true;

        breakTime = breakTimeRef;
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

