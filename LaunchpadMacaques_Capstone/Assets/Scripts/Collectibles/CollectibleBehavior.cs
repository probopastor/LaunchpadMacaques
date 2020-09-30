using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleBehavior : MonoBehaviour
{
    private bool collected;

    private CollectibleController collectibleController;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collectibleController = GameObject.FindGameObjectWithTag("Collectible Controller").GetComponent<CollectibleController>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public abstract void Collect();

    public abstract void DestroyCollectible();

    public virtual void IncrementCollectibleCount()
    {

    }

    public virtual void DecrementCollectibleTotal()
    {

    }

    public bool GetCollected()
    {
        return collected;
    }

    public void SetCollected(bool true_or_false)
    {
        collected = true_or_false;
    }


    public CollectibleController GetCollectibleController()
    {
        return collectibleController;
    }

}
