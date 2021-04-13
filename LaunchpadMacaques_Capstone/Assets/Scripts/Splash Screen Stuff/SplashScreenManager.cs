using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{

    private SplashScreen[] splashScreens;

    private void Awake()
    {
        splashScreens = FindObjectsOfType<SplashScreen>();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (splashScreens.Length <= 1)
        {
            StartCoroutine(WaitTime(splashScreens[0].GetWaitTime()));
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SkipWarning();
        }
    }

    private IEnumerator WaitTime(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("MainMenu");

    }

    public void SkipWarning()
    {

        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");

    }


}
