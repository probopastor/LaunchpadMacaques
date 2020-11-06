using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToWorldGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x -= pos.x % 0.875f;
        pos.y -= pos.y % 0.875f;
        pos.z -= pos.z % 0.875f;

        transform.position = pos;
    }
}
