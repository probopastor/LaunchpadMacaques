using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PassTrigger : MonoBehaviour
{
    public GameObject target;
    private bool activated = false;
    private int triggers = 0;
    public int triggerGoal = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (target != null && triggers == triggerGoal)
            {
                //target.activated = true;
                triggers = 0;
            }
            Debug.Log("Triggered");
            activated = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            activated = true;
        }

    }
}