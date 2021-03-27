/*
* Launchpad Macaques - Trial and Error
* Zack Seiple
* Pinwheel.cs
* Handles the Pinwheel object behavior. Pinwheels rotate towards the player upon being grappled to.
*/

using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pinwheel : MonoBehaviour
{
    private GameObject wheel;
    private Dictionary<Transform, int> grapplePoints;
    private float currentRotationAmount;
    private int numGrapplePoints = 4;

    [SerializeField, Tooltip("Amount the wheel will rotate when the player grapples onto a grapple point (in degrees)")]
    private float rotationAmount = 90f;
    [SerializeField, Tooltip("The time it takes for the wheel to rotate to its stopping point (in seconds)")]
    private float rotationTime = 1.0f;
    private float currentTime = 0.0f;

    private void Start()
    {
        wheel = transform.GetChild(0).gameObject;

        //Get all Grapple Points attached to the wheel
        grapplePoints = new Dictionary<Transform, int>();
        grapplePoints.Add(wheel.transform.Find("Point 0"), 0);
        grapplePoints.Add(wheel.transform.Find("Point 1"), 1);
        grapplePoints.Add(wheel.transform.Find("Point 2"), 2);
        grapplePoints.Add(wheel.transform.Find("Point 3"), 3);
        numGrapplePoints = grapplePoints.Count;
    }

    /// <summary>
    /// Public function that can be called by other scripts to trigger the rotation of the pinwheel
    /// </summary>
    /// <param name="grapplePointUsed">The transform of the grapple point on the pinwheel that was attached to</param>
    /// <param name="lookDir">The direcion the camera of the player is looking, used to determine the direction the pinwheel should move</param>
    public void TriggerRotation(Transform grapplePointUsed, Vector3 lookDir)
    {
        StartCoroutine(Rotate(grapplePointUsed, lookDir));
    }

    bool clockwise;
    bool lastGrappleTop = false;
    /// <summary>
    /// Function that handles the rotation of the pinwheel
    /// </summary>
    /// <param name="grapplePoint">The transform of the grapple point on the pinwheel that was attached to</param>
    /// <param name="lookPos">The direcion the camera of the player is looking, used to determine the direction the pinwheel should move</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator Rotate(Transform grapplePoint, Vector3 lookPos)
    {
        bool prevClockwise = clockwise;

        //Use player's looking orientation and wheel's orientation to determine direction wheel should move
        float lookAngle = Vector3.Angle(transform.right.normalized, lookPos.normalized);
        if (lookAngle > 90f && grapplePoints[grapplePoint] != 0)
        {
            clockwise = true;
            lastGrappleTop = false;
        }
        else if (lookAngle <= 90f && grapplePoints[grapplePoint] != 0)
        {
            clockwise = false;
            lastGrappleTop = false;
        }
        //Check to make sure that it doesn't do the same check on the top one (Would result in player swapping directions
        //at the top if they were just trying to follow the wheel around in one direction
        else if (grapplePoints[grapplePoint] == 0)
        {
            clockwise = prevClockwise;
            lastGrappleTop = true;
        }

        //Set Rotation angle based on clockwise or not
        currentRotationAmount = clockwise ? -rotationAmount : rotationAmount;

        //Change Position numbers in dictionary
        int modifier = clockwise ? (-1) : (1);
        Transform currTransform;
        for (int i = 0; i < numGrapplePoints; i++)
        {
            currTransform = wheel.transform.GetChild(i);
            grapplePoints[currTransform] = WrapIndex(grapplePoints[currTransform] + (modifier * (int)(rotationAmount / 90)), 0, numGrapplePoints);

        }
        //Move the Wheel
        float currRotationTime = 0;
        float targetRotationTime = rotationTime;
        while (currRotationTime < targetRotationTime)
        {
            Vector3 newRotationVector = Vector3.zero;
            newRotationVector.y = currentRotationAmount / targetRotationTime;

            wheel.transform.Rotate(newRotationVector * Time.deltaTime);
            currRotationTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Wraps a given index to be within a certain minimum (inclusive) and maximum (exclusive). If the index is greater
    /// than or equal to the max it will wrap around to the min value. If the index is less than the min it will wrap around to the max - 1.
    /// If the index is already within the provided range, the index will be returned unmodified
    /// </summary>
    /// <param name="index">The index to be wrapped</param>
    /// <param name="min">The minimum value of the wrap (inclusive)</param>
    /// <param name="max">The maximum value of the wrap (exclusive)</param>
    /// <returns>An index within the given min (inclusive) and max (exclusive)</returns>
    private int WrapIndex(int index, int min, int max)
    {
        if (index >= max)
            return min;
        else if (index < min)
            return max - 1;
        else
            return index;

    }
}
