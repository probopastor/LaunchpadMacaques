﻿/*
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
    private float randomIntervalMin = 0;
    [SerializeField, Tooltip("The upper end of the interval that a random trigger can be called")]
    private float randomIntervalMax = 0;
    [SerializeField, Tooltip("The time (in seconds) that the player must be falling in order for the \"Player Hitting Ground\" event to trigger")]
    private float fallTime = 0;

    //UI variables
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private TMP_Text dialogueText;
    [SerializeField]
    private GameObject[] nameplate;
    [SerializeField]
    private TMP_Text[] nameplateText;
    [SerializeField]
    private GameObject clickToContinue;
    [SerializeField]
    private GameObject viewLog;

    private bool mouseOverButton = false;

    //Dialogue Options
    [SerializeField, Range(0, 1), Tooltip("The percentage of opacity the nameplates will be when not talking")]
    private float nameplateFadedOpacity = 0.5f;
    [SerializeField, Tooltip("The time (in seconds) nameplates will take to change opacity")]
    private float nameplateTransitionTime = 0.25f;
    [SerializeField, Tooltip("The time (in seconds) between the flashing of the \"Click to continue\" prompt")]
    float flashInterval = 0.5f;


    [System.Serializable]
    public class Trigger
    {
        [Tooltip("The type of activation used for this Trigger")]
        public TriggerType type;

        //Text
        [SerializeField]
        public Dialogue dialogue;

        [Tooltip("Whether this Trigger can be activated multiple times (true) or only once (false)")]
        public bool repeatable;

        [Tooltip("Whether a camera movement should be associated with this trigger activation or not")]
        public bool hasCameraMovement;
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
        dialogueText = transform.Find("DialogueCanvas/Background/TextContainer/DialogueText").GetComponent<TMP_Text>();
        canvas = dialogueText.GetComponentInParent<Canvas>().gameObject;

        nameplate = new GameObject[2];
        nameplateText = new TMP_Text[2];
        //Nameplate 1
        nameplate[0] = transform.Find("DialogueCanvas/Background/Character1Nameplate").gameObject;
        nameplateText[0] = nameplate[0].GetComponentInChildren<TMP_Text>();
        nameplate[0].SetActive(false);
        //Nameplate 2
        nameplate[1] = transform.Find("DialogueCanvas/Background/Character2Nameplate").gameObject;
        nameplateText[1] = nameplate[1].GetComponentInChildren<TMP_Text>();
        nameplate[1].SetActive(false);

        clickToContinue = transform.Find("DialogueCanvas/Background/ClickToContinue").gameObject;
        clickToContinue.SetActive(false);

        viewLog = transform.Find("DialogueCanvas/Background/ViewLog").gameObject;

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

        if (trigger.dialogue != null)
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

        if(trigger.hasCameraMovement && trigger.cameraPoint != null && trigger.cameraTarget != null)
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
        dialogueText.text = "";
        canvas.SetActive(false);
    }

    /// <summary>
    /// Run the dialogue on a given trigger
    /// </summary>
    /// <param name="trigger">The trigger to run the dialogue on</param>
    /// <returns></returns>
    IEnumerator RunDialogue(Trigger trigger)
    {
        if (trigger.dialogue == null)
            yield break;

        trigger.isRunning = true;

        //Turn on canvas
        canvas.SetActive(true);

        //Get ready to start pushing conversation to the log
        Log.instance.StartNewConversation();

        //Get Every line in the dialogue
        Dialogue.Line currentLine;
        int lastNameplateUsed = -1;
        while ((currentLine = trigger.dialogue.NextLine()) != null)
        {
            //If break, turn nameplates off
            if(currentLine.GetLineType() == Dialogue.Line.Type.Break)
            {
                nameplate[0].SetActive(false);
                nameplate[1].SetActive(false);
                lastNameplateUsed = -1;
                continue;
            }
            //If character line, push character line to log
            else if(currentLine.GetLineType() == Dialogue.Line.Type.CharacterLine)
            {
                Log.instance.PushToLog(currentLine);
            }
            //If narration line, push text to log as action text
            else if(currentLine.GetLineType() == Dialogue.Line.Type.NarrationLine)
            {
                Log.instance.PushToLog(currentLine.text);
            }

            //Start adding lines to the log


                //Update nameplates
                //No nameplate yet, activate the first one
                if (currentLine.GetLineType() == Dialogue.Line.Type.CharacterLine &&
                lastNameplateUsed == -1)
            {
                nameplate[0].SetActive(true);
                nameplateText[0].text = currentLine.character.characterName;
                nameplateText[0].color = currentLine.character.textColor;
                //Initialize to transparent
                nameplate[0].GetComponent<CanvasRenderer>().SetAlpha(0);
                nameplateText[0].GetComponent<CanvasRenderer>().SetAlpha(0);
                yield return null;
                //Transition to opaque
                nameplate[0].GetComponent<Image>().CrossFadeAlpha(1, nameplateTransitionTime, true);
                nameplateText[0].CrossFadeAlpha(1, nameplateTransitionTime, true);
                
                lastNameplateUsed = 0;
            }
            //New character talking, switch which nameplate is highlighted
            else if(currentLine.GetLineType() == Dialogue.Line.Type.CharacterLine && 
                currentLine.character.characterName != nameplateText[lastNameplateUsed].text)
            {
                //New nameplate is either 0 or 1, opposite of whatever the last nameplate was
                int newNameplate = (lastNameplateUsed == 0 ? 1 : 0);

                //Activate and set new nameplate to 0 opacity
                if (!nameplate[newNameplate].activeSelf)
                {
                    nameplate[newNameplate].SetActive(true);
                    //Initialize to 0 opacity
                    nameplate[newNameplate].GetComponent<CanvasRenderer>().SetAlpha(0);
                    nameplateText[newNameplate].GetComponent<CanvasRenderer>().SetAlpha(0);
                    nameplateText[newNameplate].color = currentLine.character.textColor;
                    yield return null;
                }
                nameplateText[newNameplate].text = currentLine.character.characterName;

                //Fade new in
                nameplate[newNameplate].GetComponent<Image>().CrossFadeAlpha(1, nameplateTransitionTime, true);
                nameplateText[newNameplate].CrossFadeAlpha(1, nameplateTransitionTime, true);

                //Fade old out
                nameplate[lastNameplateUsed].GetComponent<Image>().CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);
                nameplateText[lastNameplateUsed].CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);

                lastNameplateUsed = newNameplate;

            }
            //Narration Line
            else if(currentLine.GetLineType() == Dialogue.Line.Type.NarrationLine)
            {
                if (nameplate[0].activeSelf)
                {
                    nameplate[0].GetComponent<Image>().CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);
                    nameplateText[0].CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);
                }
                if (nameplate[1].activeSelf)
                {
                    nameplate[1].GetComponent<Image>().CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);
                    nameplateText[1].CrossFadeAlpha(nameplateFadedOpacity, nameplateTransitionTime, true);
                }
            }
            //Line being said by a character, make sure nameplate is there where it should be
            else
            {
                if (nameplate[lastNameplateUsed].activeSelf == false)
                {
                    nameplate[lastNameplateUsed].SetActive(true);
                }
                    nameplate[lastNameplateUsed].GetComponent<Image>().CrossFadeAlpha(1f, nameplateTransitionTime, true);
                    nameplateText[lastNameplateUsed].CrossFadeAlpha(1f, nameplateTransitionTime, true);
            }

            //Run text effects and apply them to the dialogue window
            TextEffectHandler.instance.RunText(dialogueText, currentLine.text);

            //Play audio for associated line if applicable
            FMOD.Studio.EventDescription desc;
            //If the trigger has an audio source assigned, play the associated sound (if it has one)
            if (FMODUnity.RuntimeManager.StudioSystem.getEvent(currentLine.audioToPlay, out desc) == FMOD.RESULT.OK)
            {
                FMOD.Studio.EventInstance playAudio = FMODUnity.RuntimeManager.CreateInstance(currentLine.audioToPlay);
                desc.getLength(out trigger.dialogue.textDisplayTime);
                playAudio.start();
            }

            //Dialogue type is click to proceed
            if (trigger.dialogue.pauseForDuration)
            {
                //Freeze time and let the user move their mouse around to hit the log button if desired
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                viewLog.SetActive(true);

                //Wait until effects are done or player has clicked to skip
                while(TextEffectHandler.instance.RunningEffectCount > 0)
                {
                    if(Input.GetButtonDown("Fire1") && !Log.instance.IsActive() && !mouseOverButton)
                    {
                        TextEffectHandler.instance.SkipToEndOfEffects();
                    }
                    yield return null;
                }

                //Text has finished all its effects, prompt the player to click to continue
                clickToContinue.SetActive(true);
                StartCoroutine(Flash(clickToContinue));

                //Effects are done, player must click to proceed to the next 
                while(!Input.GetButtonDown("Fire1") || Log.instance.IsActive() || mouseOverButton)
                {
                    yield return null;
                }

                //Stop flashing
                shouldFlash = false;
            }
            //Dialogue type is show for a pre-determined time
            else
            {
                viewLog.SetActive(false);
                //Wait until effects are done before starting the timer
                while(TextEffectHandler.instance.RunningEffectCount > 0)
                {
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(trigger.dialogue.textDisplayTime);
            }
            TextEffectHandler.instance.StopText();

            yield return null;
        }

        //Turn off text
        if (canvas.gameObject.activeSelf)
        {
            dialogueText.text = "";
        }

        //Resume game
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Reset Nameplates
        foreach(TMP_Text text in nameplateText)
        {
            text.text = "";
        }
        foreach(GameObject nameplate in nameplate)
        {
            nameplate.SetActive(false);
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

    bool shouldFlash = false;
    private IEnumerator Flash(GameObject objectToFlash)
    {
        shouldFlash = true;
        float currentTime = 0;
        while (shouldFlash)
        {
            if (currentTime >= flashInterval)
            {
                objectToFlash.SetActive(!objectToFlash.activeSelf);
                currentTime = 0;
            }

            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }
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

    public NarrativeTriggerHandler(string[] triggerNames)
    {
        this.triggerNames = triggerNames;
    }

    public NarrativeTriggerHandler(bool isPanning)
    {
        this.isPanning = isPanning;
    }

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

    /// <summary>
    /// Returns if the mouse is over the "View Log" button. Used to make sure text doesn't proceed when player is clicking the log button
    /// </summary>
    /// <returns>True if mouse is over button, false if not</returns>
    public bool GetMouseOverButton()
    {
        return mouseOverButton;
    }

    /// <summary>
    /// Marks whether the mouse is over the "View Log" button is moused over or not
    /// </summary>
    /// <param name="mouseOverButton">Whether its true or false</param>
    public void SetMouseOverButton(bool mouseOverButton)
    {
        this.mouseOverButton = mouseOverButton;
    }
    #endregion

}
