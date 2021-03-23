﻿/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Unknown) 
* (RespawnSystem.cs) 
* (Will handle re spawning the player, as well as reseting objects when the player dies) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    #region Public Variables

    [Header("Respawn Zone Variables")]
    [SerializeField, Tooltip("The tags that will re spawn the player if collided with. ")] private string[] respawnTags;

    [SerializeField, Tooltip("If true, will disable past respawn zones when a new one is activated. ")] private bool disableRespawnZonesWhenActive;
    [SerializeField, Tooltip("All respawn zones in the scene. Must be in order that the player will activate them. ")] private GameObject[] respawnZones;

    [Header("Death Effects")]
    [SerializeField] float delayBeforePlayerRespawns = 1;
    [SerializeField, Tooltip("The particles that will play when the player is respawned. ")] private ParticleSystem[] deathParticles;
    #endregion

    #region Private Variables

    // The amount of respawn zones currently active. Used to track which respawn zones should be enabled / disabled.
    private int zonesActive = 0;

    Vector3 currentRespawnPosition;

    GameObject currentRespawnObject;

    private GrapplingGun gg;
    private PushPullObjects pushPullObjectsRef;

    private GrapplePoint[] disappearingGrapplePoints;
    private DisappearingPlatform[] disappearingPlatforms;

    ButtonTransitionManager transitionManger;

    Matt_PlayerMovement player;

    private bool deathParticlesPlaying = false;
    private bool deathInProgress = false;
    #endregion

    #region Start Methods
    private void Start()
    {
        SetObjects();
        currentRespawnPosition = transform.position;
        player = FindObjectOfType<Matt_PlayerMovement>();
        transitionManger = FindObjectOfType<ButtonTransitionManager>();

        // Enables all respawn zones if they are capable of being disabled. 
        if (disableRespawnZonesWhenActive)
        {
            foreach (GameObject zone in respawnZones)
            {
                zone.SetActive(true);
                zonesActive = 0;
            }
        }
    }


    /// <summary>
    /// Will find and Set this scripts private object references
    /// </summary>
    private void SetObjects()
    {
        gg = FindObjectOfType<GrapplingGun>();
        pushPullObjectsRef = FindObjectOfType<PushPullObjects>();
        disappearingGrapplePoints = FindObjectsOfType<GrapplePoint>();

        disappearingPlatforms = FindObjectsOfType<DisappearingPlatform>();
    }
    #endregion

    #region Collision Methods
    private void OnTriggerEnter(Collider other)
    {
        // If The player collides with a death zone will call the re-spawn player method
        for (int i = 0; i < respawnTags.Length; i++)
        {
            if (other.gameObject.CompareTag(respawnTags[i]))
            {
                if (!deathInProgress)
                {
                    deathInProgress = true;
                    StopAllCoroutines();
                    StartCoroutine(KillPlayer());
                }
            }
        }

        // If player collides with a checkpoint object that is not already the newest checkpoint
        // Will set the re-spawn position for the player
        if (other.gameObject.CompareTag("Checkpoint") && other.gameObject != currentRespawnObject)
        {
            currentRespawnObject = other.gameObject;
            currentRespawnPosition = transform.position;

            // Disables past respawn zones when a new one is activated.
            if (disableRespawnZonesWhenActive)
            {
                zonesActive++;

                for (int i = 0; i < zonesActive - 1; i++)
                {
                    respawnZones[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// If player collides with a checkpoint object that is not already the newest checkpoint
    /// Will set the re-spawn position for the player
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Checkpoint") && other.gameObject != currentRespawnObject)
        {
            currentRespawnObject = other.gameObject;
            currentRespawnPosition = transform.position;
        }
    }

    #endregion

    /// <summary>
    /// Will re-spawn player back at spawn point 
    /// Will reset disappearing platforms/grapple points
    /// Will Drop an object, if one is currently held
    /// </summary>
    //public void KillPlayer()
    //{
    //    // Enables all the Disappearing Grapple Points
    //    foreach(GrapplePoint dGP in disappearingGrapplePoints)
    //    {
    //        dGP.EnablePoint();
    //    }

    //    // Enables all the Disappearing Platforms
    //    foreach(DisappearingPlatform platform in disappearingPlatforms)
    //    {
    //        platform.EnablePlatform();
    //    }

    //    // If the player is holding an object, stop holding the object. 
    //    if (pushPullObjectsRef.IsGrabbing())
    //    {
    //        pushPullObjectsRef.DropObject();
    //    }

    //    // Stops the player from grappling
    //    gg.StopGrapple();

    //    // Resets the player position/velocity
    //    this.transform.position = currentRespawnPosition;
    //    this.GetComponent<Rigidbody>().velocity = Vector3.zero;

    //    gg.StopGrapple();
    //}


    IEnumerator KillPlayer()
    {
        player.SetPlayerCanMove(false);

        // Play death particles
        if (!deathParticlesPlaying)
        {
            SetDeathParticleStatus(true);
        }

        foreach (GrapplePoint dGP in disappearingGrapplePoints)
        {
            dGP.EnablePoint();
        }

        // Enables all the Disappearing Platforms
        foreach (DisappearingPlatform platform in disappearingPlatforms)
        {
            platform.EnablePlatform();
        }

        // If the player is holding an object, stop holding the object. 
        if (pushPullObjectsRef.IsGrabbing())
        {
            pushPullObjectsRef.DropObject();
        }

        // Stops the player from grappling
        gg.StopGrapple();

        yield return new WaitForSeconds(delayBeforePlayerRespawns);

        transitionManger.RespawnPlayerTranstion();
    }


    public void RespawnPlayer()
    {
        this.transform.position = currentRespawnPosition;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;

        gg.StopGrapple();
        deathInProgress = false;
    }

    public void PlayerCanMove()
    {
        player.SetPlayerCanMove(true);
    }

    /// <summary>
    /// Starts or stops the death particles. 
    /// </summary>
    /// <param name="playParticles">If true, death particles play. If false they stop. </param>
    public void SetDeathParticleStatus(bool playParticles)
    {
        if (playParticles)
        {
            // PLAY FIRE VFX HERE
            foreach (ParticleSystem fireDeathParticles in deathParticles)
            {
                deathParticlesPlaying = true;
                fireDeathParticles.Play();
            }
        }
        else if (!playParticles)
        {
            // STOP FIRE VFX HERE
            foreach (ParticleSystem fireDeathParticles in deathParticles)
            {
                fireDeathParticles.Stop();
                deathParticlesPlaying = false;
            }
        }
    }

    /// <summary>
    /// Are the death particles currently playing?
    /// </summary>
    /// <returns>Returns true if playing, false otherwise. </returns>
    public bool GetDeathParticlesStatus()
    {
        return deathParticlesPlaying;
    }

}
