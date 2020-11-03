using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    Vector3 currentRespawnPosition;

    GameObject currentRespawnObject;

    private GrapplingGun gg;

    private void Start()
    {
        gg = FindObjectOfType<GrapplingGun>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RespawnPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint") && other.gameObject != currentRespawnObject)
        {
            currentRespawnObject = other.gameObject;
            currentRespawnPosition = transform.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Checkpoint") && other.gameObject != currentRespawnObject)
        {
            currentRespawnObject = other.gameObject;
            currentRespawnPosition = transform.position;
        }
    }

    public void RespawnPlayer()
    {
        this.transform.position = currentRespawnPosition;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gg.StopGrapple();
    }
}
