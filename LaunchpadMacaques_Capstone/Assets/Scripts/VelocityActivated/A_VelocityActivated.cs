/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (A_VelocityActivated.CS) 
* (The Abstract class that handles Velocity Based Activation 
*/

using UnityEngine;

public abstract class A_VelocityActivated : MonoBehaviour
{
    [SerializeField][Tooltip("The Velocity The Player Needs to Activate this Object")] float neededVelocity = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(Vector3.Magnitude(other.gameObject.GetComponent<Rigidbody>().velocity) >= neededVelocity)
            {
                Activate();
            }
        }
    }

    /// <summary>
    /// The Abstract Class that will implamented by other classes, to make things happen
    /// </summary>
    public abstract void Activate();
}
