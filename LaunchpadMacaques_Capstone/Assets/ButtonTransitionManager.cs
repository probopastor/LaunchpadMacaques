/* 
* Launchpad Macaques 
* Jamey Colleen 
* ButtonTransitionManager.cs 
* Handles animation functionality for Main Menu and other buttons that require use
* of the animation for making transitions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTransitionManager : MonoBehaviour
{

    [SerializeField, Tooltip("Animator for transitions. ")] private Animator transition;

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;

    
    [SerializeField, Tooltip("Used to enable animation image.")] private GameObject transitionObject;

    public GameObject disable;
    public GameObject enable;


    // Start is called before the first frame update
    void Start()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        if (transitionObject == null)
        {
            transitionObject = GameObject.FindGameObjectWithTag("Transition");
        }

        transitionObject.SetActive(true);
        
    }

    //Gets UI pane to enable from Button
    public void AssignEnable(GameObject enableTarget)
    {
        enable = enableTarget;
    }

    //Gets UI pane to disable from Button
    public void AssignDisable(GameObject disableTarget)
    {
        disable = disableTarget;
    }

    //Executes from button in order to switch UI panels via transition
    public void SwitchPanel()
    {
        StartCoroutine(Transition(disable, enable));
    }

    public void SwitchScene(string levelname)
    {
        StartCoroutine(LevelLoader(levelname));
    }

    //Handles loading level with transition from button
    IEnumerator LevelLoader(string levelName)
    {
        transition.SetTrigger("Start");
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(transitionLength);
        SceneManager.LoadScene(levelName);
    }

    //Handles Transition of UI and delay to properly maintain visual cohesion
    IEnumerator Transition(GameObject disable, GameObject enable)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionLength);
        disable.SetActive(false);
        enable.SetActive(true);
    }


}
