/* 
* Launchpad Macaques - Neon Oblivion
* CJ Green, William Nomikos 
* VanishingPlatform.cs)
* This script handles the functionality for platforms disappearing and reappearing. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{
    #region variables 
    [SerializeField, Tooltip("This is the timer for how long each group of vanishing objects will be enabled for. ")]
    private float timerValue = 2.5f;

    // Grappling Gun reference
    private GrapplingGun grappleGun;

    // Vanishing Manager reference
    private VanishingManager vanishingManager;

    // The platform tracks from Vanishing Manager
    private List<VanishingManager.PlatformTracks> platformTracks;

    // Index for the current track enabled 
    private int currentEnabledTrack = 0;

    // The pause manager in the scene. 
    private PauseManager pauseManagerReference;
    #endregion

    #region Start Functions
    // Start is called before the first frame update
    void Start()
    {
        AssignValues();
        StartCoroutine(Vanish());
    }

    /// <summary>
    /// Assigns values to necessary objects.
    /// </summary>
    private void AssignValues()
    {
        vanishingManager = GetComponent<VanishingManager>();
        grappleGun = FindObjectOfType<GrapplingGun>();
        pauseManagerReference = FindObjectOfType<PauseManager>();
        platformTracks = vanishingManager.GetPlatformTracks();

        currentEnabledTrack = 0;
        EnableDisableObjects();
    }

    #endregion

    #region Vanishing Methods
    /// <summary>
    /// A Coroutine that handles the disappearing and reappearing of the platforms.
    /// </summary>
    /// <returns></returns>
    IEnumerator Vanish()
    {
        // If the game is paused, do not disable or reenable platforms. 
        while (pauseManagerReference.GetPaused())
        {
            yield return null;
        }

        yield return new WaitForSeconds(timerValue);

        // Increments the current enabled track reference.
        currentEnabledTrack++;
        if(currentEnabledTrack > platformTracks.Count - 1)
        {
            currentEnabledTrack = 0;
        }

        // Enable and disable objects after the current track is incremented.
        EnableDisableObjects();
        StartCoroutine(Vanish());
    }

    /// <summary>
    /// Enables and disables groups of objects based on the current value for currentEnabledTrack. 
    /// </summary>
    private void EnableDisableObjects()
    {
        // Cycles through all platformTracks structs.
        for (int i = 0; i <= platformTracks.Count - 1; i++)
        {
            // Stores the gameObject list found in each platformTracks object
            List<GameObject> objectsInPlatformTrack = platformTracks[i].gameObjects;

            // If the current enabled track is equal to i, then all game objects in this list are set to active.
            if (i == currentEnabledTrack)
            {
                foreach (GameObject currentObject in objectsInPlatformTrack)
                {
                    currentObject.SetActive(true);
                }
            }
            // If the current enabled track does not equal i, disable all game objects in this list.
            else
            {
                foreach (GameObject currentObject in objectsInPlatformTrack)
                {
                    currentObject.SetActive(false);

                    // If the player is grappling to the object that was disabled, break the grapple. 
                    if (grappleGun.IsGrappling() && grappleGun.GetCurrentGrappledObject() == currentObject)
                    {
                        grappleGun.StopGrapple();
                    }
                }
            }
        }
    }

    #endregion
}
