using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardSceneGroup : MonoBehaviour
{
    private static StandardSceneGroup _instance;

    public static StandardSceneGroup Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }
}
