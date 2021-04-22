using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroductionManager : MonoBehaviour
{
    [SerializeField, Tooltip("The TMPro of the information text. ")] private TextMeshProUGUI informationText;
    [SerializeField, TextArea, Tooltip("The text that will be displayed over the background image. ")] private string information;
    [SerializeField, Tooltip("The text effect handler. ")] private TextEffectHandler textEffects;
    [SerializeField, Tooltip("The time the introduction will last for after all text effects have stopped playing. ")] private float introductionDurationAfterEffects = 0f;

    [SerializeField, Tooltip("The tutorial scene to be loaded after the introduction is finished." )] string tutorialLevelName;

    // Start is called before the first frame update
    void Start()
    {
        SetInformation();
        StartCoroutine(IntroductionSequence());
    }

    private void Update()
    {
        
    }

    private void SetInformation()
    {
        if (informationText != null)
        {
            textEffects.RunText(informationText, information);
        }
    }

    private IEnumerator IntroductionSequence()
    {
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
}
