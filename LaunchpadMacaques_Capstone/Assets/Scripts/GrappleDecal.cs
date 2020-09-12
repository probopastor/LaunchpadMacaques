using UnityEngine;

public class GrappleDecal : MonoBehaviour
{
    [SerializeField] private GameObject grappleAimingDecal;
    [SerializeField] private LayerMask whatIsGrappleable;

    private ConfigJoint configJoint;

    private GameObject grappleDecalObj;

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

    private void DisplayDecal()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, configJoint.GetMaxGrappleDistance(), whatIsGrappleable))
        {
            grappleDecalObj.SetActive(true);
            MoveDecal(hitInfo);
        }
        else
        {
            grappleDecalObj.SetActive(false);
        }
    }

    private void MoveDecal(RaycastHit info)
    {
        grappleDecalObj.transform.position = info.point;
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);
    }
}
