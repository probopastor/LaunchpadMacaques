using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PushableObj : MonoBehaviour
{
    private bool beingPushed;
    private bool pickedUp = false;

    [SerializeField] GameObject throwDecal;
    private GameObject thisDecal;
    [SerializeField] Vector3 objectVelocity;
    [SerializeField] int predStepsPerFrame;

    private float tempSpeed;
    private GameObject tempCam;

    private LineRenderer lr;
    private void Start()
    {
        thisDecal = Instantiate(throwDecal);
        thisDecal.SetActive(false);
        beingPushed = false;
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }
    public void StartPush(float speed, GameObject cam)
    {
        beingPushed = true;
        objectVelocity = speed * cam.transform.forward;
    }

    private void Update()
    {
        if (beingPushed) PushObject();

        else if (pickedUp) ShowLine();
    }

    private void PushObject()
    {
        pickedUp = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 point1 = this.transform.position;
        float stepSize = 1.0f / predStepsPerFrame;


        for (float step = 0; step < 1; step += stepSize)
        {
            objectVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + objectVelocity * stepSize * Time.deltaTime;

            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, (point2 - point1).magnitude))
            {
                StopPushingObject();
            }


            point1 = point2;


        }

        this.transform.position = point1;
    }

    private void ShowLine()
    {
        Vector3 point1 = this.transform.position;
        Vector3 predObjectVelocity = objectVelocity;
        predObjectVelocity = tempSpeed * tempCam.transform.forward;
        float stepSize = .1f;
        lr.positionCount = 2;
        lr.SetPosition(0, this.transform.position);
        int count = 1;
        for (float step = 0; step < 500; step += stepSize)
        {
            predObjectVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 + predObjectVelocity * stepSize;

            RaycastHit hit;
            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude))
            {
                lr.positionCount = count;
                thisDecal.SetActive(true);
                MoveDecal(hit);
                break;
            }

            lr.SetPosition(count, point2);
            point1 = point2;
            count++;
            lr.positionCount++;


        }

    }

    private void MoveDecal(RaycastHit info)
    {
        thisDecal.transform.position = info.point;
        thisDecal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);

    }


    public void PickedUpObject(float speed, GameObject cam)
    {
        tempSpeed = speed;
        tempCam = cam;
        pickedUp = true;

    }

    public void DroppedObject()
    {
        pickedUp = false;
        beingPushed = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopPushingObject();
    }

    private void StopPushingObject()
    {
        beingPushed = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
