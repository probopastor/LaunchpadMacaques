﻿/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * TextEffectHandler.cs
 * This script contains multiple functions for 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CharTween;

public  class TextEffectHandler : MonoBehaviour
{
    public static TextEffectHandler instance;
    private delegate IEnumerator TextEffect(TMP_Text textObj, int startIndex, int endIndex);
    private Dictionary<string, TextEffect> effectLibrary;
    private CharTweener tweener;

    //Available Text Effects
    TextEffect shaky;
    TextEffect typewriter;
    TextEffect rainbow;

    struct EffectApplicationDetails
    {
        public EffectApplicationDetails(string effectName, int startTag_startIndex, int endTag_startIndex)
        {
            this.effectName = effectName;
            this.startTag = string.Format("<{0}>", effectName);
            this.endTag = string.Format("</{0}>", effectName);

            this.startTag_startIndex = startTag_startIndex;
            this.endTag_startIndex = endTag_startIndex;
            this.effectStartIndex = startTag_startIndex + startTag.Length;
            this.effectEndIndex = endTag_startIndex;
        }

        public string effectName;
        public int startTag_startIndex, endTag_startIndex;
        public int effectStartIndex, effectEndIndex;

        public string startTag, endTag;
    }


    private void OnEnable()
    {
        //Assign delegates to their corresponding functions
        shaky += ShakyText;
        typewriter += TypewriterText;
        rainbow += RainbowText;
    }

    private void OnDisable()
    {
        //Assign delegates to their corresponding functions
        shaky -= ShakyText;
        typewriter -= TypewriterText;
        rainbow -= RainbowText;
    }
    private void Awake()
    {
        instance = this;

        shaky += ShakyText;
        typewriter += TypewriterText;
        rainbow += RainbowText;

        //Load Text Effects into effectLibrary
        effectLibrary = new Dictionary<string, TextEffect>()
        {
            {"shaky", shaky },
            {"typewriter", typewriter },
            {"rainbow", rainbow }
        };
    }

    /// <summary>
    /// Public function called by other scripts to run text, calls ParseText
    /// </summary>
    /// <param name="textObject">The TMPro Text object that the result will be displayed on</param>
    /// <param name="textToRun">The string text to run through and put on the TMPro object</param>
    public void RunText(TMP_Text obj, string textToRun)
    {
        ParseText(obj, textToRun);
    }

    public void StopText()
    {
        tweener.DOKill();
        StopAllCoroutines();
    }

    /// <summary>
    /// Searches through a string looking for effect tags, runs corresponding effects on the text inside the tags.
    /// Throws an error if there are any unmatched tags or tags that don't exist
    /// </summary>
    private void ParseText(TMP_Text obj, string textToParse)
    {
        List<EffectApplicationDetails> effectsToApply = new List<EffectApplicationDetails>();
        string tagName = "";
        int startTag_startIndex, endTag_startIndex;
        //Counts start strings seen along the way of the same tag when looking for end tag
        int repeatTagCountCurrent = 0, repeatTagCountTotal = 0;
        char[] separators = { '<', '>' };
        //Counts the number of times an effect is seen an encountered for during step 1
        Dictionary<string, int> effectSeenCounter = new Dictionary<string, int>();
        foreach(string name in effectLibrary.Keys)
        {
            effectSeenCounter.Add(name, 0);
        }
        

        string[] splitText = textToParse.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

        //**1. Cycle through the split string to locate tags, where to apply effects, and prepare them for removal**//
        for (int i = 0; i < splitText.Length; i++)
        {
            //Test to see if element is an accepted tag, if it isn't move on to the next string 
            if (!effectLibrary.ContainsKey(splitText[i]))
            {
                continue;
            }

            tagName = splitText[i];
            string startTag = string.Format("<{0}>", tagName);
            string endTag = string.Format("</{0}>", tagName);

            int prevEffectSeenIndex = 0;
            for (int j = 0; j <= effectSeenCounter[tagName]; j++)
            {
                prevEffectSeenIndex = textToParse.IndexOf(startTag, j == 0 ? prevEffectSeenIndex : prevEffectSeenIndex + 1);
            }
            startTag_startIndex = prevEffectSeenIndex;
            //effectSeenCounter[tagName]++;

            //Element is in dictionary, so find end tag
            for (int j = i + 1; j < splitText.Length; j++)
            {
                //End tag found
                if (splitText[j] == "/" + tagName)
                {
                    //If hit a duplicate beginning tag on the way, don't pay attention to the first end tag
                    if(repeatTagCountCurrent > 0)
                    {
                        repeatTagCountCurrent--;
                        continue;
                    }

                    //Find End Tag, skip one end tag for each repeat tag found, runs once if not
                    int prevRepeatIndex = 0;
                    for(int k = 0; k <= effectSeenCounter[tagName]; k++)
                    {
                        prevRepeatIndex = textToParse.IndexOf(endTag, k == 0 ? prevRepeatIndex : prevRepeatIndex + 1);
                    }
                    endTag_startIndex = prevRepeatIndex;


                    //Queue this effect up to be applied
                    effectsToApply.Add(new EffectApplicationDetails(tagName, startTag_startIndex, endTag_startIndex));
                    
                    //End tag found, no longer need to look for end tag, go back to the outer loop and continue looking for start tags
                    break;
                }
                else if(splitText[j] == tagName)
                {
                    repeatTagCountCurrent++;
                    repeatTagCountTotal++;
                }
            }
            effectSeenCounter[tagName]++;
        }

        //**2. Begin removing tags and updating indexes on where effects should occur to the shorter string**//
        for (int i = 0; i < effectsToApply.Count; i++)
        {
            EffectApplicationDetails currentEffect = effectsToApply[i];
            
            textToParse = textToParse.Remove(currentEffect.endTag_startIndex, currentEffect.endTag.Length)
                       .Remove(currentEffect.startTag_startIndex, currentEffect.startTag.Length);

            //Adjust all other tags to make up for the removal
            for(int j = 0; j < effectsToApply.Count; j++)
            {
                EffectApplicationDetails thisOtherEffect = effectsToApply[j];
                int modifierValue = 0;
                bool modifyEnd = true;
                bool modifyStart = true;

                if (i == j)
                {
                    continue;
                }
                //Other effect is completely nested within the currentEffect; Have to account for the start tag of the current effect shifting the other effect
                //     <currentEffect><otherEffect>Text</otherEffect></currentEffect>
                else if (thisOtherEffect.startTag_startIndex > currentEffect.startTag_startIndex
                      && thisOtherEffect.endTag_startIndex < currentEffect.endTag_startIndex)
                {
                    modifierValue = currentEffect.startTag.Length;
                }
                //Other effect is
                //    <otherEffect><currentEffect>Text<currentEffect></otherEffect>
                else if (thisOtherEffect.startTag_startIndex < currentEffect.startTag_startIndex
                     && thisOtherEffect.endTag_startIndex > currentEffect.endTag_startIndex)
                {
                    modifierValue = (currentEffect.startTag.Length + currentEffect.endTag.Length);
                    modifyStart = false;
                }
                //Other effect is after the current effect entirely; Have to account for both the start and end tags of current effect shifting the other effect
                //    <currentEffect>Text</currentEffect><otherEffect>Text</otherEffect>
                else if (thisOtherEffect.startTag_startIndex > currentEffect.endTag_startIndex 
                      && thisOtherEffect.endTag_startIndex > currentEffect.endTag_startIndex)
                {
                    modifierValue = (currentEffect.startTag.Length + currentEffect.endTag.Length);
                }
                //Other effect starts within the current effect, but ends outside current effect (really fringe case in case those pesky designers decide to get creative)
                //    <currentEffect>Text<otherEffect></currentEffect>Text</otherEffect>
                else if (thisOtherEffect.startTag_startIndex > currentEffect.startTag_startIndex
                      && thisOtherEffect.startTag_startIndex < currentEffect.endTag_startIndex
                      && thisOtherEffect.endTag_startIndex > currentEffect.endTag_startIndex)
                {
                    modifierValue = currentEffect.endTag.Length;
                    modifyStart = false;
                }
                //Other effect starts outside current effect, but ends inside current effect(really fringe case in case those pesky designers decide to get creative)
                //    <otherEffect>Text<currentEffect>Text</otherEffect></currentEffect>
                else if (thisOtherEffect.startTag_startIndex < currentEffect.startTag_startIndex
                      && thisOtherEffect.endTag_startIndex > currentEffect.startTag_startIndex
                      && thisOtherEffect.endTag_startIndex < currentEffect.endTag_startIndex)
                {
                    modifierValue = currentEffect.startTag.Length;
                    modifyEnd = false;
                }
                //else, the effect is completely behind, but doesn't matter cause it won't be affected by the removal of the tags

                //Apply modifiers
                if (modifyStart)
                {
                    thisOtherEffect.startTag_startIndex -= modifierValue;
                    thisOtherEffect.effectStartIndex -= modifierValue;
                }
                if (modifyEnd)
                {
                    thisOtherEffect.endTag_startIndex -= modifierValue;
                    thisOtherEffect.effectEndIndex -= modifierValue;
                }

                //EffectApplicationDetails is a struct (only passes by value), so must assign the corresponding struct in list to this *new* struct to assign it
                effectsToApply[j] = thisOtherEffect;

            }

            //Adjust current Effect for the tag removals
            currentEffect.effectStartIndex -= currentEffect.startTag.Length;
            currentEffect.effectEndIndex -= currentEffect.startTag.Length;
            currentEffect.startTag_startIndex -= currentEffect.startTag.Length;
            currentEffect.endTag_startIndex -= currentEffect.startTag.Length;

            effectsToApply[i] = currentEffect;
        }

        //Apply shortening now that effect details are harvested
        obj.text = textToParse;

        //**3. Apply the effect to the now shortened string**//

        tweener = obj.GetCharTweener();

        for (int i = 0; i < effectsToApply.Count; i++)
        {
            EffectApplicationDetails currentDetails = effectsToApply[i];
            Debug.Log(currentDetails.effectName +  " | " + currentDetails.effectStartIndex + " | " + currentDetails.effectEndIndex);
            StartCoroutine(effectLibrary[currentDetails.effectName](obj, currentDetails.effectStartIndex, currentDetails.effectEndIndex));
        }
    }

    /// <summary>
    /// Removes the first instance of a string in a second string. 
    /// https://stackoverflow.com/questions/2201595/c-sharp-simplest-way-to-remove-first-occurrence-of-a-substring-from-another-st Daniel Felipe's solution used as reference.
    /// </summary>
    /// <param name="initialString"></param> The initial string.
    /// <param name="stringToRemove"></param> The string that will be removed.
    /// <returns></returns>
    public string RemoveStringFromString(string initialString, string stringToRemove)
    {
        int startIndex = initialString.IndexOf(stringToRemove);
        return startIndex < 0 ? initialString : initialString.Remove(startIndex, stringToRemove.Length);
    }

    /// <summary>
    /// The shaky text effect. Applies the effect onto the TextMeshPro object from the indicated start vertex to the indicated end vertex
    /// </summary>
    /// <param name="textObject">The TextMeshPro object to work on</param>
    /// <param name="startVertex">The vertex to start the effect on</param>
    /// <param name="endVertex">The vertex to end the effect on</param>
    /// <returns></returns>
    private IEnumerator ShakyText(TMP_Text textObject, int startIndex, int endIndex)
    {
        for(int i = startIndex; i < endIndex; i++)
        {
            var timeOffset = Mathf.Lerp(0, 1, (i - startIndex) / (float)(endIndex - startIndex + 1));
            tweener.DOShakePosition(i, 10, 1.5f, 10, 90, false, false)
                .SetLoops(-1, LoopType.Restart);
        }
        yield return null;
    }

    /// <summary>
    /// The typewriter text effect. Applies the effect onto the TextMeshPro object from the indicated start vertex to the indicated end vertex
    /// </summary>
    /// <param name="textObject">The TextMeshPro object to work on</param>
    /// <param name="startVertex">The vertex to start the effect on</param>
    /// <param name="endVertex">The vertex to end the effect on</param>
    /// <returns></returns>
    private IEnumerator TypewriterText(TMP_Text textObject, int startIndex, int endIndex)
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            tweener.SetAlpha(i, 0);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        for (int i = startIndex; i < endIndex; i++)
        {
            tweener.SetAlpha(i, 1);
            yield return new WaitForSecondsRealtime(0.125f);
        }

    }

    /// <summary>
    /// The rainbow text effect. Applies the effect onto the TextMeshPro object from the indicated start vertex to the indicated end vertex
    /// </summary>
    /// <param name="textObject">The TextMeshPro object to work on</param>
    /// <param name="startVertex">The vertex to start the effect on</param>
    /// <param name="endVertex">The vertex to end the effect on</param>
    /// <returns></returns>
    private IEnumerator RainbowText(TMP_Text textObject, int startIndex, int endIndex)
    {
        bool[] activated = new bool[tweener.Text.textInfo.characterCount];
        for(int i = 0; i < activated.Length; i++)
        {
            activated[i] = false;
        }

        while (true)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var timeOffset = Mathf.Lerp(0, 1, (i - startIndex) / (float)(endIndex - startIndex + 1));
                if (tweener.GetAlpha(i) != 0 && activated[i] == false)
                {
                    activated[i] = true;
                    var colorTween = tweener.DOColor(i, UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, tweener.GetAlpha(i), tweener.GetAlpha(i)),
                        0.5f)
                        .SetLoops(-1, LoopType.Yoyo);
                    colorTween.fullPosition = timeOffset;
                }
            }
            yield return null;
        }

        
    }
}
