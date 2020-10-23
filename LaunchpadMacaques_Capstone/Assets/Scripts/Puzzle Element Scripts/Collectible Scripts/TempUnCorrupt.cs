/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (TempUnCorrupt.CS) 
* (The Temporay script for the collectable that will clear the corruption in a radius) 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUnCorrupt : MonoBehaviour
{
    [SerializeField][Tooltip("The Size of the area the object will clear of courrption")]float clearRadius = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<MakeSpotNotGrappleable>().ClearCorruption(this.transform.position,clearRadius);
        }
    }
}
