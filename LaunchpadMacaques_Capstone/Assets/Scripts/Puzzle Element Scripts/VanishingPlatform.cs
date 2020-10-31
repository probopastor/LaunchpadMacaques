/* 
* (Launchpad Macaques - [Neon Oblivion]) 
* (CJ Green) 
* (VanishingPlatform.cs) 
* (This script handles the functionality of the platforms disappearing and reappearing.) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{
    [SerializeField, Tooltip("This is the timer for how long the timer will dissappear and reappear for.")]
    private float timerValue = 2.5f;

    private bool isVisible = true;

    private GrapplingGun grappleGun;

    private VanishingManager vanishingManager;

    private List<GameObject> firstList;
    private List<GameObject> secondList;

    // Start is called before the first frame update
    void Start()
    {
        AssignValues();
        StartCoroutine(Vanish());

        //foreach (GameObject vaninshingPlatform in vanishingManager.GetPrimaryVanishingPlatforms())
        //{
        //    isVisible = true;
        //    StartCoroutine(Vanish(vanishingManager.GetPrimaryVanishingPlatforms()));
        //}
        //foreach (GameObject vaninshingPlatform in vanishingManager.GetSecondaryVanishingPlatforms())
        //{
        //    isVisible = false;
        //    StartCoroutine(Vanish(vanishingManager.GetSecondaryVanishingPlatforms()));
        //}
    }

    private void AssignValues()
    {
        vanishingManager = FindObjectOfType<VanishingManager>();
        grappleGun = FindObjectOfType<GrapplingGun>();

        firstList = vanishingManager.GetPrimaryVanishingPlatforms();
        secondList = vanishingManager.GetSecondaryVanishingPlatforms();

        Disappear(secondList);
        Reappear(firstList);

        isVisible = true;
    }

    /// <summary>
    /// A Coroutine that handles the disappearing and reappearing of the platforms.
    /// </summary>
    /// <returns></returns>
    IEnumerator Vanish()
    {
        //if (isVisible)
        //{
        //    for (int index = 0; index <= desiredList.Count - 1; index++)
        //    {
        //        if (grappleGun.IsGrappling() && (vanishingManager.GetPrimaryVanishingPlatforms()[index]))
        //        {
        //            grappleGun.StopGrapple();
        //        }
        //    }

        //    Disappear(desiredList);

        //    yield return new WaitForSecondsRealtime(timerValue);

        //    isVisible = false;

        //    StartCoroutine(Vanish(desiredList));
        //}
        //else if (!isVisible)
        //{
        //    Reappear(desiredList);

        //    yield return new WaitForSecondsRealtime(timerValue);

        //    isVisible = true;

        //    StartCoroutine(Vanish(desiredList));
        //}

        yield return new WaitForSecondsRealtime(timerValue);

        if (isVisible)
        {
            isVisible = false;
            Disappear(firstList);
            Reappear(secondList);
        }
        else if(!isVisible)
        {
            isVisible = true;
            Disappear(secondList);
            Reappear(firstList);
        }

        StartCoroutine(Vanish());
    }

    /// <summary>
    /// Makes game objects in list disappear.
    /// </summary>
    public void Disappear(List<GameObject> desiredList)
    {
        foreach (GameObject platform in desiredList)
        {
            //platform.GetComponent<MeshRenderer>().enabled = false;
            //platform.GetComponent<BoxCollider>().enabled = false;

            platform.SetActive(false);
            if(grappleGun.IsGrappling() && grappleGun.GetCurrentGrappledObject() == platform)
            {
                grappleGun.StopGrapple();
            }
        }
    }

    /// <summary>
    /// Makes game objects in list reappear.
    /// </summary>
    public void Reappear(List<GameObject> desiredList)
    {
        foreach (GameObject platform in desiredList)
        {
            //platform.GetComponent<MeshRenderer>().enabled = true;
            //platform.GetComponent<BoxCollider>().enabled = true;

            platform.SetActive(true);
        }
    }
}
