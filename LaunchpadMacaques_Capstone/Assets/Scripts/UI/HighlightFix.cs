/*****************************************************************************
// File Name : HighlightFix.cs
// Author: John Doran, Brennan Carlyle
// Creation Date: ENTER DATE
//
// Brief Description : Fixes errors related to highlighting menu objects when selected
*****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    public Sprite originalSprite;
    
    public void Start()
    {
      
    }

    public void UnloadSelf()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.gameObject.GetComponent<Button>().interactable)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            GetComponent<Image>().sprite = GetComponent<Button>().spriteState.highlightedSprite;
            
            if (!EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

    }

    public void OnSelect(BaseEventData eventData)
    {
        GetComponent<Image>().sprite = GetComponent<Button>().spriteState.highlightedSprite;
        
        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(this.gameObject);
    }



    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
        GetComponent<Image>().sprite = originalSprite;
      
        this.GetComponent<Selectable>().OnPointerExit(null);
    }

    private void OnDisable()
    {
        GetComponent<Image>().sprite = originalSprite;
        
    }
}
