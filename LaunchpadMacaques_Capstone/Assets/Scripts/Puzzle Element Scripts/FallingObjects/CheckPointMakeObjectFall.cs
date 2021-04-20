using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointMakeObjectFall : MonoBehaviour
{
    private bool objectsFell = false;
    public List<CheckPointFallingObject> fallingObjects = new List<CheckPointFallingObject>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!objectsFell && fallingObjects.Count > 0)
            {
                objectsFell = true;
                foreach(CheckPointFallingObject c in fallingObjects)
                {
                    c.HitCheckpoint();
                }
            }
        }
    }

    public void AddObject(CheckPointFallingObject obj)
    {
        fallingObjects.Add(obj);
    }
}
