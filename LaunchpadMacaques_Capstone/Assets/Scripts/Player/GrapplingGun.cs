using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private float distanceFromPoint;

    public float explosionRadius = 5f;
    public float explosionPower = 10.0f;

    //this is the value that is updated every frame to set the grapples max distance equal to the player distance from the desired point by this value
    public float grappleSpeed = 50f;

    //values that affect how the spring joint grapple behaves
    public float springValue = 10f;
    public float springDamp = 10f;
    public float springMass = 5f;
    private RaycastHit grappleRayHit;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(IsGrappling())
        {
            if(joint.maxDistance <= 0)
            {
                joint.maxDistance = 0;
            }
            else
            {
                joint.maxDistance = distanceFromPoint - grappleSpeed * Time.deltaTime;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Explode();
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    //adds explosion force to raycast point when called
    void Explode()
    {
        print("Explode");
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider rbHit in colliders)
            {
                Rigidbody rb = rbHit.GetComponent<Rigidbody>();

                if(rb != null)
                {
                    rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius, 0.0f, ForceMode.Impulse);
                }
            }

        }
    }


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grappleRayHit = hit;

            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //Adjust these values to fit your game.

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint;
            joint.minDistance = 0;

            //Default Vaules
            //joint.maxDistance = distanceFromPoint * 0.8f;
            //joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = springValue;
            joint.damper = springDamp;
            joint.massScale = springMass;

            //Default values
            //joint.spring = 4.5f;
            //joint.damper = 7f;
            //joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public float GetMaxGrappleDistance()
    {
        return maxDistance;
    }

    public RaycastHit GetGrappleRayHit()
    {
        return grappleRayHit;
    }
}