using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public Transform player;
    private Camera cam;
    private void Start()
    {
        cam = this.GetComponentInChildren<Camera>();
        //if (PlayerPrefs.HasKey("FOV"))
        //{
        //    cam.fieldOfView = PlayerPrefs.GetFloat("FOV");
        //}
    }
    private void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}