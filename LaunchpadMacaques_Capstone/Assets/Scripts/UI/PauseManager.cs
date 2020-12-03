/* 
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
    private bool gameWon;
    public GameObject HTPMenu;
    public GameObject pausePanel;

    [SerializeField, Tooltip("The scene this pause manager is located in. ")] private string thisScene;
    [SerializeField, Tooltip("The main menu scene name. ")] private string mainMenuScene;

    [SerializeField, Tooltip("The pause panel that is being used as a menu. ")] private GameObject PauseCanvas;
    //[SerializeField] private GameObject LoseCanvas;
    [SerializeField, Tooltip("The win panel that is being used as a win state. ")] private GameObject WinCanvas;
    [SerializeField, Tooltip("The cursor object. ")] private GameObject CursorCanvas;
    [SerializeField, Tooltip("The UI panel. ")] private GameObject UICanvas;
    [SerializeField, Tooltip("The Information Post Text. ")] private GameObject InformationPostText;
    [SerializeField, Tooltip("The rope length text. ")] private GameObject RopeLengthText;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        gameLost = false;

        paused = false;
        gameWon = false;
        PauseCanvas.SetActive(false);
        //LoseCanvas.SetActive(false);
        WinCanvas.SetActive(false);
        UICanvas.SetActive(true);

        if (InformationPostText != null)
        {
            InformationPostText.SetActive(true);
        }

        if(RopeLengthText != null)
        {
            RopeLengthText.SetActive(true);
        }

        pausePanel = PauseCanvas.GetComponentInChildren<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameLost && !gameWon && !HTPMenu.activeSelf)
            {
                PauseGame();
            }

            if(HTPMenu.activeSelf)
            {
                HTPMenu.SetActive(false);
                pausePanel.SetActive(true);
            }
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
            CursorCanvas.SetActive(false);

            if (InformationPostText != null)
            {
                InformationPostText.SetActive(false);
            }

            if (RopeLengthText != null)
            {
                RopeLengthText.SetActive(false);
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

            if(InformationPostText != null)
            {
                InformationPostText.SetActive(true);
            }

            if (RopeLengthText != null)
            {
                RopeLengthText.SetActive(true);
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
