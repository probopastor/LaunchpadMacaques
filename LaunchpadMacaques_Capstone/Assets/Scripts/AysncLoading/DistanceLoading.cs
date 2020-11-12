using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceLoading :_ActivateNextLevel
{
    [SerializeField] float activationDistance = 10;
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

    private void CheckDistance()
    {
        if (playerRB.velocity.magnitude != 0)
        {
            Debug.Log("1");
            if (Vector3.Distance(this.gameObject.transform.position, player.transform.position) < activationDistance)
            {
                Debug.Log("J");
                LoadNextLevel();
            }
        }

    }
}
