using System.Collections.Generic;
using UnityEngine;

public class FallingObjectManager : MonoBehaviour
{
    public List<DistanceFallingObjects> distanceObjects  = new List<DistanceFallingObjects>();
    public List<CheckPointFallingObjects> checkPointObjects = new List<CheckPointFallingObjects>();
    public List<StartOfLevelFallingObjects> startOfLevelObjects = new List<StartOfLevelFallingObjects>();

    [SerializeField] List<string> deafultDeathTags;
    // Start is called before the first frame update
    private void Awake()
    {
        
        foreach(DistanceFallingObjects d in distanceObjects)
        {
            if(d.obj != null)
            {
                d.deathTags.AddRange(deafultDeathTags);
                d.obj.AddComponent<FallingObjectDistance>();
                d.obj.GetComponent<FallingObjectDistance>().SetVariables(d.fallSpeed, d.delayBeforeFalling, d.deathTags, d.shakeSpeed, d.shakeAmmount, d.playerCanStandOn, d.respawnOnDeath);
                d.obj.GetComponent<FallingObjectDistance>().SetSpecificVariables(d.distanceToStartFalling);
            }
  
        }

        foreach(StartOfLevelFallingObjects s in startOfLevelObjects)
        {
            if(s.obj != null)
            {
                s.deathTags.AddRange(deafultDeathTags);
                s.obj.AddComponent<StartOfLevelFallingObject>();
                s.obj.GetComponent<StartOfLevelFallingObject>().SetVariables(s.fallSpeed, s.delayBeforeFalling, s.deathTags, s.shakeSpeed, s.shakeAmmount, s.playerCanStandOn, s.respawnOnDeath);
            }

        }

        foreach(CheckPointFallingObjects c in checkPointObjects)
        {
            if(c.obj != null && c.checkPointObjet != null)
            {
                c.deathTags.AddRange(deafultDeathTags);
                c.obj.AddComponent<CheckPointFallingObject>();
                c.obj.GetComponent<CheckPointFallingObject>().SetVariables(c.fallSpeed, c.delayBeforeFalling, c.deathTags, c.shakeSpeed, c.shakeAmmount, c.playerCanStandOn, c.respawnOnDeath);

                if (!c.checkPointObjet.GetComponent<CheckPointMakeObjectFall>())
                {
                    c.checkPointObjet.AddComponent<CheckPointMakeObjectFall>();
                }


                if (c.checkPointObjet.GetComponent<CheckPointMakeObjectFall>() && c.obj.GetComponent<CheckPointFallingObject>())
                {
                    c.checkPointObjet.GetComponent<CheckPointMakeObjectFall>().AddObject(c.obj.GetComponent<CheckPointFallingObject>());
                }
            }

            

        }
    }

    void Reset()
    {
        distanceObjects = new List<DistanceFallingObjects>()
     {
           new DistanceFallingObjects()
     };

        checkPointObjects = new List<CheckPointFallingObjects>()
        {
            new CheckPointFallingObjects()
         };

        startOfLevelObjects = new List<StartOfLevelFallingObjects>()
        {
            new StartOfLevelFallingObjects()
        };
    }
}

[System.Serializable]
public class DistanceFallingObjects: FallingObjects
{


    [Header("Falling Settings")]
    [Tooltip("The Distance the player has to be within for this object to begin falling")] public float distanceToStartFalling = 20;
  
}

[System.Serializable]
public class CheckPointFallingObjects: FallingObjects
{
    public GameObject checkPointObjet;
}

[System.Serializable]
public class StartOfLevelFallingObjects: FallingObjects
{

}

[System.Serializable]
public abstract class FallingObjects
{
    public GameObject obj;
    [Tooltip("The speed at which this object falls")] public float fallSpeed = 50;
    [Tooltip("The Delay time before this object actually begins to fall")] public float delayBeforeFalling = 5;

    [Header("Death Settings")]
    [Tooltip("The Tags that will kill this object")] public List<string> deathTags = new List<string>();

    [Header("Shake Settings")]
    public float shakeSpeed = 40;
    public float shakeAmmount = 5;

    [Header("Other Settings")]
    public bool playerCanStandOn = false;
    public bool respawnOnDeath = false;

}
