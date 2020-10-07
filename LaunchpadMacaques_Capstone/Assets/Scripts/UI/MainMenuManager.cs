/* 
* Launchpad Macaques 
* William Nomikos
* MainMenuManager.cs 
* Handles Main Menu functionality, including starting and quitting the game.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string pushPullScene;
    [SerializeField] private string swingScene;

    private void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    //public void StartGame()
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(pushPullScene);
    //}

    public void StartPushPullScene()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(pushPullScene);
    }

    public void StartSwingScene()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(swingScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}