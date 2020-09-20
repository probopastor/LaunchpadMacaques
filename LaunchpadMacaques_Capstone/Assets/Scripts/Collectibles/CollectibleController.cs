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

public class CollectibleController : MonoBehaviour
{
    public int totalCollectibles;
    public TextMeshProUGUI totalCollectiblesText;

    // Start is called before the first frame update
    void Start()
    {
        totalCollectiblesText.SetText("Total Collectibles: 0 / " + FindObjectsOfType<CollectibleSphereScript>().Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }
}
