using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectDistance : FallingObject
{
    public float distanceToStartFalling;

   public override void Update()
    {
        CheckPlayerDistance();   
    }

    public void SetSpecificVariables(float distance)
    {
        distanceToStartFalling = distance;
    }

    private void CheckPlayerDistance()
    {
        if (!falling && Vector3.Distance(this.transform.position, player.transform.position) < distanceToStartFalling)
        {
            StartCoroutine(Falling());
        }
    }
}
