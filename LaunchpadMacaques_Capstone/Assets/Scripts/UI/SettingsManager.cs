/* 
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

public class SettingsManager : MonoBehaviour
{

    [Header("Graphic UI Elements")]
    [SerializeField, Tooltip("The Resolution Dropdown Box")] TMP_Dropdown resolutionDropdown;
    [SerializeField, Tooltip("The Full Screen Toggle")] Toggle fullScreenToggle;
    [SerializeField, Tooltip("The Graphics Quality Dropdown Box")] TMP_Dropdown graphicsQualityDropdown;

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

    [Header("Settings Holder")]
    [SerializeField, Tooltip("Will hold all the settings Menu")] GameObject settingsHolder;
    [SerializeField] GameObject audioSettings;
    [SerializeField] GameObject videoSettings;
    [SerializeField] GameObject gameplaySettings;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject mainMenu;

    [SerializeField, Tooltip("The max volume for the Volume Sliders")] float maxVolume = 1;
    [SerializeField, Tooltip("The min volume for the Volume Sliders")] float minVolume = 0;

    [SerializeField, Tooltip("Animator for transitions. ")] private Animator transition;

    [SerializeField, Tooltip("Time to wait to start second half of transition. ")] private int transitionLength = 1;

    [SerializeField] Button playButton;

    Resolution[] reslolutions;

    // The Deafult variables the sliders will be set to, upon an ititial launch (Player has never played game before)
    #region Deafult Variables
    private int deafultGraphicsQuality = 1;
    private float deafultDialouge = .5f;
    private float deafultMusic = .5f;
    private float deafultSoundEffects = .5f;

    private float deafultMouseSensitivity = 50;
    private int deafultFOV = 60;
    private float deafultMaster = 1;


    #endregion
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            settingsHolder.SetActive(true);


            InitialFullScreen();
            InitialQuality();
            InitialDialouge();
            InitialMusic();
            InitialMaster();
            InitialSFX();
            InitialInvertY();
            InitialMouseSensitivity();
            SetResolutionsDropDown();
            InitialFOV();

            DisableStuff();


        }

    }

    public void UpdateGameplay()
    {
        InitialFOV();
        InitialInvertY();
        InitialMouseSensitivity();
        InitialFullScreen();
    }

    public void UpdateSound()
    {
        InitialDialouge();
        InitialMusic();
        InitialMaster();
        InitialSFX();
    }

    public void UpdateVisuals()
    {
        SetResolutionsDropDown();
        InitialFullScreen();
        InitialQuality();
    }

    private void Update()
    {
        HandleEscapeKey();
    }

    private void DisableStuff()
    {

        audioSettings.SetActive(false);
        videoSettings.SetActive(false);
        gameplaySettings.SetActive(false);
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


    private void HandleEscapeKey()
    {
        if (/*SceneManager.GetActiveScene().name == "MainMenu"*/ true)
        {
            StartCoroutine(SwitchPanel());
        }

    }

    IEnumerator SwitchPanel()
    {
        if (Input.GetButtonDown("Back") && settingsHolder.activeSelf)
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            if (optionsMenu.activeSelf)
            {
                transition.SetTrigger("Start");
                yield return new WaitForSeconds(transitionLength);
                mainMenu.SetActive(true);
                settingsHolder.SetActive(false);
            }

            else if (videoSettings.activeSelf)
            {
                transition.SetTrigger("Start");
                yield return new WaitForSeconds(transitionLength);
                optionsMenu.SetActive(true);
                videoSettings.SetActive(false);
            }

            else if (audioSettings.activeSelf)
            {
                transition.SetTrigger("Start");
                yield return new WaitForSeconds(transitionLength);
                optionsMenu.SetActive(true);
                audioSettings.SetActive(false);
            }

            else if (gameplaySettings.activeSelf)
            {
                transition.SetTrigger("Start");
                yield return new WaitForSeconds(transitionLength);
                optionsMenu.SetActive(true);
                gameplaySettings.SetActive(false);
            }

            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
        }
    }

    public bool InSubSettings()
    {
        if (gameplaySettings.activeSelf || videoSettings.activeSelf || audioSettings.activeSelf)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}