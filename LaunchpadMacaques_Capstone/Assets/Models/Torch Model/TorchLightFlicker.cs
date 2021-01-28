/*
 * TorchLightFlicker.cs
 * Author: Matt Kirchoff
 * Made: 12/6/2020
 * 
 * Manages the lights for the wall torches, random intervals, change lighting color, and flicker light as if it were a torch
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLightFlicker : MonoBehaviour
{
    public Light light;

    public Color color1;
    public Color color2;

    float startTime;

    public float speed = 1.0f;

    private float intitalIntentisty;
    private float minIntensity = 0f;
    private float maxIntensity = 1f;

    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;


    void Start()
    {
        int temp = Random.Range(0, 10);
        //slight randomization for not all flashing and changing at the same time.
        startTime = Time.time + temp; // time since game started
        smoothing = Random.Range(smoothing - 5, smoothing + 5);

        //initialize values
        intitalIntentisty = light.intensity;
        minIntensity = intitalIntentisty - intitalIntentisty * .5f;
        maxIntensity = intitalIntentisty + intitalIntentisty * .5f;

        //switch colors, so as to not have every light start the same color
        if(temp > 5)
        {
            Color c = color1;
            color1 = color2;
            color2 = c;
        }


        smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (light == null)
        {
            light = GetComponent<Light>();
        }

        //light = GetComponent<Light>(); // making a reference to the material so we can enable the emission keyword if you do not want this to affect every object using this
    }

    void Update()
    {
        //color changes
        float frac = (Mathf.Sin(Time.time - startTime) * speed);

        light.color = Color.Lerp(color1, color2, frac);

        //light.intensity = Mathf.Lerp((intitalIntentisty - intitalIntentisty * .5f), (intitalIntentisty + intitalIntentisty * .5f), frac);

        //flickering
        // pop off an item if too big
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calculate new smoothed average
        light.intensity = lastSum / (float)smoothQueue.Count;

    }

}


