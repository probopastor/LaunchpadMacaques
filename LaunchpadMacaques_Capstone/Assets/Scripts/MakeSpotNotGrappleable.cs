using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MakeSpotNotGrappleable : MonoBehaviour
{
    #region Public Values
    [Header("Size Settings")]
    [SerializeField][Tooltip("The Size of How much of the object will be made not Grappable")] float notGrappableSize = 5;
    [SerializeField][Tooltip("The Max Size of an object for the entire object to become un Grappable")] float maxObjectSize = 13;
    [Header("Visuals")]
    [SerializeField][Tooltip("The Object that is spawned to make part of object not Grappable")] GameObject corruptedVisual;
    [SerializeField][Tooltip("The Material an object will switch to if the entire object becomes un Grappable")] Material corruptedMaterial;

    [SerializeField] [Tooltip("The Prefab that creates the Decal that is placed on part of Object when it becomes un Grappable")] private GameObject decal;
    [SerializeField] private LayerMask whatIsGrappleable;
    #endregion

    #region Private Values
    private GameObject grappleDecalObj;
    private float objectSize;


    private Matt_PlayerMovement player;
    private Camera cam;
    #endregion



    void Start()
    {
        SetUp();
    }


    /// <summary>
    /// Will find and set the needed objects
    /// </summary>
    private void SetUp()
    {
        cam = FindObjectOfType<Camera>();
        player = FindObjectOfType<Matt_PlayerMovement>();
    }


    /// <summary>
    /// What is Called to Determine how an object/spot is made un Grappable
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="hitObject"></param>
    public void MakeSpotNotGrappable(RaycastHit spotPos, GameObject hitObject)
    {
        Vector3 objectVector = hitObject.GetComponent<Renderer>().bounds.size;
        objectSize = objectVector.x + objectVector.y + objectVector.z;

        if(objectSize > maxObjectSize)
        {
            MakePartOfObjectNotGrappable(spotPos, hitObject);
        }

        else
        {
            MakeEntireObjectNotGrappable(hitObject);
        }
    }

    /// <summary>
    /// Will Make part of an object un Grappable and will place a decal there
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="hitObject"></param>
    private void MakePartOfObjectNotGrappable(RaycastHit spotPos, GameObject hitObject)
    {
        GameObject temporaryCorruptedVisual = Instantiate(corruptedVisual);
        temporaryCorruptedVisual.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        Debug.Log("Object Rotation: "+ transform.rotation);
        temporaryCorruptedVisual.transform.position = spotPos.point;
        temporaryCorruptedVisual.transform.localScale = new Vector3(notGrappableSize, notGrappableSize, .5f);
        //temporaryCorruptedVisual.transform.parent = hitObject.transform;



        Color objectColor = hitObject.GetComponent<MeshRenderer>().material.color;

        grappleDecalObj = Instantiate(decal);
        grappleDecalObj.transform.position = spotPos.point;
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        grappleDecalObj.GetComponent<DecalProjector>().size = new Vector3(notGrappableSize * 1.25f, notGrappableSize * 1.25f, notGrappableSize * 1.25f);
        //grappleDecalObj.transform.parent = hitObject.transform;
    }

    /// <summary>
    /// Will Make an entire object un Grappable and will change its material
    /// </summary>
    /// <param name="hitObject"></param>
    private void MakeEntireObjectNotGrappable(GameObject hitObject)
    {
        StartCoroutine(ChangeObjectLayer(hitObject));
        hitObject.GetComponent<Renderer>().material = corruptedMaterial;
    }


    /// <summary>
    /// Will Change the Layer of the Object to "CantGrapple" After short ammount of time
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator ChangeObjectLayer(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);
        obj.layer = LayerMask.NameToLayer("CantGrapple");
    }
}
