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
    [SerializeField] [Tooltip("The Object that is spawned to make part of object not Grappable")] GameObject invisibleCorruptionSpot = null;
    [SerializeField] [Tooltip("The Material an object will switch to if the entire object becomes un Grappable")] Material corruptedMaterial;

    [SerializeField] [Tooltip("The Prefab that creates the Decal that is placed on part of Object when it becomes un Grappable")] private GameObject decal = null;
    [SerializeField] [Tooltip("The Layer for objects that the player can Grapple to")] private LayerMask whatIsGrappleable;
    #endregion

    #region Private Values
    private GameObject grappleDecalObj;
    private float objectSize;
    private List<GameObject> corruptedSpots = new List<GameObject>();
    private List<GameObject> corruptedDecals = new List<GameObject>();
    private List<GameObject> corruptedObjects = new List<GameObject>();

    public List<GameObject> CorruptedSpots { get => corruptedSpots; set => corruptedSpots = value; }
    public List<GameObject> CorruptedDecals { get => corruptedDecals; set => corruptedDecals = value; }
    public List<GameObject> CorruptedObjects { get => corruptedObjects; set => corruptedObjects = value; }
    #endregion

    #region Corrupt Objects

    /// <summary>
    /// What is Called to Determine how an object/spot is made un Grappable
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="hitObject"></param>
    public void MakeSpotNotGrappable(RaycastHit spotPos, GameObject hitObject)
    {
        
        // Will check to see if the object should/should not be corrupted
        if (spotPos.collider.gameObject.layer != LayerMask.NameToLayer("DontCorrupt"))
        {

            // Will get the size of the object that was grappled
            Vector3 objectVector = hitObject.GetComponent<Renderer>().bounds.size;
            objectSize = objectVector.x + objectVector.y + objectVector.z;

            // If the object is to big, will only make part of object corrupted
            if (objectSize > maxObjectSize)
            {
                MakePartOfObjectNotGrappable(spotPos, hitObject);
            }

            // If the object is not to big will corrupt the entire object
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
        // Get the highest value of the 3 axis in the normal vector
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

        CreateInvisibleCorruptionSpot(spotPos ,point, hitObject);
        CreateCorruptionSpotDecal(spotPos,point, hitObject);
     
    }

    /// <summary>
    /// Will create the object that will block part of an object from being grappled
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="point"></param>
    /// <param name="hitObject"></param>
    private void CreateInvisibleCorruptionSpot(RaycastHit spotPos,Vector3 point, GameObject hitObject)
    {
        // Will spawn an instance of the corrupted visual object (An Invisible object that blocks a point from being grappled()
        GameObject temporaryCorruptionSpot = Instantiate(invisibleCorruptionSpot);

        // Will set its rotation to face upwards
        temporaryCorruptionSpot.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);

        // Will place the object at the specified point on the object, and set its scale
        temporaryCorruptionSpot.transform.position = point;
        temporaryCorruptionSpot.transform.localScale = new Vector3(notGrappableSize, notGrappableSize, .5f);

        // Will ad the corrupted visual to the list of corrupted visuals
        corruptedSpots.Add(temporaryCorruptionSpot);

        temporaryCorruptionSpot.transform.parent = hitObject.transform;
    }
    
    /// <summary>
    /// Will create the visual decal to show part of an object is corrupted
    /// </summary>
    /// <param name="spotPos"></param>
    /// <param name="point"></param>
    /// <param name="hitObject"></param>
    private void CreateCorruptionSpotDecal(RaycastHit spotPos,Vector3 point, GameObject hitObject)
    {
        // Will create and place the Corrupted Decal
        grappleDecalObj = Instantiate(decal);
        grappleDecalObj.transform.position = point;

        // Will set the decal's rotation to face upwards, and set its scale
        grappleDecalObj.transform.rotation = Quaternion.FromToRotation(new Vector3(Vector3.up.x, Vector3.up.y, Vector3.up.z + 90), spotPos.normal);
        grappleDecalObj.GetComponent<DecalProjector>().size = new Vector3(notGrappableSize * 1.25f, notGrappableSize * 1.25f, 0.1f);
        
        
        // Will add the decal to the corruptedDecal list
        corruptedDecals.Add(grappleDecalObj);
        grappleDecalObj.transform.parent = hitObject.transform;
    }


    /// <summary>
    /// Will Make an entire object un Grappable and will change its material
    /// </summary>
    /// <param name="hitObject"></param>
    private void MakeEntireObjectNotGrappable(GameObject hitObject, RaycastHit hit)
    {
        CorruptableObject objectToCorrupt = hitObject.GetComponent<CorruptableObject>();

        // Checks to see if the object has the ability to become corrupted
        if (objectToCorrupt != null)
        {
            if (!corruptedObjects.Contains(hitObject))
            {
                // Starts the objects corruption
                objectToCorrupt.StartCorrupting(hitObject.transform.position);
            }
        }

        // Adds the corrupted object to the corruptedObjects List
        corruptedObjects.Add(hitObject.gameObject);
    }


    #endregion

    #region UnCorrupt Objects

    /// <summary>
    /// Uncorruptes the passed in object if it is corruptible. 
    /// </summary>
    /// <param name="uncorruptObject"></param>
    public void UncorruptSingleObject(GameObject uncorruptObject)
    {
        CorruptableObject objectToCorrupt = uncorruptObject.GetComponent<CorruptableObject>();

        /// Will check to see if the object has the ability to be uncoruppted
        if (objectToCorrupt != null)
        {
            if (corruptedObjects.Contains(uncorruptObject))
            {
                // Uncorrupts the object
                objectToCorrupt.UncorruptInstantly();
                corruptedObjects.Remove(uncorruptObject);
            }
        }

        // Change the objects layer so the player can grapple on it again
        uncorruptObject.layer = LayerMask.NameToLayer("CanGrapple");
    }


    /// <summary>
    /// Will loop through the Corrupted Objects in a certain radius, removing the corruption
    /// </summary>
    /// <param name="collectableLocation"></param>
    /// <param name="resetRadius"></param>
    public void ClearCorruption(Vector3 collectableLocation, float resetRadius)
    {

        //  Loops through  all the corrupted objects
        for (int i = 0; i < corruptedObjects.Count; i++)
        {
            // Checks to see if an object's location is within the reset radius
            if (Vector3.Distance(corruptedObjects.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
            {

                // Changes the objects layer so the player can grapple it again
                GameObject temp = corruptedObjects.ElementAt(i);
                temp.layer = LayerMask.NameToLayer("CanGrapple");


                // Checks to see if the object has the ability to be uncorrupted
                if (temp.GetComponent<CorruptableObject>() != null)
                {
                    // Uncorrupts the object
                    temp.GetComponent<CorruptableObject>().UncorruptInstantly();

                }
                

                // Removes the corrupted object from the list
                corruptedObjects.RemoveAt(i);
                i--;
            }
        }
        // Will loop through and delete all the corrupted decals in a certain radius from the collectible
        ClearCorruptionHelper(corruptedDecals, collectableLocation, resetRadius);

        // Will loop through and delete all the corrupted spots in a certain radius from the collectible
        ClearCorruptionHelper(corruptedSpots, collectableLocation, resetRadius);
    }

    /// <summary>
    /// Loops through a given lists and removes each object that is within a certain range of the collectableLocation
    /// </summary>
    /// <param name="list"></param>
    /// <param name="collectableLocation"></param>
    /// <param name="resetRadius"></param>
    private void ClearCorruptionHelper(List<GameObject> list,Vector3 collectableLocation, float resetRadius)
    {
        // Loops through each object in the list
        for (int i = 0; i < list.Count; i++)
        {

            // Checks to see if an object in the list is within the reset radius
            if (Vector3.Distance(list.ElementAt(i).transform.position, collectableLocation) <= resetRadius)
            {

                // Removes the object from the list
                GameObject temp = list.ElementAt(i);
                list.RemoveAt(i);

                // Deletes the oject
                Destroy(temp.gameObject);
                i--;

            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Public method that allows a Decal to be added to the corruptedDecal List
    /// </summary>
    /// <param name="obj"></param>
    public void AddCorruptedDecals(GameObject obj)
    {
        corruptedDecals.Add(obj);
    }

    /// <summary>
    /// Public Method that allows a Decal to be removed from the corruptedDecal List
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveCorruptedDecal(GameObject obj)
    {
        corruptedDecals.Remove(obj);
    }
    #endregion
}
