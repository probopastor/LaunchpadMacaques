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

    /// <summary>
    /// Obsolete. Previously brought players to the push pull level.
    /// </summary>
    public void StartPushPullScene()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(pushPullScene);
    }

    /// <summary>
    /// Starts the swing sandbox level upon button click.
    /// </summary>
    public void StartSwingScene()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(swingScene);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}