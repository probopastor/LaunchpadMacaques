using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMesh : MonoBehaviour
{

    MeshRenderer Cube;

    // Start is called before the first frame update
    void Start()
    {
        Cube = GetComponent<MeshRenderer>();
        Cube.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
