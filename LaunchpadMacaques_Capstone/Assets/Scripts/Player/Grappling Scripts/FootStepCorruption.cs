
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FootStepCorruption : MonoBehaviour
{
    #region Public Variables
    [SerializeField, Tooltip("The Layer that the Footsteps can be placed")] LayerMask ground;

    [Header("Decal Settings")]
    [SerializeField, Tooltip("The Decal Projector that is used for the Foot prints")] GameObject footDecal;
    [SerializeField, Tooltip("The size of the Foot Prints")] float decalSize = 0;
    [SerializeField, Tooltip("The max amount of Foot steps that can be spawned, will remove the oldest ones")] int maxFootSteps = 0;

    [Header("Time Settings")]
    [SerializeField, Tooltip("The base time between foot prints, while the player is moving")] float timeBetweenSteps = 0;
    [SerializeField, Tooltip("The amount that is multiplied by the players velocity to scale timeBetweenSteps")] float scaleAmount = .1f;



    [Header("Feet Positions")]
    [SerializeField, Tooltip("The position where a ray-cast will be drawn down to determine where left foot print should go")] GameObject leftFootPos;
    [SerializeField, Tooltip("The position where a ray-cast will be drawn down to determine where right foot print should go")] GameObject rightFootPos;

    #endregion

    #region Private Variables
    // A bool which tracks which footstep should currently be created
    private bool rightFoot;


    private Matt_PlayerMovement player;
    private Rigidbody playerRB;
    private MakeSpotNotGrappleable coruptedTracker;
    private GameObject decal;
    RaycastHit spotPos;
    List<GameObject> decals;

    [SerializeField] GameObject cam;

    private float xAngle = 0;

    public FootStepCorruption(GameObject rightFootPos, GameObject leftFootPos, GameObject footDecal)
    {
        this.rightFootPos = rightFootPos;
        this.leftFootPos = leftFootPos;
        this.footDecal = footDecal;
    }

    public FootStepCorruption(LayerMask ground)
    {
        this.ground = ground;
    }

    Transform playerOrientation;
    #endregion]


    #region Start Methods
    void Start()
    { 
        SetObjects();
        rightFoot = true;

        StartCoroutine(FootSteps());
    }

    /// <summary>
    /// Will find and set this scripts object references
    /// </summary>
    private void SetObjects()
    {
        player = FindObjectOfType<Matt_PlayerMovement>();
        playerRB = player.GetComponent<Rigidbody>();
        coruptedTracker = FindObjectOfType<MakeSpotNotGrappleable>();
        playerOrientation = player.GetOrientaion();
        decals = new List<GameObject>();
    }
    #endregion

    /// <summary>
    /// The Co-routine that will handle the creation of footstep decals
    /// </summary>
    /// <returns></returns>
    IEnumerator FootSteps()
    {
        while (true)
        {
            // Creates a decal if the player is on the ground and moving, at the correct foot position
            CreateDecal(DetermineWhichFoot());

            // Makes sure not to many decals are alive at the same time
            ManageDecalList();

            // Will wait a certain amount of time based on how fast the player is moving (Faster equals less time)
            yield return new WaitForSeconds(DetermineWaitTime());
        }
    }


    #region Helper Functions
    /// <summary>
    /// Returns which foot should be used for creating a footstep
    /// </summary>
    /// <returns></returns>
    private Transform DetermineWhichFoot()
    {

        if (rightFoot)
        {
            xAngle = -180;
            rightFoot = false;
            return rightFootPos.transform;

        }

        else
        {
            xAngle = 0;
            rightFoot = true;
            return leftFootPos.transform;

        }

    }

    /// <summary>
    /// Will make sure there are not to many foot step decals, if there is remove the decal
    /// </summary>
    private void ManageDecalList()
    {
        // Checks to see if there are to many footprint decals
        if (decals.Count >= maxFootSteps)
        {
            coruptedTracker.RemoveCorruptedDecal(decals[0]);
            GameObject temp = decals[0];

            decals.RemoveAt(0);
            Destroy(temp.gameObject);
        }
    }

    /// <summary>
    /// Will return how long the routine should wait based on how fast the player is moving
    /// </summary>
    /// <returns></returns>
    private float DetermineWaitTime()
    {
        // If the player is moving will return a footstep speed based on the players speed times the scale amount
        if (playerRB.velocity.magnitude > 5)
        {
            return timeBetweenSteps * (1 / (playerRB.velocity.magnitude * scaleAmount));
        }

        // If player is not moving will not create footprints
        else
        {
            return 0;
        }
    }

    #endregion

    #region Create FootStep
    /// <summary>
    /// Will create the foot decal at a specific position, and will snap it to the grid
    /// </summary>
    /// <param name="tempTrans"></param>
    private void CreateDecal(Transform tempTrans)
    {
        // Will check under the player to see if the player is standing on a ground object
        // Also checks to make sure the player is moving
        // If the player is both of these it will create a footstep decal
        if (Physics.Raycast(tempTrans.position, Vector3.down, out spotPos, 5, ground) && playerRB.velocity.magnitude > 5)
        {
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



            // Creates the decal at the correct point
            CreateDecalHelper(point, tempTrans.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="temp"></param>
    private void CreateDecalHelper(Vector3 point, GameObject obj)
    {
        // Creates the decal at the correct position and sets its size
        decal = Instantiate(footDecal);
        decal.transform.position = point;
        decal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        decal.GetComponent<DecalProjector>().size = new Vector3(decalSize, decalSize, .1f);

        // Adds the decal to the corrupted decal list
        coruptedTracker.AddCorruptedDecals(decal);




 


        decal.transform.parent = spotPos.transform;

        decal.transform.localRotation = Quaternion.Euler(xAngle, 0, player.GetDesiredX() - 90);


        decals.Add(decal);
    }

    #endregion
}
