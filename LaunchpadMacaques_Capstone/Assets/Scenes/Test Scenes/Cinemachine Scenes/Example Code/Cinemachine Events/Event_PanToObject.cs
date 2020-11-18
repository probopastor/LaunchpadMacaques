/* 
* (Launchpad Macaques - [Neon Oblivion]) 
* (CJ Green) 
* (Event_PanToObject.cs) 
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

    [SerializeField] [Tooltip("The camera that will be the camera that the Main Camera will pan to.")]
    private CinemachineVirtualCamera panningCamera;

    [SerializeField] [Tooltip("The time it takes before the camera switches back to the regular view.")]
    private float panTime;

    // The Cinemachine camera priority value. This determines what camera Cinemachine is using/looking through.
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
