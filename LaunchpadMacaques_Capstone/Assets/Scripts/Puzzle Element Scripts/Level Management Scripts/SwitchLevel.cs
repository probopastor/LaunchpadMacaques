/* 
* Launchpad Macaques 
* William Nomikos
* SwitchLevel.cs 
* Handles basic scene and level switching when triggering a set object. 
*/

using UnityEngine;

public class SwitchLevel : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the scene this object should load when triggered. ")] private string nextLevelName = null;

    Matt_PlayerMovement player;

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
        player.SetPlayerCanMove(false);
        FindObjectOfType<HandleSaving>().LevelCompleted();
        FindObjectOfType<ButtonTransitionManager>().SwitchScene(nextLevelName);
        
    }
}
