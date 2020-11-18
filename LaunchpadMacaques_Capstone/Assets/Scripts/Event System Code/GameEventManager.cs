/* 
* (Launchpad Macaques - [Neon Obivion]) 
* (CJ Green - Main Author) 
* ( GameEventManager.cs) 
* (WILL DESCRIBE LATER) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This is a Unity Event dictionary that holds the string/name value of the Unity Event that is contained within.")]
    private Dictionary<string, UnityEvent> eventDictionary;

    private static GameEventManager gameEventManager;

    /// <summary>
    /// Instance of the EventManager reference. Partial-Singlton pattern syle.
    /// </summary>
    public static GameEventManager gameEventManagerinstance
    {
        get
        {
            if(!gameEventManager)
            {
                gameEventManager = FindObjectOfType<GameEventManager>();

                //gameEventManager = FindObjectOfType(typeof(GameEventManager)) as GameEventManager;

                if(!gameEventManager)
                {
                    Debug.LogError("There needs to be at least one active EventManager script on a GameObject in the scene.");
                }
                else
                {
                    // Initialize the eventManager.
                    gameEventManager.Initialize();
                }

            }

            return gameEventManager;

        }

    }

    /// <summary>
    /// If there is no dictionary in the EventManager it initializes and assigns one to UnityEvent dictionary.
    /// </summary>
    private void Initialize()
    {

       if(eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }

    }

    /// <summary>
    /// Adds a listener to the Unity Event that is found inside the dictionary.
    /// If no UnityEvent is found, it adds the event, adds the listener, and then adds that event into the GameEventManager's UnityEvent dictionary.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening(string eventName, UnityAction listener)
    {

        UnityEvent thisEvent = null;

        // Checks to see if the dictionary contains an event of this kind and adds a listener if it does.
        if (gameEventManagerinstance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        // If the check is unsuccessful, a new UnityEvent is created, a listener is added, and the UnityEvent is added to the dictionary.
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            gameEventManagerinstance.eventDictionary.Add(eventName, thisEvent);
        }

    }

    /// <summary>
    /// Removes the listener from the corresponding UnityEvent that is located inside the GameEventManager's Unity Event dictionary.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(string eventName, UnityAction listener)
    {

        if(gameEventManager == null)
        {
            Debug.Log("You cannot call this method as the EventManager no longer exists. It was either destroyed or not found");
            return;
        }

        UnityEvent thisEvent;

        if(gameEventManagerinstance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }

    }

    /// <summary>
    /// Simply Invokes / Triggers the desired event when the correct conditions are met according to the listener(s).
    /// </summary>
    /// <param name="eventName"></param>
    public static void TriggerEvent(string eventName)
    {

        UnityEvent thisEvent = null;

        if(gameEventManagerinstance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }

    }    
}
