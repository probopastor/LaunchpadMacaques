/* 
* Launchpad Macaques 
* Colin Bugbee
* CJ Green
* CollectibleSphereScript.cs 
* Destroys collectibles upon player collision.
*/

using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityStandardAssets._2D;

public class CollectibleSphereScript : CollectibleBehavior
{

    private int totalSpheres;

    public override void Collect()
    {
        IncrementCollectibleCount();
        GetCollectibleController().testDisplayText.SetText("Test Display: This is a test. Total Count: " + totalSpheres);
        DestroyCollectible();
    }

    public override void DestroyCollectible()
    {
        Destroy(this.gameObject);
    }

    public override void IncrementCollectibleCount()
    {
        totalSpheres++;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Collect();
        }
    }

    #region Old Code
    //private CollectibleController collectibleController;
    //[SerializeField] private int plusOne = 1;

    ////private int totalSpheres;

    //private void Awake()
    //{
    //    collectibleController = FindObjectOfType<CollectibleController>();
    //}

    //private void Collect()
    //{
    //    collectibleController.AddToTotalCollectibles(plusOne);
    //    Destroy(this.gameObject);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.CompareTag("Player"))
    //    {
    //        Collect();
    //    }
    //}
    #endregion
}
