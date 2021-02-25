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

    HandleSaving handleSaving;


    private void Start()
    {
        handleSaving = FindObjectOfType<HandleSaving>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
       
            SwitchScenes();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SwitchScenes();
        }
    }

    /// <summary>
    /// Switches the scene the player is in to a designated scene. 
    /// </summary>
    private void SwitchScenes()
    {
        handleSaving.LevelCompleted();
        SceneManager.LoadScene(nextLevelName);
        
    }
}
