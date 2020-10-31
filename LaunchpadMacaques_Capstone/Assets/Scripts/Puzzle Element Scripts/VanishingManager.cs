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
    [System.Serializable]
    public struct PlatformTracks
    {
        public List<GameObject> gameObjects;
    }

    [SerializeField, Tooltip("Groups of vanishing objects. Enabled objects will cycle through these groups. ")] private List<PlatformTracks> vanishingGroups;

    public List<PlatformTracks> GetPlatformTracks()
    {
        return vanishingGroups;
    }
}
