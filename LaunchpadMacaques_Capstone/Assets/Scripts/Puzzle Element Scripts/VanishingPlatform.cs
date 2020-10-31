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

    //private bool isVisible = true;

    private GrapplingGun grappleGun;

    private VanishingManager vanishingManager;

    private List<VanishingManager.PlatformTracks> platformTracks;

    private int currentEnabledTrack;

    // Start is called before the first frame update
    void Start()
    {
        AssignValues();
        StartCoroutine(Vanish());
    }

    private void AssignValues()
    {
        vanishingManager = FindObjectOfType<VanishingManager>();
        grappleGun = FindObjectOfType<GrapplingGun>();
        platformTracks = vanishingManager.GetPlatformTracks();

        currentEnabledTrack = 0;
        EnableDisableObjects();
    }

    /// <summary>
    /// A Coroutine that handles the disappearing and reappearing of the platforms.
    /// </summary>
    /// <returns></returns>
    IEnumerator Vanish()
    {
        yield return new WaitForSecondsRealtime(timerValue);

        currentEnabledTrack++;
        if(currentEnabledTrack > platformTracks.Count - 1)
        {
            currentEnabledTrack = 0;
        }

        EnableDisableObjects();

        StartCoroutine(Vanish());
    }

    private void EnableDisableObjects()
    {
        for (int i = 0; i <= platformTracks.Count - 1; i++)
        {
            List<GameObject> objectsInPlatformTrack = platformTracks[i].gameObjects;

            if (i == currentEnabledTrack)
            {
                foreach (GameObject currentObject in objectsInPlatformTrack)
                {
                    currentObject.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject currentObject in objectsInPlatformTrack)
                {
                    currentObject.SetActive(false);

                    if (grappleGun.IsGrappling() && grappleGun.GetCurrentGrappledObject() == currentObject)
                    {
                        grappleGun.StopGrapple();
                    }
                }
            }
        }
    }
}
