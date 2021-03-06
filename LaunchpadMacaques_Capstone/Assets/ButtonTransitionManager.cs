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

public class ButtonTransitionManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField, Tooltip("The Intro Transition that should play when this scene is loaded")] IntroTransitionTypes introTransition;
    [SerializeField, Tooltip("The Outro Transition that should be used before loading the next scene")] OutroTransistionTypes outroTransition;

    [Header("Panel Transition Settings")]
    [SerializeField, Tooltip("The Transitions that should be played between changing Main Menu Panels")] PanelTransitionTypes mainMenuPanelsTransitions;
    [SerializeField, Tooltip("Time to wait to start second half of panel changing transition. ")] private int panelTransitionLength = 1;
    #endregion

    #region Enums
    public enum IntroTransitionTypes { none, swipe };
    public enum OutroTransistionTypes { none, swipe };
    public enum PanelTransitionTypes { none, swipe};
    #endregion

    [HideInInspector] public GameObject disable;
    [HideInInspector] public GameObject enable;

    #region Private Variables
    GameObject nextSelectedGameObject;
    EventSystem eventSystem;

    private bool inTransisiton = false;
    private Animator transition;
    Vector3 startingPos;
    #endregion


    private void Awake()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        eventSystem = FindObjectOfType<EventSystem>();
        startingPos = this.GetComponent<RectTransform>().position;
        IntroTransition();
    }


    #region Public Start Transition Methods
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
    public void StartTransisiton()
    {
        if (!inTransisiton)
        {
            StartCoroutine(PanelTransition());
        }
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
                break;
        }
    }
    #endregion

    #region Panel Transition
    /// <summary>
    /// Handles the panel swipe Coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator PanelTransition()
    {
        if(mainMenuPanelsTransitions != PanelTransitionTypes.none)
        {
            inTransisiton = true;

            transition.StopPlayback();

            this.GetComponent<RectTransform>().position = startingPos;

            transition.SetTrigger(GetPanelTransitionTrigger());
            yield return new WaitForSeconds(panelTransitionLength);

            ChangePanels();
            inTransisiton = false;
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
                break;
        }

        return returnString;
    }


    /// <summary>
    /// Will Enable and Disable the correct panel objects
    /// </summary>
    private void ChangePanels()
    {
        disable.SetActive(false);
        enable.SetActive(true);

        if (nextSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(nextSelectedGameObject);
        }

        nextSelectedGameObject = null;
        disable = null;
        enable = null;
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

            // Starts loading the next scene, but stops it from fully loading
            AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
            async.allowSceneActivation = false;

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
            yield return new WaitForSecondsRealtime(anim[0].clip.length * .75f);

            // Allows the next scene to be loaded
            async.allowSceneActivation = true;
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
