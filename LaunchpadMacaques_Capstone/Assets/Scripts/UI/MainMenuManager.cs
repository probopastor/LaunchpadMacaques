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
    private GameObject levelSelect_Panel;

    [SerializeField]
    private List<GameObject> menuPanels = new List<GameObject>();

    [SerializeField, Tooltip("Animator for transitions. ")] private Animator transition;

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;

    public Animator anim;
    private void Start()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        mainMenu_Panel.SetActive(true);
        levelSelect_Panel.SetActive(false);

        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        anim.Play("MainMenuBG");
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

        if (Input.GetButtonDown("Back"))
        {

            for (index = 0; index <= menuPanels.Count - 1; index++)

            {
                StartCoroutine(SwitchPanel(index));
            }
        }
    }


    IEnumerator SwitchPanel( int index)
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
        switch (menuPanels[index].activeSelf)
        {

            case true:

                switch (menuPanels[index].name)
                {
                    case "Level Select Page1":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        mainMenu_Panel.SetActive(true);

                        break;

                    case "Level Select Page2":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        menuPanels[1].SetActive(true);

                        break;

                    case "HowToPlay Panel":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        mainMenu_Panel.SetActive(true);

                        break;

                    case "Credits Panel":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        mainMenu_Panel.SetActive(true);

                        break;

                    case "OptionsMenu":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        mainMenu_Panel.SetActive(true);

                        break;

                    case "GameplayOptionsPanel":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        menuPanels[5].SetActive(true);

                        break;

                    case "VideoOptionsPanel":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        menuPanels[5].SetActive(true);

                        break;

                    case "AudioOptionsPanel":

                        transition.SetTrigger("Start");
                        yield return new WaitForSeconds(transitionLength);
                        menuPanels[index].SetActive(false);
                        menuPanels[5].SetActive(true);

                        break;

                }

                break;

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
}