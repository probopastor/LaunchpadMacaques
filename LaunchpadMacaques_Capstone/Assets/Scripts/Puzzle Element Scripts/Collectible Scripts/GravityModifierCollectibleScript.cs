/* 
* Launchpad Macaques 
* CJ Green
* GravityModifierCollectibleScript.cs 
* This class inherits from the abstract class and defines the behavior of the circle collectibles.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifierCollectibleScript : CollectibleBehavior
{
    [SerializeField, Tooltip("Time between disabling and enabling of collectible")] public int timerValue = 6;
    [SerializeField, Tooltip("Time between disabling and enabling of collectible")] public bool respawnAfterTime = true;

    /// <summary>
    /// Calls the OnTrigger functionality from the base class.
    /// </summary>
    /// <param name="other"></param>
    public override void OnTriggerEnter(Collider other)
    {
        // This is where collect is called from the base class
        base.OnTriggerEnter(other);
    }

    /// <summary>
    /// Defines the functionality the gravity collectible's Collect function. 
    /// </summary>
    public override void Collect()
    {
        //StartCoroutine(GetCollectibleController().EffectTimer());

        //GetCollectibleController().SetGravityIsCollected(true);
        //GetCollectibleController().SetIsActive(true);

        collectibleController = FindObjectOfType<CollectibleController>();
        //collectibleController.SetGravityIsCollected(true);
        collectibleController.SetIsActive(true);

        DestroyCollectible();
    }

    /// <summary>
    /// Destroys the current collectible.
    /// </summary>
    public override void DestroyCollectible()
    {
        if(respawnAfterTime)
        {
            StartCoroutine(Enabler());
        }
        else if(!respawnAfterTime)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Enabler()
    {
        Debug.Log("Script Firing");
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSecondsRealtime(timerValue);

        Debug.Log("Waited Succesfully");

        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }
}
