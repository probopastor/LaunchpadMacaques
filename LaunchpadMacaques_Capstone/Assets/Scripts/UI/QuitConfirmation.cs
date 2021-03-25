/* 
* (Launchpad Macaques - [Neon Oblivion]) 
* (CJ Green) 
* (QuitConfirmation.cs) 
* (This script contains the logic and functionality of confriming if that player wanted to quit that application or not.) 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitConfirmation : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the Unity EvenSystem.")]
    private EventSystem eventSystem_Ref = null;

    [SerializeField, Tooltip("Keeps track of the last selected UI element.")]
    private GameObject lastSelectedObject;

    [SerializeField, Tooltip("Button to swtich to when the confrim quit panel is activated.")]
    private GameObject confirmButtonObject = null;

    [SerializeField, Tooltip("List of all the UI buttons active in the scene.")]
    private Button[] uiButtons;

    [SerializeField, Tooltip("Panel that contains all the elements for cofrim quit.")]
    private GameObject confirm_panel;

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

    /// <summary>
    /// Function that 
    /// </summary>
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

    /// <summary>
    /// This function handles the functionality of what happens when the player interacts with the Confirm Quit UI. 
    /// Whether it Quits the application or Exits Play Mode is also determined in this function.
    /// </summary>
    /// <param name="choice"></param>
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

    /// <summary>
    /// Keeps track of the last selected UI button.
    /// </summary>
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
