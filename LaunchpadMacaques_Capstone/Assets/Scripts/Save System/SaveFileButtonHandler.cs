
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveFileButtonHandler : MonoBehaviour
{
    [SerializeField] Button save1;
    [SerializeField] Button save2;
    [SerializeField] Button save3;

    [SerializeField] Button deleteSave1;
    [SerializeField] Button deleteSave2;
    [SerializeField] Button deleteSave3;

    TextMeshProUGUI save1Text;
    TextMeshProUGUI save2Text;
    TextMeshProUGUI save3Text;

    TextMeshProUGUI save1Percent;
    TextMeshProUGUI save2Percent;
    TextMeshProUGUI save3Percent;


    Save_System saveSystem;

    public Button DeleteSave3 { get => deleteSave3; set => deleteSave3 = value; }
    public Button DeleteSave1 { get => deleteSave1; set => deleteSave1 = value; }
    public Button Save1 { get => save1; set => save1 = value; }
    public Button Save3 { get => save3; set => save3 = value; }
    public Button Save2 { get => save2; set => save2 = value; }
    public Button DeleteSave2 { get => deleteSave2; set => deleteSave2 = value; }

    private void Start()
    {
        saveSystem = this.GetComponent<Save_System>();
        save1Text = save1.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        save2Text = save2.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        save3Text = save3.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();


        save1Percent = save1.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        save2Percent = save2.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        save3Percent = save3.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        SetSaves();



    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DeleteSaveFiles();
        }
    }

    private void SetSaves()
    {
        if (CanFindFile("Save 1"))
        {
            save1Text.text = "Save File 1";
            save1Percent.text = "" + CompletedPercentage("Save 1") + "%";
            deleteSave1.gameObject.SetActive(true);

        }

        else
        {
            save1Text.text = "New Save";
            save1Percent.gameObject.SetActive(false);
            deleteSave1.gameObject.SetActive(false);
        }

        if (CanFindFile("Save 2"))
        {
            save2Text.text = "Save File 2";
            save2Percent.text = "" + CompletedPercentage("Save 2") + "%";
            deleteSave2.gameObject.SetActive(true);
        }

        else
        {
            save2Text.text = "New Save";
            save2Percent.gameObject.SetActive(false);
            deleteSave2.gameObject.SetActive(false);
        }

        if (CanFindFile("Save 3"))
        {
            save3Text.text = "Save File 3";
            save3Percent.text = "" + CompletedPercentage("Save 3") + "%";
            deleteSave3.gameObject.SetActive(true);
        }

        else
        {
            save3Text.text = "New Save";
            save3Percent.gameObject.SetActive(false);
            deleteSave3.gameObject.SetActive(false);
        }


        FindObjectOfType<EventSystem>().SetSelectedGameObject(save1.gameObject);
    }

    private void DeleteSaveFiles()
    {
        string[] list = new string[3];
        list[0] = "Save 1";
        list[1] = "Save 2";
        list[2] = "Save 3";

        saveSystem.DeleteAllFiles(list);

        SetSaves();
    }

    public void DeleteSaveFile(string file)
    {
        saveSystem.DeleteFile(file);

        SetSaves();
    }
    private bool CanFindFile(string fileName)
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

            if (levels[i].completed == 1)
            {
                numCompleted++;
            }
        }

        return (int) ((numCompleted / levels.Length) * 100); 
    }

    public void SetSaveFile(string saveFileName)
    {
        PlayerPrefs.SetString("SaveFile", saveFileName);
    }
}
