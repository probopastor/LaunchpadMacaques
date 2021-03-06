using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonTransitionManager : MonoBehaviour
{

    private Animator transition;

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;

    [HideInInspector] public GameObject disable;
    [HideInInspector] public GameObject enable;

    GameObject nextSelectedGameObject;
    EventSystem eventSystem;

    private bool inTransisiton = false;


    // Start is called before the first frame update
    void Start()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public void AssignEnable(GameObject enableTarget)
    {
        enable = enableTarget;
    }

    public void AssignDisable(GameObject disableTarget)
    {
        disable = disableTarget;
    }

    public void AssignCurrentSelected(GameObject selectedObject)
    {
        nextSelectedGameObject = selectedObject;
    }

    public void SwitchScene(string levelname)
    {
        StartCoroutine(LevelLoader(levelname));
    }

    public void SwitchScene()
    {
        StartCoroutine(LevelLoader(FindObjectOfType<StartGamePanel>().GetCorrectLevelName()));
    }

    public void EndOfLevelSwitchScene(string sceneName)
    {
        // StopAllCoroutines();
        StartCoroutine(ExitFadeTransition(sceneName));
    }

    IEnumerator LevelLoader(string levelName)
    {
        transition.SetTrigger("Start");
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(transitionLength);
        SceneManager.LoadScene(levelName);
    }



    public void StartTransisiton()
    {
        StartCoroutine(Transition());
    }
    private IEnumerator Transition()
    {
        inTransisiton = true;
        transition.SetTrigger("Start");
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

    IEnumerator ExitFadeTransition(string levelName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = false;
        transition.SetTrigger("Exit_Status");
        Debug.Log((transition.GetCurrentAnimatorStateInfo(0).length));
        yield return new WaitForSeconds(transition.GetCurrentAnimatorStateInfo(0).length - .1f);

        async.allowSceneActivation = true;
    
    }


    public bool IsInTransition()
    {
        return inTransisiton;
    }

}
