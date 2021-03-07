/*
* Launchpad Macaques - Neon Oblivion
* Levi Schoof, Jamey Colleen
* ButtonTransitionManager.cs
* Handles Screen Transistions
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTransitionManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField, Tooltip("The Intro Transition that should play when this scene is loaded")] IntroTransitionTypes introTransition;
    [SerializeField, Tooltip("The Outro Transition that should be used before loading the next scene")] OutroTransistionTypes outroTransition;
    [SerializeField, Tooltip("The Transition that will be used when the player respawns")] RespawnTransitionTypes respawnTransition;

    [Header("Panel Transition Settings")]
    [SerializeField, Tooltip("The Transitions that should be played between changing Main Menu Panels")] PanelTransitionTypes mainMenuPanelsTransitions;
    #endregion

    #region Enums
    public enum IntroTransitionTypes { none, swipe, FadeIn };
    public enum OutroTransistionTypes { none, swipe, FadeOut };
    public enum PanelTransitionTypes { none, swipe };

    public enum RespawnTransitionTypes { none, swipe }
    #endregion

    [HideInInspector] public GameObject disable;
    [HideInInspector] public GameObject enable;


    private GameObject previousEnable;
    private GameObject previousDisable;
    private GameObject previousSelectObject;

    #region Private Variables
    GameObject nextSelectedGameObject;
    EventSystem eventSystem;

    private bool inTransisiton = false;
    private Animator transition;
    Vector3 startingPos;

    MainMenuManager mainMenu;

    SettingsManager settings;
    #endregion


    private void Awake()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        eventSystem = FindObjectOfType<EventSystem>();
        startingPos = this.GetComponent<RectTransform>().position;
        IntroTransition();
        mainMenu = FindObjectOfType<MainMenuManager>();
        settings = FindObjectOfType<SettingsManager>();
    }


    #region Public Start Transition Methods


    public void RespawnPlayerTranstion()
    {
        if (!inTransisiton)
        {
            StartCoroutine(RespawnTransition());
        }

    }
    /// <summary>
    /// Will load the given scene using a Transition
    /// </summary>
    /// <param name="levelname"></param>
    public void SwitchScene(string levelname)
    {
        if (!inTransisiton)
        {
            StartCoroutine(ExitTransition(levelname));
        }

    }

    /// <summary>
    /// Will load the correct opening scene Tutorial/Hub
    /// </summary>
    public void StartGame()
    {
        if (!inTransisiton)
        {
            StartCoroutine(ExitTransition(FindObjectOfType<StartGamePanel>().GetCorrectLevelName()));
        }

    }

    /// <summary>
    /// Will Start the swipe transition between panels
    /// </summary>
    public void StartTransisiton(bool useTransition = true)
    {
        if (inTransisiton)
        {
            StopAllCoroutines();
            ChangePanels();
        }

        if (useTransition && mainMenu)
        {
            if (mainMenu)
            {
                mainMenu.SetUseEscapeTransition(enable.name);
            }

            if(settings && useTransition)
            {
                settings.SetTransitionObject(enable);
            }
   
        }
        StartCoroutine(PanelTransition(useTransition));
    }
    #endregion

    #region Transitions/Transition Helpers


    #region IntroTransition
    /// <summary>
    /// Handles Intro Transitions on a scene
    /// </summary>
    private void IntroTransition()
    {
        switch (introTransition)
        {
            case IntroTransitionTypes.swipe:
                transition.SetTrigger("Swipe_In");
                Debug.Log(transition.GetCurrentAnimatorStateInfo(0).speed);
                break;
            case IntroTransitionTypes.FadeIn:
                transition.SetTrigger("Fade_In");
                break;
        }
    }
    #endregion

    #region Panel Transition
    /// <summary>
    /// Handles the panel swipe Coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator PanelTransition(bool useTransition)
    {
        previousEnable = enable;
        previousDisable = disable;
        previousSelectObject = nextSelectedGameObject;
        if (mainMenuPanelsTransitions != PanelTransitionTypes.none && useTransition)
        {
            inTransisiton = true;
            yield return null;
            inTransisiton = false;

            transition.StopPlayback();

            transition.Rebind();
            transition.Update(0f);

            string triggerName = GetPanelTransitionTrigger();
            transition.SetTrigger(triggerName);
            yield return null;
            AnimatorClipInfo[] anim = transition.GetCurrentAnimatorClipInfo(0);

            // A fix to handle if the animation is not started upon first clicking button
            while (anim.Length < 1)
            {
                transition.SetTrigger(triggerName);
                anim = transition.GetCurrentAnimatorClipInfo(0);
                yield return null;
            }

            transition.ResetTrigger(triggerName);
            yield return new WaitForSeconds(anim[0].clip.length * (1 / transition.GetCurrentAnimatorStateInfo(0).speed) * .75f);

            ChangePanels();
            yield return null;
            while (anim.Length < 1)
            {
                anim = transition.GetCurrentAnimatorClipInfo(0);
                yield return null;
            }

            yield return new WaitForSeconds(anim[0].clip.length * (1 / transition.GetCurrentAnimatorStateInfo(0).speed));
            //inTransisiton = false;
        }

        else
        {
            ChangePanels();
        }

    }

    /// <summary>
    /// Returns the correct Triger String for panel transitions
    /// </summary>
    /// <returns></returns>
    private string GetPanelTransitionTrigger()
    {
        string returnString = "";
        switch (mainMenuPanelsTransitions)
        {
            case PanelTransitionTypes.swipe:
                returnString = "Swipe_Panel";
                this.GetComponent<RectTransform>().position = startingPos;
                break;
        }
        return returnString;
    }


    /// <summary>
    /// Will Enable and Disable the correct panel objects
    /// </summary>
    private void ChangePanels()
    {
        if (previousDisable)
        {
            previousDisable.SetActive(false);
        }

        if (previousEnable)
        {
            previousEnable.SetActive(true);
        }


        if (previousSelectObject)
        {
            eventSystem.SetSelectedGameObject(previousSelectObject);
        }

        previousSelectObject = null;
        previousDisable = null;
        previousEnable = null;
    }

    #endregion

    #region OutroTransition
    /// <summary>
    /// Handles the Exit Transition
    /// </summary>
    /// <param name="levelName"></param>
    /// <returns></returns>
    IEnumerator ExitTransition(string levelName)
    {
        // Will Start A transition if one is selected
        if (outroTransition != OutroTransistionTypes.none)
        {
            inTransisiton = true;
            Time.timeScale = 1;

            // Finds the trigger the chosen outro is supposed to use
            string triggerName = FindOutroTriggerString();

            // Starts the outro animation and gets a reference to it
            transition.SetTrigger(triggerName);
            AnimatorClipInfo[] anim = transition.GetCurrentAnimatorClipInfo(0);

            // A fix to handle if the animation is not started upon first clicking button
            while (anim.Length < 1)
            {
                transition.SetTrigger(triggerName);
                anim = transition.GetCurrentAnimatorClipInfo(0);
                yield return null;
            }

            // Will wait until 75 percent of the animation is done
            yield return new WaitForSecondsRealtime((anim[0].clip.length * (1 / transition.GetCurrentAnimatorStateInfo(0).speed)));


            SceneManager.LoadScene(levelName);
            inTransisiton = false;
        }

        // If no outro is selected, will just load the next scene
        else
        {
            SceneManager.LoadScene(levelName);
        }


    }


    /// <summary>
    /// Will Return the correct Trigger String for the outro animation
    /// </summary>
    /// <returns></returns>
    private string FindOutroTriggerString()
    {
        string triggerName = "";
        switch (outroTransition)
        {
            case OutroTransistionTypes.swipe:
                transition.SetTrigger("Exit_Status");
                triggerName = "Exit_Status";
                break;
            case OutroTransistionTypes.FadeOut:
                transition.SetTrigger("Fade_Out");
                triggerName = "Fade_Out";
                break;
        }

        return triggerName;
    }

    IEnumerator RespawnTransition()
    {
        // Will Start A transition if one is selected
        if (respawnTransition != RespawnTransitionTypes.none)
        {
            inTransisiton = true;
            Time.timeScale = 1; ;

            // Finds the trigger the chosen outro is supposed to use
            string triggerName = FindTransitionTriggerString();

            // Starts the outro animation and gets a reference to it
            transition.SetTrigger(triggerName);
            AnimatorClipInfo[] anim = transition.GetCurrentAnimatorClipInfo(0);

            // A fix to handle if the animation is not started upon first clicking button
            while (anim.Length < 1)
            {
                transition.SetTrigger(triggerName);
                anim = transition.GetCurrentAnimatorClipInfo(0);
                yield return null;
            }

            // Will wait until 75 percent of the animation is done
            yield return new WaitForSecondsRealtime(anim[0].clip.length * (1 / transition.GetCurrentAnimatorStateInfo(0).speed));

            FindObjectOfType<RespawnSystem>().RespawnPlayer();
            yield return null;

            anim = transition.GetCurrentAnimatorClipInfo(0);
            while (anim.Length < 1)
            {
                anim = transition.GetCurrentAnimatorClipInfo(0);
                yield return null;
            }

            yield return new WaitForSecondsRealtime((anim[0].clip.length * (1 / transition.GetCurrentAnimatorStateInfo(0).speed)) * .5f);

            FindObjectOfType<RespawnSystem>().PlayerCanMove();
            inTransisiton = false;
        }

        else
        {
            FindObjectOfType<RespawnSystem>().RespawnPlayer();
            FindObjectOfType<RespawnSystem>().PlayerCanMove();
        }
    }

    private string FindTransitionTriggerString()
    {
        string triggerName = "";
        switch (respawnTransition)
        {
            case RespawnTransitionTypes.swipe:
                transition.SetTrigger("Swipe_Respawn");
                triggerName = "Swipe_Respawn";
                break;
        }

        return triggerName;
    }
    #endregion

    #endregion

    #region Getters/Setters
    /// <summary>
    /// Returns if a transition is happening
    /// </summary>
    /// <returns></returns>
    public bool IsInTransition()
    {
        return inTransisiton;
    }

    /// <summary>
    /// Assigns the object that will be enabled after a transition is done
    /// </summary>
    /// <param name="enableTarget"></param>
    public void AssignEnable(GameObject enableTarget)
    {
        enable = enableTarget;
    }

    /// <summary>
    /// Assigns the object that will be disabled after a transition is done
    /// </summary>
    /// <param name="disableTarget"></param>
    public void AssignDisable(GameObject disableTarget)
    {
        disable = disableTarget;
    }

    /// <summary>
    /// Assigns the object that will be selected after the transition
    /// </summary>
    /// <param name="selectedObject"></param>
    public void AssignCurrentSelected(GameObject selectedObject)
    {
        nextSelectedGameObject = selectedObject;
    }

    #endregion
}
