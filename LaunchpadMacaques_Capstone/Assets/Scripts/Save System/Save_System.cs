/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi/Adrian/Jamie) 
* (SaveSystem.cs) 
* (Handles the creation and loading of Save Files) 
*/
using UnityEngine;
using System.IO;


public class Save_System : MonoBehaviour
{
    #region Loading/Saving
    /// <summary>
    /// A method that takes the player object and a levels array
    /// This will then save that information to the PlayerData file
    /// </summary>
    /// <param name="player"></param>
    /// <param name="levels"></param>
    public void SavePlayer(GameObject player, Level[] levels)
    {
        PlayerDataNew data = new PlayerDataNew(player, levels);

        string playerData = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/" + PlayerPrefs.GetString("SaveFile") + ".json", playerData);
    }


    /// <summary>
    /// Takes in a position, a string, and a array of levels
    /// Will save that to a save file 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="levelName"></param>
    /// <param name="levels"></param>
    public void SavePlayer(Vector3 pos, string levelName, Level[] levels)
    {
        PlayerDataNew data = new PlayerDataNew(pos, levelName, levels);

        string playerData = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/" + PlayerPrefs.GetString("SaveFile") + ".json", playerData);
    }


    public void DeleteFile()
    {
        if (CanFindFile(PlayerPrefs.GetString("SaveFile")))
        {
            File.Delete(Application.persistentDataPath + "/" + PlayerPrefs.GetString("SaveFile") + ".json");
        }

    }

    public void DeleteFile(string file)
    {
        if (CanFindFile(file))
        {
            File.Delete(Application.persistentDataPath + "/" + file + ".json");
        }

    }
    public void DeleteAllFiles(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            if (CanFindFile(files[i]))
            {
                File.Delete(Application.persistentDataPath + "/" + files[i] + ".json");
            }
        }
    }

    /// <summary>
    /// Will return a instance of player data
    /// </summary>
    /// <returns></returns>
    public PlayerDataNew LoadPlayer()
    {

        if (CanFindFile(PlayerPrefs.GetString("SaveFile")))
        {
            string fileInfo = File.ReadAllText(Application.persistentDataPath + "/" + PlayerPrefs.GetString("SaveFile") + ".json");
            return JsonUtility.FromJson<PlayerDataNew>(fileInfo);

        }

        else
        {
            Debug.LogError("Save file not found in");
            return null;
        }
    }



    /// <summary>
    /// Will load save file with the given name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public PlayerDataNew LoadPlayer(string fileName)
    {
        string fileInfo = File.ReadAllText(Application.persistentDataPath + "/" + fileName + ".json");
        return JsonUtility.FromJson<PlayerDataNew>(fileInfo);
    }

    #endregion

    #region HelperMethods
    /// <summary>
    /// Will return if it can find a file with the givin file name (Don't include .type)
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool CanFindFile(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/" + fileName + ".json");
    }
    #endregion
}
