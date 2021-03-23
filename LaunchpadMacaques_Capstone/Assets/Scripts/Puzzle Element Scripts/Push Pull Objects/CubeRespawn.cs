/* 
* Launchpad Macaques 
* Jamey Colleen, William Nomikos
* CubeRespawn.cs 
* Handles Throwable Cube respawning. 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRespawn : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("The triggers that the button should read. Other triggers are ignored. ")] private string[] triggerTags;
    [SerializeField, Tooltip("The Game Object at the position that this cube starts at. ")] private GameObject respawnPos;
    private GameObject originalSpawnPos;

    [SerializeField, Tooltip("A modifier to the position the Game Object should be respawned at. ")] private Vector3 respawnPosModifiers = new Vector3(0, 0, 0);
    [SerializeField, Tooltip("If true, cube will only respawn at the passed in cube holders. ")] private bool respawnAtSpecificHolders = false;
    [SerializeField, Tooltip("The places this cube can respawn at. Will only respawn at these if respawnAtSpecificHolders is set to true. ")] private GameObject[] cubeHolders;

    private Quaternion respawnAngle;
    private PushPullObjects pushPullObjectsRef;
    private PushableObj pushableObjRef;

    private GameObject[] holderRespawnPos;

    private GameObject playerRef;

    private bool startRespawn;
    private bool keepDefaultPos;

    public string[] TriggerTags { get => triggerTags; set => triggerTags = value; }
    public GameObject[] CubeHolders { get => cubeHolders; set => cubeHolders = value; }
    #endregion

    #region Start Functions
    // Start is called before the first frame update
    void Awake()
    {
        SetValues();
    }

    /// <summary>
    /// Sets start variables and values. 
    /// </summary>
    private void SetValues()
    {
        startRespawn = true;
        respawnAngle = transform.rotation;
        pushPullObjectsRef = FindObjectOfType<PushPullObjects>();
        pushableObjRef = GetComponent<PushableObj>();
        playerRef = FindObjectOfType<Matt_PlayerMovement>().gameObject;

        if (!respawnAtSpecificHolders)
        {
            holderRespawnPos = GameObject.FindGameObjectsWithTag("Cube Holder");
            
            if(holderRespawnPos == null)
            {
                //respawnPos = gameObject;
                keepDefaultPos = true;
            }
            else if(holderRespawnPos.Length < 1)
            {
                keepDefaultPos = true;
            }
        }
        else
        {
            if(cubeHolders == null)
            {
                //respawnPos = gameObject;
                keepDefaultPos = true;
            }
            else if(cubeHolders.Length < 1)
            {
                keepDefaultPos = true;
            }
        }

        originalSpawnPos = respawnPos;

        RespawnCube();
        startRespawn = false;
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
        if (respawnPos != null)
        {
            // If the player is holding the object, stop holding the object. 
            if (pushPullObjectsRef.IsGrabbing() && gameObject == pushPullObjectsRef.GetHeldCube() && pushPullObjectsRef.GetHeldCube() != null)
            {
                pushPullObjectsRef.DropObject();
            }

            // If the cube is being pushed from a throw, stop the push. 
            if (pushableObjRef.GetPushStatus())
            {
                pushableObjRef.StopPushingObject();
            }

            // Zero the velocity of the cube.
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Respawn the cube at the closest valid respawn point to the player. 
            if (respawnAtSpecificHolders && !startRespawn && !keepDefaultPos)
            {
                respawnPos = FindClosestPosToPlayer(playerRef, cubeHolders);
            }
            else if (!respawnAtSpecificHolders && !startRespawn && !keepDefaultPos)
            {
                respawnPos = FindClosestPosToPlayer(playerRef, holderRespawnPos);
            }

            // If the respawn position already has a cube on it, respawn the cube at its original position.
            if (respawnPos.CompareTag("Cube Holder Currently Holding Cube") && !startRespawn && !keepDefaultPos)
            {
                respawnPos = originalSpawnPos;
            }

            if (respawnPos != null)
            {
                // Set the respawn position, and the respawn rotation. 
                float xPos = respawnPos.transform.position.x + respawnPosModifiers.x;
                float yPos = respawnPos.transform.position.y + respawnPosModifiers.y;
                float zPos = respawnPos.transform.position.z + respawnPosModifiers.z;

                gameObject.transform.position = new Vector3(xPos, yPos, zPos);
                gameObject.transform.rotation = respawnAngle;

                SetCubeHolderPickupTag(true, respawnPos);
            }
        }
     
    }

    /// <summary>
    /// Returns the closest object to the player from a passed in array of objects.
    /// </summary>
    /// <param name="player">The player's game object reference. </param>
    /// <param name="otherObjs">An array of objects being compared. </param>
    /// <returns></returns>
    private GameObject FindClosestPosToPlayer(GameObject player, GameObject[] otherObjs)
    {
        GameObject closestObj = otherObjs[0];

        float closestPos = Mathf.Infinity;

        for (int i = 0; i < otherObjs.Length; i++)
        {
            float distanceFromPlayer = (otherObjs[i].transform.position - playerRef.transform.position).magnitude;

            if (distanceFromPlayer < closestPos)
            {
                closestPos = distanceFromPlayer;
                closestObj = otherObjs[i];
            }
        }

        return closestObj;
    }

    /// <summary>
    /// Sets the tag for the cube holder. Pass in true if a cube is off the holder, pass in false if it's on the holder.
    /// </summary>
    /// <param name="pickedUp">The bool to determine if a cube is on or off the cube holder. True if picked up, false if otherwise. </param>
    /// <param name="thisCubeHolder">The cube holder that is being looked at. </param>
    public void SetCubeHolderPickupTag(bool pickedUp, GameObject thisCubeHolder)
    {
        if(!keepDefaultPos)
        {
            if (pickedUp)
            {
                thisCubeHolder.tag = "Cube Holder";
            }
            else if (!pickedUp)
            {
                thisCubeHolder.tag = "Cube Holder Currently Holding Cube";
            }
        }
    }
    #endregion 
}
