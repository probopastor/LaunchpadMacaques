/* 
* Launchpad Macaques - Neon Oblivion 
* William Nomikos, Jamey Colleen
* (CheckCubeSpawn.CS)
* Handles determining the tags for cube respawn locations.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCubeSpawn : MonoBehaviour
{
    private CubeRespawn cubeRespawnRef;
    private int cubeCount = 0;

    private bool startSetup = false;

    // Start is called before the first frame update
    void Awake()
    {
        startSetup = true;
        cubeRespawnRef = FindObjectOfType<CubeRespawn>();
    }

    private void OnCollisionStay(Collision other)
    {
        if (startSetup)
        {
            if (other.gameObject.CompareTag("PlayerCube"))
            {
                if (cubeCount <= 0)
                {
                    cubeRespawnRef.SetCubeHolderPickupTag(false, this.gameObject);
                }

                cubeCount++;
            }

            startSetup = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerCube") && !startSetup)
        {
            if (cubeCount <= 0)
            {
                cubeRespawnRef.SetCubeHolderPickupTag(false, this.gameObject);
            }

            cubeCount++;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerCube"))
        {
            cubeCount--;
        }

        if (cubeCount < 0)
        {
            cubeCount = 0;
        }

        if (cubeCount <= 0)
        {
            cubeRespawnRef.SetCubeHolderPickupTag(true, this.gameObject);
        }
    }
}
