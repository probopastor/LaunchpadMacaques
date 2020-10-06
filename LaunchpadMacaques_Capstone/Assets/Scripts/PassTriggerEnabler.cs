using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTriggerEnabler : MonoBehaviour
{
    public GameObject[] targets;
    public bool activated = false;
    public bool targetEnabled = false;
    public bool timer = false;
    public int timerValue = 5;
    private int triggers = 0;
    public int triggerEnableGoal = 2;
    public int triggerDisableGoal = 1;
    public Material active;
    public Material inactive;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            triggers++;

            //uses manual disable if there is no timer
            if (timer == false)
            {

                //enables targets
                if (targets != null && targetEnabled == false && triggers == triggerEnableGoal)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].SetActive(true);
                    }
                    targetEnabled = true;
                    triggers = 0;
                    GetComponent<MeshRenderer>().material = active;
                }

                //disables targets
                else if (targets != null && targetEnabled == true && triggers == triggerDisableGoal)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].SetActive(false);
                    }
                    targetEnabled = false;
                    triggers = 0;
                    GetComponent<MeshRenderer>().material = inactive;
                }
                Debug.Log("Triggered");
                activated = false;
            }

            // if timer is enabled, enables targets then starts timer
            else if (timer == true)
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
                Debug.Log("Triggered");
                activated = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "PlayerCube")
        {
            activated = true;
        }

    }

    //waits and disables targets
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

