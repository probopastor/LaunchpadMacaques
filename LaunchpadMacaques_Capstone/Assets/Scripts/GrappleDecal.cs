using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleDecal : MonoBehaviour
{
    [SerializeField] private GameObject grappleAimingDecal;
    [SerializeField] private LayerMask whatIsGrappleable;

    private ConfigJoint configJoint;

    // Start is called before the first frame update
    void Start()
    {
        configJoint = FindObjectOfType<ConfigJoint>();
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
            grappleAimingDecal.SetActive(true);
            MoveDecal(hitInfo);
        }
        else
        {
            grappleAimingDecal.SetActive(false);
        }
    }

    private void MoveDecal(RaycastHit info)
    {
        //grappleAimingDecal.transform.rotation = Quaternion.FromToRotation(Vector3.right, info.normal);
        //grappleAimingDecal.transform.rotation = new Quaternion(grappleAimingDecal.transform.rotation.x, grappleAimingDecal.transform.rotation.y, grappleAimingDecal.transform.rotation.z, grappleAimingDecal.transform.rotation.w);

        //grappleAimingDecal.transform.position = info.point;
        //grappleAimingDecal.transform.rotation = new Quaternion(info.normal.x, info.normal.y, info.normal.z, grappleAimingDecal.transform.rotation.w);

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    Instantiate(grappleAimingDecal, info.point, Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal));

        grappleAimingDecal.transform.position = info.point;
        grappleAimingDecal.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);
    }
}
