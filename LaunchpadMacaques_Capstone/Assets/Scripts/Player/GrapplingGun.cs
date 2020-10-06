using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private GameObject hitObject;
    private GameObject hitObjectClone;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField, Tooltip("The amount of length subtraced from grapple length on each subsequent grapple. ")] private float grappleLengthModifier = 10;
    [SerializeField] private float wheelSensitivity = 2;
    private float maxGrappleDistance = 100f;
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

    private bool swingLockToggle;
    private bool canApplyForce;

    private GameObject grappledObj;

    [SerializeField] private float minSwingAngle = -90f;
    [SerializeField] private float maxSwingAngle = 90f;


    [SerializeField] private GameObject grappleToggleEnabledText;
    [SerializeField] private GameObject grappleToggleDisabledText;
    [SerializeField] private LayerMask whatIsNotGrappleable;


    private MakeSpotNotGrappleable corruptObject;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        if (joint)
        {
            Debug.Log("Joint was created on awake");
            Destroy(joint);
        }

        swingLockToggle = false;

        if (grappleToggleEnabledText != null)
        {
            grappleToggleEnabledText.SetActive(false);
        }

        if (grappleToggleDisabledText != null)
        {
            grappleToggleDisabledText.SetActive(false);
        }

        corruptObject = FindObjectOfType<MakeSpotNotGrappleable>();
    }

    void Update()
    {
        if (IsGrappling())
        {
            if (joint.maxDistance <= 0)
            {
                joint.maxDistance = 0;
            }
            else
            {
                joint.maxDistance = distanceFromPoint - grappleSpeed * Time.deltaTime;
            }

            grapplePoint = hitObjectClone.transform.position;

            if (grappledObj != null)
            {
                Vector3 objectDirection = (grappledObj.transform.position - player.transform.position).normalized;
                Vector3 groundDirection = Vector3.down;

                float angle = Vector3.Angle(objectDirection, groundDirection);

                Debug.Log("angle " + angle);

                if (angle < minSwingAngle || angle > maxSwingAngle)
                {
                    canApplyForce = true;
                }
                else
                {
                    canApplyForce = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !IsGrappling())
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonDown(0) && IsGrappling())
        {
            StopGrapple();
            StartGrapple();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrappling())
        {
            if (!swingLockToggle)
            {
                swingLockToggle = true;
                grappleToggleEnabledText.SetActive(true);
                grappleToggleDisabledText.SetActive(false);
            }
            else
            {
                swingLockToggle = false;
                grappleToggleEnabledText.SetActive(false);
                grappleToggleDisabledText.SetActive(true);
            }
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Explode();
        //}

        if (lr == null && joint)
        {
            Debug.Log("Line Render Was Dead but Joint was still there");
            StopGrapple();
        }


        ChangeDistance();
    }

    private void ChangeDistance()
    {
        var wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput < 0)
        {
            distance += wheelSensitivity;
            if (distance > maxDistance)
            {
                distance = maxDistance;
            }
        }

        else if (wheelInput > 0)
        {
            Debug.Log("Go Down");
            distance -= wheelSensitivity;

            if (distance < minDistance)
            {
                distance = minDistance;
            }
        }

        if (joint)
        {
            joint.minDistance = distance;
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
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider rbHit in colliders)
            {
                Rigidbody rb = rbHit.GetComponent<Rigidbody>();

                if (rb != null)
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
        RaycastHit secondHit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {

            float dist = Vector3.Distance(camera.position, hit.collider.gameObject.transform.position);

            if (!(Physics.Raycast(camera.position, camera.forward, out secondHit, distance, whatIsNotGrappleable)))
            {
                //grapplePoint = hit.point;
                grappleRayHit = hit;

                hitObjectClone = Instantiate(hitObject);
                hitObjectClone.transform.position = hit.point;
                hitObjectClone.transform.parent = hit.transform;
                grapplePoint = hitObjectClone.transform.position;

                grappledObj = hit.transform.gameObject;

                corruptObject.MakeSpotNotGrappable(hit, grappledObj);

                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance = distanceFromPoint;
                joint.minDistance = dist;

                distance = dist - grappleLengthModifier;

                joint.enableCollision = false;

                joint.spring = springValue;
                joint.damper = springDamp;
                joint.massScale = springMass;


                lr.positionCount = 2;
                currentGrapplePosition = hitObjectClone.transform.position;
                GetComponent<FMODUnity.StudioEventEmitter>().Play();

                //Pinwheel
                Pinwheel pinwheel = null;
                if (pinwheel = hit.collider.GetComponentInParent<Pinwheel>())
                {
                    pinwheel.TriggerRotation(hit.collider.transform, camera.forward);
                }

                //Temporary lock UI disabled after completing a grapple
                if (grappleToggleEnabledText != null)
                {
                    grappleToggleEnabledText.SetActive(false);
                }
                if (grappleToggleDisabledText != null)
                {
                    grappleToggleDisabledText.SetActive(true);
                }
            }

            else
            {
                Debug.Log(secondHit.collider.gameObject.name);
                Debug.Log(secondHit.collider.gameObject.transform.position);
            }

        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        swingLockToggle = false;

        //Temporary lock UI disabled after completing a grapple
        if (grappleToggleEnabledText != null)
        {
            grappleToggleEnabledText.SetActive(false);
        }
        if (grappleToggleDisabledText != null)
        {
            grappleToggleDisabledText.SetActive(false);
        }

        if (hitObjectClone)
        {
            Destroy(hitObjectClone.gameObject);
        }

        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    /// <summary>
    /// Draws the line from the grapple gun to the current grapple point.
    /// </summary>
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        if (lr.positionCount == 0) return;

        currentGrapplePosition = grapplePoint;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    /// <summary>
    /// Booleon function that determines if the player is grappleing or not.
    /// </summary>
    /// <returns></returns>
    public bool IsGrappling()
    {
        return joint != null;
    }

    /// <summary>
    /// Vector function that returns the grapple point
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    /// <summary>
    /// Returns the max grapple distance possible.
    /// </summary>
    /// <returns></returns>
    public float GetMaxGrappleDistance()
    {
        return maxGrappleDistance;
    }

    /// <summary>
    /// Returns the Rayhit from the grappling Raycast.
    /// </summary>
    /// <returns></returns>
    public RaycastHit GetGrappleRayhit()
    {
        return grappleRayHit;
    }

    public bool GetSwingLockToggle()
    {
        return swingLockToggle;
    }

    public bool GetCanApplyForce()
    {
        return canApplyForce;
    }
}
