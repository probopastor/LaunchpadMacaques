/* 
* Launchpad Macaques - Neon Oblivion
* CJ Green, William Nomikos
* VanishingManager.cs
* Creates a struct for PlatformTracks, and provides a getter to access a list of PlatformTracks.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingManager : MonoBehaviour
{
    #region Structs 
    /// <summary>
    /// PlatformTracks structure. Each one holds a list of Game Objects. 
    /// </summary>
    [System.Serializable]
    public struct PlatformTracks
    {
        public List<GameObject> gameObjects;
    }
    #endregion

    [SerializeField, Tooltip("Groups of vanishing objects. Enabled objects will cycle through these groups. ")] private List<PlatformTracks> vanishingGroups;

    /// <summary>
    /// Returns a list of PlatformTracks structures. 
    /// </summary>
    /// <returns></returns>
    public List<PlatformTracks> GetPlatformTracks()
    {
        return vanishingGroups;
    }
}
