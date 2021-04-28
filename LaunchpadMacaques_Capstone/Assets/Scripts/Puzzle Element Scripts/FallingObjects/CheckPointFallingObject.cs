using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointFallingObject : FallingObject
{
    public void HitCheckpoint()
    {
        StartCoroutine(Falling());
    }
}
