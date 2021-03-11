using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueBuilder : EditorWindow
{
    public static DialogueBuilder instance;

    //Serialized Objects are weird with Scriptable Objects...it just has to be this way
    public SerializedObject currentObject, dialogueSubObject;
    public SerializedProperty currentLine;
    public SerializedProperty Dialogue
    {
        get { return dialogue; }
        set
        {
            //Set to passed dialogue value
            dialogue = value;
            selectedCharacterIndexes = new List<int>();

            dialogueSubObject = new SerializedObject(dialogue.objectReferenceValue);
            
            //Get each line in dialogue and match up the already assigned characters
            for (int i = 0; i < dialogueSubObject.FindProperty("lines").arraySize; i++)
            {
                currentLine = dialogueSubObject.FindProperty("lines").GetArrayElementAtIndex(i);

                selectedCharacterIndexes.Add(
                    GetIndexFromCharacter(
                        new Dialogue.Character(currentLine.FindPropertyRelative("character").FindPropertyRelative("characterName").stringValue,
                                               currentLine.FindPropertyRelative("character").FindPropertyRelative("imgPath").stringValue,
                                               currentLine.FindPropertyRelative("character").FindPropertyRelative("textColor").colorValue)
                        )
                    );
            }
        }
    }
    private SerializedProperty dialogue;
    private string currentDialogueName;
    private List<int> selectedCharacterIndexes;

    //UI Variables
    private bool characterFoldout = false;
    private bool dialogueFoldout = true;
    private Vector2 scrollPos;


    [SerializeField]
    private List<Dialogue.Character> characterPool;

    private Dialogue.Character GetCharacterFromIndex(int index)
    {
        if (index < characterPool.Count && index > 0)
            return characterPool[index];
        else
            return null;
    }

    private int GetIndexFromCharacter(Dialogue.Character character)
    {
        if (characterPool != null)
            return characterPool.FindIndex((currCharacter) => { return currCharacter.characterName == character.characterName; });
        return 0;
    }

    void OnEnable()
    {
        instance = this;

        //Load settings (if any) from EditorPrefs
        var data = EditorPrefs.GetString("DialogueBuilder", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);

        if (characterPool == null)
            characterPool = new List<Dialogue.Character>();

        characterFoldout = false;
        dialogueFoldout = true;
    }

    private void OnDisable()
    { 
        //Save settings to EditorPrefs
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("DialogueBuilder", data);
    }

    /// <summary>
    /// Function to open the Dialogue Builder Window
    /// </summary>
    /// <param name="dialogue">The dialogue object to examine and modify</param>
    /// <param name="name">The name of the dialogue instance (trigger name)</param>
    public static void ShowWindow(ref NarrativeTriggerHandler obj, int triggerIndex)
    {
        if(obj == null)
        {
            Debug.Log("Dialogue is being passed as null");
        }
        EditorWindow.GetWindow(typeof(DialogueBuilder), false, "Dialogue Builder");

        DialogueBuilder.instance.currentObject = new SerializedObject(obj);
        DialogueBuilder.instance.currentObject.Update();
        DialogueBuilder.instance.Dialogue = DialogueBuilder.instance.currentObject.FindProperty("triggers").GetArrayElementAtIndex(triggerIndex).FindPropertyRelative("dialogue");
        DialogueBuilder.instance.currentDialogueName = string.Format("Trigger {0}", triggerIndex + 1);
        
    }

    /// <summary>
    /// Defines the GUI for the Dialogue Builder
    /// </summary>
    private void OnGUI()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            Close();

        //Make sure object is properly updated
        currentObject.Update();

        //Start Scroll part
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        //Label to say what Dialogue instance is being edited
        GUILayout.Label(string.Format("Currently Editing: {0}", currentDialogueName), EditorStyles.boldLabel);

        GUILayout.Space(10f);

        //Display Global Settings for this dialogue
        GUILayout.Label("Dialogue Settings", EditorStyles.boldLabel);
        //Should time pause and the player be locked in (forced to click to proceed)
        EditorGUILayout.PropertyField(dialogueSubObject.FindProperty("pauseForDuration"));
        //If not pause for duration, choose a set a set amount of time for the text to be displayed for
        if (dialogueSubObject.FindProperty("pauseForDuration").boolValue == false)
        {
            EditorGUILayout.PropertyField(dialogueSubObject.FindProperty("textDisplayTime"));
        }

        GUILayout.Space(10f);

        //Character foldout. Characters are global
        characterFoldout = EditorGUILayout.Foldout(characterFoldout, "Characters", true, EditorStyles.foldoutHeader);
        if(characterFoldout)
        {
            CharactersFoldoutContent();
        }

        GUILayout.Space(20f);

        //Dialogue Foldout
        dialogueFoldout = EditorGUILayout.Foldout(dialogueFoldout, "Dialogue", true, EditorStyles.foldoutHeader);
        if (dialogueFoldout && characterPool.Count > 0)
        {
            DialogueFoldoutContent();
        }
        //No characters, so can't make any lines (no characters to speak them)
        else if(dialogueFoldout && characterPool.Count == 0)
        {
            EditorGUILayout.HelpBox("Need to add at least one character first", MessageType.Warning);
        }

        GUILayout.Space(20f);

        GUILayout.EndScrollView();

        currentObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Contents of the Character Foldout of the Dialogue Builder
    /// </summary>
    private void CharactersFoldoutContent()
    {
        //if there aren't any characters, there can't be any dialogue
        if (characterPool.Count == 0)
        {
            GUILayout.Label("No characters created! Click \"Add New Character\" to begin.");
        }

        for (int i = 0; i < characterPool.Count; i++)
        {

            GUILayout.BeginHorizontal();
            //If the name is empty, put [no name] for this label
            GUILayout.Label(characterPool[i].characterName == "" ? "[no name]" : characterPool[i].characterName, EditorStyles.boldLabel);
            if (GUILayout.Button("Delete Character"))
            {
                characterPool.RemoveAt(i);
                return;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            //Name Field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name ");
            characterPool[i].characterName = GUILayout.TextField(characterPool[i].characterName, 24);
            GUILayout.EndHorizontal();

            //Text Color Field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Text Color ");
            characterPool[i].textColor = EditorGUILayout.ColorField(characterPool[i].textColor);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        //Create new character button
        if (GUILayout.Button("Add New Character"))
        {
            characterPool.Add(new Dialogue.Character());
        }
    }

    private void DialogueFoldoutContent()
    {
        //If no characters, send out a warning
        if (characterPool.Count == 0)
        {
            EditorGUILayout.HelpBox("Need to add at least one character first", MessageType.Warning);
            return;
        }

        //Enough Characters but no lines, instruct user to click add button
        if (dialogueSubObject.FindProperty("lines").arraySize == 0)
        {
            GUILayout.Label("No Lines Added! Click \"Add New Line\" to begin");
        }
        else
        {
            EditorGUILayout.HelpBox("Text Effect Tag Format ex:) <EffectName>Text</EffectName>", MessageType.Info);
            EditorGUILayout.HelpBox("Available Effects: shaky, typewriter, rainbow", MessageType.Info);
        }

        //Display each of the lines.
        for (int i = 0; i < dialogueSubObject.FindProperty("lines").arraySize; i++)
        {
            GUILayout.Space(5f);

            currentLine = dialogueSubObject.FindProperty("lines").GetArrayElementAtIndex(i);

            //Line Tital and Delete Line Option
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Line {0}", i + 1));
            if (GUILayout.Button("Delete Line"))
            {
                dialogueSubObject.Update();
                selectedCharacterIndexes.RemoveAt(i);
                dialogueSubObject.FindProperty("lines").DeleteArrayElementAtIndex(i);
                dialogueSubObject.ApplyModifiedProperties();
                break;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            //Character Section
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Character", "The character speaking this line"));
            //Generate possible character options from character pool
            List<string> options = new List<string>();
            characterPool.ForEach((character) => { options.Add(character.characterName); });

            selectedCharacterIndexes[i] = EditorGUILayout.Popup(selectedCharacterIndexes[i], options.ToArray());

            //Find character object in character pool from selection in dropdown menu
            Dialogue.Character characterFound = GetCharacterFromIndex(selectedCharacterIndexes[i]);
            if (characterFound == null)
                characterFound = characterPool[0];

            //Assign the character in character pool back to the dialogue object on the narrative trigger handler
            currentLine.FindPropertyRelative("character").FindPropertyRelative("characterName").stringValue = characterFound.characterName;
            currentLine.FindPropertyRelative("character").FindPropertyRelative("imgPath").stringValue = characterFound.imgPath;
            currentLine.FindPropertyRelative("character").FindPropertyRelative("textColor").colorValue = characterFound.textColor;
            dialogueSubObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();

            //Text to say in this line
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Text ", "The text to be spoken"));
            currentLine.FindPropertyRelative("text").stringValue = EditorGUILayout.TextArea(currentLine.FindPropertyRelative("text").stringValue);
            GUILayout.EndHorizontal();

            //FMOD Event Audio to play
            EditorGUILayout.PropertyField(currentLine.FindPropertyRelative("audioToPlay"), new GUIContent("Audio to Play", "The audio to be said along with the line"));

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        if (GUILayout.Button("Add New Line"))
        {
            selectedCharacterIndexes.Add(0);
            dialogueSubObject.Update();
            dialogueSubObject.FindProperty("lines").InsertArrayElementAtIndex(dialogueSubObject.FindProperty("lines").arraySize);
            dialogueSubObject.ApplyModifiedProperties();
        }
    }
}
