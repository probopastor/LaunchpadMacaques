/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi/Adrian/Jamie) 
* (SaveSystem.cs) 
* (Handles the creation and loading of Save Files) 
*/
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        //BinaryFormatter formatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/player.something";
        //FileStream stream = new FileStream(path, FileMode.Create);


        //formatter.Serialize(stream, data);
        //stream.Close();
        PlayerDataNew data = new PlayerDataNew(player, levels);

        string playerData = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerData);
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
        File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerData);
    }


    public void DeleteFile()
    {
        if (CanFindFile("PlayerData"))
        {
            File.Delete(Application.persistentDataPath + "/PlayerData.json");
        }

    }

    /// <summary>
    /// Will return a instance of player data
    /// </summary>
    /// <returns></returns>
    public PlayerDataNew LoadPlayer()
    {
        //string path = Application.persistentDataPath + "/player.something";
        //if(File.Exists(path))
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    FileStream stream = new FileStream(path, FileMode.Open);

        //    PlayerDataNew data = formatter.Deserialize(stream) as PlayerDataNew;
        //    stream.Close();

        //    return data;
        //}
        //else
        //{
        //    Debug.LogError("Save file not found in" + path);
        //    return null;
        //}


        if (CanFindFile("PlayerData"))
        {
            string fileInfo = File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData" + ".json");
            return JsonUtility.FromJson<PlayerDataNew>(fileInfo);

        }

        else
        {
            Debug.LogError("Save file not found in");
            return null;
        }
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
