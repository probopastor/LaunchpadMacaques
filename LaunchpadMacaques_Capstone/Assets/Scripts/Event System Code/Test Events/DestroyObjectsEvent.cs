﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DestroyObjectsEvent : MonoBehaviour
{
    [SerializeField]
    private UnityAction eventListener;
    [SerializeField]
    private string eventName = "DestroyObjects";

    [SerializeField]
    private List<GameObject> destroyableObjects = new List<GameObject>();

    private float elapsedTime;

    private float destroyTime;

    public Text displayText;

    private void Awake()
    {
        eventListener = new UnityAction(DestroyObjects);
    }

    private void Update()
    {

        //elapsedTime += Time.deltaTime;
        destroyTime -= Time.deltaTime;
        displayText.text = "Imminent doom in " + Mathf.Round(destroyTime);


        if (destroyableObjects.Count == 0)
        {
            displayText.text = "The objects have been destroyed!!";

        }

        if (destroyTime <= 0)
        {
            GameEventManager.TriggerEvent(eventName);
        }

    }

    private void OnEnable()
    {

        destroyTime = 10;

        displayText.gameObject.SetActive(true);

        GameEventManager.StartListening(eventName, eventListener);
    }

    private void OnDisable()
    {
        GameEventManager.StopListening(eventName, eventListener);

        displayText.gameObject.SetActive(false);

    }

    private void DestroyObjects()
    {
        foreach (GameObject destroyableObject in destroyableObjects.ToList())
        {
            Destroy(destroyableObject);
            destroyableObjects.Remove(destroyableObject);
        }
    }

}