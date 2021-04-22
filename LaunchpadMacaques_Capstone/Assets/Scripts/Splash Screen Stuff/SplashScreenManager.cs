using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{

    private SplashScreen[] splashScreens;

    private Image mainMenuBackground;

    [SerializeField]
    private bool isFaddingDone = false;

    private void Awake()
    {
        splashScreens = FindObjectsOfType<SplashScreen>();
        //mainMenuBackground = FindObjectOfType<Canvas>().transform.GetChild(0).GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Fade things in at start.
       //StartCoroutine(FadeTo(mainMenuBackground, 1f, 5f));

        StartCoroutine(WaitTime(splashScreens[0].GetWaitTime()));
    }

    private void Update()
    {

        Debug.Log(isFaddingDone);

        if (Input.GetKeyDown(KeyCode.P))
        {
            SkipWarning();
        }

        //if (splashScreens.Length <= 1 && isFaddingDone)
        //{
        //    StartCoroutine(WaitTime(splashScreens[0].GetWaitTime()));
        //}

    }

    public IEnumerator FadeTo(Image imageColor, float targetOpacity, float duration)
    {

        isFaddingDone = false;

        // How many seconds we've been fading for.
        float elapsedTime = 0;
        
        // Cached reference of Image color.
        Color tempColor = imageColor.color;

        // Referance to starting opacity.
        float startingOpacity = tempColor.a;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float blend = Mathf.Clamp01(elapsedTime / duration);

            tempColor.a = Mathf.Lerp(startingOpacity, targetOpacity, blend);

            mainMenuBackground.color = tempColor;

            yield return null;

            if(elapsedTime >= duration)
            {
                StopCoroutine("FadeTo");
                isFaddingDone = true;
            }
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
