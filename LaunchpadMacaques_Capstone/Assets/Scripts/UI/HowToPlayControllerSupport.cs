/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* HowToPlayControllerSupport.CS
* Handles the swithcing of images for the how to play screen
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayControllerSupport : A_InputType
{
    [SerializeField] HTPSprites[] howToPlaySprites;
    public override void ChangeUI()
    {
        if (controllerDetected)
        {
            ControllerConnected();
        }

        else
        {
            ControllerDisconetted();
        }
    }

    private void ControllerConnected()
    {
        for (int i = 0; i < howToPlaySprites.Length; i++)
        {
            howToPlaySprites[i].image.sprite = howToPlaySprites[i].controllerSprite;
        }
    }

    private void ControllerDisconetted()
    {
        for (int i = 0; i < howToPlaySprites.Length; i++)
        {
            howToPlaySprites[i].image.sprite = howToPlaySprites[i].keyboardSprite;
        }
    }
}

[System.Serializable]

public class HTPSprites
{
    public Image image;
    public Sprite controllerSprite;
    public Sprite keyboardSprite;
}
