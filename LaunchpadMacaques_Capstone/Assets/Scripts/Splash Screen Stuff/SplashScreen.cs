using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashScreen : MonoBehaviour
{

    [SerializeField]
    private string splashScreenName = "Default";

    [SerializeField]
    private float waitTime = 5.0f;

    [SerializeField]
    private TextMeshProUGUI text;

    private bool isFaddingDone = false;

    [SerializeField]
    private List<GameObject> faddingObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(FadeTo(faddingObjects[0].GetComponent<Image>().color, 1f, 5f));
        //StartCoroutine(FadeTo(faddingObjects[1].GetComponent<TextMeshProUGUI>().color, 0f, 5f));

        //var TempColor = faddingObjects[1].GetComponent<TextMeshProUGUI>().color;
        //TempColor.a = 0.5f;
        //faddingObjects[1].GetComponent<TextMeshProUGUI>().color = TempColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeTo(Color imageColor, float targetOpacity, float duration)
    {

        isFaddingDone = false;

        // How many seconds we've been fading for.
        float elapsedTime = 0;

        // Cached reference of Image color.
        Color tempColor = imageColor;

        // Referance to starting opacity.
        float startingOpacity = tempColor.a;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float blend = Mathf.Clamp01(elapsedTime / duration);

            tempColor.a = Mathf.Lerp(startingOpacity, targetOpacity, blend);

            imageColor = tempColor;

            yield return null;

            if (elapsedTime >= duration)
            {
                StopCoroutine("FadeTo");
                isFaddingDone = true;
            }
        }
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
