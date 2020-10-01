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
    [Header("Text Elements")]
    public TextMeshProUGUI totalCollectiblesText;
    public TextMeshProUGUI testDisplayText;
    public TextMeshProUGUI gravityText;

    [Header("Script References")]
    [SerializeField] [Tooltip("Thing")]
    private PauseManager pauseManager;
    [SerializeField] [Tooltip("Thing")]
    private Matt_PlayerMovement playerMovement;

    [Header("Variables")]
    [SerializeField] [Tooltip("Thing")]
    private int totalCollectibles, totalCollectedCollectibles;

    [SerializeField] [Tooltip("Thing")]
    private bool isActive = false, gravityIsCollected = false;

    public float effectDuration = 5f;


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

    /// <summary>
    /// Coroutine that acts as an effect timer for the gravity collectible.
    /// </summary>
    /// <returns></returns>
    public IEnumerator EffectTimer()
    {
        yield return new WaitForSeconds(effectDuration);

        isActive = false;
    }

    #region Getters

    /// <summary>
    /// Pause Manager Script getter.
    /// </summary>
    /// <returns></returns>
    public PauseManager GetPauseManagerReference()
    {
        return pauseManager;
    }

    /// <summary>
    /// PLayer Movement Script getter.
    /// </summary>
    /// <returns></returns>
    public Matt_PlayerMovement GetPlayerMovementReference()
    {
        return playerMovement;
    }

    /// <summary>
    /// Getter for the isActive bool.
    /// </summary>
    /// <returns></returns>
    public bool GetIsActive()
    {
        return isActive;
    }

    /// <summary>
    /// Getter for the gravityIsCollected bool.
    /// </summary>
    /// <returns></returns>
    public bool GetGravityIsCollected()
    {
        return gravityIsCollected;
    }

    /// <summary>
    /// Getter for the totalCollectibles int.
    /// </summary>
    /// <returns></returns>
    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }

    /// <summary>
    /// Getter for the totalCollectedCollectibles int.
    /// </summary>
    /// <returns></returns>
    public int GetTotalCollectedCollectibles()
    {
        return totalCollectedCollectibles;
    }

    #endregion


    #region Setters

    /// <summary>
    /// Setter for the isActive bool.
    /// </summary>
    /// <param name="true_or_false"></param>
    public void SetIsActive(bool true_or_false)
    {
        isActive = true_or_false;
    }

    /// <summary>
    /// Setter for the gravityIsCollected bool.
    /// </summary>
    /// <param name="true_or_false"></param>
    public void SetGravityIsCollected(bool true_or_false)
    {
        gravityIsCollected = true_or_false;
    }

    public void SetTotalCollectedCollectibles()
    {
        totalCollectedCollectibles++;
    }

    public void SetTotalCollectibles()
    {
        totalCollectibles--;
    }

    #endregion

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
