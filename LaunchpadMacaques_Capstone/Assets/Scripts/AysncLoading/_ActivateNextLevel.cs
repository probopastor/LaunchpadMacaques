using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class _ActivateNextLevel : MonoBehaviour
{
   [SerializeField] protected AysncLoading loading;

    protected void LoadNextLevel()
    {
        loading.AllowLoading();
    }
}
