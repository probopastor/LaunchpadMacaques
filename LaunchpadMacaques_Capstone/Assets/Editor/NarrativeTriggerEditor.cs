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
using Cinemachine;

[CustomEditor(typeof(NarrativeTriggerHandler)), CanEditMultipleObjects]
public class NarrativeTriggerEditor : Editor
{
    NarrativeTriggerHandler narrativeTriggerHandler;
    //The foldout that contains every trigger
    bool triggerMainFoldout = true;

    private void OnEnable()
    {
        narrativeTriggerHandler = (NarrativeTriggerHandler)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorStyles.textField.wordWrap = true;

        //Random Trigger Settings (Universal)
        EditorGUILayout.LabelField("Global Trigger Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Random Trigger Settings", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomIntervalMin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomIntervalMax"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomCancelChance"));
        //Player Hitting Ground Event Settings (Universal)
        EditorGUILayout.LabelField("Player Hitting Ground Settings", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fallTime"));
        EditorGUILayout.LabelField("Look At Object Event Settings", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAtObjectCheckDistance"));
        if (serializedObject.FindProperty("randomIntervalMin").floatValue > serializedObject.FindProperty("randomIntervalMax").floatValue)
        {
            EditorGUILayout.HelpBox("randomIntervalMax must be bigger than or equal to randomIntervalMin", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        SerializedProperty array = serializedObject.FindProperty("triggers");
        SerializedProperty arraySub = serializedObject.FindProperty("triggerSubFoldout");
        SerializedProperty arrayNames = serializedObject.FindProperty("triggerNames");

        if (array.arraySize > 0)
            triggerMainFoldout = EditorGUILayout.Foldout(triggerMainFoldout, "Triggers", true);
        //If the main foldout is out and all triggers are being shown
        if (triggerMainFoldout)
        {
            EditorGUI.indentLevel++;
            //Iterate through and display each trigger in a foldout
            for (int i = 0; i < array.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                arraySub.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(arraySub.GetArrayElementAtIndex(i).boolValue, arrayNames.GetArrayElementAtIndex(i).stringValue);
                if (GUILayout.Button("Delete Trigger"))
                {
                    if (narrativeTriggerHandler.triggers[i].areaTrigger != null)
                    {
                        DestroyImmediate(narrativeTriggerHandler.triggers[i].areaTrigger);
                    }

                    array.DeleteArrayElementAtIndex(i);

                    arrayNames.DeleteArrayElementAtIndex(i);
                    //triggerNames.RemoveAt(i);
                    arraySub.DeleteArrayElementAtIndex(i);
                    //triggerSubFoldout.RemoveAt(i);
                    serializedObject.ApplyModifiedProperties();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                //If the subfoldout should be out, display all the details for this trigger
                if (arraySub.GetArrayElementAtIndex(i).boolValue)
                {

                    SerializedProperty element = array.GetArrayElementAtIndex(i);
                    EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("type"));
                    //Time in level triggers should NEVER be repeatable as it only occurs once
                    if (!((element.FindPropertyRelative("type").enumValueIndex == (int)NarrativeTriggerHandler.TriggerType.OnEvent) &&
                       (element.FindPropertyRelative("eventType").enumValueIndex == (int)NarrativeTriggerHandler.EventType.TimeInLevel)))
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("repeatable"));

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Text Options", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Edit Dialogue"))
                    {
                        serializedObject.Update();
                        if (element.FindPropertyRelative("dialogue").objectReferenceValue == null /*|| element.FindPropertyRelative("dialogue").objectReferenceValue != null*/)
                        {
                            Debug.Log("Creating new dialogue");
                            element.FindPropertyRelative("dialogue").objectReferenceValue = ScriptableObject.CreateInstance<Dialogue>();
                            serializedObject.ApplyModifiedProperties();
                        }
                        DialogueBuilder.ShowWindow(ref narrativeTriggerHandler, i);
                    }
                    if (GUILayout.Button("Reset Dialogue"))
                    {
                        serializedObject.Update();
                        element.FindPropertyRelative("dialogue").objectReferenceValue = ScriptableObject.CreateInstance<Dialogue>();
                        serializedObject.ApplyModifiedProperties();
                        DialogueBuilder.CloseWindow();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Camera Options
                    EditorGUILayout.LabelField("Camera Options", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("hasCameraMovement"));
                    if (element.FindPropertyRelative("hasCameraMovement").boolValue == true)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("cameraTime"));
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        //Camera Point
                        if (GUILayout.Button("Spawn Camera Point"))
                        {
                            CinemachineVirtualCamera virtualCam = SpawnExclusiveObject(/*property=*/ref element, /*variableToAccess=*/"cameraPoint", /*nameOfObjectToCreate=*/"Camera Point " + (i + 1))
                                .AddComponent<CinemachineVirtualCamera>();

                            //If target already exists, assign it to be looked at
                            GameObject potentialTarget;
                            if ((potentialTarget = (GameObject)element.FindPropertyRelative("cameraTarget").objectReferenceValue) != null)
                            {
                                virtualCam.LookAt = potentialTarget.transform;
                            }
                            //Otherwise don't have it look at anything
                            else
                            {
                                virtualCam.LookAt = null;
                            }

                            virtualCam.AddCinemachineComponent<CinemachineHardLookAt>();
                            virtualCam.m_Lens.FieldOfView = Camera.main.fieldOfView;

                        }
                        //Only create a "Go to" button if an object is already assigned
                        if (element.FindPropertyRelative("cameraPoint").objectReferenceValue != null)
                        {
                            if (GUILayout.Button("Go to Camera Point"))
                            {
                                JumpToObject(narrativeTriggerHandler.triggers[i].cameraPoint);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("cameraPoint"));

                        //Camera Target
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Spawn Camera Target"))
                        {
                            GameObject camTarget = SpawnExclusiveObject(/*property=*/ref element, /*variableToAccess=*/"cameraTarget", /*nameOfObjectToCreate=*/"Camera Target " + (i + 1));

                            //If a virtual cam already exists, properly assign this new target to that camera
                            GameObject potentialVirtualCam;
                            if ((potentialVirtualCam = (GameObject)element.FindPropertyRelative("cameraPoint").objectReferenceValue) != null)
                            {
                                potentialVirtualCam.GetComponent<CinemachineVirtualCamera>().LookAt = camTarget.transform;
                            }
                        }
                        if (element.FindPropertyRelative("cameraTarget").objectReferenceValue != null)
                        {
                            if (GUILayout.Button("Go to Camera Target"))
                            {
                                JumpToObject(narrativeTriggerHandler.triggers[i].cameraTarget);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("cameraTarget"));
                    }
                    //Have Camera Movement has been unchecked, clean up any remaining objects (if any)
                    else
                    {
                        Object current;
                        if ((current = element.FindPropertyRelative("cameraPoint").objectReferenceValue) != null
                            && current.name.Contains("Camera Point"))
                            DestroyImmediate(current);
                        if ((current = element.FindPropertyRelative("cameraTarget").objectReferenceValue) != null
                            && current.name.Contains("Camera Target"))
                            DestroyImmediate(current);
                    }

                    int type = element.FindPropertyRelative("type").enumValueIndex;
                    //Display Area Trigger specific optionsin
                    if (type == (int)NarrativeTriggerHandler.TriggerType.Area)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Area Trigger Options", EditorStyles.boldLabel);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Triggering Tag");
                        element.FindPropertyRelative("triggeringTag").stringValue = EditorGUILayout.TagField(element.FindPropertyRelative("triggeringTag").stringValue);
                        EditorGUILayout.EndHorizontal();

                        //Trigger Zone Buttons
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Spawn Trigger Zone"))
                        {
                            GameObject newAreaTrigger = SpawnExclusiveObject(/*property=*/ref element, /*variableToAccess=*/"areaTrigger", /*nameOfObjectToCreate=*/"Area Trigger " + (i + 1));
                            newAreaTrigger.transform.localScale = new Vector3(5, 5, 5);
                        }
                        if (narrativeTriggerHandler.triggers[i].areaTrigger != null)
                        {
                            if (GUILayout.Button("Go to Trigger"))
                            {
                                JumpToObject(narrativeTriggerHandler.triggers[i].areaTrigger);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("areaCenter"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("boxSize"));
                    }
                    //If not area trigger, remove active trigger object (if any)
                    else
                    {
                        Object current;
                        if ((current = element.FindPropertyRelative("areaTrigger").objectReferenceValue) != null)
                            DestroyImmediate(current);
                    }

                    //Display onEvent Trigger specific options if applicable
                    if (type == (int)NarrativeTriggerHandler.TriggerType.OnEvent)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Event Trigger Options", EditorStyles.boldLabel);
                        SerializedProperty eventType = element.FindPropertyRelative("eventType");
                        EditorGUILayout.PropertyField(eventType);
                        //Time in Level
                        if (eventType.enumValueIndex == (int)NarrativeTriggerHandler.EventType.TimeInLevel)
                        {
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("timeInLevelBeforeTrigger"));
                        }
                        //LookAtObject
                        else if (eventType.enumValueIndex == (int)NarrativeTriggerHandler.EventType.LookAtObject)
                        {
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("triggeringObjects"));
                        }
                        //LevelCompleted
                        else if (eventType.enumValueIndex == (int)NarrativeTriggerHandler.EventType.LevelCompleted)
                        {
                            string[] levelNames = new string[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings];
                            for (int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; j++)
                            {
                                levelNames[j] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(j));
                            }

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(new GUIContent("Level", "The level which, upon completion, will activate this trigger"));
                            element.FindPropertyRelative("levelNum").intValue = EditorGUILayout.Popup(element.FindPropertyRelative("levelNum").intValue, levelNames);
                            EditorGUILayout.EndHorizontal();
                        }
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

            //Create new Dialogue
            array.GetArrayElementAtIndex(array.arraySize - 1).FindPropertyRelative("dialogue").objectReferenceValue = ScriptableObject.CreateInstance<Dialogue>();
            serializedObject.ApplyModifiedProperties();


            arrayNames.InsertArrayElementAtIndex(index);
            arrayNames.GetArrayElementAtIndex(index).stringValue = "Trigger " + (index + 1);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.cyan;

        //For area triggers, draw wireframe cubes and give them handles to move around
        for (int i = 0; i < narrativeTriggerHandler.triggers.Length; i++)
        {
            GameObject currentTriggerObject;
            if ((currentTriggerObject = narrativeTriggerHandler.triggers[i].areaTrigger) != null)
            {
                CreateHandlesForObject(/*Object=*/currentTriggerObject, /*labelName=*/string.Format("Trigger {0} Area", i + 1),
                    /*includePosition=*/true, /*includeRotation=*/true, /*includeScale=*/true);
                narrativeTriggerHandler.triggers[i].areaCenter = currentTriggerObject.transform.position;
                narrativeTriggerHandler.triggers[i].boxSize = currentTriggerObject.transform.localScale;
            }

            if ((currentTriggerObject = narrativeTriggerHandler.triggers[i].cameraPoint) != null)
            {
                CreateHandlesForObject(/*Object=*/currentTriggerObject, /*labelName=*/string.Format("Trigger {0} Cam Point", i + 1),
                    /*includePosition=*/true, /*includeRotation=*/false, /*includeScale=*/false);
            }

            if ((currentTriggerObject = narrativeTriggerHandler.triggers[i].cameraTarget) != null)
            {
                CreateHandlesForObject(/*Object=*/currentTriggerObject, /*labelName=*/string.Format("Trigger {0} Cam Target", i + 1),
                    /*includePosition=*/true, /*includeRotation=*/false, /*includeScale=*/false);
            }
        }

    }

    /// <summary>
    /// Spawns a new GameObject named [(string) nameOfCreatedObject], and sets it to variable [(SerializedObject) property].[(string) objectVariableName]. Destroys prexisting object if any are present
    /// </summary>
    /// <param name="property">The SerializedProperty to access and check object on</param>
    /// <param name="objectVariableName">The string variable name to access on the property</param>
    /// <param name="nameOfCreatedObject">The name of the new object that will be created</param>
    /// <returns></returns>
    private GameObject SpawnExclusiveObject(ref SerializedProperty property, string objectVariableName, string nameOfCreatedObject)
    {
        //Destroy old object (if any)
        if (property.FindPropertyRelative(objectVariableName).objectReferenceValue != null)
        {
            DestroyImmediate(property.FindPropertyRelative(objectVariableName).objectReferenceValue);
        }

        //Create point
        Vector3 spawnPoint = Selection.activeGameObject.transform.position;
        spawnPoint.y -= 2 * Selection.activeGameObject.transform.localScale.y;

        GameObject newCameraPoint = new GameObject(nameOfCreatedObject);
        newCameraPoint.transform.position = spawnPoint;
        property.FindPropertyRelative(objectVariableName).objectReferenceValue = newCameraPoint;
        serializedObject.ApplyModifiedProperties();

        //Move Camera/Selection
        Selection.activeGameObject = newCameraPoint;
        SceneView.FrameLastActiveSceneView();
        Selection.activeGameObject = narrativeTriggerHandler.gameObject;

        return newCameraPoint;
    }

    /// <summary>
    /// Jumps the camera to the given GameObject's position without switching the targeting
    /// </summary>
    /// <param name="obj">The GameObject to jump to</param>
    private void JumpToObject(GameObject obj)
    {
        Selection.activeGameObject = obj;
        SceneView.FrameLastActiveSceneView();
        Selection.activeGameObject = narrativeTriggerHandler.gameObject;
    }

    /// <summary>
    /// Creates a wireframe cube and control handles for a given object
    /// </summary>
    /// <param name="obj">The GameObject to affect</param>
    /// <param name="labelName">The name of the label to be placed on the object</param>
    /// <param name="includePosition">If set to true, position handles will be added</param>
    /// <param name="includeRotation">If set to true, rotation handles will be added</param>
    /// <param name="includeScale">If set to true, scaling handles will be added</param>
    private void CreateHandlesForObject(GameObject obj, string labelName, bool includePosition = true, bool includeRotation = false, bool includeScale = false)
    {
        Vector3 position = Vector3.zero, scale = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        EditorGUI.BeginChangeCheck();

        Handles.DrawWireCube(obj.transform.position, obj.transform.localScale);
        Vector3 positionforLabel = obj.transform.position;
        positionforLabel.y += obj.transform.localScale.y / 2;

        GUIStyle style = new GUIStyle();
        style.richText = true;
        labelName = string.Format("<color=yellow>{0}</color>", labelName);

        Handles.Label(positionforLabel, labelName, style);
        if (includePosition)
            position = Handles.PositionHandle(obj.transform.position, Quaternion.identity);
        if (includeScale)
            scale = Handles.ScaleHandle(obj.transform.localScale, obj.transform.position, Quaternion.identity, 10);
        if (includeRotation)
            rotation = Handles.RotationHandle(obj.transform.localRotation, obj.transform.position);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Trigger Properties");
            if (includePosition)
                obj.transform.position = position;
            if (includeScale)
                obj.transform.localScale = scale;
            if (includeRotation)
                obj.transform.localRotation = rotation;
        }
    }
}
