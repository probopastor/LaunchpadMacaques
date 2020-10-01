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

    public PauseManager pauseManager;
    public Matt_PlayerMovement playerMovement;


    public TextMeshProUGUI totalCollectiblesText;
    public TextMeshProUGUI testDisplayText;
    public TextMeshProUGUI gravityText;

    public int totalCollectibles;
    public int totalCollectedCollectibles;

    public float effectDuration = 5f;

    public bool isActive = false;
    public bool gravityIsCollected = false;


    private void Awake()
    {
        pauseManager = FindObjectOfType<PauseManager>();
        playerMovement = FindObjectOfType<Matt_PlayerMovement>();
        totalCollectibles = FindObjectsOfType<CollectibleSphereScript>().Length;
    }

    // Start is called before the first frame update
    void Start()
    {

        totalCollectiblesText.SetText("Total Sphere Count: " + totalCollectedCollectibles + " / " + totalCollectibles);

        //gravityText.SetText("Gravity: " + playerMovement.gravity);

        //totalCollectiblesText.SetText("Original Text");

    }

    private void Update()
    {
        gravityText.SetText("Gravity: " + playerMovement.gravity);

        Debug.Log("Is the gravity power-up is active: " + isActive);
        Debug.Log("The gravity mutiplier is currently: " + playerMovement.gravity);
        Debug.Log("Gravity Object is collected: " + gravityIsCollected);

        if(gravityIsCollected)
        {
            StartCoroutine(EffectTimer());
        }

    }

    public IEnumerator EffectTimer()
    {
        yield return new WaitForSeconds(effectDuration);

        isActive = false;
    }

    //public int GetTotalCollectibles()
    //{
    //    return totalCollectibles;
    //}

    //public void AddToTotalCollectibles(int plusOne)
    //{
    //    totalCollectibles += plusOne;
    //    totalCollectiblesText.SetText("Total Collectibles: " + totalCollectibles + " / " + totalSpheres);

    //    if(totalCollectibles >= totalSpheres)
    //    {
    //        pauseManager.SetGameWin();
    //    }
    //}
}
