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
using TMPro;

public class HowToPlayControllerSupport : A_InputType
{
    [SerializeField] HTPSprites[] howToPlaySprites;



    HandleSaving handleSaving;
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

    private new void OnEnable()
    {
        base.OnEnable();
        handleSaving = FindObjectOfType<HandleSaving>();
        ChangeUIWithSaveInfo();
    }

    public void ChangeUIWithSaveInfo()
    {
        foreach(HTPSprites htpElement in howToPlaySprites)
        {
            foreach(ChangeHTPBasedOnSave saveInfo in htpElement.changeInfo)
            {
                if (handleSaving.UnlockedAbility(saveInfo.abilityNeeded))
                {
                    htpElement.text.transform.parent.gameObject.SetActive(true);

                    if (!htpElement.text.text.Contains(saveInfo.newText))
                    {
                        string newString = htpElement.text.text;

                        newString += saveInfo.newText;

                        htpElement.text.text = newString;
                    }

                }
            }
        }

        UpdateUI();
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
    public TextMeshProUGUI text;
    public Image image;
    public Sprite controllerSprite;
    public Sprite keyboardSprite;

    public ChangeHTPBasedOnSave[] changeInfo;
}

[System.Serializable]

public class ChangeHTPBasedOnSave
{
    [Header("Save Sensitive UI")]
    public Ability.AbilityType abilityNeeded;

    [TextArea]
    public string newText;
}
