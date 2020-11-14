/*
* Jake Buri
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

    private bool breaking;
    private GrapplingGun grapplingGun;
    private MakeSpotNotGrappleable notGrappleableReference;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        grapplingGun = FindObjectOfType<GrapplingGun>();
        notGrappleableReference = FindObjectOfType<MakeSpotNotGrappleable>();
        breaking = false;
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
        breaking = true;
        yield return new WaitForSeconds(breakTime);

        if (gameObject == grapplingGun.GetCurrentGrappledObject())
        {
            //Force the Grapple to Stop
            //For protection, add this check in GrappleGun.cs
            grapplingGun.StopGrapple();
        }

        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Renderer>().enabled = false;
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

