/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * NarrativeTriggerHandler.cs
 * This script controls the narrative triggers that cna be placed throughout a scene to play audio and trigger text appearing
 * It allows for several type of triggers such as: 
 * Area: Triggers when a specified GameObject enters a specified area
 * Random: Waits for a random period within a specified amount of time and then pick one of any of the Random type triggers to activate
 * WIP:::: On Event: Triggers that occur when a specific event occurs in another script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(GameEventManager))]
public class NarrativeTriggerHandler : MonoBehaviour
{
    public enum TriggerType { Area, Random, OnEvent };

    #region Events
    public enum EventType { PlayerHitGround , TimeInLevel, LookAtObject}

    UnityAction onPlayerHitGround;
    #endregion

    [SerializeField, Tooltip("Contains all triggers in the scene. Each array element is a separate trigger with it's own type, " +
        "as well as its own text and audio outputs")]
    public Trigger[] triggers;
    
    [SerializeField]
    private bool[] triggerSubFoldout;
    [SerializeField]
    private string[] triggerNames;

    [SerializeField, Tooltip("The lower end of the interval that a random trigger can be called")]
    private float randomIntervalMin;
    [SerializeField, Tooltip("The upper end of the interval that a random trigger can be called")]
    private float randomIntervalMax;
    [SerializeField, Tooltip("The time (in seconds) that the player must be falling in order for the \"Player Hitting Ground\" event to trigger")]
    private float fallTime;
    [SerializeField]
    private TMP_Text dialogue;


    [System.Serializable]
    public class Trigger
    {
        [Tooltip("The type of activation used for this Trigger")]
        public TriggerType type;
        [SerializeField, Tooltip("The text to display on Trigger activation")]
        public string textToDisplay;
        [Tooltip("The time text should be displayed")]
        public float textDisplayTime;
        [Tooltip("The audio to play on Trigger activation")]
        public AudioClip audioToPlay;
        [Tooltip("The audio source the audio will play from")]
        public AudioSource audioSource;
        [Tooltip("Whether this Trigger can be activated multiple times (true) or only once (false)")]
        public bool repeatable;
        public bool hasRan = false;
        public bool isRunning = false;

        //Only appear on TriggerType.Area
        [Tooltip("The tag of objects that will activate the Trigger when it enters the trigger area")]
        public string triggeringTag;
        public GameObject areaTrigger;
        [Tooltip("The center of the trigger zone")]
        public Vector3 areaCenter;
        [Tooltip("The size of the trigger zone relative to the center")]
        public Vector3 boxSize;

        //Only appear on TriggerType.OnEvent
        [Tooltip("The type of event for this trigger")]
        public EventType eventType;
        [Tooltip("The amount of time (in seconds) the player has to be in the level for this to trigger")]
        public float timeInLevelBeforeTrigger = 0;
        [Tooltip("The array of objects that, when looked at, will activate this trigger")]
        public GameObject[] triggeringObjects;
        
    }

    private void OnEnable()
    {
        GameEventManager.StartListening("onPlayerHitGround", onPlayerHitGround);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += TimeInLevelCountStart;
    }

    private void OnDisable()
    {
        GameEventManager.StopListening("onPlayerHitGround", onPlayerHitGround);
    }

    private void Awake()
    {
        dialogue = GetComponentInChildren<TMP_Text>();
        onPlayerHitGround += PlayerHitGroundActivation;
    }

    private void Start()
    {
        StartCoroutine(CheckForTriggerActivations());
    }

    /// <summary>
    /// Checks every frame for specified Triggers (Area, Random) and then activates them if the condition is met
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckForTriggerActivations()
    {
        float randomCount = 0;
        float randomTarget = Random.Range(randomIntervalMin, randomIntervalMax);

        while (true)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                if (triggers[i].type == TriggerType.Area && AreaCheck(triggers[i]))
                {
                    ActivateTrigger(triggers[i]);
                }
            }

            if(RandomCheck(ref randomCount, randomTarget))
            {

                randomCount = 0;
                randomTarget = Random.Range(randomIntervalMin, randomIntervalMax);

                //Find random triggers in triggers array and put them into their own list
                List<Trigger> randomTriggers = new List<Trigger>();
                foreach (Trigger x in triggers)
                {
                    if (x.type == TriggerType.Random)
                    {
                        randomTriggers.Add(x);
                    }
                }

                //No random triggers found
                if(randomTriggers.Count == 0)
                {
                    continue;
                }

                int triggerToActivate = Random.Range(0, (int)randomTriggers.Count);

                ActivateTrigger(randomTriggers[triggerToActivate]);
            }


            yield return null;
        }
    }

    /// <summary>
    /// Activates trigger at given index
    /// </summary>
    /// <param name="trigger">The index of the trigger to activate</param>
    public void ActivateTrigger(Trigger trigger)
    {

        if (trigger.repeatable == false && trigger.hasRan == true)
        {
            return;
        }

        trigger.hasRan = true;

        if (trigger.audioSource != null)
        {
            trigger.audioSource.clip = trigger.audioToPlay;
            trigger.audioSource.Play();
        }

        if (trigger.textToDisplay != "")
        {
            StartCoroutine(RunDialogue(trigger));
        }

        //Text appearing functionality
    }

    public void ObjectInSightCheck(GameObject obj)
    {
        if (obj == null)
            return;

        Trigger currentTrigger;
        for(int i = 0; i < triggers.Length; i++)
        {
            currentTrigger = triggers[i];
            if(currentTrigger.type == TriggerType.OnEvent && currentTrigger.eventType == EventType.LookAtObject && currentTrigger.triggeringObjects.Length > 0)
            {
                for(int j = 0; j < currentTrigger.triggeringObjects.Length; j++)
                {
                    if(currentTrigger.triggeringObjects[j] == obj && currentTrigger.isRunning == false)
                    {
                        ActivateTrigger(currentTrigger);
                    }
                }
            }
        }
    }

    IEnumerator RunDialogue(Trigger trigger)
    {
        trigger.isRunning = true;
        dialogue.text = trigger.textToDisplay;
        yield return new WaitForSeconds(trigger.textDisplayTime);

        dialogue.CrossFadeAlpha(0, 2f, false);

        dialogue.text = "";

        dialogue.CrossFadeAlpha(1, 0f, false);
        trigger.isRunning = false;
    }

    private bool AreaCheck(Trigger triggerToCheck)
    {
        Collider[] hit = Physics.OverlapBox(triggerToCheck.areaCenter, triggerToCheck.boxSize / 2, Quaternion.identity);

        foreach(Collider x in hit)
        {
            if (x.tag == triggerToCheck.triggeringTag)
                return true;
        }

        return false;
    }

    private bool RandomCheck(ref float currentCount, float target)
    {
        //Target Reached
        if(currentCount >= target)
        {
            return true;
        }
        else
        {
            currentCount += Time.deltaTime;
            return false;
        }
    }

    private void PlayerHitGroundActivation()
    {
        //Filter triggers to locate only those that are OnEvent Triggers and are the type of event that activates when the player hits the ground
        List<Trigger> filteredList = new List<Trigger>();
        Trigger currentTrigger;
        for(int i = 0; i < triggers.Length; i++)
        {
            currentTrigger = triggers[i];
            if(currentTrigger.type == TriggerType.OnEvent && currentTrigger.eventType == EventType.PlayerHitGround)
            {
                filteredList.Add(currentTrigger);
            }
        }

        //No PlayerHitGround Triggers found :(
        if(filteredList.Count <= 0)
            return;

        //Pick a random one out of the list to activate
        currentTrigger = filteredList[Random.Range(0, filteredList.Count)];
        ActivateTrigger(currentTrigger);

    }

    #region TimeInLevelEvent
    float timeInLevel = 0;
    private void TimeInLevelCountStart(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        timeInLevel = 0;
        StartCoroutine(TimeInLevelCount());
    }

    private IEnumerator TimeInLevelCount()
    {
        while (true)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                if (CheckTimeSinceLevelRequirement(triggers[i]))
                {

                    ActivateTrigger(triggers[i]);
                }
            }
            timeInLevel += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Checks if the current total time in the level is greater than 
    /// </summary>
    /// <param name="triggerToCheck">The trigger to check</param>
    /// <returns>boolean true if the requirements are met, false if not</returns>
    private bool CheckTimeSinceLevelRequirement(Trigger triggerToCheck)
    {
        return triggerToCheck.type == TriggerType.OnEvent
             && triggerToCheck.eventType == EventType.TimeInLevel
             && timeInLevel >= triggerToCheck.timeInLevelBeforeTrigger
             && !triggerToCheck.hasRan;
    }
    #endregion

    #region Getters/Setters
    public string GetTriggerName(int index)
    {
        return triggerNames[index];
    }

    public float GetFallTime()
    {
        return fallTime;
    }
    #endregion

}
