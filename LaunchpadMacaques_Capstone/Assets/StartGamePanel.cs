using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGamePanel : MonoBehaviour
{
    Save_System save;

    [SerializeField] string tutorialLevelName;
    [SerializeField] string hubLevelName;


    private bool playedTutorial;

    public StartGamePanel(string tutorialLevelName, string hubLevelName)
    {
        this.tutorialLevelName = tutorialLevelName;
        this.hubLevelName = hubLevelName;
    }

    private void Start()
    {
        save = this.GetComponent<Save_System>();

        if (save.CanFindFile(PlayerPrefs.GetString("SaveFile")))
        {
            PlayerDataNew data = save.LoadPlayer(PlayerPrefs.GetString("SaveFile"));
            Level[] levels = data.levels;

            foreach (Level l in levels)
            {
                if (l.levelName == tutorialLevelName)
                {
                    if (l.completed == 1)
                    {
                        playedTutorial = true;
                    }

                    else
                    {
                        playedTutorial = false;
                    }
                }
            }
        }

        else
        {
            playedTutorial = false;
        }

    }


    public void StartGame()
    {
        if (!playedTutorial)
        {
            SceneManager.LoadScene(tutorialLevelName);
        }

        else
        {
            SceneManager.LoadScene(hubLevelName);
        }
    }


    public string GetCorrectLevelName()
    {
        if (!playedTutorial)
        {
            return tutorialLevelName;
        }

        else
        {
            return hubLevelName;
        }
    }
}
