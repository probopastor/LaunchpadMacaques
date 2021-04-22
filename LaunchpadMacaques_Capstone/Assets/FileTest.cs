using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileTest : MonoBehaviour
{
    private bool canSeeHidenLevels;


    public Button nextPageButton;
    // Start is called before the first frame update
    void Start()
    {
        CreateText();
    }

    void CreateText()
    {
        canSeeHidenLevels = false;



        string filePath = Application.dataPath + "/../" + "HiddenLevels.txt";

        if (File.Exists(filePath))
        {
            if (File.ReadAllText(filePath) != "Modify this file at all to gain access to hidden levels (Do so at your own risk)")
            {
                canSeeHidenLevels = true;
            }

        }

        else
        {
            canSeeHidenLevels = true;
        }



        nextPageButton.gameObject.SetActive(canSeeHidenLevels);
    }
}
