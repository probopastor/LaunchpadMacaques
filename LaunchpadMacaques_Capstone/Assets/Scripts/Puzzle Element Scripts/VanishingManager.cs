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
    [SerializeField]
    private List<GameObject> primaryVanishingPlatforms = new List<GameObject>();

    [SerializeField]
    private List<GameObject> secondaryVanishingPlatforms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> GetPrimaryVanishingPlatforms()
    {
        return primaryVanishingPlatforms;
    }

    public List<GameObject> GetSecondaryVanishingPlatforms()
    {
        return secondaryVanishingPlatforms;
    }
}
