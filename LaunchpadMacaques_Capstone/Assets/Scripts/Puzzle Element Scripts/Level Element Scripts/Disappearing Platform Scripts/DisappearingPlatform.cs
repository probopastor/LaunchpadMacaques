/* 
* Launchpad Macaques - Neon Oblivion
* William Nomikos
* DisappearingPlatform.cs
* Script handles grappling disappearing platforms.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("The triggers that the button should read. Other triggers are ignored. ")] private string[] triggerTags;
    [SerializeField, Tooltip("The time it takes before the platform destroys itself after triggered. ")] private float timeUntilDisappear;
    [SerializeField, Tooltip("The distance the raycast will travel to check if the player is on top of a disappearing platform. ")] private float groundCheckDistance = 5f;

    private GrapplingGun grapplingGunRef;
    private bool platformBroken;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Awake()
    {
        grapplingGunRef = FindObjectOfType<GrapplingGun>();
        platformBroken = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks the triggerTags array to determine if the object should be checked. 
        for (int i = 0; i < triggerTags.Length; i++)
        {
            // If the object should be checked, shoot a raycast from the object down to determine if the object is above the platform. 
            if (other.CompareTag(triggerTags[i]))
            {
                RaycastHit checkDownHit;

                // If the platform is within range (and is the correct platform), break the platform
                if (Physics.Raycast(other.transform.position, -Vector3.up, out checkDownHit, groundCheckDistance))
                {
                    if ((checkDownHit.collider.gameObject == gameObject) && !platformBroken)
                    {
                        platformBroken = true;
                        StartCoroutine(BreakPlatform());
                    }

                }
            }
        }
    }

    /// <summary>
    /// Deactivates the platform after a period of time. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator BreakPlatform()
    {
        // Wait for the end of the frame if there is no set timeUntilDisappear. 
        if(timeUntilDisappear != 0)
        {
            yield return new WaitForSeconds(timeUntilDisappear);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        // If the player is grappling to this platform, break the grapple. 
        if(grapplingGunRef.IsGrappling() && grapplingGunRef.GetCurrentGrappledObject() == gameObject)
        {
            grapplingGunRef.StopGrapple();
        }

        Collider[] colliders = gameObject.GetComponents<Collider>();

        // Disables each collider on this platform.
        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }

        this.gameObject.GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// Reenables disappearing platform.
    /// </summary>
    public void EnablePlatform()
    {
        Collider[] colliders = gameObject.GetComponents<Collider>();

        // Turns off each collider on the platform.
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        this.gameObject.GetComponent<Renderer>().enabled = true;
        platformBroken = false;
    }
    #endregion 
}
