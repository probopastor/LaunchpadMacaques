using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PushableObj : MonoBehaviour
{
    #region Inspector Vars
    [Header("Visual Settings")]
    [SerializeField] GameObject throwDecal;
 
    [Header("Movement Settings")]
    public float gravityScaler = 5;
    [SerializeField] float distance;


 
    [Header("Change Distance Settings")]
    [SerializeField] private float wheelSensitivity = 5;
    [SerializeField] bool changeDistance = true;
    [SerializeField] float minDistance = 5;
    [SerializeField] float maxDistance = 40;
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
    #endregion
    private void Start()
    {
        thisDecal = Instantiate(throwDecal);
        thisDecal.SetActive(false);
        beingPushed = false;
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }
    public void StartPush(GameObject cam)
    {
        beingPushed = true;
        objectVelocity = distance * cam.transform.forward;
    }

    private void Update()
    {
        if (changeDistance & pickedUp)
        {
            ChangeDistance();
        }
  
    }

    private void FixedUpdate()
    {
        if (beingPushed) PushObject();

        else if (pickedUp) ShowLine();
    }

    private void ChangeDistance()
    {
        var wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput > 0)
        {
            distance += wheelSensitivity;
            if(distance > maxDistance)
            {
                distance = maxDistance;
            }
        }

        else if (wheelInput < 0)
        {
            distance -= wheelSensitivity;

            if(distance < minDistance)
            {
                distance = minDistance;
            }
        }
    }

    private void PushObject()
    {
        pickedUp = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 point1 = this.transform.position;
        float stepSize = 1.0f / predStepsPerFrame;


        for (float step = 0; step < 1; step += stepSize)
        {
            objectVelocity += (Physics.gravity * gravityScaler )* stepSize * Time.deltaTime;
            Vector3 point2 = point1 + objectVelocity * stepSize * Time.deltaTime;

            RaycastHit hit;
            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, out hit ,(point2 - point1).magnitude))
            {
                if (!hit.collider.isTrigger)
                {
                    StopPushingObject();
                }
            }


            point1 = point2;
        }

        this.transform.position = point1;
    }

    #region Line
    private void ShowLine()
    {
        tempSpeed = distance;
        Vector3 point1 = this.transform.position;
        Vector3 predObjectVelocity = objectVelocity;
        predObjectVelocity = tempSpeed * tempCam.transform.forward;
        float stepSize = .1f;
        lr.positionCount = 2;
        lr.SetPosition(0, this.transform.position);
        int count = 1;
        for (float step = 0; step < 500; step += stepSize)
        {
            predObjectVelocity += (Physics.gravity * gravityScaler) * stepSize;
            Vector3 point2 = point1 + predObjectVelocity * stepSize;

            RaycastHit hit;
            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude))
            {
                if (!hit.collider.isTrigger)
                {
                    lr.positionCount = count;
                    thisDecal.SetActive(true);
                    MoveDecal(hit);
                    break;
                }

            }

            lr.SetPosition(count, point2);
            point1 = point2;
            count++;
            lr.positionCount++;


        }

    }

    #endregion


    private void MoveDecal(RaycastHit info)
    {
        thisDecal.transform.position = info.point;
        thisDecal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);

    }

    #region Pick Up/Drop Object

    public void PickedUpObject(GameObject cam)
    {
        tempCam = cam;
        beingPushed = false;
        pickedUp = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    public void DroppedObject()
    {
        pickedUp = false;
        beingPushed = false;
        GetComponent<Rigidbody>().useGravity = true;
        lr.positionCount = 0;
        thisDecal.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopPushingObject();
    }

    private void StopPushingObject()
    {
        beingPushed = false;
        GetComponent<Rigidbody>().useGravity = true;
        pickedUp = false;
        lr.positionCount = 0;
        thisDecal.SetActive(false);

    }

    #endregion
}
