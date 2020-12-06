using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessing : MonoBehaviour
{

    float startTime;

    public float speed = 1.0f;

    Volume postProcessing;

    void Start()
    {
        startTime = Time.time + Random.Range(0, 10); // time since game started
        postProcessing = FindObjectOfType<Volume>();
        //light = GetComponent<Light>(); // making a reference to the material so we can enable the emission keyword if you do not want this to affect every object using this
    }

    void Update()
    {
        float frac = (Mathf.Sin(Time.time - startTime) * speed);

        //postProcessing.sharedProfile.TryGet<ColorAdjustments>( = Mathf.Lerp(-180, 180, frac);

    }

}