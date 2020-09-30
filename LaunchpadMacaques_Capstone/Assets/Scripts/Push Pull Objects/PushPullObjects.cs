using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullObjects : MonoBehaviour
{
    [SerializeField] float maxGrabDistance;
    [SerializeField] GameObject objectHolder;
    [SerializeField] LayerMask canBePickedUp;
    [SerializeField] float objectFollowSpeed = 10;



    private Rigidbody objectRB;
    private GameObject cam;
    private bool grabbing = false;
    private LineRenderer lr;
    //// Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        lr = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

    private void PickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrabDistance, canBePickedUp))
        {
            objectRB = hit.rigidbody;
            objectRB.isKinematic = true;

            grabbing = true;

            lr.positionCount = 2;
            objectRB.GetComponent<PushableObj>().PickedUpObject(cam);


        }
    }

    private void DropObject()
    {
        grabbing = false;
        objectRB.isKinematic = false;
        objectRB.GetComponent<PushableObj>().DroppedObject();
        objectRB = null;
        lr.positionCount = 0;

    }

    private void ThrowObject()
    {
        objectRB.isKinematic = false;
        objectRB.GetComponent<PushableObj>().StartPush(cam);
        objectRB.useGravity = false;
        grabbing = false;
        lr.positionCount = 0;
        objectRB = null;

    }


    void DrawRope()
    { 
        lr.SetPosition(0, objectRB.transform.position);
        lr.SetPosition(1, this.transform.position);
    }

    public bool IsGrabbing()
    {
        return grabbing;
    }

}
