using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField, Tooltip("The tags that will respawn the player if collided with. ")] private string[] respawnTags;
    Vector3 currentRespawnPosition;

    GameObject currentRespawnObject;

    private GrapplingGun gg;
    private PushPullObjects pushPullObjectsRef;

    //private GrapplePointManager gpm;

    private GrapplePoint[] disappearingGrapplePoints;
    private DisappearingPlatform[] disappearingPlatforms;

    private void Start()
    {
        gg = FindObjectOfType<GrapplingGun>();
        pushPullObjectsRef = FindObjectOfType<PushPullObjects>();
        disappearingGrapplePoints = FindObjectsOfType<GrapplePoint>();
        //gpm = FindObjectOfType<GrapplePointManager>();

        disappearingPlatforms = FindObjectsOfType<DisappearingPlatform>();
        currentRespawnPosition = transform.position;
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
        for (int i = 0; i < respawnTags.Length; i++)
        {
            if (other.gameObject.CompareTag(respawnTags[i]))
            {
                RespawnPlayer();
            }
        }

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
        //if (gpm)
        //{
        //    gpm.TurnOnPoints();
        //}

        foreach(GrapplePoint dGP in disappearingGrapplePoints)
        {
            dGP.EnablePoint();
        }

        foreach(DisappearingPlatform platform in disappearingPlatforms)
        {
            platform.EnablePlatform();
        }

        // If the player is holding an object, stop holding the object. 
        if (pushPullObjectsRef.IsGrabbing())
        {
            pushPullObjectsRef.DropObject();
        }

        gg.StopGrapple();
        this.transform.position = currentRespawnPosition;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
