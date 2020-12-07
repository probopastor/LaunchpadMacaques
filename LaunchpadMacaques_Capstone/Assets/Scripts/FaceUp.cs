/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof, William Nomikos 
* FaceUp.CS 
* Rotates object to always face up.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUp : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        MakeObjectFaceUp();
    }

    /// <summary>
    /// Rotates particles to always face up. 
    /// </summary>
    private void MakeObjectFaceUp()
    {
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }
}
