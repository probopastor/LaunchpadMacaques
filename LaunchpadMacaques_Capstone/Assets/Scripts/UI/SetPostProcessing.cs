using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class SetPostProcessing : MonoBehaviour
{
    [SerializeField] Volume cameraVolume = null;

    string cameraVolumeName;

    VolumeProfile profile;

    string volumeName;


    private void Start()
    {
        volumeName = cameraVolume.profile.name;
        SetBloom();
    }

    private void Update()
    {
        if (volumeName != cameraVolume.profile.name)
        {
            volumeName = cameraVolume.profile.name;
            SetBloom();
        }
    }

    public void SetBloom()
    {
        profile = cameraVolume.profile;
        Debug.Log(profile.components.Count);

        if (profile.TryGet<Bloom>(out var bloom))
        {
            bloom.active = (PlayerPrefs.GetInt("Bloom") == 1);
        }
    }

    public void SetVignete(bool turnOn, Color color ,float intensity = 0)
    {
        profile = cameraVolume.profile;

        Vignette vig;

        if (profile.TryGet<Vignette>(out vig))
        {
            vig.active = (turnOn);
        }

        else
        {
            if (turnOn)
            {
                profile.Add<Vignette>();
            }

            if (profile.TryGet<Vignette>(out vig))
            {
                vig.active = (turnOn);
            }
        }


        if (turnOn)
        {

           ColorParameter colorState = vig.color;

            colorState.value = color;
            colorState.overrideState = true;

            vig.color = colorState;
            ClampedFloatParameter p = vig.intensity;

            p.overrideState = true;
            p.value = intensity;

            vig.intensity = p;
        }

    }
}
