/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Event_PanToObject : MonoBehaviour
{

    private UnityAction cameraListener;
    private string eventName = "CameraPanToObject";

    [SerializeField]
    private CinemachineVirtualCamera panningCamera;
    [SerializeField]
    private float panTime;

    private int initPriority;

    private void Awake()
    {
        panningCamera = GameObject.FindGameObjectWithTag("PanningCamera").GetComponent<CinemachineVirtualCamera>();
        cameraListener = new UnityAction(PanCamera);
    }

    private void OnEnable()
    {
        GameEventManager.StartListening(eventName, cameraListener);
    }

    private void OnDisable()
    {
        GameEventManager.StopListening(eventName, cameraListener);
    }

    public void PanCamera()
    {
        StartCoroutine(PanCamreaToDoor(panningCamera, panTime));
    }


    private IEnumerator PanCamreaToDoor(CinemachineVirtualCamera camToPan, float time)
    {
        initPriority = panningCamera.m_Priority;
        Time.timeScale = .99f;
        panningCamera.m_Priority = 20;
        yield return new WaitForSeconds(panTime);
        panningCamera.m_Priority = initPriority;
        Time.timeScale = 1f;
    }

}
