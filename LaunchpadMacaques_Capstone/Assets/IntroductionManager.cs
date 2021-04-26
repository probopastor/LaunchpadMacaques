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

    [Tooltip("The text iterator that maintains which text should currently be played. ")] private int textIterator = 1;
    [Tooltip("Determines whether the next scene should be loaded. ")] private bool queueSceneSwitch = false;
    [Tooltip("Maintains whether or not text is currently being played. ")] private bool textInProgress = false;
    [Tooltip("Determines whether the next text should occur immediately or after its set delay. ")] private bool startTextImmediately = false;

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
        textIterator = 1;
        queueSceneSwitch = false;
        textInProgress = false;
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
        if (!informationTextToPlay[textIterator - 1].IsActive())
        {
            informationTextToPlay[textIterator - 1].enabled = true;
        }

        // If a text effect is running, finish it. 
        if (textEffects.EffectsRunning() != 0)
        {
            textEffects.SkipToEndOfEffects();
        }

        // Set the next set of text to occur immediatly without a delay
        startTextImmediately = true;

        // Sets the text in progress so that text can be properly iterated in Intro()
        textInProgress = true;

        // Restarts Intro() with new conditionals
        StopCoroutine(Intro());
        StartCoroutine(Intro());
    }

    /// <summary>
    /// Handles Introduction text behavior. Cycles through text events as they are completed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Intro()
    {
        // If text is not in progress, play text
        if (textEffects.EffectsRunning() == 0 && !queueSceneSwitch && !textInProgress)
        {
            textInProgress = true;
            informationTextToPlay[textIterator - 1].enabled = true;
            textEffects.RunText(informationTextToPlay[textIterator - 1], textToPlay[textIterator - 1]);
        }
        // If text just finished being in progress, iterate the text to be played.
        else if (textEffects.EffectsRunning() == 0 && !queueSceneSwitch && textInProgress)
        {
            // Sets the text to stop being in progress
            textInProgress = false;

            // Iterate which text should be played
            textIterator++;

            // Check to see if all text has been played.
            if (textIterator >= textToPlay.Length + 1)
            {
                // If it has, the Scene Switch should occur when the last text ends
                queueSceneSwitch = true;
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

            StartCoroutine(Intro());
        }
        // If all text has been played, begin the Scene Switch
        else if (textEffects.EffectsRunning() == 0 && queueSceneSwitch)
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(IntroductionSequence());
        }

        // Cycle through the Intro() coroutine until all text has been played
        yield return new WaitForEndOfFrame();
        StartCoroutine(Intro());
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
