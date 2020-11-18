/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * NarrativeTriggerEditor.cs
 * This script handles the custom inspector for NarrativeTriggerHandler.cs, adds some buttons for easier access as well as
 * hides certain items from a trigger if they aren't relevant
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NarrativeTriggerHandler))]
public class NarrativeTriggerEditor : Editor
{
    NarrativeTriggerHandler narrativeTriggerHandler;
    bool triggerMainFoldout = true;

    private void OnEnable()
    {
        narrativeTriggerHandler = (NarrativeTriggerHandler)target;

        //for(int i = 0; i < narrativeTriggerHandler.triggers.Length; i++)
        //{
        //    triggerSubFoldout[i] = new bool();
        //    triggerNames[i] = "Trigger " + (i + 1);
        //}
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Random Trigger Settings (Universal)
        EditorGUILayout.LabelField("Global Random Trigger Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomIntervalMin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomIntervalMax"));

        EditorGUILayout.Space();

        SerializedProperty array = serializedObject.FindProperty("triggers");
        SerializedProperty arraySub = serializedObject.FindProperty("triggerSubFoldout");
        SerializedProperty arrayNames = serializedObject.FindProperty("triggerNames");

        if(array.arraySize > 0)
            triggerMainFoldout = EditorGUILayout.Foldout(triggerMainFoldout, "Triggers", true);
        if (triggerMainFoldout)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < array.arraySize; i++)
            {
                Debug.Log(array.arraySize + " | " + arraySub.arraySize + " | " + arrayNames.arraySize);

                EditorGUILayout.BeginHorizontal();

                arraySub.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(arraySub.GetArrayElementAtIndex(i).boolValue, arrayNames.GetArrayElementAtIndex(i).stringValue);
                if(GUILayout.Button("Delete Trigger"))
                {
                    array.DeleteArrayElementAtIndex(i);

                    arrayNames.DeleteArrayElementAtIndex(i);
                    //triggerNames.RemoveAt(i);
                    arraySub.DeleteArrayElementAtIndex(i);
                    //triggerSubFoldout.RemoveAt(i);
                    serializedObject.ApplyModifiedProperties();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                if (arraySub.GetArrayElementAtIndex(i).boolValue)
                {

                    SerializedProperty element = array.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("type"));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Text to Display", GUILayout.Width(120));
                    narrativeTriggerHandler.triggers[i].textToDisplay = EditorGUILayout.TextArea(narrativeTriggerHandler.triggers[i].textToDisplay, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(element.FindPropertyRelative("audioToPlay"));
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("audioSource"));
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("repeatable"));


                    int type = element.FindPropertyRelative("type").enumValueIndex;
                    if (type == (int)NarrativeTriggerHandler.TriggerType.Area)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("triggeringObject"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("areaCenter"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("boxSize"));
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Trigger"))
        {
            int index = array.arraySize;
            array.InsertArrayElementAtIndex(index);
            //triggerSubFoldout.Add(new bool());
            arraySub.InsertArrayElementAtIndex(index);

            arrayNames.InsertArrayElementAtIndex(index);
            arrayNames.GetArrayElementAtIndex(index).stringValue = "Trigger " + (index + 1);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
