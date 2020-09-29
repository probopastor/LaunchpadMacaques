/* 
* Launchpad Macaques 
* CJ Green
* CollectibleController.cs 
* Handles the UI when collecting stuff.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityStandardAssets._2D;

public class CollectibleController : MonoBehaviour
{
    public int totalCollectibles;
    public TextMeshProUGUI totalCollectiblesText;
    private int totalSpheres;
    private PauseManager pauseManager;

    public TextMeshProUGUI testDisplayText;

    // Start is called before the first frame update
    void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>();
        totalCollectiblesText.SetText("Total Collectibles: 0 / " + FindObjectsOfType<CollectibleSphereScript>().Length);
        totalSpheres = FindObjectsOfType<CollectibleSphereScript>().Length;
    }

    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }

    public void AddToTotalCollectibles(int plusOne)
    {
        totalCollectibles += plusOne;
        totalCollectiblesText.SetText("Total Collectibles: " + totalCollectibles + " / " + totalSpheres);

        if(totalCollectibles >= totalSpheres)
        {
            pauseManager.SetGameWin();
        }
    }
}
