/* 
* Launchpad Macaques - Neon Oblivion 
* William Nomikos 
* IntroductionManager.cs
* Handles the behavior for the introduction scene. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class IntroductionManager : MonoBehaviour
{
    [SerializeField, TextArea, Tooltip("The text that will be displayed over the background image. ")] private string[] textToPlay;
    [SerializeField, Tooltip("The TMPro of the information text. Must have the same number of TMPros as the text in the textToPlay array. ")] private TextMeshProUGUI[] informationTextToPlay;

    [SerializeField, Tooltip("The text effect handler. ")] private TextEffectHandler textEffects;
    [SerializeField, Tooltip("The time each section of text will wait for before a new section appears. ")] private float introductionDurationDuringEffects = 0f;
    [SerializeField, Tooltip("The time the introduction will last for after all text effects have stopped playing. ")] private float introductionDurationAfterEffects = 0f;

    [SerializeField, Tooltip("The tutorial scene to be loaded after the introduction is finished.")] string tutorialLevelName;

    [Tooltip("The text iterator that maintains which text should currently be played. ")] private int textIterator = 0;
    [Tooltip("Determines whether the next text should occur immediately or after its set delay. ")] private bool startTextImmediately = false;
    [Tooltip("Determines whether the input to skip text has been pressed")] private bool textSkipped = false;

    [SerializeField] private bool scaleImage = false;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float startImageScale = 100f;
    [SerializeField] private float endImageScale = 1f;
    [SerializeField] private float imageScaleChangeRate = 0.5f;
    private float currentImageScale = 0f;

    // Start is called before the first frame update
    void Start()
    {
        SetInformation();
    }

    private void Update()
    {
        if(scaleImage)
        {
            ImageScaling();
        }
    }


    public void SkipIntroInput()
    {
        SkipToNextText();
    }
    private void SetInformation()
    {
        startTextImmediately = false;

        for (int i = 0; i < informationTextToPlay.Length; i++)
        {
            informationTextToPlay[i].enabled = false;
        }

        if (scaleImage)
        {
            currentImageScale = startImageScale;
            backgroundImage.transform.localScale = new Vector2(currentImageScale, currentImageScale);
        }

        StartCoroutine(Intro());
    }

    /// <summary>
    /// Skips the current text that is being played.
    /// </summary>
    private void SkipToNextText()
    {
        if(textEffects.EffectsRunning() != 0 && !textSkipped)
        {
            // If a text effect is running, finish it. 
            textEffects.SkipToEndOfEffects();
            // Set the next set of text to occur immediatly without a delay
            startTextImmediately = true;
            //Mark text to skip
            textSkipped = true;
        }
    }

    /// <summary>
    /// Handles Introduction text behavior. Cycles through text events as they are completed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Intro()
    {
        // Just keep running until scene needs to be changed
        for(; textIterator < textToPlay.Length; textSkipped = false, textIterator++)
        {
            //Don't start until any leftover effects are skipped
            while(textEffects.EffectsRunning() != 0)
            {
                yield return null;
            }

            informationTextToPlay[textIterator].text = "";
            informationTextToPlay[textIterator].enabled = true;
            yield return null;
            textEffects.RunText(informationTextToPlay[textIterator], textToPlay[textIterator]);


            //Reset text skipped to wait for input
            textSkipped = false;
            //Wait for the text to finish running
            while (textEffects.EffectsRunning() != 0 && !textSkipped)
            {
                yield return null;
            }

            // If the previous text section was skipped, the next one will be loaded immediately. 
            if (!startTextImmediately)
            {
                yield return new WaitForSeconds(introductionDurationDuringEffects);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                startTextImmediately = false;
            }

            yield return null;
         }


        //All text has been played, scene switch
        yield return new WaitForSeconds(3f);
        StartCoroutine(IntroductionSequence());
    }

    /// <summary>
    /// Coroutine handles switching the scene to the Tutorial upon completion of the Intro. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator IntroductionSequence()
    {
        // If no text effects are running, load the next scene
        if (textEffects.EffectsRunning() == 0)
        {
            yield return new WaitForSeconds(introductionDurationAfterEffects);
            SceneManager.LoadScene(tutorialLevelName);
        }
        else
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(IntroductionSequence());
        }

        yield return new WaitForEndOfFrame();
    }

    private void ImageScaling()
    {
        if (currentImageScale > endImageScale)
        {
            currentImageScale -= imageScaleChangeRate * Time.deltaTime;
            backgroundImage.transform.localScale = new Vector2(currentImageScale, currentImageScale);
        }
    }
}
