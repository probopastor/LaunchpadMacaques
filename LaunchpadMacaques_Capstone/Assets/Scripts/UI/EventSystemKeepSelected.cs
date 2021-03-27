/*****************************************************************************
// File Name : EventSystemKeepSelected
// Author : Brennan Carlyle
//          
// Creation Date : 11/2/20
//
// Brief Description : Prevents buttons from deselecting upon the mouse being  
// clicked elsewhere on the screen.  
// 
*****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemKeepSelected : MonoBehaviour
{
  
    private EventSystem eventSystem;
    private GameObject lastSelected = null;
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (eventSystem != null)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                lastSelected = eventSystem.currentSelectedGameObject;
            }
            else
            {
                eventSystem.SetSelectedGameObject(lastSelected);
            }
        }
    }

}
