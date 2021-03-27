/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (_ActivateNextLevel.CS) 
* (The Abstract Class that holds the logic to activate the loading of a level) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class _ActivateNextLevel : MonoBehaviour
{
   [SerializeField, Tooltip("The Async Loading object this trigger uses")] protected AysncLoading loading;
    

    /// <summary>
    /// The method that will be called to Allow Loading
    /// </summary>
    protected void LoadNextLevel()
    {
        loading.AllowLoading();
    }
}
