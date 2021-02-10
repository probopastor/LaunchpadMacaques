/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof, William Nomikos) 
* (PushPullObjects.CS) 
* (The Script that is placed on the player to handle picking and throwing objects) 
*/

using System.Collections;
using UnityEngine;

public class PushPullObjects : MonoBehaviour
{
    #region Inspector Values

    [Header("The Held Cube Positions")]
    [SerializeField, Tooltip("The Position the Cube will go to when Top Left is Chosen")] GameObject topLeft;
    [SerializeField, Tooltip("The Position the Cube will go to when Bottom Left is Chosen")] GameObject bottomLeft;

    [SerializeField, Tooltip("The Position the Cube will go to when TopRight is Chosen")] GameObject topRight;
    [SerializeField, Tooltip("The Position the Cube will go to when Bottom Right is Chosen")] GameObject bottomRight;

    [SerializeField, Tooltip("The Position the Cube will go to when Top Middle is Chosen")] GameObject topMiddle;
    [SerializeField, Tooltip("The Position the Cube will go to when Bottom Middle is Chosen")] GameObject bottomMiddle;
    [SerializeField] [Tooltip("The Max Distance the player can pick up an object")] float maxGrabDistance;
    [SerializeField] [Tooltip("The Layer for object that can be picked up")] LayerMask canBePickedUp;


    [Header("Object Following Settings")]
    [SerializeField, Tooltip("Which position the moveable cube will move towards")] ObjectFollowPostion objectFollowPos;

    [SerializeField, Tooltip("The Speed at which the cube will follow the player")] float objectFollowSpeed = 20;

    [Header("Pick Up Settings")]
    [SerializeField, Tooltip("The speed at which the Line Will connect to object")] float startAttachSpeed = 20;
    [SerializeField, Tooltip("The speed at which the Line will speed up over time")] float attachSpeedIncrese = 1;
    [SerializeField, Tooltip("The Inital Speed the cube will move before reaching its follow position")] float pickupSpeed = 8;
    #endregion

    [SerializeField]
    Animator anim;





    #region Private Variables
    public enum ObjectFollowPostion { topLeft, bottomLeft, topRight, bottomRight, topMiddle, bottomMiddle };
    private Rigidbody objectRB;
    private GameObject cam;
    private bool grabbing = false;
    private LineRenderer lr;

    GrapplingGun grapplingGun;


    GameObject objectHolder;
    GameObject currentHoveredObj;

    private bool inTempFix = false;

    private bool drawingLine = false;

    private float orgFollowSpeed;

    private bool holdingDownStartGrapple;
    private bool holdingDownStopGrapple;
    #endregion

    private void Awake()
    {
        orgFollowSpeed = objectFollowSpeed;
        cam = FindObjectOfType<Camera>().gameObject;
        lr = this.GetComponent<LineRenderer>();
        grapplingGun = this.GetComponent<GrapplingGun>();

        SetFollowPosition(objectFollowPos);
    }


    /// <summary>
    /// Will Set the Cubes follow position
    /// </summary>
    /// <param name="newFollowPos"></param>
    public void SetFollowPosition(ObjectFollowPostion newFollowPos)
    {
        switch (newFollowPos)
        {
            case ObjectFollowPostion.topLeft:
                objectHolder = topLeft;
                break;
            case ObjectFollowPostion.bottomLeft:
                objectHolder = bottomLeft;
                break;
            case ObjectFollowPostion.topRight:
                objectHolder = topRight;
                break;
            case ObjectFollowPostion.bottomRight:
                objectHolder = bottomRight;
                break;
            case ObjectFollowPostion.topMiddle:
                objectHolder = topMiddle;
                break;
            case ObjectFollowPostion.bottomMiddle:
                objectHolder = bottomMiddle;
                break;
        }
    }

    void Update()
    {
        PickUpFeedback();
        UserInput();
        ResetObjectFollowSpeed();

        if(Input.GetAxis("Start Grapple") < 1 && holdingDownStartGrapple)
        {
            holdingDownStartGrapple = false;
        }

        if(Input.GetAxis("Start Grapple") > -1 && holdingDownStopGrapple)
        {
            holdingDownStopGrapple = false;
        }
    }

    /// <summary>
    /// Will reset the object follow speed when cube reaches certain distance from player
    /// </summary>
    private void ResetObjectFollowSpeed()
    {
        if (grabbing)
        {
            if (objectFollowSpeed < orgFollowSpeed)
            {
                if (Vector3.Distance(this.transform.position, objectRB.position) < 5)
                {
                    objectFollowSpeed = orgFollowSpeed;
                }
            }


        }
    }

    /// <summary>
    /// Will handle the mouse input from the player
    /// </summary>
    private void UserInput()
    {
        if ((Input.GetButtonDown("Start Grapple") || GetStartGrappleDown()) && Time.timeScale != 0)
        {
            if (!grabbing)
            {
                PickUpObject();
            }
            else if (grabbing)
            {
                ThrowObject();
            }

        }
        else if ((Input.GetButtonDown("Stop Grapple") || GetStopGrappleDown()) && Time.timeScale != 0)
        {
            if (grabbing)
            {
                DropObject();
            }
        }

    }

    private bool GetStartGrappleDown()
    {
        if (Input.GetAxis("Start Grapple") > 0)
        {
            if (holdingDownStartGrapple)
            {
                return false;
            }

            else
            {
                return true;
            }
        }

        else return false;
    }

    private bool GetStopGrappleDown()
    {
        if (Input.GetAxis("Start Grapple") < 0)
        {
            if (holdingDownStopGrapple)
            {
                return false;
            }

            else
            {
                return true;
            }
        }

        else return false;
    }

    private void FixedUpdate()
    {
        MoveBox();
    }


    /// <summary>
    /// If the player is holding a box
    /// Will determine if the box can move and will move the box
    /// </summary>
    private void MoveBox()
    {
        if (grabbing && !inTempFix)
        {
            float distance = Vector3.Distance(objectHolder.transform.position, objectRB.gameObject.transform.position);
            RaycastHit hit;
            if (!Physics.Raycast(objectRB.gameObject.transform.position, objectHolder.transform.position - objectRB.transform.position, out hit, distance))
            {
                objectRB.MovePosition(Vector3.Lerp(objectRB.position, objectHolder.transform.position, Time.fixedDeltaTime * objectFollowSpeed));

            }

            else
            {
                if (hit.collider.isTrigger)
                {
                    objectRB.MovePosition(Vector3.Lerp(objectRB.position, objectHolder.transform.position, Time.fixedDeltaTime * objectFollowSpeed));

                }
            }
        }
    }

    private void LateUpdate()
    {
        if (grabbing)
        {
            DrawRope();
        }

    }

    /// <summary>
    /// The Method that is called to pick up an object
    /// </summary>
    private void PickUpObject()
    {
        RaycastHit hit = CanSeeBox();
        if (hit.collider != null)
        {
            holdingDownStartGrapple = true;
            anim.ResetTrigger("Throw");
            anim.ResetTrigger("Drop");
            anim.SetTrigger("PickUp");


            grapplingGun.StopGrapple();
            objectRB = hit.rigidbody;
            objectRB.useGravity = false;
            // objectRB.isKinematic = true;


            StartCoroutine(LineAnimation());
            objectRB.GetComponent<PushableObj>().PickedUpObject(cam);


        }
    }


    /// <summary>
    /// Will handle the particle effects/Outline features when the player hovers over a moveable cube
    /// </summary>
    private void PickUpFeedback()
    {
        RaycastHit hit = CanSeeBox();
        if (hit.collider != null)
        {
            if (currentHoveredObj != hit.collider.gameObject)
            {
                hit.collider.gameObject.GetComponent<PushableObj>().EnableDisableOutline(true);
                hit.collider.gameObject.GetComponent<PushableObj>().ObjectHovered(true);
                hit.collider.gameObject.GetComponent<PushableObj>().CheckParticleStatus();
                currentHoveredObj = hit.collider.gameObject;
            }
        }

        else
        {

            if (currentHoveredObj)
            {
                currentHoveredObj.GetComponent<PushableObj>().EnableDisableOutline(false);
                currentHoveredObj.GetComponent<PushableObj>().ObjectHovered(false);
                currentHoveredObj.GetComponent<PushableObj>().CheckParticleStatus();
                currentHoveredObj = null;
            }

        }

    }

    /// <summary>
    /// Will return if the player can currently see the a box
    /// </summary>
    /// <returns></returns>
    public RaycastHit CanSeeBox()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrabDistance, canBePickedUp) && !grapplingGun.IsGrappling())
        {
            return hit;
        }

        else return hit;
    }

    /// <summary>
    /// Method that is called to drop the currently held object
    /// </summary>
    public void DropObject()
    {
        holdingDownStopGrapple = true;
        anim.ResetTrigger("PickUp");
        anim.SetTrigger("Drop");
        //currentHoveredObj.GetComponent<PushableObj>().CheckParticleStatus();
        StartCoroutine(GrabbingFalse());
        objectRB.isKinematic = false;
        objectRB.useGravity = true;
        objectRB.GetComponent<PushableObj>().DroppedObject();
        objectRB = null;
        lr.positionCount = 0;

    }


    /// <summary>
    /// The Method that is called to start throwing the currently held object
    /// </summary>
    private void ThrowObject()
    {
        anim.ResetTrigger("PickUp");
        anim.SetTrigger("Throw");

        if (objectRB)
        {
            objectRB.isKinematic = false;
            objectRB.GetComponent<PushableObj>().StartPush(cam);
            objectRB.useGravity = false;
            StartCoroutine(GrabbingFalse());
            lr.positionCount = 0;
            objectRB = null;
        }


    }

    /// <summary>
    /// Will Draw The Rope from the players hand to the grabbed object
    /// </summary>
    void DrawRope()
    {
        if (lr.positionCount > 0 && !drawingLine)
        {
            lr.SetPosition(0, objectRB.transform.position);
            lr.SetPosition(1, this.transform.position);
        }

    }

    /// <summary>
    /// Will handle the extending of rope animation
    /// </summary>
    /// <returns></returns>
    IEnumerator LineAnimation()
    {
        drawingLine = true;
        lr.positionCount = 2;
        Vector3 grappled = objectRB.transform.position;
        float dist = Vector3.Distance(this.transform.position, grappled);

        float counter = 0;

        objectFollowSpeed = pickupSpeed;


        lr.SetPosition(0, grappled);


        float tempAttachSpeed = startAttachSpeed;
        while (counter < dist)
        {

            Vector3 point1 = this.transform.position;
            Vector3 point2 = objectRB.transform.position;

            lr.SetPosition(0, this.transform.position);
            counter += tempAttachSpeed * Time.deltaTime;

            Vector3 pointAlongLine = (counter) * Vector3.Normalize(point2 - point1) + point1;

            lr.SetPosition(1, pointAlongLine);

            tempAttachSpeed += attachSpeedIncrese * Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        grabbing = true;
        drawingLine = false;
    }

    /// <summary>
    /// will be called after letting go of the cube
    /// will not allow the player to grapple or grab another box
    /// Until the player lets go of the Left Mouse Button
    /// </summary>
    /// <returns></returns>
    IEnumerator GrabbingFalse()
    {
        inTempFix = true;
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(0);
        }

        grabbing = false;
        inTempFix = false;
    }


    #region Getters
    /// <summary>
    /// Returns if the Player Is Grabbing Something
    /// </summary>
    /// <returns></returns>
    public bool IsGrabbing()
    {
        if (grabbing || drawingLine)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the game object of the currently held cube. 
    /// </summary>
    /// <returns></returns>
    public GameObject GetHeldCube()
    {
        return objectRB.gameObject;
    }

    public GameObject GetHoverdObject()
    {
        return currentHoveredObj;
    }

    #endregion

}
