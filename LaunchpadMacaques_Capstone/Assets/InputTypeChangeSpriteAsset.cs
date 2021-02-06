using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputTypeChangeSpriteAsset : A_InputType
{
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] SpriteHandler[] sprites;

    [SerializeField] InformationPost informationPost;

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


    private void ControllerConnected()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            ReplaceSprite(sprites[i].keyBoardSpriteIndex, sprites[i].controllerSpriteIndex);
        }
    }

    private void ControllerDisconeted()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            ReplaceSprite(sprites[i].controllerSpriteIndex, sprites[i].keyBoardSpriteIndex);
        }
    }
    private void ReplaceSprite(int oldSprite, int newSprite)
    {
        string old = informationPost.GetInformationPostTest();
        string output = old.Replace("<sprite=" + oldSprite + ">", "<sprite=" + newSprite + ">");
        
        if (textBox.text.Contains("<sprite=" + oldSprite + ">"))
        {
            textBox.text = textBox.text.Replace("<sprite=" + oldSprite + ">", "<sprite=" + newSprite + ">");
        }


        informationPost.SetInformationPost(output);
    }
}

[System.Serializable]
public class SpriteHandler
{
    public int keyBoardSpriteIndex;
    public int controllerSpriteIndex;
}
