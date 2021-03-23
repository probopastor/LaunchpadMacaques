/* 
* Launchpad Macaques 
* William Nomikos 
* GrappleDecal_Swing.cs 
* Creates and handles the location and activity of the aiming decal indicator while swinging
* that appears when looking at an object the player can grapple to. 
* 
* Temporary Script For AB Testing
*/

using UnityEngine;

public class GrappleDecal_Swing : MonoBehaviour
{
    #region Inspector Variables 
    [Header("Decal Settings")]
    [Tooltip("The Prefab to be used as an aiming decal.")] [SerializeField] private GameObject grappleAimingDecal = null;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;
    #endregion

    #region Private Variables 
    private GrapplingGun grapplingGun;
    private GameObject grappleDecalObj;
    private GameObject player;

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        grapplingGun = FindObjectOfType<GrapplingGun>();
        grappleDecalObj = Instantiate(grappleAimingDecal);
        DontDestroyOnLoad(grappleDecalObj);
        player = FindObjectOfType<Matt_PlayerMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayDecal();
    }

    /// <summary>
    /// Displays a decal onto objects that can be grappled from.
    /// </summary>
    private void DisplayDecal()
    { 
        if (grapplingGun.CanFindGrappleLocation())
        {
            grappleDecalObj.SetActive(true);
            MoveDecal(grapplingGun.GetGrappleRayhit());
        }
        else
        {
            grappleDecalObj.SetActive(false);
        }
    }

    /// <summary>
    /// Moves the aiming decal to where the player is looking.
    /// Locks decal in place to the spot the player is grappling to while the player grapples. 
    /// </summary>
    /// <param name="info"></param>
    private void MoveDecal(RaycastHit info)
    {
        if (!grapplingGun.IsGrappling())
        {
            grappleDecalObj.transform.position = info.point;
            grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);
        }
        else if (grapplingGun.IsGrappling())
        {
            grappleDecalObj.transform.position = grapplingGun.GetGrapplePoint();
            grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), grapplingGun.GetGrappleRayhit().normal);
        }
    }
}
