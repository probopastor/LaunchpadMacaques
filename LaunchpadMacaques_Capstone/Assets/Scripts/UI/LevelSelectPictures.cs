/*****************************************************************************
// File Name : LevelSelectPictures
// Author : Brennan Carlyle
//          
// Creation Date : 11/1/20
//
// Brief Description : Changes image within the picture box to that of the level 
// currently selected. 
// 
*****************************************************************************/


using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPictures : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button level4Button;
    public Button level5Button;
    public Button backButton;

    public Image pictureHolder;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Sprite sprite5;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnSelect(BaseEventData eventData)
    {
        GetComponent<Image>().sprite = pictureHolder.GetComponent<Image>().sprite;
        //GetComponent<Image>().sprite = GetComponent<Button>().spriteState.highlightedSprite;
        if (EventSystem.current.currentSelectedGameObject.Equals(level1Button))
        {
            pictureHolder.sprite = sprite1;
        }
        else if (EventSystem.current.currentSelectedGameObject.Equals(level2Button))
        {
            pictureHolder.sprite = sprite2;
        }
        else if (EventSystem.current.currentSelectedGameObject.Equals(level3Button))
        {
            pictureHolder.sprite = sprite3;
        }
        else if (EventSystem.current.currentSelectedGameObject.Equals(level4Button))
        {
            pictureHolder.sprite = sprite4;
        }
        else if (EventSystem.current.currentSelectedGameObject.Equals(level5Button))
        {
            pictureHolder.sprite = sprite5;
        }
        else if (EventSystem.current.currentSelectedGameObject.Equals(backButton))
        {
            pictureHolder.sprite = null;
        }

    }
}
