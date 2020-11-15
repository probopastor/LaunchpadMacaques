using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    [SerializeField, Tooltip("The position relative to the door's current position to move it to on activation. ")] private Vector3 doorMovementDirection = new Vector3(0, 0, 0);
    [Tooltip("The position the door starts at. ")] private Vector3 originalDoorPos;
    [Tooltip("The new position of the door after it's moved. ")] private Vector3 newDoorPos = new Vector3(0,0,0);

    private bool lerpDoor;
    private bool activateDoor;

    private float doorMoveTime = 0;
    private bool doorEnabled;
    float timeElapsed;

    private void Start()
    {
        originalDoorPos = gameObject.transform.position;
        newDoorPos = originalDoorPos;
        lerpDoor = false;
        activateDoor = false;
    }

    private void Update()
    {
        // If the door should be lerped, check to see if it should be activated or deactivated 
        if(lerpDoor)
        {
            if(!activateDoor)
            {
                if (timeElapsed < doorMoveTime)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(originalDoorPos.x + doorMovementDirection.x, originalDoorPos.y + doorMovementDirection.y, originalDoorPos.z + doorMovementDirection.z), timeElapsed / doorMoveTime);

                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    Debug.Log("Door should now be disabled");
                    newDoorPos = gameObject.transform.position;
                    lerpDoor = false;

                    ActivationDoor thisDoor = GetComponentInParent<ActivationDoor>();
                    thisDoor.DisableDoor(gameObject);
                }
            }
            else if(activateDoor)
            {
                if(!doorEnabled)
                {
                    doorEnabled = true;
                    ActivationDoor thisDoor = GetComponentInParent<ActivationDoor>();
                    thisDoor.EnableDoor(gameObject);
                }

                if (timeElapsed < doorMoveTime)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                        new Vector3(newDoorPos.x - doorMovementDirection.x, newDoorPos.y - doorMovementDirection.y, newDoorPos.z - doorMovementDirection.z), timeElapsed / doorMoveTime);

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
