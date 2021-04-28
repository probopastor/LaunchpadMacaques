using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOfLevelFallingObject: FallingObject
{

    private void Start()
    {
        StartCoroutine(Falling());
    }
}
