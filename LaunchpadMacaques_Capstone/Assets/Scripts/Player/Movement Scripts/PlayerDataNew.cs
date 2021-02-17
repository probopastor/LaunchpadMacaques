using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerDataNew 
{
    public float[] position;
    public int level;
    

    public PlayerDataNew(Matt_PlayerMovement player)
    {

        level = player.level;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        

        
    }
}
