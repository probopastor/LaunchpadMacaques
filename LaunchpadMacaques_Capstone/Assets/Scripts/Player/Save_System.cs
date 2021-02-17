using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save_System

{
    public static void SavePlayer(Matt_PlayerMovement player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.something";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerDataNew data = new PlayerDataNew(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerDataNew LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.something";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerDataNew data = formatter.Deserialize(stream) as PlayerDataNew;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}
