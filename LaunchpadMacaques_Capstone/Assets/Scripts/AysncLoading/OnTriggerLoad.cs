/*
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* OnTriggerLoad.cs
* Uses the _ActivateNextLevel abstract class
* When player hits a trigger will load the next level.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnTriggerLoad : _ActivateNextLevel
{
    /// <summary>
    /// When player collides with this object, will load the next level
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }
}
