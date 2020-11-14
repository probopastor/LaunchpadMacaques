/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (PushPullObjects.CS) 
* (The Script that is placed on the player to handle picking and throwing objects) 
*/

using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PushPullObjects : MonoBehaviour
{
    #region Inspector Values

    [Header("The Held Cube Positions")]
    [SerializeField] GameObject topLeft;
    [SerializeField] GameObject bottomLeft;

    [SerializeField] GameObject topRight;
    [SerializeField] GameObject bottomRight;

    [SerializeField] GameObject topMiddle;
    [SerializeField] GameObject bottomMiddle;
    [SerializeField] [Tooltip("The Max Distance the player can pick up an object")] float maxGrabDistance;
    [SerializeField] [Tooltip("The Layer for object that can be picked up")] LayerMask canBePickedUp;



    enum ObjectFollowPostion { topLeft, bottomLeft, topRight, bottomRight, topMiddle, bottomMiddle };

    [SerializeField, Tooltip("Which position the moveable cube will move towards")] ObjectFollowPostion objectFollowPos;

    [SerializeField, Tooltip("The Speed at which the cube will follow the player")] float objectFollowSpeed = 20;
    #endregion

    [SerializeField]
    Animator anim;

    #region Private Variables
    private Rigidbody objectRB;
    private GameObject cam;
    private bool grabbing = false;
    private LineRenderer lr;

    GrapplingGun grapplingGun;


    GameObject objectHolder;



    private bool inTempFix = false;
    #endregion

    private void Awake()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        lr = this.GetComponent<LineRenderer>();
        grapplingGun = this.GetComponent<GrapplingGun>();

        switch (objectFollowPos)
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

        if (Input.GetMouseButtonDown(0) /*&& !grapplingGun.IsGrappling()*/)
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
        else if (Input.GetMouseButtonDown(1))
        {
            if (grabbing)
            {
                DropObject();
            }
        }

    }

    
    private void FixedUpdate()
    {
        if (grabbing && !inTempFix)
        {
            Debug.DrawRay(objectRB.position, objectHolder.transform.position - objectRB.transform.position);
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
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrabDistance, canBePickedUp) && !grapplingGun.IsGrappling())
        {

            anim.ResetTrigger("Throw");
            anim.ResetTrigger("Drop");
            anim.SetTrigger("PickUp");


            grapplingGun.StopGrapple();
            objectRB = hit.rigidbody;
            objectRB.useGravity = false;
            // objectRB.isKinematic = true;

            grabbing = true;

            lr.positionCount = 2;
            objectRB.GetComponent<PushableObj>().PickedUpObject(cam);


        }
    }

    /// <summary>
    /// Method that is called to drop the currently held object
    /// </summary>
    public void DropObject()
    {
        anim.ResetTrigger("PickUp");
        anim.SetTrigger("Drop");

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

        objectRB.isKinematic = false;
        objectRB.GetComponent<PushableObj>().StartPush(cam);
        objectRB.useGravity = false;
        StartCoroutine(GrabbingFalse());
        lr.positionCount = 0;
        objectRB = null;

    }

    /// <summary>
    /// Will Draw The Rope from the players hand to the grabbed object
    /// </summary>
    void DrawRope()
    {
        if (lr.positionCount > 0)
        {
            lr.SetPosition(0, objectRB.transform.position);
            lr.SetPosition(1, this.transform.position);
        }

    }

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

    /// <summary>
    /// Returns if the Player Is Grabbing Something
    /// </summary>
    /// <returns></returns>
    public bool IsGrabbing()
    {
        return grabbing;
    }

    /// <summary>
    /// Returns the game object of the currently held cube. 
    /// </summary>
    /// <returns></returns>
    public GameObject GetHeldCube()
    {
        return objectRB.gameObject;
    }

}
