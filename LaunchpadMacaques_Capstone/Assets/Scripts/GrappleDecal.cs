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
        grappleAimingDecal.transform.position = info.point;
        //grappleAimingDecal.transform.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
        //grappleAimingDecal.transform.
    }
}
