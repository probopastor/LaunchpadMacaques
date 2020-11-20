/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (FootStepCorruption.cs) 
* (Will handle the creation of footstep corruption) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FootStepCorruption : MonoBehaviour
{
    [SerializeField, Tooltip("The Layer that the Footsteps can be placed")] LayerMask ground;

    [Header("Decal Settings")]
    [SerializeField, Tooltip("The Decal Projector that is used for the Foot prints")] GameObject footDecal;
    [SerializeField, Tooltip("The size of the Foot Prints")] float decalSize;
    [SerializeField, Tooltip("The max ammount of Foot steps that can be spawned, will remove the oldest ones")] int maxFootSteps;
 
    [Header("Time Settings")]
    [SerializeField, Tooltip("The base time between foot prints, while the player is moving")] float timeBetweenSteps;
    [SerializeField, Tooltip("The ammount that is multiplied by the players velocity to scale timeBetweenSteps")] float scaleAmount = .1f;



    [Header("Feet Positions")]
    [SerializeField, Tooltip("The position where a raycast will be drawn down to determine where left foot print should go")] GameObject leftFootPos;
    [SerializeField, Tooltip("The position where a raycast will be drawn down to determine where right foot print should go")] GameObject rightFootPos;

    private bool rightFoot;
    private Matt_PlayerMovement player;
    private Rigidbody playerRB;
    private MakeSpotNotGrappleable coruptedTracker;
    private GameObject decal;
    RaycastHit spotPos;
    List<GameObject> decals;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Matt_PlayerMovement>();
        playerRB = player.GetComponent<Rigidbody>();
        coruptedTracker = FindObjectOfType<MakeSpotNotGrappleable>();

        decals = new List<GameObject>();
        rightFoot = true;

        StartCoroutine(FootSteps());
    }

    /// <summary>
    /// Will Handle the creation of the FootSteps
    /// </summary>
    /// <returns></returns>
    IEnumerator FootSteps()
    {
        while (true)
        {
            CreateDecal(DetermineWhichFoot());
            ManageDecalList();
            yield return new WaitForSeconds(DetermineWaitTime());
        }
    }

    /// <summary>
    /// Returns which foot should be used
    /// </summary>
    /// <returns></returns>
    private Transform DetermineWhichFoot()
    {

        if (rightFoot)
        {
            rightFoot = false;
            return rightFootPos.transform;

        }

        else
        {
            rightFoot = true;
            return leftFootPos.transform;

        }

    }

    /// <summary>
    /// Will make sure there are not to many foot step decals, if there is remove the decal
    /// </summary>
    private void ManageDecalList()
    {
        if (decals.Count >= maxFootSteps)
        {
            coruptedTracker.RemoveCorruptedDecal(decals[0]);
            GameObject temp = decals[0];

            decals.RemoveAt(0);
            Destroy(temp.gameObject);
        }
    }

    /// <summary>
    /// Will return how long the routine should wait based on how fas the player is moving
    /// </summary>
    /// <returns></returns>
    private float DetermineWaitTime()
    {
        if (playerRB.velocity.magnitude > 5)
        {
            return timeBetweenSteps * (1 / (playerRB.velocity.magnitude * scaleAmount));
        }

        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Will create the foot decal as a specific position, and will snap it to the grid
    /// </summary>
    /// <param name="tempTrans"></param>
    private void CreateDecal(Transform tempTrans)
    {
        if (Physics.Raycast(tempTrans.position, Vector3.down, out spotPos, 5, ground) && playerRB.velocity.magnitude > 5)
        {
            Debug.Log("Made THing");
            float maxNormal = Mathf.Max(Mathf.Max(Mathf.Abs(spotPos.normal.x), Mathf.Abs(spotPos.normal.y)), Mathf.Abs(spotPos.normal.z));

            Vector3 point = spotPos.point;

            // Lock the decal to a grid, but ignore the axis that is closest to the normal. (to prevent the decal being forced off of the surface)
            if (Mathf.Abs(spotPos.normal.x) != maxNormal)
            {
                point.x -= (point.x % 0.625f);
            }
            if (Mathf.Abs(spotPos.normal.y) != maxNormal)
            {
                point.y -= (point.y % 0.625f);
            }
            if (Mathf.Abs(spotPos.normal.z) != maxNormal)
            {
                point.z -= (point.z % 0.625f);
            }

            decal = Instantiate(footDecal);
            decal.transform.position = point;
            decal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
            decal.GetComponent<DecalProjector>().size = new Vector3(decalSize, decalSize, .1f);
            coruptedTracker.AddCorruptedDecals(decal);
            decal.transform.parent = spotPos.transform;

            decals.Add(decal);
        }
    }
}
