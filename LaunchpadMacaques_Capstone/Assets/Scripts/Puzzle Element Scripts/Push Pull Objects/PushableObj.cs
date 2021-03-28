﻿/* 
* (Launchpad Macaques - [Trial and Error])
* (Levi Schoof, William Nomikos)
* (PushableObj.CS)
* (The Script placed on object the player can pick up and throw)
*/

using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;

public class PushableObj : MonoBehaviour
{
    #region Inspector Vars
    [Header("Visual Settings")]
    [SerializeField] [Tooltip("The Decal that will be placed to make part of object look Corrupted")] GameObject throwDecal;

    [Header("Movement Settings")]
    [SerializeField] [Tooltip("The Variable that will be multiplyed by deafult grabity to apply gravity to this object")] float gravityScaler = 1.75f;
    [SerializeField] [Tooltip("The distance an object will fly, when thrown")] float distance;
    [SerializeField] LayerMask layersThisCanLandOn;



    [Header("Change Distance Settings")]
    [SerializeField] [Tooltip("How much the distance will change when the player moves the mouse wheel")] float wheelSensitivity = 5;
    [SerializeField] [Tooltip("The Bool which will determine if the player can change the Object Fly Distance")] bool changeDistance = true;
    [SerializeField] [Tooltip("The Min Fly Distance for the Object")] float minDistance = 5;
    [SerializeField] [Tooltip("The Max Fly Distance for the Object")] float maxDistance = 40;

    [SerializeField, Tooltip("The Layer that is ground")] LayerMask ground;

    [Header("Particle Settings")]
    [SerializeField, Tooltip("If True particles effects will scale depending on how far away the player is")] bool scaleWithDistance = false;
    [SerializeField, Tooltip("The amount the size of the particles will scale with player distance")] float sizeScaleAmount = 0;
    [SerializeField, Tooltip("The amount the speed of the particles will scale with player distance")] float speedScaleAmount = 0;
    [SerializeField, Tooltip("The grappling point layers that should respawn the throwable cube. ")] private LayerMask grapplingPointLayers;
    #endregion

    #region Private Vars
    private float tempSpeed;
    private GameObject tempCam;
    private GameObject thisDecal;
    private Vector3 objectVelocity;
    private bool beingPushed;
    private bool pickedUp = false;
    int predStepsPerFrame = 6;
    private LineRenderer lr;
    private Vector3 respawnPos;

    private Gravity grav;

    private CollectibleController cc;

    private StudioEventEmitter soundEmitter;

    private ParticleSystem particles;

    private ParticleSystem.MainModule main;

    private ParticleSystem.MinMaxCurve particleStartingSpeed;
    private ParticleSystem.MinMaxCurve particleStartingSize;
    private PushPullObjects pushPull;

    private ParticleTrigger Trigger;
    private GameObject outlineObj;

    private float cubeRadius;

    private bool objectHovered;
    private CubeRespawn respawnRef;

    Matt_PlayerMovement player;
    #endregion

    PlayerControlls controls;
    float wheelInput;
    private void Awake()
    {
        controls = new PlayerControlls();
        controls.GamePlay.Scroll.performed += ChangeDistance;
        controls.GamePlay.Scroll.canceled += ChangeDistance;

        pushPull = FindObjectOfType<PushPullObjects>();
        respawnRef = GetComponent<CubeRespawn>();
        CreateDecalAndLine();

        Trigger = this.GetComponent<ParticleTrigger>();

        grav = this.GetComponent<Gravity>();
        cc = FindObjectOfType<CollectibleController>();
        soundEmitter = GetComponent<StudioEventEmitter>();
        particles = this.GetComponentInChildren<ParticleSystem>();
        //particles = this.GetComponent<ParticleSystem>();

        outlineObj = gameObject.transform.Find("OutlineObj").gameObject;
        outlineObj.GetComponent<Renderer>().enabled = false;

        main = particles.main;

        particles.Stop();

        particleStartingSpeed = main.startSpeed;
        particleStartingSize = main.startSize;

        cubeRadius = this.gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2;

        objectHovered = false;

        player = FindObjectOfType<Matt_PlayerMovement>();
    }

    private void Start()
    {
        TurnOnOffParticles(true);
    }


    /// <summary>
    /// Will Create and Set the decals and Line Renderer
    /// </summary>
    private void CreateDecalAndLine()
    {
        respawnPos = this.transform.position;
        thisDecal = Instantiate(throwDecal);
        thisDecal.SetActive(false);
        beingPushed = false;
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    /// <summary>
    /// The Public method that is called to start the push of the object
    /// </summary>
    /// <param name="cam"></param>
    public void StartPush(GameObject cam)
    {
        beingPushed = true;
        objectVelocity = distance * cam.transform.forward;
    }

    private void Update()
    {
        if (cc.GetIsActive())
        {
            grav.gravity = cc.GetNewGravity();
        }

        else
        {
            grav.gravity = grav.GetOrgGravity();
        }

        if(pickedUp && changeDistance)
        {
            ApplyDistanceChange();
        }

        ResetObjectPos();

        if (particles.isPlaying && scaleWithDistance)
        {
            var dis = Vector3.Distance(this.gameObject.transform.position, pushPull.transform.position);
            main.startSpeed = particleStartingSpeed.constant * (1 + (dis * speedScaleAmount));
            main.startSize = particleStartingSize.constant * (1 + (dis * sizeScaleAmount));
        }
    }


    /// <summary>
    /// If the object flys off the map, the object will be reset to starting position
    /// </summary>
    private void ResetObjectPos()
    {
        if (this.transform.position.y < -100)
        {
            this.transform.position = respawnPos;
        }
    }

    private void FixedUpdate()
    {
        if (beingPushed) PushObject();

        else if (pickedUp) ShowLine();
    }

    /// <summary>
    /// Will Get The User's Mouse Wheel Input to Change Object Distance
    /// </summary>
    private void ChangeDistance(InputAction.CallbackContext cxt)
    {
        wheelInput = cxt.ReadValue<Vector2>().y;


    }

    private void ApplyDistanceChange()
    {
        if (wheelInput > 0)
        {
            distance += wheelSensitivity;
            if (distance > maxDistance)
            {
                distance = maxDistance;
            }
        }

        else if (wheelInput < 0)
        {
            distance -= wheelSensitivity;

            if (distance < minDistance)
            {
                distance = minDistance;
            }
        }
    }


    /// <summary>
    /// The Private method that is called to actually push an object
    /// </summary>
    private void PushObject()
    {
        controls.Disable();
        pickedUp = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        grav.UseGravity(true);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().freezeRotation = false;
        Vector3 point1 = this.transform.position;
        float stepSize = 1.0f / predStepsPerFrame;


        /// Will run through and will predict where the object will go, creating the Line as it goes
        for (float step = 0; step < 1; step += stepSize)
        {
            objectVelocity += ((transform.up * -1) * grav.GetGravityAmmount() * Time.deltaTime) * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + objectVelocity * stepSize * Time.deltaTime;

            RaycastHit hit;
            Ray ray = new Ray(point1, point2 - point1);

            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude, layersThisCanLandOn))
            {
                if (!hit.collider.isTrigger)
                {
                    StopPushingObject();
                    return;
                }
            }


            point1 = point2;
        }

        this.transform.position = point1;
        soundEmitter.EventInstance.triggerCue();
    }

    #region Line
    /// <summary>
    /// Will Create a line showing the prediction of where the object will go
    /// </summary>
    private void ShowLine()
    {
        bool hitCollectable = false;
        tempSpeed = distance;
        Vector3 point1 = this.transform.position;
        Vector3 predObjectVelocity = objectVelocity;
        predObjectVelocity = tempSpeed * tempCam.transform.forward;
        float stepSize = .01f;
        lr.positionCount = 2;
        lr.SetPosition(0, this.transform.position);
        int count = 1;
        for (float step = 0; step < 500; step += stepSize)
        {
            predObjectVelocity += ((transform.up * -1) * grav.GetGravityAmmount() * Time.deltaTime) * stepSize;
            Vector3 point2 = point1 + predObjectVelocity * stepSize;

            RaycastHit hit;
            Ray ray = new Ray(point1, point2 - point1);

            /// If the thing predicts that it will run into a non trigger object it will stop the line there, and place a decal there.
            /// It if is an object the cube can affect the  line will turn green
            if (Physics.SphereCast(ray, cubeRadius, out hit, (point2 - point1).magnitude, layersThisCanLandOn))
            {
                if (hit.collider.gameObject.CompareTag("Collectible") || hit.collider.gameObject.CompareTag("PassBy"))
                {
                    hitCollectable = true;
                }

                if (!hit.collider.isTrigger)
                {
                    lr.positionCount = count;
                    thisDecal.SetActive(true);
                    MoveDecal(hit);
                    break;
                }
                else
                {
                    thisDecal.SetActive(false);
                }


            }

            if (lr.positionCount > 1000)
            {
                break;
            }


            /// Sets the new position of the Line Renderer and Resets the points
            lr.SetPosition(count, point2);
            point1 = point2;
            count++;
            lr.positionCount++;

            /// An If statement to stop the game from breaking if the Line becomes to big



        }

        if (hitCollectable)
        {
            lr.startColor = Color.green;
            lr.endColor = Color.green;
        }

        else if (CheckForLanding())
        {
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }

        else
        {
            lr.startColor = Color.red;
            lr.endColor = Color.red;
        }

    }

    /// <summary>
    /// Will check to see if this cube will land on solid ground
    /// </summary>
    /// <returns></returns>
    private bool CheckForLanding()
    {
        int pos = lr.positionCount;

        Vector3 posLocation;
        if (pos > 5)
        {
            posLocation = lr.GetPosition(pos - 5);
        }

        else
        {
            posLocation = lr.GetPosition(pos - 1);
        }

        RaycastHit hit;

        if (Physics.Raycast(posLocation, Vector3.down, out hit, 20, layersThisCanLandOn))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    #endregion


    /// <summary>
    /// Will Place The Decal at givin spot
    /// </summary>
    /// <param name="info"></param>
    private void MoveDecal(RaycastHit info)
    {
        thisDecal.transform.position = info.point;
        thisDecal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);

    }

    #region Pick Up/Drop Object

    /// <summary>
    /// Public method that is called to pick up this object
    /// </summary>
    /// <param name="cam"></param>
    public void PickedUpObject(GameObject cam)
    {
        controls.Enable();
        //TurnOnOffParticles(false);
        tempCam = cam;
        beingPushed = false;
        CheckParticleStatus();
        pickedUp = true;
        grav.UseGravity(false);
        GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;

        if (soundEmitter.IsPlaying()) soundEmitter.Stop();
        soundEmitter.Play();
    }

    /// <summary>
    /// Public method that will be called to drop the object
    /// </summary>
    public void DroppedObject()
    {
        controls.Disable();
        pickedUp = false;
        beingPushed = false;
        CheckParticleStatus();
        grav.UseGravity(true);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().freezeRotation = false;
        lr.positionCount = 0;
        thisDecal.SetActive(false);
        soundEmitter.EventInstance.triggerCue();
    }

    /// <summary>
    /// When this object hits an object, will stop being pushed
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");

        if (beingPushed)
        {
            StopPushingObject();
        }

        if (!pickedUp)
        {
            CheckIfOnGrapplePoint();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");

        if (!pickedUp)
        {
            CheckIfOnGrapplePoint();
        }
    }

    /// <summary>
    /// Method that is called to stop pushing this object
    /// </summary>
    public void StopPushingObject()
    {
        beingPushed = false;
        CheckParticleStatus();
        grav.UseGravity(true);
        pickedUp = false;
        lr.positionCount = 0;
        thisDecal.SetActive(false);
    }

    /// <summary>
    /// Checks to see if the throwable cube landed on a grapple point. If it did, respawn the cube.
    /// </summary>
    private void CheckIfOnGrapplePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f, grapplingPointLayers))
        {
            respawnRef.RespawnCube();
        }
    }

    /// <summary>
    /// Returns true if the object is being pushed from a throw.
    /// </summary>
    /// <returns></returns>
    public bool GetPushStatus()
    {
        return beingPushed;
    }


    #endregion

    #region Feedback Settings
    /// <summary>
    /// Will Turn On or Off the Feedback particles
    /// </summary>
    /// <param name="onOff"></param>
    public void TurnOnOffParticles(bool onOff)
    {
        if (onOff)
        {
            particles.Play();
            Trigger.Trigger();
            
        }

        else
        {
            particles.Stop();
        }
    }

    /// <summary>
    /// Checks whether or not the cube particles should be enabled or disabled.
    /// </summary>
    public void CheckParticleStatus()
    {
        if (GetPushStatus() || objectHovered || pickedUp)
        {
            TurnOnOffParticles(false);
        }
        else if (!GetPushStatus() && !objectHovered && !pickedUp)
        {
            TurnOnOffParticles(true);
        }
    }

    /// <summary>
    /// Sets whether or not the current object is being hovered over.
    /// </summary>
    /// <param name="isHovering"></param>
    public void ObjectHovered(bool isHovering)
    {
        objectHovered = isHovering;
    }

    /// <summary>
    /// Will Turn On or Off the Feedback outline
    /// </summary>
    /// <param name="enable"></param>
    public void EnableDisableOutline(bool enable)
    {
        if (enable && pushPull.CanPickUpObjects())
        {
            outlineObj.GetComponent<Renderer>().enabled = true;
        }
        else if (!enable)
        {
            outlineObj.GetComponent<Renderer>().enabled = false;
        }
    }

    #endregion
}
