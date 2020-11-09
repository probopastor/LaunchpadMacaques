using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DoorAudio : MonoBehaviour
{
    private StudioEventEmitter soundEmitter;

    // Start is called before the first frame update
    void Start()
    {
        soundEmitter = GetComponent<StudioEventEmitter>();
    }

    /// <summary>
    /// Plays door sound. 
    /// </summary>
    /// <param name="enable"></param>
    public void PlayDoorSound(bool enable)
    {
        if(!enable)
        {
            if (!soundEmitter.IsPlaying())
            {
                soundEmitter.Play();
            }

            soundEmitter.EventInstance.triggerCue();
        }
        else if(enable)
        {
            soundEmitter.Play();
        }
    }
}
