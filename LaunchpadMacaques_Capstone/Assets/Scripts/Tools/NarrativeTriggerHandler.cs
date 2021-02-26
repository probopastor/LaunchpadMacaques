/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * NarrativeTriggerHandler.cs
 * This script creates and controls the narrative triggers that can be placed throughout a scene to play audio and display text to accompany it
 * It allows for several type of triggers: 
 * Area: Triggers when a specified GameObject enters a specified area
 * Random: Waits for a random period within a specified amount of time and then pick one of any of the Random type triggers to activate
 * On Event: Triggers that occur when a specific event occurs in another script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using FMODUnity;

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
    
    //Editor variables
    [SerializeField]
    private bool[] triggerSubFoldout;
    [SerializeField]
    private string[] triggerNames;

    //Handler Variables
    [SerializeField, Tooltip("The lower end of the interval that a random trigger can be called")]
    private float randomIntervalMin;
    [SerializeField, Tooltip("The upper end of the interval that a random trigger can be called")]
    private float randomIntervalMax;
    [SerializeField, Tooltip("The time (in seconds) that the player must be falling in order for the \"Player Hitting Ground\" event to trigger")]
    private float fallTime;

    //UI variables
    [SerializeField]
    private TMP_Text dialogue;
    [SerializeField]
    private GameObject canvas;


    [System.Serializable]
    public class Trigger
    {
        [Tooltip("The type of activation used for this Trigger")]
        public TriggerType type;
        [SerializeField, Tooltip("The text to display on Trigger activation")]
        public string textToDisplay;
        [Tooltip("The time text should be displayed")]
        public int textDisplayTime;
        [Tooltip("The audio to play on Trigger activation"), FMODUnity.EventRef]
        public string audioToPlay;
        [Tooltip("The audio source the audio will play from")]
        public AudioSource audioSource;
        [Tooltip("Whether this Trigger can be activated multiple times (true) or only once (false)")]
        public bool repeatable;

        [Tooltip("Whether a camera movement should be associated with this trigger activation or not")]
        public bool haveCameraMovement;
        [Tooltip("The time (in seconds) the camera will be at the destination")]
        public float cameraTime;
        [Tooltip("The point the camera will move to")]
        public GameObject cameraPoint;
        [Tooltip("The GameObject for the camera to look at")]
        public GameObject cameraTarget;

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
        onPlayerHitGround += PlayerHitGroundActivation;
        GameEventManager.StartListening("onPlayerHitGround", onPlayerHitGround);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += TimeInLevelCountStart;
    }

    private void OnDisable()
    {
        GameEventManager.StopListening("onPlayerHitGround", onPlayerHitGround);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= TimeInLevelCountStart;
        onPlayerHitGround -= PlayerHitGroundActivation;
    }

    private void Awake()
    {
        dialogue = GetComponentInChildren<TMP_Text>();
        canvas = dialogue.GetComponentInParent<Canvas>().gameObject;
        canvas.SetActive(false);
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

        //Always be checking for trigger activations
        while (true)
        {
            //If an area trigger should be activated, activate it
            for (int i = 0; i < triggers.Length; i++)
            {
                if (triggers[i].type == TriggerType.Area && AreaCheck(triggers[i]))
                {
                    ActivateTrigger(triggers[i]);
                }
            }

            //Pick one of the triggers labeled random, at random, and trigger it when the count reaches target number
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
                    yield return null;
                    continue;
                }

                int triggerToActivate = Random.Range(0, (int)randomTriggers.Count);

                ActivateTrigger(randomTriggers[triggerToActivate]);
            }


            yield return null;
        }
    }

    /// <summary>artL
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

        FMOD.Studio.EventDescription desc;
        //If the trigger has an audio source assigned, play the associated sound (if it has one)
        if (FMODUnity.RuntimeManager.StudioSystem.getEvent(trigger.audioToPlay, out desc) == FMOD.RESULT.OK)
        {
            FMOD.Studio.EventInstance playAudio = FMODUnity.RuntimeManager.CreateInstance(trigger.audioToPlay);
            FMODUnity.RuntimeManager.GetEventDescription(trigger.audioToPlay).getLength(out trigger.textDisplayTime);
            playAudio.start();
        }

        if (trigger.textToDisplay != "")
        {
            if (FindObjectOfType<InformationPost>())
            {
                if (!FindObjectOfType<InformationPost>().GetTutorialCanvas())
                {
                    StartCoroutine(RunDialogue(trigger));
                }
            }

            else
            {
                StartCoroutine(RunDialogue(trigger));
            }
  
        }

        if(trigger.haveCameraMovement && trigger.cameraPoint != null && trigger.cameraTarget != null)
        {
            StartCoroutine(PanCamera(trigger));
        }

        //Text appearing functionality
    }

    /// <summary>
    /// Performs a check on an object in sight to see if it matches up with any triggers, if so, run that trigger
    /// </summary>
    /// <param name="obj">The object in sight to check</param>
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

    /// <summary>
    /// Stop any running dialogue from appearing
    /// </summary>
    public void TurnOffDialouge()
    {
        dialogue.text = "";
        canvas.SetActive(false);
    }

    /// <summary>
    /// Run the dialogue on a given trigger
    /// </summary>
    /// <param name="trigger">The trigger to run the dialogue on</param>
    /// <returns></returns>
    IEnumerator RunDialogue(Trigger trigger)
    {
    
        trigger.isRunning = true;

        //Turn on canvas
        canvas.SetActive(true);

        //Run text effects and apply them to the dialogue window
        TextEffectHandler.instance.RunText(dialogue, trigger.textToDisplay);

        //Wait for the time given by the trigger
        yield return new WaitForSecondsRealtime(trigger.textDisplayTime);

        TextEffectHandler.instance.StopText();

        //Turn off text
        if (canvas.gameObject.activeSelf)
        {
            dialogue.text = "";
        }




        canvas.gameObject.SetActive(false);

        trigger.isRunning = false;
    }

    /// <summary>
    /// For area triggers, check the area defined by the trigger for any GameObject that is in the area and matches the tags defined by the trigger
    /// </summary>
    /// <param name="triggerToCheck">The trigger to check</param>
    /// <returns>true if the trigger being checked has an object in the specified area that also matches the specified tag</returns>
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

    /// <summary>
    /// Checks to see if the count has reached the current target number
    /// </summary>
    /// <param name="currentCount">The current count as a float (in seconds)</param>
    /// <param name="target">The target number the count is supposed to reach as a float (in seconds)</param>
    /// <returns>True if the count has reached the target number, false if not</returns>
    private bool RandomCheck(ref float currentCount, float target)
    {
        //Target Reached
        if(currentCount >= target)
        {
            return true;
        }
        //Target not reached, add to the counter
        else
        {
            currentCount += Time.deltaTime;
            return false;
        }
    }


    bool isPanning = false;
    private IEnumerator PanCamera(Trigger triggerWithCamInfo)
    {
        isPanning = true;

        Time.timeScale = 0;

        CinemachineVirtualCamera camera = triggerWithCamInfo.cameraPoint.GetComponent<CinemachineVirtualCamera>();
        int originalPriorityValue = camera.m_Priority;
        camera.m_Priority = 420;
        yield return new WaitForSecondsRealtime(triggerWithCamInfo.cameraTime);
        camera.m_Priority = originalPriorityValue;

        Time.timeScale = 1;

        isPanning = false;
    }

    #region PlayerHitGround Event
    /// <summary>
    /// The function that will be called when the player hits the ground, picks one random trigger out of all the PlayerHitGround Triggers and activates it
    /// </summary>
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
    #endregion

    #region TimeInLevel Event
    float timeInLevel = 0;
    Coroutine currentCount;
    /// <summary>
    /// Starts counting on scene load and restarts it on changing scenes, used for triggers that activate a set amount of time after the player is in the scene
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    private void TimeInLevelCountStart(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        timeInLevel = 0;
        if(currentCount != null)
            StopCoroutine(currentCount);
        currentCount = StartCoroutine(TimeInLevelCount());
    }

    /// <summary>
    /// Counting and checking for triggers to activate
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Getter function for the name of the trigger based on the index
    /// </summary>
    /// <param name="index">the index of the trigger you want the name of</param>
    /// <returns>the name of the trigger</returns>
    public string GetTriggerName(int index)
    {
        return triggerNames[index];
    }

    /// <summary>
    /// Getter function for the fall time (the set amount of time the player must be falling for a PlayerHitGround trigger to activate
    /// </summary>
    /// <returns>The fall time in seconds</returns>
    public float GetFallTime()
    {
        return fallTime;
    }
    #endregion

}
