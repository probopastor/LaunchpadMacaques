/* 
* Launchpad Macaques 
* William Nomikos, CJ Green (Added Escape key functionality.)
* MainMenuManager.cs 
* Handles Main Menu functionality, including starting and quitting the game.
*/

using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //[SerializeField] private string pushPullScene;
    //[SerializeField] private string swingScene;

    [SerializeField]
    private GameObject mainMenu_Panel;

    [SerializeField]
    private GameObject levelSelect_Panel;

    [SerializeField]
    private List<GameObject> menuPanels = new List<GameObject>();

    private void Start()
    {

        mainMenu_Panel.SetActive(true);
        levelSelect_Panel.SetActive(false);

        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    private void Update()
    {
        EscapeKey();
    }

    /// <summary>
    /// Handles the functionality for making the escape key back out of menus.
    /// </summary>
    public void EscapeKey()
    {
        int index = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            for (index = 0; index <= menuPanels.Count - 1; index++)

            {

                switch (menuPanels[index].activeSelf)
                {

                    case true:

                        switch (menuPanels[index].name)
                        {
                            case "Level Select Page1":

                                menuPanels[index].SetActive(false);
                                mainMenu_Panel.SetActive(true);

                                break;

                            case "Level Select Page2":

                                menuPanels[index].SetActive(false);
                                menuPanels[1].SetActive(true);

                                break;

                            case "HowToPlay Panel":

                                menuPanels[index].SetActive(false);
                                mainMenu_Panel.SetActive(true);

                                break;

                            case "Credits Panel":

                                menuPanels[index].SetActive(false);
                                mainMenu_Panel.SetActive(true);

                                break;

                            case "OptionsMenu":

                                menuPanels[index].SetActive(false);
                                mainMenu_Panel.SetActive(true);

                                break;

                            case "GameplayOptionsPanel":

                                menuPanels[index].SetActive(false);
                                menuPanels[5].SetActive(true);

                                break;

                            case "VideoOptionsPanel":

                                menuPanels[index].SetActive(false);
                                menuPanels[5].SetActive(true);

                                break;

                            case "AudioOptionsPanel":

                                menuPanels[index].SetActive(false);
                                menuPanels[5].SetActive(true);

                                break;

                        }

                        break;

                }
            }
        }
    }

    //public void StartGame()
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(pushPullScene);
    //}

    /// <summary>
    /// Obsolete. Previously brought players to the push pull level.
    /// </summary>
    //public void StartPushPullScene()
    //{
    //    Time.timeScale = 1;
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    SceneManager.LoadScene(pushPullScene);
    //}

    ///// <summary>
    ///// Starts the swing sandbox level upon button click.
    ///// </summary>
    //public void StartSwingScene()
    //{
    //    Time.timeScale = 1;
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    SceneManager.LoadScene(swingScene);
    //}

    public void LoadScene(string sceneName)
{
    Time.timeScale = 1;
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    SceneManager.LoadScene(sceneName);
}

/// <summary>
/// Exits the game.
/// </summary>
public void QuitGame()
{
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}
}