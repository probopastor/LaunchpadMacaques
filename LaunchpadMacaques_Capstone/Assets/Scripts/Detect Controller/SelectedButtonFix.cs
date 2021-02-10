/* 
* Launchpad Macaques - Neon Oblivion
* Matt Kirchoff, Levi Schoof
* SelectedButtonFix.cs
* Place on the First Button in a menu
* Fixes issue with no buttons being highlted upon returning to a menu
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedButtonFix : MonoBehaviour, IPointerEnterHandler
{

    [SerializeField, Tooltip("Is this button the first one in a menu")] bool starterButton = true;


    #region CallStartButtonMethods
    /// <summary>
    ///Called on start
    /// Will set this to be the button that gets selected if no buttons are selected
    /// </summary>
    private void Start()
    {
        SetStartButton();
    }


    /// <summary>
    /// Called when this button is enabled
    /// Will set this to be the button that gets selected if no buttons are selected
    /// </summary>
    public void OnEnable()
    {
        SetStartButton();
    }

    /// <summary>
    /// Called when this button is hovered over
    /// Will set this to be the button that gets selected if no buttons are selected
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetStartButton();
    }

    public void OnSelect(BaseEventData eventData)
    {

        SetStartButton();
    }
    #endregion

    #region Helper
    /// <summary>
    /// Sets the starter button the Controller Detection script
    /// </summary>
    private void SetStartButton()
    {
        if (starterButton)
        {
            if (DetectController.instance)
            {
                DetectController.instance.selectedGameObject = this.gameObject;
            }

            else
            {
                FindObjectOfType<DetectController>().selectedGameObject = this.gameObject;
            }
        }
    }
    #endregion
}
