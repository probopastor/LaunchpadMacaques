/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi/Adrian) 
* (HandleSaving.cs) 
* (Handles the Saving in each scene/loading) 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HandleSaving : MonoBehaviour
{
    [SerializeField, Tooltip("A array of levels that the player can complete")] Level[] levels;
    [SerializeField] Ability[] abilities;
    public static HandleSaving instance;
    Save_System saveSystem;

    private Vector3 playerPos;
    private string levelName;

    private void Awake()
    {
        Singleton();
        SetLevels();
    }

    /// <summary>
    /// Will handle the creation of a static instance of this object that is shared across scenes
    /// </summary>
    private void Singleton()
    {
        this.gameObject.transform.parent = null;
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
        }

        saveSystem = FindObjectOfType<Save_System>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SavePlayer();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadPlayer();
        }

    }


    /// <summary>
    /// Saves the game
    /// </summary>
    public void SavePlayer()
    {
        saveSystem.SavePlayer(FindObjectOfType<Matt_PlayerMovement>().gameObject, levels);
    }

    /// <summary>
    /// Will only save the level information
    /// </summary>
    public void JustSaveLevels()
    {
        saveSystem.SavePlayer(playerPos, levelName, levels);
    }

    /// <summary>
    /// Will load the player and place them back in the correct scene/position
    /// </summary>
    public void LoadPlayer()
    {
        PlayerDataNew data = saveSystem.LoadPlayer();

        StopAllCoroutines();
        StartCoroutine(LoadScene(data));

    }

    /// <summary>
    /// If a save file is found
    /// Will set the levels array to be equal to the save files array of levels
    /// </summary>
    private void SetLevels()
    {
        if (saveSystem.CanFindFile("PlayerData"))
        {
            PlayerDataNew data = saveSystem.LoadPlayer();

            foreach (Level l in data.levels)
            {
                foreach (Level l2 in levels)
                {
                    if (l.levelName == l2.levelName)
                    {
                        l2.completed = l.completed;
                    }
                }
            }

            playerPos.x = data.position[0];
            playerPos.y = data.position[1];
            playerPos.z = data.position[2];


            levelName = data.level;
        }


    }


    /// <summary>
    /// Will handle the loading of a level
    /// And the placement of the player in that level
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    IEnumerator LoadScene(PlayerDataNew data)
    {

        AsyncOperation async = SceneManager.LoadSceneAsync(data.level);

        while (!async.isDone)
        {
            yield return null;
        }

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        playerPos.x = data.position[0];
        playerPos.y = data.position[1];
        playerPos.z = data.position[2];


        levelName = data.level;

        FindObjectOfType<Matt_PlayerMovement>().gameObject.transform.position = position;
    }


    /// <summary>
    /// Will set the level to be completed in the levels array
    /// Will save that to the save file
    /// </summary>
    public void LevelCompleted()
    {
        string name = SceneManager.GetActiveScene().name;

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].levelName == name)
            {
                levels[i].completed = 1;
            }
        }

        JustSaveLevels();
    }

    /// <summary>
    /// Returns whether the given level has been completed
    /// </summary>
    /// <param name="levelName"></param>
    /// <returns></returns>
    public bool IsLevelComplete(string levelName)
    {
        foreach (Level l in levels)
        {
            if (l.levelName == levelName)
            {
                if (l.completed == 1)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    private bool AreLevelsComplete(string[] levelNames)
    {
        foreach(string str in levelNames)
        {
            bool found = false;

            foreach(Level l in levels)
            {
                if(str == l.levelName)
                {
                    if(l.completed == 1)
                    {
                        found = true;
                    }

                    else
                    {
                        return false;
                    }
                }
            }

            if(found == false)
            {
                return false;
            }
        }

        return true;
    }


    public bool UnlockedAbility(Ability.AbilityType ability)
    {
        foreach(Ability a in abilities)
        {
            if(a.thisAbility == ability)
            {
                return AreLevelsComplete(a.levels);
            }
        }

        return false;
    }
}


/// <summary>
/// A public class that stores information about one level
/// </summary>
[System.Serializable]
public class Level
{
    public string levelName;
    public int completed = 0;
}


[System.Serializable]
public class Ability 
{
    public enum AbilityType { Dash, Batman}

    public AbilityType thisAbility;
    public string[] levels;

}


