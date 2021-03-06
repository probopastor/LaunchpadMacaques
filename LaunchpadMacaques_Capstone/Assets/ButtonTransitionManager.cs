/*
* Launchpad Macaques - Neon Oblivion
* Levi Schoof Jamey Colleen
* ButtonTransitionManager.cs
* Handles Screen Transistions
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonTransitionManager : MonoBehaviour
{

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;
    [SerializeField, Tooltip("The Intro Transition that should play when this scene is loaded")] IntroTransitionTypes introTransition;
    [SerializeField, Tooltip("The Outro Transition that should be used before loading the next scene")] OutroTransistionTypes outroTransition;

    [HideInInspector] public GameObject disable;
    [HideInInspector] public GameObject enable;

    GameObject nextSelectedGameObject;
    EventSystem eventSystem;

    private bool inTransisiton = false;
    private Animator transition;

    public enum IntroTransitionTypes { none, swipe };
    public enum OutroTransistionTypes { swipe };
    Vector3 startingPos;


    private void Awake()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        eventSystem = FindObjectOfType<EventSystem>();
        startingPos = this.GetComponent<RectTransform>().position;
        IntroTransition();
    }

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



    /// <summary>
    /// Will load the given scene using a Transition
    /// </summary>
    /// <param name="levelname"></param>
    public void SwitchScene(string levelname)
    {
        if (!inTransisiton)
        {
            StartCoroutine(ExitFadeTransition(levelname));
        }

    }

    /// <summary>
    /// Will load the correct opening scene Tutorial/Hub
    /// </summary>
    public void SwitchScene()
    {
        if (!inTransisiton)
        {
            StartCoroutine(ExitFadeTransition(FindObjectOfType<StartGamePanel>().GetCorrectLevelName()));
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

    /// <summary>
    /// Handles the panel swipe Coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator PanelTransition()
    {
 
        inTransisiton = true;

        transition.StopPlayback();

        this.GetComponent<RectTransform>().position = startingPos;

        transition.SetTrigger("Swipe_Panel");
        yield return new WaitForSeconds(transitionLength);


        disable.SetActive(false);
        enable.SetActive(true);

        if (nextSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(nextSelectedGameObject);
        }



        nextSelectedGameObject = null;
        disable = null;
        enable = null;

        inTransisiton = false;


    }

    /// <summary>
    /// Handles the Exit Transition
    /// </summary>
    /// <param name="levelName"></param>
    /// <returns></returns>
    IEnumerator ExitFadeTransition(string levelName)
    {
        inTransisiton = true;
        Time.timeScale = 1;
        AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = false;
        transition.ResetTrigger("Exit_Status");

        switch (outroTransition)
        {
            case OutroTransistionTypes.swipe:
                transition.SetTrigger("Exit_Status");
                break;
        }

        yield return new WaitForEndOfFrame();

        AnimatorClipInfo[] anim = transition.GetCurrentAnimatorClipInfo(0);

        while (anim.Length < 1)
        {
            transition.SetTrigger("Exit_Status");
            anim = transition.GetCurrentAnimatorClipInfo(0);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(anim[0].clip.length * .75f);

        async.allowSceneActivation = true;
        inTransisiton = false;

    }


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
