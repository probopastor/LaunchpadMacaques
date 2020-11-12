using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerLoad : _ActivateNextLevel
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }
}
