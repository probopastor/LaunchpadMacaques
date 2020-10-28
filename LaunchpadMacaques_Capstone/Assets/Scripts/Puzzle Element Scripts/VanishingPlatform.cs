/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
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

    [SerializeField, Tooltip("This is an array of tag names that determine the variation of ")]
    private string[] tagNames = { "Platform 1", "Platform 2" };

    private GrapplingGun grappleGun;

    // Start is called before the first frame update
    void Start()
    {

        grappleGun = FindObjectOfType<GrapplingGun>().GetComponent<GrapplingGun>();


        if (gameObject.tag == tagNames[0])
        {
            isVisible = true;
            StartCoroutine(Vanish());
        }
        else if (gameObject.tag == tagNames[1])
        {
            isVisible = false;
            StartCoroutine(Vanish());
        }
    }

    IEnumerator Vanish()
    {
        if (isVisible)
        {
            Debug.Log("Dissappearing...");

            grappleGun.StopGrapple();

            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            yield return new WaitForSecondsRealtime(timerValue);

            isVisible = false;

            StartCoroutine(Vanish());
        }

        if (!isVisible)
        {
            Debug.Log("Reappearing!!");

            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;

            yield return new WaitForSecondsRealtime(timerValue);

            isVisible = true;

            StartCoroutine(Vanish());
        }
    }

}
