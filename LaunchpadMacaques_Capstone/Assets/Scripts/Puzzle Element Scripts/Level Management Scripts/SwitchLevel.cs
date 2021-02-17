/* 
* Launchpad Macaques 
* William Nomikos
* SwitchLevel.cs 
* Handles basic scene and level switching when triggering a set object. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SwitchLevel : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the scene this object should load when triggered. ")] private string nextLevelName;
    [SerializeField, Tooltip("Animator for transitions. ")] private Animator transition;
    [SerializeField, Tooltip("Time to wait for transition. ")] private int transitionLength = 2;

    public bool levelComplete = false;

    private void Start()
    {
        transition = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SwitchScenes());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(SwitchScenes());
        }
    }

    /// <summary>
    /// Switches the scene the player is in to a designated scene. 
    /// </summary>
    IEnumerator SwitchScenes()
    {
        GameObject.Find("Player").GetComponent<Matt_PlayerMovement>().canMove = false;

        transition.SetTrigger("Exit_Status");
        yield return new WaitForSeconds(transitionLength);
        levelComplete = true;
        levelComplete = false;
        SceneManager.LoadScene(nextLevelName);
        
    }
}
