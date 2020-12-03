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

using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPictures : MonoBehaviour
{
    public GameObject level1Button;
    public GameObject level2Button;
    public GameObject level3Button;
    public GameObject level4Button;
    public GameObject level5Button;
    public GameObject backButton;

    public Image pictureHolder;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Sprite sprite5;
    public Sprite sprite6;

    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        SetImages();
    }

    private void SetImages()
    {
        if (eventSystem.currentSelectedGameObject == level1Button)
        {
            pictureHolder.GetComponent<Image>().sprite = sprite1;
        }
        else if (eventSystem.currentSelectedGameObject == (level2Button))
        {
            pictureHolder.GetComponent<Image>().sprite = sprite2;
        }
        else if (eventSystem.currentSelectedGameObject == (level3Button))
        {
            pictureHolder.GetComponent<Image>().sprite = sprite3;
        }
        else if (eventSystem.currentSelectedGameObject == (level4Button))
        {
            pictureHolder.GetComponent<Image>().sprite = sprite4;
        }
        else if (eventSystem.currentSelectedGameObject == (level5Button))
        {
            pictureHolder.GetComponent<Image>().sprite = sprite5;
        }
        else if (eventSystem.currentSelectedGameObject == (backButton))
        {
            pictureHolder.GetComponent<Image>().sprite = sprite6;
        }

    }
}
