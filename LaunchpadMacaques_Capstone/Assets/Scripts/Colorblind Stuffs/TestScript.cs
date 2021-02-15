/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TestScript : MonoBehaviour
{
    public enum Colorblindness { NORMAL = 0, PROTANOPIA = 1, PROTANOMALY = 2, DEUTERANOPIA = 3, DEUTERANOMALY = 4, TRITANOPIA = 5, TRITANOMALY = 6, ACHROMATOPSIA = 7, ACHROMATOMALY = 8 };


    public Colorblindness currentColorblindMode = Colorblindness.NORMAL;

    [SerializeField]
    private Volume camreaVolume;

    //[SerializeField]
    //private VolumeProfile[] volumeProfiles;

    private string profilePath = "Volume Profiles";

    private Object[] profileObjects;


    private Dictionary<string, VolumeProfile> volumeProfiles;


    private void Awake()
    {

        camreaVolume = FindObjectOfType<Camera>().GetComponent<Volume>();

        LoadProfiles();
        SetProfile(currentColorblindMode);

        if (currentColorblindMode == Colorblindness.NORMAL)
        {
            Debug.Log("Where the fuck is my pizza bruh? " + currentColorblindMode.ToString());
        }

        Debug.Log("The current length of the volumeProfiles Dictionary is: " + volumeProfiles.Count);

        //Object[] profileObjects = Resources.LoadAll(profilePath, typeof(VolumeProfile));

        //volumeProfiles = new VolumeProfile[profileObjects.Length];


    }

    private void Update()
    {
        SetProfile(currentColorblindMode);
    }

    private void SetProfile(Colorblindness currentColorBlindness)
    {
        if (currentColorblindMode == Colorblindness.NORMAL)
        {
            camreaVolume.profile = volumeProfiles["Normal"];
            Debug.Log("The current Colorblindness mode is: " + currentColorblindMode.ToString());
        }
        else if (currentColorblindMode == Colorblindness.PROTANOPIA)
        {
            camreaVolume.profile = volumeProfiles["Protanopia"];
            Debug.Log("The current Colorblindness mode is: " + currentColorblindMode.ToString());
        }
    }

    private void LoadProfiles()
    {
        if (profileObjects == null && volumeProfiles == null)
        {

            profileObjects = Resources.LoadAll(profilePath, typeof(VolumeProfile));

            volumeProfiles = new Dictionary<string, VolumeProfile>(profileObjects.Length)
            {

                {"Normal", (VolumeProfile)profileObjects[0] },
                {"Protanopia", (VolumeProfile)profileObjects[1] }

            };

        }
        else if(profileObjects == null)
        {
            profileObjects = Resources.LoadAll(profilePath, typeof(VolumeProfile));
        }
        else if(volumeProfiles == null)
        {
            volumeProfiles = new Dictionary<string, VolumeProfile>(profileObjects.Length)
            {

                {"Normal", (VolumeProfile)profileObjects[0] },
                {"Protanopia", (VolumeProfile)profileObjects[1] }

            };
        }
    }
}