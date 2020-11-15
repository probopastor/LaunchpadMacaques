using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FootStepCorruption : MonoBehaviour
{
    [SerializeField] LayerMask ground;
    [SerializeField] GameObject footDecal;
    [SerializeField] float decalSize;
    [SerializeField] float timeBetweenSteps;
    [SerializeField] bool scaleWithPlayerSpeed = true;
    [SerializeField] float scaleAmount = .1f;
    [SerializeField] int maxFootSteps;

    [SerializeField] GameObject leftFootPos;
    [SerializeField] GameObject rightFootPos;

    private bool rightFoot;

    private Matt_PlayerMovement player;
    private Rigidbody playerRB;

    private bool inCourtine = false;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(!inCourtine && playerRB.velocity.magnitude != 0)
        {
        
            StartCoroutine(FootSteps());
        }

        //else if(playerRB.velocity.magnitude == 0)
        //{
        //    StopCoroutine(FootSteps());
        //    inCourtine = false;
        //}
    }

    IEnumerator FootSteps()
    {
        inCourtine = true;
        while (true)
        {
            Transform tempTrans;

            if (rightFoot)
            {
                tempTrans = rightFootPos.transform;
                rightFoot = false;
            }

            else
            {
                tempTrans = leftFootPos.transform;
                rightFoot = true;
            }
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


            if(decals.Count >= maxFootSteps)
            {
                coruptedTracker.RemoveCorruptedDecal(decals[0]);
                GameObject temp = decals[0];

                decals.RemoveAt(0);
                Destroy(temp.gameObject);
            }

            if(playerRB.velocity.magnitude > 5)
            {
                yield return new WaitForSeconds((timeBetweenSteps * (1 / (playerRB.velocity.magnitude * scaleAmount))));
            }

            else
            {
                yield return new WaitForSeconds(0);
            }
    

        }
    }
}
