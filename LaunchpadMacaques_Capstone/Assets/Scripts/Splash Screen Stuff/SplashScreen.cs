using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;

public class SplashScreen : MonoBehaviour
{

    [SerializeField]
    private string splashScreenName;

    [SerializeField]
    private float waitTime;

    [SerializeField]
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public string GetSplashScreenName()
    {
        return splashScreenName;
    }

    public float GetWaitTime()
    {
        return waitTime;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }

}
