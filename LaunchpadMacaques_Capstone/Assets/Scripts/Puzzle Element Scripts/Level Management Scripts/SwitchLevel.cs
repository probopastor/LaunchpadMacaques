/* 
* Launchpad Macaques 
* William Nomikos
* SwitchLevel.cs 
* Handles basic scene and level switching when triggering a set object. 
*/

using UnityEngine;

public class SwitchLevel : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the scene this object should load when triggered. ")] private string nextLevelName;


    [SerializeField, Tooltip("Should this portal load the next level in the level progression")] bool playNextLevel = false;

    Matt_PlayerMovement player;
    HandleSaving handleSaving;

    private void Start()
    {
        player = FindObjectOfType<Matt_PlayerMovement>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
       
            SwitchScenes();
        }
    }


    /// <summary>
    /// Switches the scene the player is in to a designated scene. 
    /// </summary>
    private void SwitchScenes()
    {
        handleSaving = FindObjectOfType<HandleSaving>();
        player.SetPlayerCanMove(false);
        handleSaving.LevelCompleted();
        if (!playNextLevel)
        {
            FindObjectOfType<ButtonTransitionManager>().SwitchScene(nextLevelName);
        }

        else
        {
            FindObjectOfType<ButtonTransitionManager>().SwitchScene(handleSaving.GetNextLevel());
        }

        
    }
}
