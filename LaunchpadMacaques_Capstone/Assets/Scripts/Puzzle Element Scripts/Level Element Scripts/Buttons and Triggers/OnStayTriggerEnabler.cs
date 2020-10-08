/* Jake Buri
 * Launchpad Macaques
 * OnStayTriggerEnabler.cs
 * Modified version of PassTriggerEnabler.cs that toggles targets when the player or cube is on top of it.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStayTriggerEnabler : MonoBehaviour
{
    [SerializeField, Tooltip("An array of targets to be altered by the Pass-By Trigger. ")] private GameObject[] targets;
    public bool activated = false;
    public bool targetEnabled = false;
    [SerializeField, Tooltip("If enabled, disables all targets after a period of time. ")] private bool timer = false;
    [SerializeField, Tooltip("Time before targets are disabled after button click (Only if useTimer is used). ")] private int timerValue = 5;
    private int triggers = 0;
    [SerializeField, Tooltip("The amount of triggers required for objects to be enabled. ")] private int triggerEnableGoal = 2;
    [SerializeField, Tooltip("The amount of triggers required for objects to be disabled. ")] private int triggerDisableGoal = 1;
    [SerializeField, Tooltip("Material of active object. ")] private Material active;
    [SerializeField, Tooltip("Material of inactive object. ")] private Material inactive;

    // Start is called before the first frame update
    void Start()
    {
        activated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            triggers++;

            // if timer is enabled, enables targets then starts timer
            if (timer == true)
            {

                if (targets != null && targetEnabled == false && triggers == triggerEnableGoal)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].SetActive(true);
                    }
                    targetEnabled = true;
                    triggers = 0;
                    GetComponent<MeshRenderer>().material = active;
                    StartCoroutine(Disabler());
                }
                Debug.Log("Door Is Active");
                activated = false;
            }
        }
    }

    /// <summary>
    /// If the player or cube is on the presure plate, disable targets
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "PlayerCube")
        {
            //disables targets
            if (targets != null && targetEnabled == true)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].SetActive(false);
                }
                targetEnabled = false;
                triggers = 0;
                GetComponent<MeshRenderer>().material = inactive;
            }
            Debug.Log("Door is Inactive");
        }
    }

    /// <summary>
    /// If the player or cube is off the presure plate, enable targets
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "PlayerCube")
        {
            //enables targets
            if (targets != null && targetEnabled == false)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].SetActive(true);
                }
                targetEnabled = true;
                triggers = 0;
                GetComponent<MeshRenderer>().material = active;
            }
            Debug.Log("Door is Active");
        }
    }

    /// <summary>
    /// waits and disables targets
    /// </summary>
    IEnumerator Disabler()
    {
        yield return new WaitForSecondsRealtime(timerValue);
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].SetActive(false);
        }
        targetEnabled = false;
        triggers = 0;
        GetComponent<MeshRenderer>().material = inactive;
    }
}

