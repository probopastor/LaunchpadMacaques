/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (DistanceLoading.CS) 
* (Implaments the _ActivateNextLevel Abstract class, will load the level when the player is within a certain distance from this object) 
*/
using UnityEngine;

public class DistanceLoading :_ActivateNextLevel
{
    [SerializeField, Tooltip("The Distance the player needs to be from this object to Activate Level Loading")] float activationDistance = 10;
    private GameObject player;
    private Rigidbody playerRB;

    private void Start()
    {
        player = FindObjectOfType<Matt_PlayerMovement>().gameObject;
        playerRB = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckDistance();
    }


    /// <summary>
    /// Will Chheck the player distance 
    /// When the player is close enough will start to load in next level
    /// </summary>
    private void CheckDistance()
    {
        if (playerRB.velocity.magnitude != 0)
        {
            if (Vector3.Distance(this.gameObject.transform.position, player.transform.position) < activationDistance)
            {
                LoadNextLevel();
            }
        }

    }
}
