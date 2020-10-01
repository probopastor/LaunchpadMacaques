/* 
* Launchpad Macaques 
* Colin Bugbee
* CJ Green
* CollectibleSphereScript.cs 
* This class inherits from the abstract class and defines the behavior of the circle collectibles.
*/

using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityStandardAssets._2D;

public class CollectibleSphereScript : CollectibleBehavior
{
    /// <summary>
    /// Defines the functionality the sphere collectible's Collect function. 
    /// </summary>
    public override void Collect()
    {
        IncrementCollectibleCount();
        GetCollectibleController().totalCollectiblesText.SetText("Total Sphere Count: " + GetCollectibleController().totalCollectedCollectibles + " / " + GetCollectibleController().totalCollectibles);
        DestroyCollectible();

        Debug.Log("The total amount of collected collectibles: " + GetCollectibleController().totalCollectedCollectibles);
        Debug.Log("The total amount of collectibles: " + GetCollectibleController().totalCollectedCollectibles);

        if (GetCollectibleController().totalCollectedCollectibles >= GetCollectibleController().totalCollectibles)
        {
            GetCollectibleController().pauseManager.SetGameWin();
        }

    }

    /// <summary>
    /// Destroys the current collectible.
    /// </summary>
    public override void DestroyCollectible()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Increment collected collectible count.
    /// </summary>
    public override void IncrementCollectibleCount()
    {
        GetCollectibleController().totalCollectedCollectibles++;
    }

    /// <summary>
    /// Cecrement total collectible object count.
    /// </summary>
    public override void DecrementCollectibleTotal()
    {
        GetCollectibleController().totalCollectibles--;
    }

    /// <summary>
    /// Calls the OnTrigger functionality from the base class.
    /// </summary>
    /// <param name="other"></param>
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
