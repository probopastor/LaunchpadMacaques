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
    private int remainingPoints;
    private bool breaking;
    private float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        breaking = false;
    }

    //After the countdown ends, stop it and disable the grapple point
    public void UpdateSubject()
    {
        StopCoroutine("Countdown");
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Renderer>().enabled = false;
    }

    //Countdown Coroutine
    private IEnumerator Countdown()
    {
        //Test if countdown started
        Debug.Log("Start Countdown");

        //Set Max time
        float timeLeft = 3f;

        //Timer
        float totalTime = 0;
        while (totalTime <= timeLeft)
        {
            totalTime += Time.deltaTime;
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        //Set timeLeft to zero
        breaking = true;
    }

    public bool isBreaking()
    {
        return breaking;
    }
}

