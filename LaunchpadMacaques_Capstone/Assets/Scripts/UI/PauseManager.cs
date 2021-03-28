﻿/* 
* Launchpad Macaques 
* William Nomikos 
* GrappleDecal.cs 
* Handles pause menu functionality, including pausing the game,
* unpausing the game, restarting the game, and quitting to the 
* main menu. Also handles enabling win and lose panels.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    private bool paused;
    private bool gameLost;
    private bool gameWon;
    public GameObject HTPMenu;
    public GameObject pausePanel;

    [SerializeField, Tooltip("The scene this pause manager is located in. ")] private string thisScene;
    [SerializeField, Tooltip("The main menu scene name. ")] private string mainMenuScene = null;
    [SerializeField, Tooltip("Options Menu")] GameObject optionsMenu = null;

    [SerializeField, Tooltip("Options Menu")] GameObject gameplaySettings = null;
    [SerializeField] GameObject lookSettings = null;
    [SerializeField] GameObject miscGamePlaySettings = null;

    [SerializeField, Tooltip("Options Menu")] GameObject videoSettings = null;
    [SerializeField] GameObject graphicsSettings = null;
    [SerializeField] GameObject postProcessingSettings = null;
    [SerializeField, Tooltip("Options Menu")] GameObject audioSettings = null;
    [SerializeField, Tooltip("Resume Button")] GameObject resumeButton;

    [SerializeField, Tooltip("The pause panel that is being used as a menu. ")] private GameObject PauseCanvas = null;
    //[SerializeField] private GameObject LoseCanvas;
    [SerializeField, Tooltip("The win panel that is being used as a win state. ")] private GameObject WinCanvas = null;
    [SerializeField, Tooltip("The cursor object. ")] private GameObject CursorCanvas = null;
    [SerializeField, Tooltip("The UI panel. ")] private GameObject UICanvas = null;
    [SerializeField, Tooltip("The Information Post Text. ")] private GameObject InformationPostText = null;
    private NarrativeTriggerHandler narrative;
    private SettingsManager settings;
    private EventSystem eventSystem;

    private Matt_PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Matt_PlayerMovement>();
        Time.timeScale = 1;
        gameLost = false;

        paused = false;
        gameWon = false;
        PauseCanvas.SetActive(false);
        //LoseCanvas.SetActive(false);
        //WinCanvas.SetActive(false);
        UICanvas.SetActive(true);

        if (InformationPostText != null)
        {
            InformationPostText.SetActive(true);
        }

        pausePanel = PauseCanvas.gameObject;
        narrative = FindObjectOfType<NarrativeTriggerHandler>();
        settings = FindObjectOfType<SettingsManager>();
        eventSystem = FindObjectOfType<EventSystem>();


    }


    public void PauseInput()
    {
        if (player.CanPlayerMove())
        {
            if (!gameLost && !gameWon && !HTPMenu.activeSelf && !optionsMenu.transform.parent.gameObject.activeSelf)
            {
                PauseGame();
            }

            else if (HTPMenu.activeSelf)
            {
                eventSystem.SetSelectedGameObject(null);
                HTPMenu.SetActive(false);
                pausePanel.SetActive(true);
            }

            else if (optionsMenu.transform.parent.gameObject.activeSelf)
            {
                FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                if (optionsMenu.activeSelf)
                {
                    optionsMenu.transform.parent.gameObject.SetActive(false);
                    pausePanel.SetActive(true);
                }

                else if (videoSettings.activeSelf)
                {
                    optionsMenu.SetActive(true);
                    videoSettings.SetActive(false);
                }

                else if (graphicsSettings.activeSelf)
                {
                    videoSettings.SetActive(true);
                    graphicsSettings.SetActive(false);
                }

                else if (postProcessingSettings.activeSelf)
                {
                    videoSettings.SetActive(true);
                    postProcessingSettings.SetActive(false);
                }

                else if (audioSettings.activeSelf)
                {
                    optionsMenu.SetActive(true);
                    audioSettings.SetActive(false);
                }

                else if (gameplaySettings.activeSelf)
                {
                    optionsMenu.SetActive(true);
                    gameplaySettings.SetActive(false);
                }

                else if (lookSettings.activeSelf)
                {
                    gameplaySettings.SetActive(true);
                    lookSettings.SetActive(false);
                }

                else if (miscGamePlaySettings.activeSelf)
                {
                    gameplaySettings.SetActive(true);
                    miscGamePlaySettings.SetActive(false);
                }

                FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            }
        }
     

    }

    public void BackInput()
    {
        if (pausePanel.activeSelf)
        {
            PauseGame();
        }
        else if (HTPMenu.activeSelf)
        {
            eventSystem.SetSelectedGameObject(null);
            HTPMenu.SetActive(false);
            pausePanel.SetActive(true);
        }

        else if (optionsMenu.transform.parent.gameObject.activeSelf)
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            if (optionsMenu.activeSelf)
            {
                optionsMenu.transform.parent.gameObject.SetActive(false);
                pausePanel.SetActive(true);
            }

            else if (videoSettings.activeSelf)
            {
                optionsMenu.SetActive(true);
                videoSettings.SetActive(false);
            }

            else if (audioSettings.activeSelf)
            {
                optionsMenu.SetActive(true);
                audioSettings.SetActive(false);
            }

            else if (gameplaySettings.activeSelf)
            {
                optionsMenu.SetActive(true);
                gameplaySettings.SetActive(false);
            }

            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
        }
    }


    /// <summary>
    /// Method handles pausing and unpausing, and enabling and disabling the proper objects.
    /// </summary>
    public void PauseGame()
    {
        if (paused == false)
        {
            paused = true;
            Time.timeScale = 0;

            PauseCanvas.SetActive(true);
          //  eventSystem.SetSelectedGameObject(resumeButton);
            CursorCanvas.SetActive(false);



            narrative.TurnOffDialouge();

            if (InformationPostText != null)
            {
                InformationPostText.SetActive(false);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

      


        }
        else if (paused == true)
        {
            Time.timeScale = 1;
            paused = false;
            PauseCanvas.SetActive(false);
            CursorCanvas.SetActive(true);

            if (InformationPostText != null)
            {
                InformationPostText.SetActive(true);
            }


            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;


        }
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1;
        //SceneManager.LoadScene(thisScene);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// Changes the scene to the main menu scene.
    /// </summary>
    public void ToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Enables the lose state of the game.
    /// </summary>
    public void SetGameLost()
    {
        gameLost = true;
        //LoseCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Enables the win state of the game.
    /// </summary>
    public void SetGameWin()
    {
        gameWon = true;
        WinCanvas.SetActive(true);
        UICanvas.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Returns true if the game is paused. False otherwise.
    /// </summary>
    /// <returns></returns>
    public bool GetPaused()
    {
        return paused;
    }

    /// <summary>
    /// Returns true if the game is won. False otherwise.
    /// </summary>
    /// <returns></returns>
    public bool GetGameWon()
    {
        return gameWon;
    }

    /// <summary>
    /// Returns true if the game is lost. False otherwise.
    /// </summary>
    /// <returns></returns>
    public bool GetGameLost()
    {
        return gameLost;
    }
    
}
