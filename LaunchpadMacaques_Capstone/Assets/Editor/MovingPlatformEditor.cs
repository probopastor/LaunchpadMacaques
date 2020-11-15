﻿/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * MovingPlatformEditor.cs
 * This script defines the editor for the Moving Platform asset, namely the clickable buttons that 
 * can quickly set and edit the points the platform will travel to
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security;
using FMODUnity;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    MovingPlatform platformScript;

    private void OnEnable()
    {
        platformScript = (MovingPlatform)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty pointsArray = serializedObject.FindProperty("points");
        
        if(pointsArray.arraySize < 2)
        {
            EditorGUILayout.HelpBox("Not Enough Points: Need at least 2 to function", MessageType.Warning);
        }

        EditorGUILayout.PropertyField(pointsArray, true);

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Current Position"))
        {
            int newIndex = pointsArray.arraySize;
            pointsArray.InsertArrayElementAtIndex(newIndex);

            serializedObject.ApplyModifiedProperties();

            platformScript.SetPoint(newIndex, platformScript.transform.position);

            serializedObject.ApplyModifiedProperties();
        }

        if(GUILayout.Button("Remove Last Point") && pointsArray.arraySize > 0)
        {
            pointsArray.DeleteArrayElementAtIndex(pointsArray.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Movement Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("holdTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("loopPattern"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Ease In And Out", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("easeInAndOut"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("easeDistance"));


        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}