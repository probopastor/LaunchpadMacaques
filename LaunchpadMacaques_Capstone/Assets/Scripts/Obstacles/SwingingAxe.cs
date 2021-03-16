using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    [SerializeField, Tooltip("The speed of which the axe swings at.")]public float speed = 5.0f;
    [SerializeField, Tooltip("The angle of of how far the axe can swing.")] public float tiltAngle = 60.0f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(tiltAngle * Mathf.Sin(Time.time * speed), 0f, 0);
    }
}
