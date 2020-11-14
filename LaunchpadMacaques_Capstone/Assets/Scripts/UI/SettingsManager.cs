using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    Resolution[] reslolutions;
    [Header("Graphic UI Elements")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] TMP_Dropdown graphicsQualityDropdown;

    [Header("Volume Sliders")]
    [SerializeField] Slider dialougeVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider soundEffectsVolume;

    [Header("Gameplay Settings")]
    [SerializeField] Toggle invertY;
    [SerializeField] Slider mouseSensitivity;

    [Header("Settings Holder")]
    [SerializeField] GameObject settingsHolder;

    [SerializeField] float maxVolume = 100;
    [SerializeField] float minVolume = 0;

    private int deafultGraphicsQuality = 1;

    private float deafultDialouge = 50;
    private float deafultMusic = 50;
    private float deafultSoundEffects = 50;

    private float deafultMouseSensitivity = 50;
    // Start is called before the first frame update
    void Start()
    {
        InitialFullScreen();
        InitialQuality();
        InitialDialouge();
        InitialMusic();
        InitialSFX();
        InitialInvertY();
        InitialMouseSensitivity();
        SetResolutionsDropDown();

        settingsHolder.SetActive(false);
    }

    #region SetInitialValues
    private void SetResolutionsDropDown()
    {
        reslolutions = Screen.resolutions;

        List<string> resolutionOptions = new List<string>();


        int currentResolutionNum = 0;

        for (int i = 0; i < reslolutions.Length; i++)
        {
            string option = reslolutions[i].width + "X" + reslolutions[i].height + " @" + reslolutions[i].refreshRate;

            if (!PlayerPrefs.HasKey("ResolutionIndex"))
            {
                if (reslolutions[i].width == Screen.currentResolution.width &&
                    reslolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionNum = i;
                }
            }

            else
            {
                currentResolutionNum = PlayerPrefs.GetInt("ResolutionIndex");
            }

            resolutionOptions.Add(option);
        }

        resolutionDropdown.ClearOptions();

        resolutionDropdown.AddOptions(resolutionOptions);

        resolutionDropdown.value = currentResolutionNum;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitialFullScreen()
    {
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            if (PlayerPrefs.GetInt("FullScreen") == 1)
            {
                fullScreenToggle.SetIsOnWithoutNotify(true);
                SetFullScreen(true);
            }

            else
            {
                fullScreenToggle.SetIsOnWithoutNotify(false);
                SetFullScreen(false);
            }
        }

        else
        {
            fullScreenToggle.SetIsOnWithoutNotify(true);
            SetFullScreen(true);
        }
    }

    private void InitialInvertY()
    {
        if (PlayerPrefs.HasKey("InvertY"))
        {
            if(PlayerPrefs.GetInt("InvertY") == 1)
            {
                invertY.SetIsOnWithoutNotify(true);
            }

            else
            {
                invertY.SetIsOnWithoutNotify(false);
            }
        }

        else
        {
            invertY.SetIsOnWithoutNotify(true);
            SetInvertY(true);
        }
    }

    private void InitialMouseSensitivity()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivity.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSensitivity"));
            SetMouseSensitivty(PlayerPrefs.GetFloat("MouseSensitivity"));
        }

        else
        {
            mouseSensitivity.SetValueWithoutNotify(deafultMouseSensitivity);
            SetMouseSensitivty(deafultMouseSensitivity);
        }
    }

    private void InitialQuality()
    {
        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            graphicsQualityDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("QualityLevel"));
            SetQuality(PlayerPrefs.GetInt("QualityLevel"));
        }

        else
        {
            graphicsQualityDropdown.SetValueWithoutNotify(deafultGraphicsQuality);
            SetQuality(deafultGraphicsQuality);
        }
    }

    private void InitialDialouge()
    {
        dialougeVolume.maxValue = maxVolume;
        dialougeVolume.minValue = minVolume;
        if (PlayerPrefs.HasKey("DialougeVolume"))
        {
            dialougeVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("DialougeVolume"));
            SetDialougeVolume(PlayerPrefs.GetFloat("DialougeVolume"));
        }

        else
        {
            dialougeVolume.SetValueWithoutNotify(deafultDialouge);
            SetDialougeVolume(deafultDialouge);
        }
    }

    private void InitialMusic()
    {
        musicVolume.maxValue = maxVolume;
        musicVolume.minValue = minVolume;
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume"));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }

        else
        {
            musicVolume.SetValueWithoutNotify(deafultMusic);
            SetMusicVolume(deafultMusic);
        }
    }

    private void InitialSFX()
    {
        soundEffectsVolume.maxValue = maxVolume;
        soundEffectsVolume.minValue = minVolume;
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            soundEffectsVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume"));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }

        else
        {
            soundEffectsVolume.SetValueWithoutNotify(deafultSoundEffects);
            SetSFXVolume(deafultSoundEffects);
        }
    }

    #endregion

    #region ApplyValues
    public void SetResolution(int index)
    {
        Resolution newResolution = reslolutions[index];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        if (isFullScreen)
        {
            PlayerPrefs.SetInt("FullScreen", 1);
        }

        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }
    }

    public void SetMouseSensitivty(float sensitivy)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivy);
    }

    public void SetInvertY(bool invert)
    {
        if (invert)
        {
            PlayerPrefs.SetInt("InvertY", 1);
        }

        else
        {
            PlayerPrefs.SetInt("InvertY", 0);
        }
    }
    public void SetQuality(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
    }

    public void SetDialougeVolume(float volume)
    {
        /// Add in code to set Dialouge Volume

        PlayerPrefs.SetFloat("DialougeVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        // Add in code to Set Music Volume

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Add in code to set SFX volume

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    #endregion
}
