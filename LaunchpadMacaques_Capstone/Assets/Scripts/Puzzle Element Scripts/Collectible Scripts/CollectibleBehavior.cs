/* 
* Launchpad Macaques 
* CJ Green
* CollectibleBehavior.cs 
* Handles the basic implementation of the collectible behaviors.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleBehavior : MonoBehaviour
{
    // CollectibleController reference
    protected CollectibleController collectibleController;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collectibleController = FindObjectOfType<CollectibleController>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    /// <summary>
    /// Triggers the collect function when sub classes collide with the player. 
    /// </summary>
    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Collect();
        }
    }

    /// <summary>
    /// Abstract Collect method for sub classes to define functionality. 
    /// </summary>
    public abstract void Collect();

    /// <summary>
    /// Virtual DestroyCollectible method for sub classes to define functionality if they want to. 
    /// </summary>
    public virtual void DestroyCollectible()
    {

    }

    /// <summary>
    /// Virtual IncrementCollectibleCount method for sub classes to define functionality if they want to. 
    /// </summary>
    public virtual void IncrementCollectibleCount()
    {

    }

    /// <summary>
    /// Virtual DecrementCollectibleTotal method for sub classes to define functionality if they want to. 
    /// </summary>
    public virtual void DecrementCollectibleTotal()
    {

    }

    /// <summary>
    /// Getter for obtaining the CollectibleController reference
    /// </summary>
    public CollectibleController GetCollectibleController()
    {
        return collectibleController;
    }

}
