using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTransitionManager : MonoBehaviour
{

    [SerializeField, Tooltip("Animator for transitions. ")] private Animator transition;

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;

    public GameObject disable;
    public GameObject enable;


    // Start is called before the first frame update
    void Start()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssignEnable(GameObject enableTarget)
    {
        enable = enableTarget;
    }

    public void AssignDisable(GameObject disableTarget)
    {
        disable = disableTarget;
    }


    public void SwitchPanel()
    {
        StartCoroutine(Transition(disable, enable));
    }

    public void SwitchScene(string levelname)
    {
        StartCoroutine(LevelLoader(levelname));
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

    IEnumerator Transition(GameObject disable, GameObject enable)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionLength);
        disable.SetActive(false);
        enable.SetActive(true);
    }


}
