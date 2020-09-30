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

    private int totalCollectibles;
    private int totalCollectedCollectibles;

    public override void Start()
    {
        base.Start();
        totalCollectedCollectibles = 0;
        totalCollectibles = FindObjectsOfType<CollectibleSphereScript>().Length;
    }

    //public override void Update()
    //{
    //    Debug.Log("The total amount of collected collectibles: " + totalCollectedCollectibles);
    //    Debug.Log("The total amount of collectibles: " + totalCollectedCollectibles);
    //}

    public override void Collect()
    {
        IncrementCollectibleCount();
        DecrementCollectibleTotal();
        GetCollectibleController().testDisplayText.SetText("Total Sphere Count: " + totalCollectedCollectibles + " / " + totalCollectibles);
        DestroyCollectible();

        Debug.Log("The total amount of collected collectibles: " + totalCollectedCollectibles);
        Debug.Log("The total amount of collectibles: " + totalCollectedCollectibles);

        if (totalCollectedCollectibles >= totalCollectibles)
        {
            GetCollectibleController().pauseManager.SetGameWin();
        }

    }

    public override void DestroyCollectible()
    {
        Destroy(this.gameObject);
    }

    public override void IncrementCollectibleCount()
    {
        //base.IncrementCollectibleCount();
        totalCollectibles++;
    }

    public override void DecrementCollectibleTotal()
    {
        //base.DecrementCollectibleTotal();
        totalCollectedCollectibles--;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
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
