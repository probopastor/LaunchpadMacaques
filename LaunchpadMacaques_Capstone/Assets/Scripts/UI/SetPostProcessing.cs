using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class SetPostProcessing : MonoBehaviour
{
    [SerializeField] Volume cameraVolume;

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
        if(volumeName != cameraVolume.profile.name)
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
}
