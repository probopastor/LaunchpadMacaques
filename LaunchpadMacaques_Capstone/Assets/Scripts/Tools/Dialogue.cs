/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * NarrativeTriggerHandler.cs
 * This script contains functionality for Dialogue and dialogue-related objects. A Dialogue instance contains a List of lines meant to be delivered in a
 * conversation
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains a list of Line objects that represent the lines to be delivered in a conversation, as well as functionality to 
/// go through those lines one at the time
/// </summary>
[System.Serializable]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    public List<Line> lines;

    [Tooltip("Determines whether this dialogue will pause everything and wait for clicks, or will not pause and will exist for preset amount of time"), SerializeField]
    public bool pauseForDuration = false;
    [Tooltip("The time the text should be displayed for"), SerializeField]
    public int textDisplayTime = 5;

    private int currentLineIndex;

    public bool EndOfDialogue { get; private set; } = false;

    public Dialogue()
    {
        lines = new List<Line>();
    }

    /// <summary>
    /// Contains the name of a character, as well as an image representing them and a unique text color if desired
    /// </summary>
    [System.Serializable]
    public class Character
    {
        [Tooltip("The name of the character"), SerializeField]
        public string characterName;
        [Tooltip("An image to represent the character"), SerializeField]
        public string imgPath;
        [Tooltip("Give the character a signature text color, if desired"), SerializeField]
        public Color textColor;

        //Constructors
        public Character() : this("", "", Color.white) { }

        public Character(string name) : this(name, "", Color.white) { }

        public Character(string name, string imgPath) : this(name, imgPath, Color.white) { }

        public Character(string name, Color textColor) : this(name, "", textColor) { }

        public Character(string name, string imgPath, Color textColor)
        {
            this.characterName = name;
            this.imgPath = imgPath;
            this.textColor = textColor;
        }
    }

    /// <summary>
    /// Represents a line in a conversation. Contains the Character object that is saying the line, the text being said, and the audio voiceover
    /// to be played alongside the text.
    /// </summary>
    [System.Serializable]
    public class Line
    {
        [Tooltip("The character saying the line"), SerializeField]
        public Character character;
        [Tooltip("The text the character is saying"), SerializeField]
        public string text;
        [Tooltip("The audio to be said along with the line"), FMODUnity.EventRef, SerializeField]
        public string audioToPlay;

        public Line()
        {
            this.character = null;
            this.text = "";
            this.audioToPlay = "";
        }

    }

    /// <summary>
    /// Get the next Line object in this Dialogue. Can be called until no more Lines are left. Must call Restart() to reset this and
    /// start getting Lines from the beginning again
    /// </summary>
    /// <returns>The next Line object in the dialogue</returns>
    public Line NextLine()
    {
        if(currentLineIndex < lines.Count)
        {
            currentLineIndex++;
            return lines[currentLineIndex - 1];
        }

        EndOfDialogue = true;
        return null;
    }

    /// <summary>
    /// Resets the End Of Dialogue flag and moves index back to the first Line in this Dialogue
    /// </summary>
    public void Restart()
    {
        currentLineIndex = 0;
        EndOfDialogue = false;
    }

}
