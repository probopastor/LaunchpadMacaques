using FMOD;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Tooltip("Enable this if you want the music to automatically play in the scene on loop.")]
    public bool autoPlay;

    public static MusicManager instance;

    private ScriptableEmitter curEmitter;

    public ScriptableEmitter[] emitters;

    private float volumeFrom;
    private float volumeTo;

    private void Awake()
    {
        instance = this;
        volumeFrom = 1;
        volumeTo = 0;
        if (autoPlay)
            SwitchTrack("Castles");
    }

    public void SwitchTrack(string trackName)
    {
        StopAllCoroutines();

        if (curEmitter != null)
        {
            StartCoroutine(FadeOut(curEmitter));
        }
        

        curEmitter = AudioUtilities.FindScriptableEmitter(emitters, trackName);

        if (!curEmitter.emitter.IsPlaying()) StartCoroutine(FadeIn(curEmitter));
    }

    public IEnumerator FadeOut(ScriptableEmitter emitter)
    {
        while (volumeFrom > 0)
        {
            emitter.emitter.SetParameter("Volume", volumeFrom);
            yield return new WaitForFixedUpdate();
            volumeFrom -= 0.01f;
        }

        emitter.emitter.Stop();
        volumeFrom = 1;
    }

    public IEnumerator FadeIn(ScriptableEmitter emitter)
    {
        emitter.emitter.Play();
        while (volumeTo < 1)
        {
            emitter.emitter.SetParameter("Volume", volumeTo);
            yield return new WaitForFixedUpdate();
            volumeTo += 0.01f;
        }

        volumeTo = 0;
    }

    private void Update()
    {
        if (!autoPlay)
        {
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                SwitchTrack("Cursed");
            }

            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                SwitchTrack("Castles");
            }
        }
    }
}
