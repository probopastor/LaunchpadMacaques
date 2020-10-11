/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (MakeSpotNotGrappleable.cs) 
* (The Script that is called to Corrupt and UnCorrupt Objects) 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MakeSpotNotGrappleable : MonoBehaviour
{
    #region Public Values
    [Header("Size Settings")]
    [SerializeField] [Tooltip("The Size of How much of the object will be made not Grappable")] float notGrappableSize = 5;
    [SerializeField] [Tooltip("The Max Size of an object for the entire object to become un Grappable")] float maxObjectSize = 13;
    [Header("Visuals")]
    [SerializeField] [Tooltip("The Object that is spawned to make part of object not Grappable")] GameObject corruptedVisual;
    [SerializeField] [Tooltip("The Material an object will switch to if the entire object becomes un Grappable")] Material corruptedMaterial;

    [SerializeField] [Tooltip("The Prefab that creates the Decal that is placed on part of Object when it becomes un Grappable")] private GameObject decal;
    [SerializeField] [Tooltip("The Layer for objets that the player can Grapple to")] private LayerMask whatIsGrappleable;
    #endregion


    #region Private Values
    private GameObject grappleDecalObj;
    private float objectSize;
    private List<GameObject> tempCorruptedVisuals = new List<GameObject>();
    private List<GameObject> corruptedDecals = new List<GameObject>();
    private List<GameObject> corruptedObjects = new List<GameObject>();
    #endregion


    #region Corrupt Objects
    /// <summary>
    /// What is Called to Determine how an object/spot is made un Grappable
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="hitObject"></param>
    public void MakeSpotNotGrappable(RaycastHit spotPos, GameObject hitObject)
    {
        
        if (spotPos.collider.gameObject.layer != LayerMask.NameToLayer("DontCorrupt"))
        {
            Vector3 objectVector = hitObject.GetComponent<Renderer>().bounds.size;
            objectSize = objectVector.x + objectVector.y + objectVector.z;

            if (objectSize > maxObjectSize)
            {
                MakePartOfObjectNotGrappable(spotPos, hitObject);
            }

            else
            {
                MakeEntireObjectNotGrappable(hitObject, spotPos);
            }
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
        Debug.Log("Object Rotation: " + transform.rotation);
        temporaryCorruptedVisual.transform.position = spotPos.point;
        temporaryCorruptedVisual.transform.localScale = new Vector3(notGrappableSize, notGrappableSize, .5f);
        tempCorruptedVisuals.Add(temporaryCorruptedVisual);

        grappleDecalObj = Instantiate(decal);
        grappleDecalObj.transform.position = spotPos.point;
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        grappleDecalObj.GetComponent<DecalProjector>().size = new Vector3(notGrappableSize * 1.25f, notGrappableSize * 1.25f, notGrappableSize * 1.25f);
        corruptedDecals.Add(grappleDecalObj);
        grappleDecalObj.transform.parent = hitObject.transform;
        temporaryCorruptedVisual.transform.parent = hitObject.transform;
    }

    /// <summary>
    /// Will Make an entire object un Grappable and will change its material
    /// </summary>
    /// <param name="hitObject"></param>
    private void MakeEntireObjectNotGrappable(GameObject hitObject, RaycastHit hit)
    {
        CorruptableObject objectToCorrupt = hitObject.GetComponent<CorruptableObject>();

        if (objectToCorrupt != null)
        {
            objectToCorrupt.StartCorrupting(hitObject.transform.position);
        }

        StartCoroutine(ChangeObjectLayer(hitObject));

        //if (hit.collider != null)
        //{
        //    if (hit.collider.GetComponent<Renderer>().enabled)
        //    {
        //        Renderer r = hit.collider.GetComponent<Renderer>();
        //        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();

        //        r.GetPropertyBlock(pBlock);

        //        pBlock.SetFloat("CorruptionStartTime", Time.time);
        //        Vector3 localPos = hit.collider.transform.InverseTransformPoint(hit.point);
        //        pBlock.SetVector("CorruptionStartPos", localPos);

        //        r.SetPropertyBlock(pBlock);
        //    }
        //}

        corruptedObjects.Add(hitObject.gameObject);
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


    #endregion


    #region UnCorrupt Objects
    /// <summary>
    /// Will loop through the Corrupted Objects in a certain radius, removing the corruption
    /// </summary>
    /// <param name="collectableLocation"></param>
    /// <param name="resetRadius"></param>
    public void ClearCorruption(Vector3 collectableLocation, float resetRadius)
    {

        //for (int i = 0; i < corruptedObjects.Count; i++)
        //{
        //    if (Vector3.Distance(corruptedObjects.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
        //    {
        //        GameObject temp = corruptedObjects.ElementAt(i);
        //        temp.layer = LayerMask.NameToLayer("CanGrapple");
        //        Renderer render = corruptedObjects.ElementAt(i).GetComponent<Renderer>();
        //        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();
        //        render.SetPropertyBlock(pBlock);
        //        corruptedObjects.RemoveAt(i);

        //        i--;
        //    }
        //}
        //for (int i = 0; i < corruptedDecals.Count; i++)
        //{
        //    if (Vector3.Distance(corruptedDecals.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
        //    {
        //        GameObject temp = corruptedDecals.ElementAt(i);
        //        corruptedDecals.RemoveAt(i);
        //        Destroy(temp.gameObject);
        //        i--;
        //    }
        //}

        //for (int i = 0; i < tempCorruptedVisuals.Count; i++)
        //{
        //    if (Vector3.Distance(tempCorruptedVisuals.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
        //    {
        //        GameObject temp = tempCorruptedVisuals.ElementAt(i);
        //        tempCorruptedVisuals.RemoveAt(i);
        //        Destroy(temp.gameObject);
        //        i--;

        //    }
        //}

        //CorruptableObject objectToCorrupt = hitObject.GetComponent<CorruptableObject>();

        //if (objectToCorrupt != null)
        //{
        //    objectToCorrupt.StartCorrupting(hitObject.transform.position);
        //}


        for (int i = 0; i < corruptedObjects.Count; i++)
        {
            if (Vector3.Distance(corruptedObjects.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
            {
                GameObject temp = corruptedObjects.ElementAt(i);
                temp.layer = LayerMask.NameToLayer("CanGrapple");

                if (temp.GetComponent<CorruptableObject>() != null)
                {
                    //temp.GetComponent<CorruptableObject>().UncorruptFromPoint(gameObject.transform.position);
                    temp.GetComponent<CorruptableObject>().UncorruptInstantly();

                }
                corruptedObjects.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < corruptedDecals.Count; i++)
        {
            if (Vector3.Distance(corruptedDecals.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
            {
                GameObject temp = corruptedDecals.ElementAt(i);
                corruptedDecals.RemoveAt(i);
                Destroy(temp.gameObject);
                i--;
            }
        }
        for (int i = 0; i < tempCorruptedVisuals.Count; i++)
        {
            if (Vector3.Distance(tempCorruptedVisuals.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
            {
                GameObject temp = tempCorruptedVisuals.ElementAt(i);
                tempCorruptedVisuals.RemoveAt(i);
                Destroy(temp.gameObject);
                i--;

            }
        }
    }
    #endregion

}
