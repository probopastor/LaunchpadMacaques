using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MakeSpotNotGrappleable : MonoBehaviour
{
    [SerializeField] float notGrappableSize = 5;
    [SerializeField] GameObject corruptedVisual;

    [Tooltip("The Prefab to be used as an aiming decal.")] [SerializeField] private GameObject decal;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;

    private GameObject grappleDecalObj;


    private Matt_PlayerMovement player;
    private Camera cam;

    //[SerializeField] Material grappleMaterial;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        player = FindObjectOfType<Matt_PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void MakeSpotNotGrappable(Vector3 spotPos, GameObject hitObject)
    //{
    //    Debug.Log("Corrupted");
    //    GameObject temporaryCorruptedVisual =  Instantiate(corruptedVisual);
    //    temporaryCorruptedVisual.transform.LookAt(player.transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
    //    temporaryCorruptedVisual.transform.position = spotPos;
    //    temporaryCorruptedVisual.transform.localScale = new Vector3(notGrappableSize, notGrappableSize, notGrappableSize);
    //    temporaryCorruptedVisual.transform.parent = hitObject.transform;
    //}

    public void MakeSpotNotGrappable(RaycastHit spotPos, GameObject hitObject)
    {

        GameObject temporaryCorruptedVisual = Instantiate(corruptedVisual);
        temporaryCorruptedVisual.transform.LookAt(player.transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
        temporaryCorruptedVisual.transform.position = spotPos.point;
        temporaryCorruptedVisual.transform.localScale = new Vector3(notGrappableSize, notGrappableSize, notGrappableSize);
        temporaryCorruptedVisual.transform.parent = hitObject.transform;


        Color objectColor = hitObject.GetComponent<MeshRenderer>().material.color;

        grappleDecalObj = Instantiate(decal);
        grappleDecalObj.transform.position = spotPos.point;
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        grappleDecalObj.GetComponent<DecalProjector>().size = new Vector3(notGrappableSize * 1.25f, notGrappableSize * 1.25f, notGrappableSize * 1.25f);
        grappleDecalObj.transform.parent = hitObject.transform;



        //Material mymat = grappleDecalObj.GetComponent<DecalProjector>().material = new Material((grappleDecalObj.GetComponent<DecalProjector>().material.shader));
        //Material mymat = grappleDecalObj.GetComponent<DecalProjector>().material;
        //mymat.SetColor("_EmissiveColor", Color.red);
    }


    private void MoveDecal(RaycastHit info)
    {
        grappleDecalObj.transform.position = info.point;
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), info.normal);
    }
}
