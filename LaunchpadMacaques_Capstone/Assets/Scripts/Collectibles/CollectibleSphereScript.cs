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
    [SerializeField]
    private CollectibleController collectibleController;
    private int plusOne = 1;

    private void Awake()
    {
        collectibleController = GameObject.Find("Collectible Controller").GetComponent<CollectibleController>();
    }

    private void Collect()
    {
        AddToTotalCollectibles(plusOne);
        Destroy(this.gameObject);
    }

    public void AddToTotalCollectibles(int plusOne)
    {
        int totalCollectibles = 0;

        totalCollectibles= collectibleController.totalCollectibles = collectibleController.GetTotalCollectibles() + plusOne;

        collectibleController.totalCollectiblesText.SetText("Total Collectibles: " + totalCollectibles);

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
}
