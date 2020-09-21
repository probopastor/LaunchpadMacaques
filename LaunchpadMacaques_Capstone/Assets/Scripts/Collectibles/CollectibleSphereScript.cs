/* 
* Launchpad Macaques 
* Colin Bugbee
* CJ Green
* CollectibleSphereScript.cs 
* Destroys collectibles upon player collision.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityStandardAssets._2D;

public class CollectibleSphereScript : MonoBehaviour
{
    private CollectibleController collectibleController;
    [SerializeField] private int plusOne = 1;

    //private int totalSpheres;

    private void Awake()
    {
        collectibleController = FindObjectOfType<CollectibleController>();
    }

    private void Collect()
    {
        collectibleController.AddToTotalCollectibles(plusOne);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
}
