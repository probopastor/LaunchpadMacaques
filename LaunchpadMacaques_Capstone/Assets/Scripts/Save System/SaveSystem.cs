using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSystem
{
    private static string saveDataPath = Path.Combine(Application.persistentDataPath, "GameData");
    private static string fileExtension = ".pog";

    private static BinaryFormatter formatter;

    public static void Save()
    {
        using (var stream = new FileStream(Path.Combine(saveDataPath, fileExtension), FileMode.Create))
        {

        }
    }
}
