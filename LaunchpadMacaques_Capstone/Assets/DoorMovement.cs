using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    [SerializeField] private Vector3 doorMovementDirection = new Vector3(0, 0, 0);
    private Vector3 originalDoorPos;
    private Vector3 newDoorPos = new Vector3(0,0,0);

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

    public void MoveDoorOnDeactivation(float moveTime)
    {
        Debug.Log("Activated");
        timeElapsed = 0;
        doorMoveTime = moveTime;
        lerpDoor = true;
        activateDoor = false;
    }

    public void MoveDoorOnActivation(float moveTime)
    {
        Debug.Log("Deactivated");
        timeElapsed = 0;
        doorMoveTime = moveTime;
        lerpDoor = true;
        activateDoor = true;
    }
}
