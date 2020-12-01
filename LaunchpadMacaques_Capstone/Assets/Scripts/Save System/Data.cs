using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public static class Data
{
    [System.Serializable]
    public class LevelData
    {
        int currentSceneIndex;
        float[] playerPos;

        public LevelData()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;



            Vector3 playerPosVector = GameObject.Find("Player").transform.position;
            playerPos = new float[] { playerPosVector.x, playerPosVector.y, playerPosVector.z };




        }
    }

    [System.Serializable]
    public class CheckpointData
    {
        
        public CheckpointData()
        {

        }
    }
}
