/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingManager : MonoBehaviour
{
    [SerializeField, Tooltip("All objects in this list will be active and inactive opposite to objects in the secondary vanishing platforms list. ")]
    private List<GameObject> primaryVanishingPlatforms = new List<GameObject>();

    [SerializeField, Tooltip("All objects in this list will be active and inactive opposite to objects in the primary vanishing platforms list. ")]
    private List<GameObject> secondaryVanishingPlatforms = new List<GameObject>();

    /// <summary>
    /// Returns the primary vanishing platforms list. 
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetPrimaryVanishingPlatforms()
    {
        return primaryVanishingPlatforms;
    }

    /// <summary>
    /// Returns the secondary vanishing platforms list. 
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetSecondaryVanishingPlatforms()
    {
        return secondaryVanishingPlatforms;
    }
}
