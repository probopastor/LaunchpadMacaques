/* 
* Launchpad Macaques 
* Colin Bugbee
* CollectibleSphereScript.cs 
* Destroys collectibles upon player collision.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSphereScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
