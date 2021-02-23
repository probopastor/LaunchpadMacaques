using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public PlayerDataNew data;

    public float[] savedPosition;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        savedPosition = data.position;
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
}
