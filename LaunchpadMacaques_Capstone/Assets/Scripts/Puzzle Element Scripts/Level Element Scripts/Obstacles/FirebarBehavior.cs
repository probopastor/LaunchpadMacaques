/*
* Launchpad Macaques - Neon Oblivion
* William Nomikos
* FirebarBehavior.cs
* Script handles Firebar rotation.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebarBehavior : MonoBehaviour
{
    [SerializeField, Tooltip("The speed at which the bar rotates. ")] private float rotationSpeed = 0f;
    [SerializeField, Tooltip("If enabled, Firebar will move along the path specified by the Moving Platform component. ")] private bool enableMovingPlatform;
    [SerializeField, Tooltip("Stops Fireball rotation when disabled. ")] private bool turnRotationOn;

    private void Start()
    {
        if (enableMovingPlatform)
        {
            gameObject.GetComponent<MovingPlatform>().enabled = true;
            gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        else if (!enableMovingPlatform)
        {
            gameObject.GetComponent<MovingPlatform>().enabled = false;
            gameObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(turnRotationOn)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}
