/* 
* Launchpad Macaques 
* William Nomikos, CJ Green (Added Escape key functionality.)
* MainMenuManager.cs 
* Handles Main Menu functionality, including starting and quitting the game.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    //[SerializeField] private string pushPullScene;
    //[SerializeField] private string swingScene;

    [SerializeField]
    private GameObject mainMenu_Panel;

    [SerializeField]
    private List<GameObject> menuPanels = new List<GameObject>();

    public Animator anim;

    private ButtonTransitionManager transitionManager;

    List<string> useTransitions;

    public GameObject MainMenu_Panel { get => mainMenu_Panel; set => mainMenu_Panel = value; }

    private void Start()
    {

        useTransitions = new List<string>();
        mainMenu_Panel.SetActive(true);

        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        anim.Play("MenuBGAnim");


        if (HandleSaving.instance)
        {
            Destroy(HandleSaving.instance.gameObject);
        }

        transitionManager = FindObjectOfType<ButtonTransitionManager>();
    }


    /// <summary>
    /// Handles the functionality for making the escape key back out of menus.
    /// </summary>
    public void EscapeKey()
    {
        int index = 0;
        for (index = 0; index <= menuPanels.Count - 1; index++)

        {
            switch (menuPanels[index].activeSelf)
            {

                case true:

                    switch (menuPanels[index].name)
                    {
                        case "Level Select Page1":
                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = menuPanels[5];
                            //transitionManager.StartTransisiton(false);

                            break;

                        case "Level Select Page2":
                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = menuPanels[0];
                           // transitionManager.StartTransisiton(false);

                            break;

                        case "HowToPlay Panel":
                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = mainMenu_Panel;
                            //transitionManager.StartTransisiton();

                            break;

                        case "Credits Panel":
                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = mainMenu_Panel;
                           // transitionManager.StartTransisiton();

                            break;

                        case "OptionsMenu":
                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = mainMenu_Panel;
                           // transitionManager.StartTransisiton();

                            break;

                        case "Save File Panel":

                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = mainMenu_Panel;
                            //transitionManager.StartTransisiton(false);
                            break;

                        case "Start Game Panel":

                            transitionManager.disable = menuPanels[index];
                            transitionManager.enable = menuPanels[4];
                            break;

                    }

                    bool useTransition = false;
                    for(int i = 0; i < useTransitions.Count; i++)
                    {
                        if(menuPanels[index].name == useTransitions[i])
                        {
                            useTransition = true;
                        }
                    }

                    transitionManager.StartTransisiton(useTransition);
                    break;


            }
        }
    }



    /// <summary>
    /// Loads a specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load (case sensitive). </param> 
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

    public void SetUseEscapeTransition(string panelName)
    {
        useTransitions.Add(panelName);
    }
}