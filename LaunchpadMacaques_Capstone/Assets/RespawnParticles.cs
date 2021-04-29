using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnParticles : MonoBehaviour
{
    public ParticleSystem inactive;
    public ParticleSystem active;
   


    // Start is called before the first frame update
    void Start()
    {
        inactive.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
