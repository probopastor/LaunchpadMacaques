using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class skyboxRadiate : MonoBehaviour
{
    Volume volume;
    GradientSky sky;
    public float radiateSpeed;
    public float radiateMin;
    public float radiateMax;
    public float increment = .1f;
    private float incrementHolder;
    private float value = 1.1f;


    // Start is called before the first frame update
    void Start()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out sky);
        InvokeRepeating("MoveValue", 0f, .1f);
        incrementHolder = increment;
        value = sky.gradientDiffusion.value;
    }

    // Update is called once per frame
    void Update()
    {
        //float temp = 0;
        //radiate sky
        //temp = Mathf.Lerp(radiateMin, radiateMax, (Mathf.Sin(radiateSpeed * Time.deltaTime) * radiateSpeed));
        //temp = Mathf.PingPong(Time.deltaTime, radiateSpeed);

        //sky.gradientDiffusion.value = temp;
    }

    void MoveValue()
    {
        if(value > radiateMax)
        {
            increment = incrementHolder * -1;
        }
        if(value < radiateMin)
        {
            increment = Mathf.Abs(incrementHolder);
        }

        value += increment;
        sky.gradientDiffusion.value = value;
    }
}
