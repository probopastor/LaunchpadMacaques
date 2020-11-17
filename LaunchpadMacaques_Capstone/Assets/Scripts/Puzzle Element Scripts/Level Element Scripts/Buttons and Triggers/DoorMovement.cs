/* 
* Launchpad Macaques - Neon Oblivion
* William Nomikos, Jamey Colleen, Jake Buri
* DoorMovement.cs
* Script handles moving doors on activation and deactivation.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    #region Door Movement Variables
    [Header("Door Movement ")]
    [SerializeField, Tooltip("The position relative to the door's current position to move it to on activation. ")] private Vector3 doorMovementDirection = new Vector3(0, 0, 0);
    [Tooltip("The position the door starts at. ")] private Vector3 originalDoorPos;
    [Tooltip("The new position of the door after it's moved. ")] private Vector3 newDoorPos = new Vector3(0, 0, 0);
    [SerializeField] private float doorMoveSpeed = 0;
    #endregion

    #region Door Rotation Variables
    [Header("Door Rotation ")]
    [SerializeField, Tooltip("If true, the door will rotate instead of move on activation. ")] private bool rotateOnActivation;
    [SerializeField] private Vector3[] doorRotationAngles;
    private Vector3 originalDoorRotation;
    private int angleIndex = 0;
    #endregion

    private bool lerpDoor;
    private bool activateDoor;

    private float doorMoveTime = 0;
    private bool doorEnabled;
    float timeElapsed;

    bool doOnce;

    private void Start()
    {
        originalDoorPos = gameObject.transform.position;
        newDoorPos = new Vector3(originalDoorPos.x + doorMovementDirection.x, originalDoorPos.y + doorMovementDirection.y, originalDoorPos.z + doorMovementDirection.z);
        originalDoorRotation = transform.rotation.eulerAngles;
        lerpDoor = false;
        activateDoor = false;
        doOnce = false;
    }

    private void Update()
    {
        if (!doOnce)
        {
            doOnce = true;
            if (GetComponentInParent<ActivationDoor>().GetEnableOnActivationStatus())
            {
                gameObject.transform.position = newDoorPos;
                GetComponentInParent<ActivationDoor>().EnableDoor(gameObject);
            }
        }

        // If the door should be lerped, check to see if it should be activated or deactivated 
        if (lerpDoor)
        {

            if (!activateDoor)
            {

                if (timeElapsed < doorMoveTime)
                {
                    if(!rotateOnActivation)
                    {
                        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(originalDoorPos.x + doorMovementDirection.x, originalDoorPos.y + doorMovementDirection.y, originalDoorPos.z + doorMovementDirection.z), timeElapsed * doorMoveSpeed);
                    }
                    else
                    {
                        if(!transform.parent.GetComponent<ActivationDoor>().GetEnableOnActivationStatus())
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(doorRotationAngles[angleIndex]), timeElapsed * doorMoveSpeed);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originalDoorRotation), timeElapsed * doorMoveSpeed);
                        }
                    }

                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    Debug.Log("Door should now be disabled");
                    lerpDoor = false;

                    ActivationDoor thisDoor = GetComponentInParent<ActivationDoor>();
                    thisDoor.DisableDoor(gameObject);
                }

            }
            else if (activateDoor)
            {
                if (!doorEnabled)
                {
                    doorEnabled = true;
                    ActivationDoor thisDoor = GetComponentInParent<ActivationDoor>();
                    thisDoor.EnableDoor(gameObject);
                }

                if (timeElapsed < doorMoveTime)
                {
                    if(!rotateOnActivation)
                    {
                        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(newDoorPos.x - doorMovementDirection.x, newDoorPos.y - doorMovementDirection.y, newDoorPos.z - doorMovementDirection.z), timeElapsed * doorMoveSpeed);
                    }
                    else
                    {
                        if(!transform.parent.GetComponent<ActivationDoor>().GetEnableOnActivationStatus())
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originalDoorRotation), timeElapsed * doorMoveSpeed);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-doorRotationAngles[angleIndex]), timeElapsed * doorMoveSpeed);
                        }
                    }

                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    Debug.Log("Door should now be enabled");
                    lerpDoor = false;
                    doorEnabled = false;
                }
            }

        }
    }

    /// <summary>
    /// Tells DoorMovement to move the door to its set position over the course of moveTime before deactivating it. 
    /// </summary>
    /// <param name="moveTime"></param>
    public void MoveDoorOnDeactivation(float moveTime)
    {
        Debug.Log("Activated");
        timeElapsed = 0;
        doorMoveTime = moveTime;
        lerpDoor = true;
        activateDoor = false;
    }

    /// <summary>
    /// Tells DoorMovement to activate the door and then move it to its set position over the course of moveTime. 
    /// </summary>
    /// <param name="moveTime"></param>
    public void MoveDoorOnActivation(float moveTime)
    {
        Debug.Log("Deactivated");
        timeElapsed = 0;
        doorMoveTime = moveTime;
        lerpDoor = true;
        activateDoor = true;
    }
}
