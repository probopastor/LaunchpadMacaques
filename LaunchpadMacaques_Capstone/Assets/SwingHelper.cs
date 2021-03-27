﻿/*
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* SwingHelper.cs
* Helps handle swinging issues
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHelper : MonoBehaviour
{
    private GrapplingGun grapplingGun;
    private Transform orientation;


   [SerializeField, Tooltip("The Minimun angle the players swinging needs to hit to be considered a good loop")]  private float minCheckAngle = 80;
   [SerializeField, Tooltip("The Maximum angle the players swingings needs to hit to be considerd a good loop")] private float maxCheckAngle = 100;

   [SerializeField, Tooltip("The Ammount of time without having a good loop before game will try to fix it")] private float timeBeforeFixing = 5;


    private bool inNegative = false;

    private int correctAngleInt = 0;

    private bool checking = false;
    private bool hitAngle = false;
    private int loop = 0;

    private bool isFixing = false;

    private Vector3 dirToTargert = Vector3.zero;
    // Start is called before the first frame update

    private void Awake()
    {
        grapplingGun = FindObjectOfType<GrapplingGun>();
        orientation = FindObjectOfType<Matt_PlayerMovement>().GetOrientaion();
        hitAngle = false;

    }


    // Update is called once per frame
    private void Update()
    {

        if (grapplingGun.IsGrappling())
        {
            if (!checking &!isFixing)
            {
                StartCoroutine(NeedFixed());
            }

            float angle = DegreesBetweenObjects(orientation.gameObject, grapplingGun.GetCurrentGrappledObject());

            // Makes sure the diretion the player is swinging in has not changed (Swinging up/Swinging Down)
            if ((inNegative && angle < 0) || (!inNegative && angle > 0))
            {

                // Checks to see if the players current swing angle is within bounds of the necessary angle for a good loop
                if (Mathf.Abs(angle) > minCheckAngle && Mathf.Abs(angle) < maxCheckAngle && !hitAngle)
                {
                    hitAngle = true;
                    correctAngleInt++;
   
                }
            }

            // Checks if the direction the player in going in has changed (Swinging up/Swinging Down)
            else if ((inNegative && angle > 0) || (!inNegative && angle < 0))
            {
                loop++;

                // Checks if the player has completed a full loop around a grapple point
                if (loop == 2)
                {
                    FinishedLoop();

                }

                hitAngle = false;
                inNegative = !inNegative;
            }

        }
    }

    /// <summary>
    /// Resets variables if player is not grappling
    /// </summary>
    public void ResetVariables()
    {
        checking = false;
        correctAngleInt = 2;
        hitAngle = false;
        loop = 0;
        dirToTargert = Vector3.zero;
        isFixing = false;
        StopAllCoroutines();
    }


    /// <summary>
    /// Checks to see if the player had a good loop
    /// </summary>
    private void FinishedLoop()
    {
        // Checks if the player hit the correct angle while swinging up and while swinging down
        if (correctAngleInt >= 2 && hitAngle)
        {
            // Resets Coroutines and variables
            Debug.Log("Good Loop");
            dirToTargert = Vector3.zero;

            StopAllCoroutines();
            isFixing = false;
            checking = false;
        }

        else
        {
            Debug.Log("Bad Loop:");
        }

        // Reset information about the loop
        correctAngleInt = 0;
        loop = 0;
    }


    IEnumerator NeedFixed()
    {
        checking = true;
        yield return new WaitForSeconds(timeBeforeFixing);
        StartCoroutine(FixDirection());
        checking = false;
    }

    IEnumerator FixDirection()
    {
        while (true)
        {
            if (grapplingGun.GetCurrentGrappledObject() != null)
            {
                dirToTargert = (grapplingGun.GetCurrentGrappledObject().transform.position - orientation.transform.position).normalized;
            }

            Debug.Log("FIXING DIRECTION");

            yield return null;
        }


    }


    /// <summary>
    /// Returns the Y angle between two objects
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    float DegreesBetweenObjects(GameObject thisObject, GameObject target)
    {
        if(target == null)
        {
            return -1;
        }
        float sideA = Vector3.Distance(thisObject.transform.position, new Vector3(target.transform.position.x, thisObject.transform.position.y, target.transform.position.z));
        float sideB = target.transform.position.y - thisObject.transform.position.y;
        float angleRad = Mathf.Atan(sideB / sideA);
        return angleRad * Mathf.Rad2Deg;
    }

    public Vector3 GetDirectionToTarget()
    {
        return dirToTargert;
    }
}
