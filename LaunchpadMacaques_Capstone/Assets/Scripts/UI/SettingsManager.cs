﻿/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* SettingsManager.CS
* Script that handles the Options/Settings Screen
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class SettingsManager : MonoBehaviour
{

    [Header("Graphic UI Elements")]
    [SerializeField, Tooltip("The Resolution Dropdown Box")] TMP_Dropdown resolutionDropdown;
    [SerializeField, Tooltip("The Full Screen Toggle")] Toggle fullScreenToggle;
    [SerializeField, Tooltip("The Graphics Quality Dropdown Box")] TMP_Dropdown graphicsQualityDropdown;
    [SerializeField, Tooltip("The Colorblind Mode Dropdown Box")] TMP_Dropdown colorblindModeDropdown;
    [SerializeField, Tooltip("The Bloom Toggle")] Toggle bloomToggle;
    [SerializeField] Toggle vsyncToggle = null;

    [Header("Volume Sliders")]
    [SerializeField, Tooltip("The Dialouge Volume Slider")] Slider dialougeVolume;
    [SerializeField, Tooltip("The Music Volume Slider")] Slider musicVolume;
    [SerializeField, Tooltip("The Sound Effects Volume Slider")] Slider soundEffectsVolume;
    [SerializeField, Tooltip("Master Volume Slider")] Slider masterSoundSlider;

    [Header("Gameplay Settings")]
    [SerializeField, Tooltip("The Invert Y Toggle Box")] Toggle invertY;
    [SerializeField, Tooltip("The Mouse Sensitivity Slider")] Slider mouseSensitivity;
    [SerializeField, Tooltip("The Field of View Slider")] Toggle fovToggle;
    [SerializeField, Tooltip("The Slider to set Starting FOV Value")] Slider fovSlider;
    [SerializeField] Toggle hoverLineToggle = null;
    [SerializeField] Toggle screenShake;

    [Header("Settings Holder")]
    [SerializeField, Tooltip("Will hold all the settings Menu")] GameObject settingsHolder;
    [SerializeField] GameObject audioSubmenu;
    [SerializeField] GameObject videoSubMenu;
    [SerializeField] GameObject gameplaySubMenu;
    [SerializeField] GameObject lookSettings;
    [SerializeField] GameObject miscGameplaySettings;
    [SerializeField] GameObject graphicsSettings;
    [SerializeField] GameObject postProcessingSettings;

    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject mainMenu;

    [SerializeField, Tooltip("The max volume for the Volume Sliders")] float maxVolume = 1;
    [SerializeField, Tooltip("The min volume for the Volume Sliders")] float minVolume = 0;

    [SerializeField] Button playButton;

    Resolution[] reslolutions;

    private Animator transition;


    ButtonTransitionManager transitionManager;


    List<GameObject> useTransitionObjects;

    private SetPostProcessing postProcessing;

    private bool masterSet = false;
    private bool musicSet = false;
    private bool dialougeSet = false;
    private bool sfxSet = false;


    // The Deafult variables the sliders will be set to, upon an ititial launch (Player has never played game before)
    #region Deafult Variables
    private int deafultGraphicsQuality = 1;
    private int defaultColorblindMode = 0;
    private float deafultDialouge = .5f;
    private float deafultMusic = .5f;
    private float deafultSoundEffects = .5f;

    private float deafultMouseSensitivity = 50;
    private int deafultFOV = 60;
    private float deafultMaster = 1;

    public GameObject PostProcessingSettings { get => postProcessingSettings; set => postProcessingSettings = value; }
    public GameObject MainMenu { get => mainMenu; set => mainMenu = value; }
    public Slider SoundEffectsVolume { get => soundEffectsVolume; set => soundEffectsVolume = value; }
    public GameObject MiscGameplaySettings { get => miscGameplaySettings; set => miscGameplaySettings = value; }
    public GameObject VideoSubMenu { get => videoSubMenu; set => videoSubMenu = value; }
    public GameObject GraphicsSettings { get => graphicsSettings; set => graphicsSettings = value; }
    public GameObject GameplaySubMenu { get => gameplaySubMenu; set => gameplaySubMenu = value; }
    public Slider FovSlider { get => fovSlider; set => fovSlider = value; }
    public Slider MouseSensitivity { get => mouseSensitivity; set => mouseSensitivity = value; }
    public Toggle FullScreenToggle { get => fullScreenToggle; set => fullScreenToggle = value; }
    public TMP_Dropdown ResolutionDropdown { get => resolutionDropdown; set => resolutionDropdown = value; }
    public Toggle BloomToggle { get => bloomToggle; set => bloomToggle = value; }
    public Toggle InvertY { get => invertY; set => invertY = value; }
    public Toggle FovToggle { get => fovToggle; set => fovToggle = value; }
    public GameObject LookSettings { get => lookSettings; set => lookSettings = value; }
    public GameObject SettingsHolder { get => settingsHolder; set => settingsHolder = value; }
    public GameObject AudioSubmenu { get => audioSubmenu; set => audioSubmenu = value; }
    public Slider DialougeVolume { get => dialougeVolume; set => dialougeVolume = value; }
    public GameObject OptionsMenu { get => optionsMenu; set => optionsMenu = value; }
    public TMP_Dropdown GraphicsQualityDropdown { get => graphicsQualityDropdown; set => graphicsQualityDropdown = value; }
    public TMP_Dropdown ColorblindModeDropdown { get => colorblindModeDropdown; set => colorblindModeDropdown = value; }
    public Toggle ScreenShake { get => screenShake; set => screenShake = value; }
    public Slider MusicVolume { get => musicVolume; set => musicVolume = value; }
    public Button PlayButton { get => playButton; set => playButton = value; }
    public Slider MasterSoundSlider { get => masterSoundSlider; set => masterSoundSlider = value; }


    #endregion
    void Start()
    {
        postProcessing = FindObjectOfType<SetPostProcessing>();
        useTransitionObjects = new List<GameObject>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            StartCoroutine(SetAllMenuElements());
        }

        transition = FindObjectOfType<ButtonTransitionManager>().GetComponent<Animator>();
        transitionManager = FindObjectOfType<ButtonTransitionManager>();
    }

    IEnumerator SetAllMenuElements()
    {
        settingsHolder.SetActive(true);
        yield return new WaitForEndOfFrame();

        InitialFullScreen();
        InitialQuality();
        InitialDialouge();
        InitialHoverLine();
        InitialMusic();
        InitialMaster();
        InitialSFX();
        InitialInvertY();
        InitialMouseSensitivity();
        SetResolutionsDropDown();
        InitialFOV();
        InitialScreenShake();
        InitialColorblindMode();
        InitialBloom();
        InitialVysnc();

        yield return new WaitForEndOfFrame();

        while (!masterSet || !dialougeSet || !musicSet || !sfxSet)
        {
            yield return null;
        }
        DisableStuff();
    }

    public void UpdateLookSettings()
    {
        InitialFOV();
        InitialInvertY();
        InitialMouseSensitivity();
        InitialFullScreen();
    }

    public void UpdateMiscGameplaySettings()
    {
        InitialScreenShake();
        InitialHoverLine();
    }

    public void UpdateSound()
    {
        InitialDialouge();
        InitialMusic();
        InitialMaster();
        InitialSFX();
    }

    public void UpdateGraphicSettings()
    {
        InitialQuality();
        SetResolutionsDropDown();
        InitialFullScreen();
        InitialVysnc();
    }

    public void UpdatePostProcessing()
    {
        InitialColorblindMode();
        InitialBloom();
    }

    private void DisableStuff()
    {

        audioSubmenu.SetActive(false);
        videoSubMenu.SetActive(false);
        gameplaySubMenu.SetActive(false);
        lookSettings.SetActive(false);
        miscGameplaySettings.SetActive(false);
        graphicsSettings.SetActive(false);
        postProcessingSettings.SetActive(false);
        settingsHolder.SetActive(false);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (DetectController.instance)
            {
                DetectController.instance.selectedGameObject = playButton.gameObject;
            }

            else
            {
                FindObjectOfType<DetectController>().selectedGameObject = playButton.gameObject;
            }
            FindObjectOfType<EventSystem>().SetSelectedGameObject(playButton.gameObject);
        }

    }

    // Will set the sliders to have a starting value equal to either a deafult variable, or the relevant player prefs
    #region SetInitialValues
    /// <summary>
    /// Caalled on start 
    /// Will fill the resoultion DropDown box with the Resolutions Unity can find that work on the current Machine
    /// Will set Resolutin Dropdown box index to use the Deafult Value, or the "ResolutionIndex" PlayerPref
    /// Then will set the current Resolution to either the PlayerPref Resolution or the Deafult Value
    /// </summary>
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

    /// <summary>
    /// Called on Start
    /// Will Set the Full Screen toggle to be either the deafult value or the "FullScreen" PlayerPrefs
    /// Will then apply that value
    /// </summary>
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

    private void InitialBloom()
    {
        if (PlayerPrefs.HasKey("Bloom"))
        {
            if (PlayerPrefs.GetInt("Bloom") == 1)
            {
                bloomToggle.SetIsOnWithoutNotify(true);
                SetBloom(true);
            }

            else
            {
                bloomToggle.SetIsOnWithoutNotify(false);
                SetBloom(false);
            }
        }

        else
        {
            bloomToggle.SetIsOnWithoutNotify(true);
            SetBloom(true);
        }
    }

    private void InitialHoverLine()
    {
        if (PlayerPrefs.HasKey("HoverLine"))
        {
            if (PlayerPrefs.GetInt("HoverLine") == 1)
            {
                hoverLineToggle.SetIsOnWithoutNotify(true);
                SetHoverLine(true);
            }

            else
            {
                hoverLineToggle.SetIsOnWithoutNotify(false);
                SetHoverLine(false);
            }
        }

        else
        {
            hoverLineToggle.SetIsOnWithoutNotify(true);
            SetHoverLine(true);
        }
    }

    private void InitialScreenShake()
    {
        if (PlayerPrefs.HasKey("ScreenShake"))
        {
            if (PlayerPrefs.GetInt("ScreenShake") == 1)
            {
                screenShake.SetIsOnWithoutNotify(true);
                SetScreenShake(true);
            }

            else
            {
                screenShake.SetIsOnWithoutNotify(false);
                SetScreenShake(false);
            }
        }

        else
        {
            screenShake.SetIsOnWithoutNotify(true);
            SetScreenShake(true);
        }
    }

    /// <summary>
    /// Called On Start
    /// Will set the InvertY Toggle to be either the deafult value or the "InvertY" PlayerPrefs
    /// Will then apply that value
    /// </summary>
    private void InitialInvertY()
    {
        if (PlayerPrefs.HasKey("InvertY"))
        {
            if (PlayerPrefs.GetInt("InvertY") == 1)
            {
                invertY.SetIsOnWithoutNotify(true);
                SetInvertY(true);
            }

            else
            {
                invertY.SetIsOnWithoutNotify(false);
                SetInvertY(false);
            }
        }

        else
        {
            invertY.SetIsOnWithoutNotify(false);
            SetInvertY(false);
        }
    }

    private void InitialVysnc()
    {
        if (PlayerPrefs.HasKey("VSync"))
        {
            if (PlayerPrefs.GetInt("VSync") == 1)
            {
                vsyncToggle.SetIsOnWithoutNotify(true);
                SetVSync(true);
            }

            else
            {
                vsyncToggle.SetIsOnWithoutNotify(false);
                SetVSync(false);
            }
        }

        else
        {
            vsyncToggle.SetIsOnWithoutNotify(false);
            SetVSync(false);
        }
    }

    /// <summary>
    /// Called on Start
    /// Will Set the Mouse Sensivity Slider to either the deafult value or the "MouseSensitivity" PlayerPrefs
    /// Will then apply that value
    /// </summary>
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

    /// <summary>
    /// Called on Start
    /// Will set the Index of the Quality Dropdown box to either the deafult value or the "QualityLevel" PlayerPrefs
    /// Will then apply that value
    /// </summary>
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

    private void InitialColorblindMode()
    {
        if (PlayerPrefs.HasKey("ColorblindMode"))
        {
            colorblindModeDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("ColorblindMode"));
            SetColorblindMode(PlayerPrefs.GetInt("ColorblindMode"));
        }
        else
        {
            SetColorblindMode(defaultColorblindMode);
            colorblindModeDropdown.SetValueWithoutNotify(defaultColorblindMode);
        }

    }

    /// <summary>
    /// Called on Start
    /// Will Set the Value of the Dialouge Volume slider to either the deafult value or the "DialougeVolume" PlayerPrefs
    /// Will then apply that value 
    /// </summary>
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

        dialougeSet = true;
    }

    private void InitialMaster()
    {
        masterSoundSlider.maxValue = maxVolume;
        masterSoundSlider.minValue = minVolume;

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume"));
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        }

        else
        {
            masterSoundSlider.SetValueWithoutNotify(deafultMaster);
            SetMasterVolume(deafultMaster);
        }

        masterSet = true;
    }

    /// <summary>
    /// Called on Start
    /// Will Set the Value of the Dialouge Volume slider to either the deafult value or the "DialougeVolume" PlayerPrefs
    /// Will then apply that value 
    /// </summary>
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

        musicSet = true;
    }

    /// <summary>
    /// Called on Start
    /// Will Set the value of the SFX slider to either the deafult value or the "SFXVolume" PlayerPrefs
    /// Will then apply that value
    /// </summary>
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

        sfxSet = true;
    }

    /// <summary>
    /// Called on Start
    /// Will Set the Value of the FOV slider to either the deafult value or the "FOV PlayerPrefs
    /// Will then apply that value 
    /// </summary>
    private void InitialFOV()
    {
        if (PlayerPrefs.HasKey("FOV"))
        {
            if (PlayerPrefs.GetInt("FOV") == 1)
            {
                fovToggle.SetIsOnWithoutNotify(true);
            }

            else
            {
                fovToggle.SetIsOnWithoutNotify(false);
            }
        }

        else
        {
            fovToggle.SetIsOnWithoutNotify(true);
        }

        if (PlayerPrefs.HasKey("FovValue"))
        {
            fovSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("FovValue"));
            SetFOVValue(PlayerPrefs.GetInt("FovValue"));
        }

        else
        {
            fovSlider.SetValueWithoutNotify(deafultFOV);
            SetFOVValue(deafultFOV);
        }
    }

    #endregion

    // Is called when the player changes a setting on the options menu
    // Will apply the changed values to the relevant player prefs
    #region ApplyValues

    /// <summary>
    /// Will be called when the player selects a new resolution
    /// Will apply that new resoultion 
    /// Will set that resolutions index to the "ResolutionIndex" PlayerPref
    /// </summary>
    /// <param name="index"></param>
    public void SetResolution(int index)
    {
        Resolution newResolution = reslolutions[index];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    /// <summary>
    /// Will be called when the player changes the Full Screen Toggle
    /// Will apply that new choice
    /// Will set that choice to the "FullScreen" Playerpref
    /// </summary>
    /// <param name="isFullScreen"></param>
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

    public void SetHoverLine(bool useHoverLine)
    {
        if (useHoverLine)
        {
            PlayerPrefs.SetInt("HoverLine", 1);
        }

        else
        {
            PlayerPrefs.SetInt("HoverLine", 0);
        }
    }

    public void SetBloom(bool useBloom)
    {
        if (useBloom)
        {
            PlayerPrefs.SetInt("Bloom", 1);
        }

        else
        {
            PlayerPrefs.SetInt("Bloom", 0);
        }

        postProcessing.SetBloom();
    }

    public void SetScreenShake(bool useScreenShake)
    {
        if (useScreenShake)
        {
            PlayerPrefs.SetInt("ScreenShake", 1);
        }

        else
        {
            PlayerPrefs.SetInt("ScreenShake", 0);
        }
    }

    /// <summary>
    /// Will be called when the player changes the sesitivity slider
    /// Will set the new float to the "MouseSensitivity" PlayerPref
    /// "MouseSensitivity" is used in the script Mat_Player_Movment.CS
    /// </summary>
    /// <param name="sensitivy"></param>
    public void SetMouseSensitivty(float sensitivy)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivy);
    }

    /// <summary>
    /// Will be called when the player changes the FOV Slider
    /// Will set the new float to the "FOV" PlayerPref
    /// "FOV" is used in the script MoveCamera.CS
    /// </summary>
    /// <param name="newFov"></param>
    public void SetFOV(bool fov)
    {
        if (fov)
        {
            PlayerPrefs.SetInt("FOV", 1);
        }

        else
        {
            PlayerPrefs.SetInt("FOV", 0);
        }
    }

    public void SetFOVValue(float value)
    {
        PlayerPrefs.SetInt("FovValue", (int)value);
    }

    /// <summary>
    /// Will be called when the player changes the InvertY toggle
    /// Will Set the new choice to the "InvertY" PlayerPref
    /// "InvertY" is used in the script "Mat_Player_Movement".CS 
    /// </summary>
    /// <param name="invert"></param>
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

    public void SetVSync(bool vsync)
    {
        if (vsync)
        {
            PlayerPrefs.SetInt("VSync", 1);
            QualitySettings.vSyncCount = 1;
        }

        else
        {
            PlayerPrefs.SetInt("VSync", 0);
            QualitySettings.vSyncCount = 0;
        }
    }

    /// <summary>
    /// Will be called when the player choices a new Graphics Quality
    /// Will apply the relevant choice in the built in Unity Quality Settings
    /// Will apply the index of that choice to the "QualityLevel" PlayerPref
    /// </summary>
    /// <param name="qualityLevel"></param>
    public void SetQuality(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetInt("QualityLevel", qualityLevel);


        vsyncToggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);
        SetVSync(QualitySettings.vSyncCount > 0);

    }

    public void SetColorblindMode(int colorblindMode)
    {
        PlayerPrefs.SetInt("ColorblindMode", colorblindMode);
    }

    /// <summary>
    /// Will be called when the player changes the Dialouge Volume Slider
    /// Will Set the new float to the "DialougeVolume" PlayerPref
    /// Will then change the FMOD Volume_VO 
    /// </summary>
    /// <param name="volume"></param>
    public void SetDialougeVolume(float volume)
    {
        PlayerPrefs.SetFloat("DialougeVolume", volume);
        ChangeFMODSound(dialougeVolume.gameObject, volume);
    }

    /// <summary>
    /// Will be called when the player changes the Music Volume Slider
    /// Will set the new float to the "MusicVolume" PlayerPref
    /// Will change the FMOD Volume_BGM
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        ChangeFMODSound(musicVolume.gameObject, volume);
    }

    /// <summary>
    /// Will be called when the player chanes the SFX Volume Slider
    /// Will set the float to the "SFXVolume" Playerpref
    /// Will change the FMOD Volume_SFX
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);

        ChangeFMODSound(soundEffectsVolume.gameObject, volume);
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// The method the Volume methods call to actually set the FMOD volume
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="volume"></param>
    private void ChangeFMODSound(GameObject obj, float volume)
    {
        StudioGlobalParameterTrigger trigger = obj.GetComponent<StudioGlobalParameterTrigger>();
        trigger.value = volume;
        trigger.TriggerParameters();
    }

    #endregion


    public void HandleEscapeKey()
    {
        if (settingsHolder.activeSelf && SceneManager.GetActiveScene().name == "MainMenu" && !transitionManager.IsInTransition())
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            if (optionsMenu.activeSelf)
            {
                transitionManager.enable = mainMenu;
                transitionManager.disable = settingsHolder;
            }

            else if (videoSubMenu.activeSelf)
            {
                transitionManager.enable = optionsMenu;
                transitionManager.disable = videoSubMenu;
            }

            else if (audioSubmenu.activeSelf)
            {
                transitionManager.enable = optionsMenu;
                transitionManager.disable = audioSubmenu;
            }

            else if (gameplaySubMenu.activeSelf)
            {
                transitionManager.enable = optionsMenu;
                transitionManager.disable = gameplaySubMenu;
            }

            else if (lookSettings.activeSelf)
            {
                transitionManager.enable = gameplaySubMenu;
                transitionManager.disable = lookSettings;
            }

            else if (miscGameplaySettings.activeSelf)
            {
                transitionManager.enable = gameplaySubMenu;
                transitionManager.disable = miscGameplaySettings;
            }

            else if (graphicsSettings.activeSelf)
            {
                transitionManager.enable = videoSubMenu;
                transitionManager.disable = graphicsSettings;
            }

            else if (postProcessingSettings.activeSelf)
            {
                transitionManager.enable = videoSubMenu;
                transitionManager.disable = postProcessingSettings;
            }


            bool useTransition = false;

            for (int i = 0; i < useTransitionObjects.Count; i++)
            {
                if (transitionManager.disable == useTransitionObjects[i])
                {
                    useTransition = true;
                }
            }

            transitionManager.StartTransisiton(useTransition);
        }

    }


    public bool InSubSettings()
    {
        if (gameplaySubMenu.activeSelf || videoSubMenu.activeSelf || audioSubmenu.activeSelf || lookSettings.activeSelf
            || miscGameplaySettings.activeSelf || graphicsSettings.activeSelf || postProcessingSettings.activeSelf)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void SetTransitionObject(GameObject newObject)
    {
        useTransitionObjects.Add(newObject);
    }


}