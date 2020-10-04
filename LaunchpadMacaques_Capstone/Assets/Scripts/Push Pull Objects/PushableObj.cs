/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (PushableObj.CS) 
* (The Script placed on object the player can pick up and throw) 
*/

using UnityEngine;

public class PushableObj : MonoBehaviour
{
    #region Inspector Vars
    [Header("Visual Settings")]
    [SerializeField] [Tooltip("The Decal that will be placed to make part of object look Corrupted")] GameObject throwDecal;

    [Header("Movement Settings")]
    [SerializeField] [Tooltip("The Variable that will be multiplyed by deafult grabity to apply gravity to this object")] float gravityScaler = 1.75f;
    [SerializeField] [Tooltip("The distance an object will fly, when thrown")] float distance;



    [Header("Change Distance Settings")]
    [SerializeField] [Tooltip("How much the distance will change when the player moves the mouse wheel")] float wheelSensitivity = 5;
    [SerializeField] [Tooltip("The Bool which will determine if the player can change the Object Fly Distance")]  bool changeDistance = true;
    [SerializeField] [Tooltip("The Min Fly Distance for the Object")]float minDistance = 5;
    [SerializeField] [Tooltip("The Max Fly Distance for the Object")]float maxDistance = 40;
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
    private Color startColor;
    private Color endColor;
    private Vector3 respawnPos;
    #endregion
    private void Start()
    {
        CreateDecalAndLine();
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

        startColor = lr.startColor;
        endColor = lr.endColor;
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
        if (changeDistance & pickedUp)
        {
            ChangeDistance();
        }

        ResetObjectPos();
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

    /// <summary>
    /// The Private method that is called to actually push an object
    /// </summary>
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


                if (hit.collider.gameObject.CompareTag("Collectible"))
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


            }

            lr.SetPosition(count, point2);
            point1 = point2;
            count++;
            lr.positionCount++;


        }

        if (hitCollectable)
        {
            lr.startColor = Color.green;
            lr.endColor = Color.green;
        }

        else
        {
            lr.startColor = startColor;
            lr.endColor = endColor;
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
        tempCam = cam;
        beingPushed = false;
        pickedUp = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    /// <summary>
    /// Public method that will be called to drop the object
    /// </summary>
    public void DroppedObject()
    {
        pickedUp = false;
        beingPushed = false;
        GetComponent<Rigidbody>().useGravity = true;
        lr.positionCount = 0;
        thisDecal.SetActive(false);
    }

    /// <summary>
    /// When this object hits an object, will stop being pushed
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        StopPushingObject();
    }

    /// <summary>
    /// Method that is called to stop pushing this object
    /// </summary>
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
