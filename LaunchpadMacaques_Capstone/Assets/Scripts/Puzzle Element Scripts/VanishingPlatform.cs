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

    [SerializeField, Tooltip("This is whether the platform is visible or not.")]
    private bool isVisible = true;

    private GrapplingGun grappleGun;

    private VanishingManager vanishingManager;

    // Start is called before the first frame update
    void Start()
    {
        vanishingManager = FindObjectOfType<VanishingManager>();

        grappleGun = FindObjectOfType<GrapplingGun>();

        if(vanishingManager.GetPrimaryVanishingPlatforms() != null)
        {
            foreach (GameObject vaninshingPlatform in vanishingManager.GetPrimaryVanishingPlatforms())
            {
                isVisible = true;
                StartCoroutine(Vanish(vanishingManager.GetPrimaryVanishingPlatforms()));
            }
        }

        if(vanishingManager.GetSecondaryVanishingPlatforms() != null)
        {
            foreach (GameObject vaninshingPlatform in vanishingManager.GetSecondaryVanishingPlatforms())
            {
                isVisible = false;
                StartCoroutine(Vanish(vanishingManager.GetSecondaryVanishingPlatforms()));
            }
        }
    }

    /// <summary>
    /// A Coroutine that handles the disappearing and reappearing of the platforms.
    /// </summary>
    /// <returns></returns>
    IEnumerator Vanish(List<GameObject> desiredList)
    {
        if(isVisible)
        {
            Debug.Log("Dissappearing...");

            for (int index = 0; index <= desiredList.Count - 1; index++)
            {
                if (grappleGun.IsGrappling() && (vanishingManager.GetPrimaryVanishingPlatforms()[index]))
                {
                    grappleGun.StopGrapple();
                }
            }

            //if (grappleGun.IsGrappling() && (grappleGun.GetCurrentGrappledObject() == gameObject))
            //{
            //    grappleGun.StopGrapple();
            //}

            Disappear(desiredList);

            yield return new WaitForSecondsRealtime(timerValue);

            isVisible = false;

            StartCoroutine(Vanish(desiredList));
        }
        else if(!isVisible)
        {
            Debug.Log("Reappearing!!");

            Reappear(desiredList);

            yield return new WaitForSecondsRealtime(timerValue);

            isVisible = true;

            StartCoroutine(Vanish(desiredList));
        }
    }

    /// <summary>
    /// Makes the game object disappear.
    /// </summary>
    public void Disappear(List<GameObject> desiredList)
    {
        foreach (GameObject platform in desiredList)
        {
            platform.GetComponent<MeshRenderer>().enabled = false;
            platform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Makes the game object reappear.
    /// </summary>
    public void Reappear(List<GameObject> desiredList)
    {
        foreach (GameObject platform in desiredList)
        {
            platform.GetComponent<MeshRenderer>().enabled = true;
            platform.GetComponent<BoxCollider>().enabled = true;
        }
    }

}
