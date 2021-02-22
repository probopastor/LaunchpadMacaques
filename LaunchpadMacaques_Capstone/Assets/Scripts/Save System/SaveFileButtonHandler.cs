
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class SaveFileButtonHandler : MonoBehaviour
{
    [SerializeField] Button save1;
    [SerializeField] Button save2;
    [SerializeField] Button save3;

    TextMeshProUGUI save1Text;
    TextMeshProUGUI save2Text;
    TextMeshProUGUI save3Text;   
    
    TextMeshProUGUI save1Percent;
    TextMeshProUGUI save2Percent;
    TextMeshProUGUI save3Percent;


    Save_System saveSystem;



    private void Start()
    {
        saveSystem = this.GetComponent<Save_System>();
        save1Text = save1.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        save2Text = save2.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        save3Text = save3.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();


        save1Percent = save1.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        save2Percent = save2.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        save3Percent = save3.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

      

        if (CanFindFile("Save 1"))
        {
            save1Text.text = "Save File 1";
            save1Percent.text = "" + CompletedPercentage("Save 1") + "%";

        }

        else
        {
            save1Text.text = "New Save";
            save1Percent.gameObject.SetActive(false);
        }

        if (CanFindFile("Save 2"))
        {
            save2Text.text = "Save File 2";
            save2Percent.text = "" + CompletedPercentage("Save 2") + "%";
        }

        else
        {
            save2Text.text = "New Save";
            save2Percent.gameObject.SetActive(false);
        }

        if (CanFindFile("Save 3"))
        {
            save3Text.text = "Save File 3";
            save3Percent.text = "" + CompletedPercentage("Save 3") + "%";
        }

        else
        {
            save3Text.text = "New Save";
            save3Percent.gameObject.SetActive(false);
        }

    }
    public bool CanFindFile(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/" + fileName + ".json");
    }

    private float CompletedPercentage(string fileName)
    {
        PlayerDataNew data = saveSystem.LoadPlayer(fileName);
        Level[] levels = data.levels;
        float numCompleted = 0;
        for (int i = 0; i < levels.Length; i++)
        {
           
            if(levels[i].completed == 1)
            {
                numCompleted++;
            }
        }
        return (numCompleted / levels.Length) * 100;
    }

    public void SetSaveFile(string saveFileName)
    {
        PlayerPrefs.SetString("SaveFile", saveFileName);
    }
}
