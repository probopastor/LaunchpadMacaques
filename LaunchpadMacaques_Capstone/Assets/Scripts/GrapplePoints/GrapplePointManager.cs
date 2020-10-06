/*
* Jake Buri
* GrapplePointManager.cs
* Handles how many grapples you have left and checks if one of the grapple points has been used
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.XR.Haptics;

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


    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        //Display UI
        //remainingGrappleText.text = remainingGrappleText.ToString();
        //remainingPointsText.text = remainingPoints.Count.ToString();

        //Starts to corrupt the grappling point
        if (grapplingGun.IsGrappling())
        {
            GameObject currentCube = grapplingGun.GetGrappleRayhit().transform.gameObject;
            currentCube.GetComponent<GrapplePoint>().StartCoroutine("Countdown");
        }

        //Checks if any grappling points are being corrputed
        Notify();
    }

    //NOT USED
    //Update the remaining grapples
    public void SetTotalGrapples(int grapples)
    {
        remainingGrapples = grapples;
    }

    //Detect if an grapplePoints is breaking and starts to remove it
    public void Notify()
    {
        foreach (GrapplePoint gp in grapplePoints)
        {
            if ((gp as GrapplePoint).isBreaking() == true)
            {
                //Remove renderer and mesh
                gp.UpdateSubject();

                //Force the Grapple to Stop
                //For protection, add this check in GrappleGun.cs
                grapplingGun.StopGrapple();

                //Remove grapplePoints to update remaining
                grapplePoints.Remove(gp);
            }
        }
    }

    //Get the remaining number of grapple points active
    public int GetRemainingPoints()
    {
        return remainingPoints;
    }

    //NOT USED
    //Get the remaining number of grapples available
    public int GetRemainingGrapples()
    {
        return remainingGrapples;
    }
}
