/*
* Launchpad Macaques - Neon Oblivion
* Jamey Colleen
* ParticleTrigger.cs
* Script handles particle system on player or moveabe cube impacting ground.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("The particle system used for the script. ")] private ParticleSystem LandingEffect;
    [SerializeField, Tooltip("Modifier to location particles spawn from. ")] private Vector3 particleLocation;
    [SerializeField, Tooltip("How long particles last. ")] private float landingParticlesDuration;
    //[SerializeField] private LayerMask particleMask;
    [SerializeField, Tooltip("Modifies speed of particles. ")] private float speedMultiplier;
    [SerializeField, Tooltip("Velocity at which particles are increased to compensate for particle speed. ")] private float velocityThreshold;
    private Vector3 currentVelocity;
    private GameObject landingParticlesObject;
    private bool onGround = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //grabs velocity if player is in air
        if (!onGround)
        {
            currentVelocity = GetComponent<Rigidbody>().velocity;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        //detects when player hits ground nd spawns particles
        onGround = true;
        if (coll.gameObject.layer == 13)
        {
            StartCoroutine(LandingParticles());
            //var main = LandingEffect.main;
            //main.startSpeed = (GetComponent<Rigidbody>().velocity.y * speedMultiplier);
            //LandingEffect.Simulate(0.0f, true, true);
            //LandingEffect.Clear();
            //LandingEffect.Stop();
            //LandingEffect.Play();
        }
    }

    private IEnumerator LandingParticles()
    {
        //sets particle spawn location and rotation
        Vector3 landingParticlesLocation = new Vector3(transform.position.x + particleLocation.x, 
                                                       transform.position.y + particleLocation.y,
                                                       transform.position.z + particleLocation.z);

        landingParticlesObject = Instantiate(LandingEffect.gameObject, landingParticlesLocation, Quaternion.Euler(-90f, 0f, 0f));

        //modifies particle amount
        if (currentVelocity.y > velocityThreshold)
        {
            var emission = landingParticlesObject.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 200;
        }

        //sets particle speed
        var main = landingParticlesObject.GetComponent<ParticleSystem>().main;
        main.startSpeed = ((currentVelocity.y) * speedMultiplier);

        yield return new WaitForSeconds(landingParticlesDuration);

        Destroy(landingParticlesObject);

        //marks player as in air
        onGround = false;
    }
}
        
        
