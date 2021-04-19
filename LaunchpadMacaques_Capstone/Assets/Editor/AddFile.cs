using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;

public class AddFile : MonoBehaviour
{
    //Script is called after the building process, BuildTarget and string pathToBuiltProject are mandatory arguments
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
         //string filePath = EditorUtility.SaveFolderPanel("Save location", "", "");
        string filePath = pathToBuiltProject + "/../" + "HiddenLevels.txt";
        File.WriteAllText(filePath, "Modify this file at all to gain access to hidden levels (Do so at your own risk)");
    }
}
