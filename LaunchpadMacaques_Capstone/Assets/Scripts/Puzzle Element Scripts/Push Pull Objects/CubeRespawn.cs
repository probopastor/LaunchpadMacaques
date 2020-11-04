/* 
* Launchpad Macaques 
* Jamey Colleen
* CubeRespawn.cs 
* Handles Cubeside actions for respawning of the throwable cube
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRespawn : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("The triggers that the button should read. Other triggers are ignored. ")] private string[] triggerTags;
    [SerializeField, Tooltip("The Game Object at the position that this cube is respawned to. ")] private GameObject respawnPos;
    [SerializeField, Tooltip("A modifier to the position the Game Object should be respawned at. ")] private Vector3 respawnPosModifiers = new Vector3(0,0,0);

    private Quaternion respawnAngle;
    private PushPullObjects pushPullObjectsRef;
    private PushableObj pushableObjRef;
    #endregion

    #region Start Functions
    // Start is called before the first frame update
    void Start()
    {
        SetValues();
    }

    /// <summary>
    /// Sets start variables and values. 
    /// </summary>
    private void SetValues()
    {
        respawnAngle = transform.rotation;
        pushPullObjectsRef = FindObjectOfType<PushPullObjects>();
        pushableObjRef = GetComponent<PushableObj>();

        RespawnCube();
    }

    #endregion

    #region Trigger Functions
    private void OnTriggerEnter(Collider other)
    {
        // Checks the triggerTags array to determine if the object should be checked. 
        for (int i = 0; i < triggerTags.Length; i++)
        {
            // If the object should be checked, respawn the cube. 
            if (other.CompareTag(triggerTags[i]))
            {
                // Respawn the object.
                RespawnCube();
            }
        }
    }
    #endregion

    #region Respawn Functions
    /// <summary>
    /// Respawns the cube at its set spawn position. 
    /// </summary>
    public void RespawnCube()
    {
        // If the player is holding the object, stop holding the object. 
        if(pushPullObjectsRef.IsGrabbing())
        {
            pushPullObjectsRef.DropObject();
        }

        // If the cube is being pushed from a throw, stop the push. 
        if(pushableObjRef.GetPushStatus())
        {
            pushableObjRef.StopPushingObject();
        }

        // Zero the velocity of the cube.
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        if(respawnPos != null)
        {
            // Set the respawn position, and the respawn rotation. 
            float xPos = respawnPos.transform.position.x + respawnPosModifiers.x;
            float yPos = respawnPos.transform.position.y + respawnPosModifiers.y;
            float zPos = respawnPos.transform.position.z + respawnPosModifiers.z;

            gameObject.transform.position = new Vector3(xPos, yPos, zPos);
            gameObject.transform.rotation = respawnAngle;
        }
    }
    #endregion 
}
