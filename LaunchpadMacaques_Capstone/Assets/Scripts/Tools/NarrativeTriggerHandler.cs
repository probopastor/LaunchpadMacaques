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

public class NarrativeTriggerHandler : MonoBehaviour
{
    public enum TriggerType { Area, Random, OnEvent };
    [SerializeField, Tooltip("Contains all triggers in the scene. Each array element is a separate trigger with it's own type, " +
        "as well as its own text and audio outputs")]
    public Trigger[] triggers;

    public bool[] triggerSubFoldout;
    public string[] triggerNames;

    [SerializeField, Tooltip("The lower end of the interval that a random trigger can be called")]
    private float randomIntervalMin;
    [SerializeField, Tooltip("The upper end of the interval that a random trigger can be called")]
    private float randomIntervalMax;


    [System.Serializable]
    public class Trigger
    {
        [Tooltip("The type of activation used for this Trigger")]
        public TriggerType type;
        [Tooltip("The text to display on Trigger activation")]
        public string textToDisplay;
        [Tooltip("The audio to play on Trigger activation")]
        public AudioClip audioToPlay;
        [Tooltip("The audio source the audio will play from")]
        public AudioSource audioSource;
        [Tooltip("Whether this Trigger can be activated multiple times (true) or only once (false)")]
        public bool repeatable;

        //Only appear on TriggerType.Area
        [Tooltip("The GameObject that will activate the Trigger when it enters the trigger area")]
        public GameObject triggeringObject;
        [Tooltip("The center of the trigger zone")]
        public Vector3 areaCenter;
        [Tooltip("The size of the trigger zone relative to the center")]
        public Vector3 boxSize;

        //Only appear on TriggerType.OnEvent
        [Tooltip("The event (from another script) that will activate this trigger")]
        public TriggerEvent triggerEvent = new TriggerEvent();
        
        /// <summary>
        /// Activate the trigger, played sound if applicable and plays 
        /// </summary>
        public void Activate()
        {
            audioSource.clip = audioToPlay;
            audioSource.Play();

            //Text appearing functionality
        }
    }
    
    public class TriggerEvent : UnityEvent {}


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
                    triggers[i].Activate();
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

                Trigger triggerToActivate = randomTriggers[Random.Range(0, (int)randomTriggers.Count)];

                triggerToActivate.Activate();
            }

            yield return null;
        }

    }

    private bool AreaCheck(Trigger triggerToCheck)
    {
        Collider[] hit = Physics.OverlapBox(triggerToCheck.areaCenter, triggerToCheck.boxSize / 2, Quaternion.identity);

        foreach(Collider x in hit)
        {
            if (x.gameObject == triggerToCheck.triggeringObject)
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


}
