/* 
* (Launchpad Macaques - [Game Name Here]) 
* (Contributors/Author(s)) 
* (File Name) 
* (Describe, in general, the code contained.) 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitConfirmation : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem_Ref;

    [SerializeField]
    private GameObject lastSelectedObject;

    [SerializeField]
    private GameObject confirmButtonObject;

    [SerializeField]
    private Button[] uiButtons;

    [SerializeField]
    private GameObject confirm_panel;

    public GameObject ConfirmButtonObject { get => confirmButtonObject; set => confirmButtonObject = value; }
    public EventSystem EventSystem_Ref { get => eventSystem_Ref; set => eventSystem_Ref = value; }

    // Start is called before the first frame update
    void Start()
    {
        confirm_panel = gameObject;
        confirm_panel.SetActive(false);

        uiButtons = FindObjectsOfType<Button>();

    }

    // Update is called once per frame
    void Update()
    {
        CheckLastSelected();
    }

    public void QuitGame()
    {

        confirm_panel.SetActive(true);

        foreach (Button button in uiButtons)
        {
            if (button.name != "Quit Button")
            {
                button.interactable = false;
            }
        }

        if (eventSystem_Ref.currentSelectedGameObject.name == "Quit Button")
        {
            eventSystem_Ref.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            lastSelectedObject = confirmButtonObject;
            eventSystem_Ref.SetSelectedGameObject(confirmButtonObject);
        }

    }

    public void ConfirmOrDeny(string choice)
    {
#if UNITY_EDITOR

        if (choice == "Confirm")
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else if (choice == "Deny")
        {
            confirm_panel.SetActive(false);

            foreach (Button button in uiButtons)
            {
                button.interactable = true;
            }
        }

#else

        if(choice == "Confirm")
        {
            Application.Quit();
        }
        else if(choice == "Deny")
        {
            confirm_panel.SetActive(false);
        }

#endif

    }


    private void CheckLastSelected()
    {
        
        if (eventSystem_Ref != null)
        {
            if (eventSystem_Ref.currentSelectedGameObject != null)
            {
                lastSelectedObject = eventSystem_Ref.currentSelectedGameObject;
            }
            else
            {
                eventSystem_Ref.SetSelectedGameObject(lastSelectedObject);
            }
        }

    }

}
