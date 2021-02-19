/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* DropDownSlider.CS
* Makes suer DropDown boxes work with Controller
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropDownSlider : MonoBehaviour
{
    private TMP_Dropdown dropdown;


    private void Start()
    {
        dropdown = this.GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        dropdown = this.GetComponent<TMP_Dropdown>();
    }


    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject && dropdown)
        {
            if (EventSystem.current.currentSelectedGameObject == dropdown.gameObject)
            {
                //if (Input.GetButtonUp("UI Veritcal"))
                //{
                //    Transform dropdownListTransform = dropdown.gameObject.transform.Find("Dropdown List");
                //    if (dropdownListTransform == null)
                //    {
                //        dropdown.Show();
                //    }
                //}
            }
            else
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0)
                {
                    if (results[0].gameObject.transform.IsChildOf(dropdown.gameObject.transform))
                    {
                        return;
                    }
                }

                if (EventSystem.current.currentSelectedGameObject.transform.IsChildOf(dropdown.gameObject.transform))
                {
                    if (EventSystem.current.currentSelectedGameObject.name.StartsWith("Item "))
                    {
                        // Skip disabled items
                        Transform parent = EventSystem.current.currentSelectedGameObject.transform.parent;
                        int activeChildren = 0;
                        int totalChildren = parent.childCount;
                        for (int childIndex = 0; childIndex < totalChildren; childIndex++)
                        {
                            if (parent.GetChild(childIndex).gameObject.activeInHierarchy)
                            {
                                activeChildren++;
                            }
                        }
                        int myActiveIndex = 0;
                        for (int childIndex = 0; childIndex < totalChildren; childIndex++)
                        {
                            if (parent.GetChild(childIndex).gameObject == EventSystem.current.currentSelectedGameObject)
                            {
                                break;
                            }
                            else if (parent.GetChild(childIndex).gameObject.activeInHierarchy)
                            {
                                myActiveIndex++;
                            }
                        }

                        if (activeChildren > 1)
                        {
                            GameObject scrollbarGameObject = GameObject.Find("Scrollbar");
                            if (scrollbarGameObject != null && scrollbarGameObject.activeInHierarchy)
                            {
                                Scrollbar scrollbar = scrollbarGameObject.GetComponent<Scrollbar>();
                                if (scrollbar.direction == Scrollbar.Direction.TopToBottom)
                                    scrollbar.value = (float)myActiveIndex / (float)(activeChildren - 1);
                                else
                                    scrollbar.value = 1.0f - (float)myActiveIndex / (float)(activeChildren - 1);
                            }
                        }
                    }
                }
            }
        }

    }
}
