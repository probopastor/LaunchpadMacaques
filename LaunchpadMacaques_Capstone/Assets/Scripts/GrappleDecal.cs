/* 
* Launchpad Macaques 
* William Nomikos 
* GrappleDecal.cs 
* Creates and handles the location and activity of the aiming decal indicator 
* that appears when looking at an object the player can grapple to. 
*/

using UnityEngine;

public class GrappleDecal : MonoBehaviour
{
    #region Inspector Variables 
    [Header("Decal Settings")]
    [Tooltip("The Prefab to be used as an aiming decal.")] [SerializeField] private GameObject grappleAimingDecal;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;
    #endregion

    #region Private Variables 
    private ConfigJoint configJoint;
    private GameObject grappleDecalObj;
    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        configJoint = FindObjectOfType<ConfigJoint>();
        grappleDecalObj = Instantiate(grappleAimingDecal);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
       
        if((Physics.Raycast(ray, out hitInfo, configJoint.GetMaxGrappleDistance(), whatIsGrappleable)) || (configJoint.IsGrappling()))
        {
            grappleDecalObj.SetActive(true);
            MoveDecal(hitInfo);
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
        if (!configJoint.IsGrappling())
        {
            grappleDecalObj.transform.position = info.point;
            grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);
        }
        else if (configJoint.IsGrappling())
        {
            grappleDecalObj.transform.position = configJoint.GetGrapplePoint();
            grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), configJoint.GetGrappleRayhit().normal);
        }
    }
}
