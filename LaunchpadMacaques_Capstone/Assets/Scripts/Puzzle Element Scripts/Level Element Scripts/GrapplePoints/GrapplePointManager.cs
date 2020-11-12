/*
* Jake Buri
* GrapplePointManager.cs
* Handles how many grapples you have left and checks if one of the grapple points has been used
*/
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrapplePointManager : MonoBehaviour
{
    [SerializeField]
    private GrapplingGun grapplingGun;

    private List<GrapplePoint> grapplePoints;

    //How many grapples the player has left
    private int remainingGrapples;

    //How many points are still enabled
    private int remainingPoints;

    /*UI Subject display
    public Text remainingGrapplesText;
    public Text remainingPointsText;
    */

    private GrapplePoint[] allPoints;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        //Initialize List
        grapplePoints = new List<GrapplePoint>();

        //NEED TO IMPLEMENT REMAINING GRAPPLE FUNCTION//
        //remainingGrapples = player.GetComponent<Matt_PlayerMovement>().GetRemainingGrapples();

        //Add grappling points to the list
        foreach (GameObject thisGP in GameObject.FindGameObjectsWithTag("GrapplePoint"))
        {
            remainingPoints++;
            GrapplePoint thisGPScript = thisGP.GetComponent<GrapplePoint>();
            grapplePoints.Add(thisGPScript);
        }

        allPoints = FindObjectsOfType<GrapplePoint>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Display UI
        //remainingGrappleText.text = remainingGrappleText.ToString();
        //remainingPointsText.text = remainingPoints.Count.ToString();

        //Starts to corrupt the grappling point
        if (grapplingGun.IsGrappling())
        {
            if (grapplingGun.GetGrappleRayhit().collider)
            {
                GameObject currentCube = grapplingGun.GetGrappleRayhit().transform.gameObject;

                if(grapplingGun.GetCurrentGrappledObject() != null)
                {
                    if(currentCube == grapplingGun.GetCurrentGrappledObject())
                    {
                        if (currentCube.GetComponent<GrapplePoint>() != null)
                        {
                            currentCube.GetComponent<GrapplePoint>().StartCoroutine("Countdown");
                        }
                    }
                }
      
            }

        }

        //Checks if any grappling points are being corrputed
        Notify();
    }

    /// <summary>
    /// Not used. Update the remaining grapples.
    /// </summary>
    /// <param name="grapples"></param>
    public void SetTotalGrapples(int grapples)
    {
        remainingGrapples = grapples;
    }

    /// <summary>
    /// Detect if an grapplePoints is breaking and starts to remove it
    /// </summary>
    public void Notify()
    {
        for(int i = 0; i < grapplePoints.Count; i++)
        {
            GrapplePoint gp = grapplePoints.ElementAt(i).GetComponent<GrapplePoint>();

            if ((gp as GrapplePoint).isBreaking() == true)
            {
                //Remove renderer and mesh
                gp.UpdateSubject();

                if (gp.gameObject == grapplingGun.GetCurrentGrappledObject())
                {
                    //Force the Grapple to Stop
                    //For protection, add this check in GrappleGun.cs
                    grapplingGun.StopGrapple();
                }

                //Remove grapplePoints to update remaining
                grapplePoints.Remove(gp);

                i--;
            }
 
        }
    }

    /// <summary>
    /// Get the remaining number of grapple points active
    /// </summary>
    /// <returns></returns>
    public int GetRemainingPoints()
    {
        return remainingPoints;
    }

    /// <summary>
    /// Not Used. Get the remaining number of grapples available
    /// </summary>
    /// <returns></returns>
    public int GetRemainingGrapples()
    {
        return remainingGrapples;
    }

    public void TurnOnPoints()
    {
        foreach(GrapplePoint gp in allPoints)
        {
            gp.TurnOnPoint();

            //resets corruptable shader on obj
            try
            {

                gp.gameObject.GetComponent<CorruptableObject>().UncorruptInstantly();

            }
            catch (System.Exception ex)
            {
                //exception (probably not needed to throw)
            }
        }


        grapplePoints.Clear();
        foreach (GameObject thisGP in GameObject.FindGameObjectsWithTag("GrapplePoint"))
        {
            remainingPoints++;
            GrapplePoint thisGPScript = thisGP.GetComponent<GrapplePoint>();
            grapplePoints.Add(thisGPScript);
        }
    }
}
