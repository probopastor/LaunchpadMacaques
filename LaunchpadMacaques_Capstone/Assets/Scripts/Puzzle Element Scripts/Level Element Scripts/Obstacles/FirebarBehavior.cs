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
    [SerializeField, Tooltip("Each fireball that rotates around the axle. ")] GameObject firebar1, firebar2, firebar3, firebar4, firebar5, firebar6, firebar7, firebar8, firebar9, firebar10;

    [SerializeField, Tooltip("The speed at which the bar rotates. ")] private float rotationSpeed = 0f;
    [SerializeField, Tooltip("If enabled, Firebar will move along the path specified by the Moving Platform component. ")] private bool enableMovingPlatform = false;
    [SerializeField, Tooltip("Stops Fireball rotation when disabled. ")] private bool turnRotationOn = false;

    const int FIREBALLSIZE = 10;
    private GameObject[] FirebarObjs { get; set; }
    [SerializeField, Tooltip("The firebars to be enabled. Can enable up to 10. Order is clockwise. ")] private bool[] enabledFirebars = new bool[FIREBALLSIZE];

    private void Awake()
    {
        FirebarObjs = new GameObject[] { firebar1, firebar2, firebar3, firebar4, firebar5, firebar6, firebar7, firebar8, firebar9, firebar10 };   
    }

    private void OnValidate()
    {
        if(enabledFirebars.Length != FIREBALLSIZE)
        {
            Debug.LogWarning("Do not change enabledFirebars int field. ");
            System.Array.Resize(ref enabledFirebars, FIREBALLSIZE);
        }
    }

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

        // Enables the correct firebars
        for(int i = 0; i < enabledFirebars.Length; i++)
        {
            // Prevents i from being greater than the amount of firebar objects created. 
            if(i < FirebarObjs.Length)
            {
                if (enabledFirebars[i])
                {
                    FirebarObjs[i].SetActive(true);
                }
                else if (!enabledFirebars[i])
                {
                    FirebarObjs[i].SetActive(false);
                }
            }
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
