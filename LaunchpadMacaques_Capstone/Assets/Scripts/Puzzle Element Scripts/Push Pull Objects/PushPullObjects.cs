/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (PushPullObjects.CS) 
* (The Script that is placed on the player to handle picking and throwing objects) 
*/

using UnityEngine;

public class PushPullObjects : MonoBehaviour
{
    #region Inspector Values
    [SerializeField][Tooltip("The Max Distance the player can pick up an object")] float maxGrabDistance;
    [SerializeField][Tooltip("The Empty Game Objec that the picked up object will move to")] GameObject objectHolder;
    [SerializeField][Tooltip("The Layer for object that can be picked up")] LayerMask canBePickedUp;
    #endregion

    #region Private Variables
    private Rigidbody objectRB;
    private GameObject cam;
    private bool grabbing = false;
    private LineRenderer lr;

    GrapplingGun grapplingGun;

    float objectFollowSpeed = 5;
    #endregion

    void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        lr = this.GetComponent<LineRenderer>();
        grapplingGun = this.GetComponent<GrapplingGun>();
    }

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0) /*&& !grapplingGun.IsGrappling()*/)
        {
    
            if (grabbing)
            {
                DropObject();
            }

            else
            {
                PickUpObject();
            }
  
        }

        if(Input.GetMouseButtonDown(1) & grabbing)
        {
           ThrowObject();
        }


    }

    private void FixedUpdate()
    {
        if (grabbing)
        {
            objectRB.MovePosition(Vector3.Lerp(objectRB.position, objectHolder.transform.position, Time.deltaTime * objectFollowSpeed));
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
    private void DropObject()
    {
        grabbing = false;
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
        objectRB.isKinematic = false;
        objectRB.GetComponent<PushableObj>().StartPush(cam);
        objectRB.useGravity = false;
        grabbing = false;
        lr.positionCount = 0;
        objectRB = null;

    }

    /// <summary>
    /// Will Draw The Rope from the players hand to the grabbed object
    /// </summary>
    void DrawRope()
    { 
        if(lr.positionCount > 0)
        {
            lr.SetPosition(0, objectRB.transform.position);
            lr.SetPosition(1, this.transform.position);
        }

    }

    /// <summary>
    /// Returns if the Player Is Grabbing Something
    /// </summary>
    /// <returns></returns>
    public bool IsGrabbing()
    {
        return grabbing;
    }

}
