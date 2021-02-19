/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi/Adrian/Jamie) 
* (PlayerDataNew.cs) 
* (Handles all the player informatin that will be saved) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerDataNew
{
    public float[] position;
    public string level;

    public Level[] levels;

    

    /// <summary>
    /// Will save the position of the given object
    /// Will save the current scenes name
    /// Will save the given array of levels
    /// </summary>
    /// <param name="player"></param>
    /// <param name="levelData"></param>
    public PlayerDataNew(GameObject player, Level[] levelData)
    {

        level = SceneManager.GetActiveScene().name;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;


        levels = levelData;
    }

    /// <summary>
    /// Will save the given vector 3 as players position
    /// Will save the given string as the last saved level name
    /// Will save the given array of levels
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="levelName"></param>
    /// <param name="levelData"></param>
    public PlayerDataNew(Vector3 pos, string levelName, Level[] levelData)
    {

        level = levelName;

        position = new float[3];
        position[0] = pos.x;
        position[1] = pos.y;
        position[2] = pos.z;


        levels = levelData;
    }
}
