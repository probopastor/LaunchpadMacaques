/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* ExampleInputType.cs
* Implements A_InputType and handles the sprite asset stuff for Tutorial Posts
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputTypeChangeSpriteAsset : A_InputType
{
    TextMeshProUGUI textBox;
    InformationPost informationPost;

    [SerializeField] SpriteHandler[] sprites;



    private new void Start()
    {
        informationPost = this.gameObject.GetComponent<InformationPost>();
        textBox = informationPost.GetTextBox();
        base.Start();

    }

    public override void ChangeUI()
    {
        if (controllerDetected)
        {
            ControllerConnected();
        }

        else
        {
            ControllerDisconeted();
        }
    }


    /// <summary>
    /// Will be called when controller is connected, switches sprites to controller version
    /// </summary>
    private void ControllerConnected()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            ReplaceSprite(sprites[i].keyBoardSpriteIndex, sprites[i].controllerSpriteIndex);
        }
    }

    /// <summary>
    /// Will be called when controller is disconnected, switches sprites to keyboard/mouse version 
    /// </summary>
    private void ControllerDisconeted()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            ReplaceSprite(sprites[i].controllerSpriteIndex, sprites[i].keyBoardSpriteIndex);
        }
    }

    /// <summary>
    /// Will handle the actual replacing of sprites
    /// </summary>
    /// <param name="oldSprite"></param>
    /// <param name="newSprite"></param>
    private void ReplaceSprite(int oldSprite, int newSprite)
    {
        if (informationPost)
        {
            // Creates a new version of the Information Post text that uses the new sprites
            string old = informationPost.GetInformationPostTest();
            string output = old.Replace("<sprite=" + oldSprite + ">", "<sprite=" + newSprite + ">");

            // Will check to see if text box has the old sprite and will replace it
            if (textBox.text.Contains("<sprite=" + oldSprite + ">"))
            {
                textBox.text = textBox.text.Replace("<sprite=" + oldSprite + ">", "<sprite=" + newSprite + ">");
            }


            // Sets the information post to use new sprites
            informationPost.SetInformationPost(output);
        }

    }
}

[System.Serializable]
public class SpriteHandler
{
    public int keyBoardSpriteIndex;
    public int controllerSpriteIndex;
}
