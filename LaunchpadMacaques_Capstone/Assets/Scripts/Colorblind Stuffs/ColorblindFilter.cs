﻿/* 
* (Launchpad Macaques - [Neon Oblivion]) 
* (CJ Green) 
* (TestScript.cs) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorblindFilter: MonoBehaviour
{
    // Enum of type Coloblindness that stores each different type of colorblindness.
    public enum Colorblindness { NORMAL = 0, PROTANOPIA = 1, PROTANOMALY = 2, DEUTERANOPIA = 3, DEUTERANOMALY = 4, TRITANOPIA = 5, TRITANOMALY = 6, ACHROMATOPSIA = 7, ACHROMATOMALY = 8 };

    // Reference to the enum type that tracks the current selected colorblindness mode.
    public Colorblindness currentColorblindMode = Colorblindness.NORMAL;

    // Reference to the current camera's Volume component.
    [SerializeField]
    private Volume camreaVolume;

    //[SerializeField]
    //private VolumeProfile[] volumeProfiles;

    private string profilePath = "Volume Profiles";

    // Object array where the loaded volume profiles are stored.
    [SerializeField]
    private Object[] profileObjects;

    // Dictionary of the profiles for easy lookup.
    private Dictionary<string, VolumeProfile> volumeProfiles;


    private void Awake()
    {

        camreaVolume = FindObjectOfType<Camera>().GetComponent<Volume>();

        LoadProfiles();
        SetProfile(PlayerPrefs.GetInt("ColorblindMode"));

        Debug.Log("The current length of the volumeProfiles Dictionary is: " + volumeProfiles.Count);
    }

    private void Update()
    {
        SetProfile(PlayerPrefs.GetInt("ColorblindMode"));
    }

    /// <summary>
    /// This method sets the Volume profile of the Camera to the correct corresponding profile based on the chosen colorblind mode.
    /// </summary>
    /// <param name="currentColorBlindness"></param>
    private void SetProfile(int currentColorBlindness)
    {

        switch(PlayerPrefs.GetInt("ColorblindMode"))
        {
            case 0:
                camreaVolume.profile = volumeProfiles["Normal"];
                currentColorblindMode = Colorblindness.NORMAL;
                break;
            case 1:
                camreaVolume.profile = volumeProfiles["Achromatopsia"];
                currentColorblindMode = Colorblindness.ACHROMATOPSIA;
                break;
            case 2:
                camreaVolume.profile = volumeProfiles["Achromatomaly"];
                currentColorblindMode = Colorblindness.ACHROMATOMALY;
                break;
            case 3:
                camreaVolume.profile = volumeProfiles["Deuteranopia"];
                currentColorblindMode = Colorblindness.DEUTERANOPIA;
                break;
            case 4:
                camreaVolume.profile = volumeProfiles["Deuteranomaly"];
                currentColorblindMode = Colorblindness.DEUTERANOMALY;
                break;
            case 5:
                camreaVolume.profile = volumeProfiles["Protanopia"];
                currentColorblindMode = Colorblindness.PROTANOPIA;
                break;
            case 6:
                camreaVolume.profile = volumeProfiles["Protanomaly"];
                currentColorblindMode = Colorblindness.PROTANOMALY;
                break;
            case 7:
                camreaVolume.profile = volumeProfiles["Tritanopia"];
                currentColorblindMode = Colorblindness.TRITANOPIA;
                break;
            case 8:
                camreaVolume.profile = volumeProfiles["Tritanomaly"];
                currentColorblindMode = Colorblindness.TRITANOMALY;
                break;
        }
    }

    /// <summary>
    /// Loads all of the profiles in the Resources folder so that the script can reference them properly.
    /// </summary>
    private void LoadProfiles()
    {

        profileObjects = Resources.LoadAll(profilePath, typeof(VolumeProfile));

        volumeProfiles = new Dictionary<string, VolumeProfile>(profileObjects.Length)
            {

                {"Normal", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Normal")] },
                {"Achromatopsia", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Achromatopsia")] },
                {"Achromatomaly", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Achromatomaly")] },
                {"Deuteranopia", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Deuteranopia")] },
                {"Deuteranomaly", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Deuteranomaly")] },
                {"Protanopia", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Protanopia")] },
                {"Protanomaly", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Protanomaly")] },
                {"Tritanopia", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Tritanopia")] },
                {"Tritanomaly", (VolumeProfile)profileObjects[FindCorrectVolumeProfile("Tritanomaly")] }


            };
    }

    /// <summary>
    /// Returns the correct index location/number in the profileObjects array of whatever colorblind mode is selected.
    /// AKA, if Protanopia mode is chosen it will search that index's name and return the correct number associated with it.
    /// </summary>
    /// <param name="dictionaryValue"></param>
    /// <returns></returns>
    private int FindCorrectVolumeProfile(string dictionaryValue)
    {

        int index = 0;

        for (int i = 0; i < profileObjects.Length; i++)
        {
            index = i;

            switch (dictionaryValue)
            {
                case "Normal":
                    if (profileObjects[index].name.Contains("Normal"))
                    {
                        Debug.Log("The index of " + dictionaryValue + " is: " + index);
                        return index;
                    }
                    break;
                case "Achromatopsia":
                    if (profileObjects[index].name.Contains("Achromatopsia"))
                    {
                        return index;
                    }
                    break;
                case "Achromatomaly":
                    if (profileObjects[index].name.Contains("Achromatomaly"))
                    {
                        return index;
                    }
                    break;
                case "Deuteranopia":
                    if (profileObjects[index].name.Contains("Deuteranopia"))
                    {
                        return index;
                    }
                    break;
                case "Deuteranomaly":
                    if (profileObjects[index].name.Contains("Deuteranomaly"))
                    {
                        return index;
                    }
                    break;
                case "Protanopia":
                    if (profileObjects[index].name.Contains("Protanopia"))
                    {
                        Debug.Log("The index of " + dictionaryValue + " is: " + index);
                        return index;
                    }
                    break;
                case "Protanomaly":
                    if (profileObjects[index].name.Contains("Protanomaly"))
                    {
                        return index;
                    }
                    break;
                case "Tritanopia":
                    if (profileObjects[index].name.Contains("Tritanopia"))
                    {
                        return index;
                    }
                    break;
                case "Tritanomaly":
                    if (profileObjects[index].name.Contains("Tritanomaly"))
                    {
                        return index;
                    }
                    break;

            }

        }

        return index;

    }

}