/* 
* Launchpad Macaques - Neon Oblivion
* William Nomikos, Jamey Colleen, Jake Buri
* DoorMovement.cs
* Script handles moving and rotating doors on activation and deactivation.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    #region Variables 
    #region Door Movement Variables
    [Header("Door Movement ")]
    [SerializeField, Tooltip("The position relative to the door's current position to move it to on activation. ")] private Vector3 doorMovementDirection = new Vector3(0, 0, 0);
    [Tooltip("The position the door starts at. ")] private Vector3 originalDoorPos;
    [Tooltip("The new position of the door after it's moved. ")] private Vector3 newDoorPos = new Vector3(0, 0, 0);
    [SerializeField] private float doorMoveSpeed = 0;
    #endregion

    #region Door Rotation Variables
    [Header("Door Rotation ")]
    [SerializeField, Tooltip("If true, the door will rotate instead of move on activation. ")] private bool rotateOnActivation = false;
    [SerializeField] private Vector3[] doorRotationAngles;
    private Vector3 originalDoorRotation;
    private int angleIndex = 0;
    #endregion

    private bool lerpDoor;
    private bool activateDoor;

    private float doorMoveTime = 0;
    float timeElapsed;

    bool doOnce;

    public Vector3[] DoorRotationAngles { get => doorRotationAngles; set => doorRotationAngles = value; }
    #endregion

    private void Start()
    {
        originalDoorPos = gameObject.transform.position;
        newDoorPos = new Vector3(originalDoorPos.x + doorMovementDirection.x, originalDoorPos.y + doorMovementDirection.y, originalDoorPos.z + doorMovementDirection.z);
        originalDoorRotation = transform.rotation.eulerAngles;
        lerpDoor = false;
        activateDoor = false;
        doOnce = false;
    }

    #region Methods
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

        if (lerpDoor)
        {
            DoorLerp();
        }
    }

    /// <summary>
    /// Lerps or Slerps the door in the direction it should move in. 
    /// </summary>
    private void DoorLerp()
    {
        // Move the door until the time elapsed is greater than the door move time.
        if (timeElapsed < doorMoveTime)
        {
            // If the door should not be rotated, move it linearly
            if (!rotateOnActivation)
            {
                // If the door is going to be deactivated, move it towards its ending position. 
                if (!activateDoor)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(originalDoorPos.x + doorMovementDirection.x, originalDoorPos.y + doorMovementDirection.y, originalDoorPos.z + doorMovementDirection.z), 1 * doorMoveSpeed);
                }
                // If the door is going to be activated, move it towards its starting position. 
                else if (activateDoor)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(newDoorPos.x - doorMovementDirection.x, newDoorPos.y - doorMovementDirection.y, newDoorPos.z - doorMovementDirection.z), 1 * doorMoveSpeed);
                }
            }
            // If the door should be rotated, rotate it.
            else if (rotateOnActivation)
            {
                // If the door starts as enabled, the door is starting at its start rotation and must rotate towards its end rotation. 
                if (!transform.parent.GetComponent<ActivationDoor>().GetEnableOnActivationStatus())
                {
                    // If the door is being deactivated, move the door towards its end rotation.
                    if (!activateDoor)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(doorRotationAngles[angleIndex]), 1 * doorMoveSpeed);
                    }
                    // If the door is being activated, move the door towards its start rotation. 
                    else if (activateDoor)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originalDoorRotation), 1 * doorMoveSpeed);
                    }
                }
                // If the door starts as disabled, the door is starting at its end rotation and must rotate towards its start rotation.
                else
                {
                    // If the door is being deactivated, move the door towards its start rotation.
                    if (!activateDoor)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originalDoorRotation), 1 * doorMoveSpeed);
                    }
                    // If the door is being activated, move the door towards its end rotation.
                    else if (activateDoor)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-doorRotationAngles[angleIndex].x, -doorRotationAngles[angleIndex].y, -doorRotationAngles[angleIndex].z)), 
                            1 * doorMoveSpeed);
                    }
                }
            }

            timeElapsed += Time.deltaTime;
        }
        else
        {
            lerpDoor = false;
            ActivationDoor thisDoor = GetComponentInParent<ActivationDoor>();

            // Play proper door audio. 
            if (thisDoor.GetComponent<DoorAudio>() != null)
            {
                if (gameObject.GetComponent<DoorAudio>() != null)
                {
                    gameObject.GetComponent<DoorAudio>().PlayDoorSound(false);
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
        timeElapsed = 0;
        doorMoveTime = moveTime;
        lerpDoor = true;
        activateDoor = true;
    }

    #endregion 
}
