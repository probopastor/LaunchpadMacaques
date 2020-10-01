using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VelocityActivated : MonoBehaviour
{
    [SerializeField] float neededVelocity = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Velocity: " + Vector3.Magnitude(other.gameObject.GetComponent<Rigidbody>().velocity));
            if(Vector3.Magnitude(other.gameObject.GetComponent<Rigidbody>().velocity) >= neededVelocity)
            {
                Activate();
            }
        }
    }
    public abstract void Activate();
}
