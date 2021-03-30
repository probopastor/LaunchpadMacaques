/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (AsyncLoading.CS) 
* (The script that is placed in each scene to handle Aysnc Loading) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AysncLoading : MonoBehaviour
{
    [SerializeField, Tooltip("The Name of the Next Scene ")] string nextSceneName = null;

    private bool allowLoading;
    private void Start()
    {
        allowLoading = false;

        StartCoroutine(AsyncLoad());
    }


    /// <summary>
    /// Handles the Loading of the next level
    /// </summary>
    /// <returns></returns>
    IEnumerator AsyncLoad()
    {
        yield return new WaitForSeconds(2);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
         
            if(asyncOperation.progress >= .9f)
            {
                asyncOperation.allowSceneActivation = allowLoading;
            }

            yield return new WaitForSeconds(0);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextSceneName));

        if (SceneManager.sceneCount >= 3)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
        }
    }

    /// <summary>
    /// Public method that will be called to activate loading
    /// </summary>
    public void AllowLoading()
    {
        allowLoading = true;
        Debug.Log("Allow Loading");
    }
}

[System.Serializable]
public struct PlatformTracks
{
    public List<GameObject> gameObjects;
}
