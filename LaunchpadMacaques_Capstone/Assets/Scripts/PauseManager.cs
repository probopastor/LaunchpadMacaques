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

public class PauseManager : MonoBehaviour
{
    private bool paused;
    private bool gameLost;

    [SerializeField] private string thisScene;
    [SerializeField] private string mainMenuScene;

    [SerializeField] private GameObject PauseCanvas;
    //[SerializeField] private GameObject LoseCanvas;
    //[SerializeField] private GameObject WinCanvas;
    [SerializeField] private GameObject CursorCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        gameLost = false;

        paused = false;
        PauseCanvas.SetActive(false);
        //LoseCanvas.SetActive(false);
        //WinCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameLost)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (paused == false)
        {
            paused = true;
            Time.timeScale = 0;
            PauseCanvas.SetActive(true);
            CursorCanvas.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (paused == true)
        {
            Time.timeScale = 1;
            paused = false;
            PauseCanvas.SetActive(false);
            CursorCanvas.SetActive(true);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(thisScene);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void SetGameLost()
    {
        gameLost = true;
        //LoseCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void SetGameWin()
    {
        //WinCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public bool GetPaused()
    {
        return paused;
    }

    public bool GetGameLost()
    {
        return gameLost;
    }
}
