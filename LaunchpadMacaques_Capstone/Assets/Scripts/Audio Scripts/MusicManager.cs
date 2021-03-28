using FMOD;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public bool randomTracks;

    private ScriptableEmitter curEmitter;

    public ScriptableEmitter[] emitters;

    private float volumeFrom;
    private float volumeTo;

    private void Awake()
    {
        
        this.gameObject.transform.parent = null;
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            volumeFrom = 1;
            volumeTo = 0;
            SceneManager.sceneLoaded += SwitchScene;

        }
        else
        {
            Destroy(this.gameObject);
        }
        
        //SwitchTrack("Main");
    }

    private void SwitchScene(Scene scene, LoadSceneMode mode)
    {
        int buildIndex = scene.buildIndex;
        switch (buildIndex) //These will likely change when scenes get removed.
        {
            case 0:
                SwitchTrack("Main");
                break;
            case 1:
                SwitchTrack("Tutorial");
                break;
            case 2:
                SwitchTrack("Dungeon"); //TEMP
                break;
            case 3:
                SwitchTrack("Castles"); //TEMP
                break;
            case 4:
                SwitchTrack("Lab");
                break;

        }
        if (buildIndex > 4) SwitchTrack("Main");
    }

    public void SwitchTrack(string trackName)
    {
        instance.StopAllCoroutines();

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
            volumeFrom -= 0.1f;
        }

        emitter.emitter.Stop();
        volumeFrom = 1;
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator FadeIn(ScriptableEmitter emitter)
    {
        emitter.emitter.Play();
        while (volumeTo < 1)
        {
            emitter.emitter.SetParameter("Volume", volumeTo);
            yield return new WaitForFixedUpdate();
            volumeTo += 0.05f;
        }

        volumeTo = 0;
        yield return new WaitForFixedUpdate();
    }
}
