using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnapTo : EditorWindow
{
    public static int gridOffset = 4;

    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<SnapTo>("Edit Mode Functions");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Snap Selected To Grid"))
        {
            SnapSelectedToGrid(Selection.activeTransform);
        }

        if (GUILayout.Button("Snap All To Grid"))
        {
            SnapAllToGrid();
        }
    }

    public static void SnapSelectedToGrid(Transform transform)
    {
        if (transform == null) return;
        
        int roundX = Mathf.FloorToInt(transform.position.x);
        int roundY = Mathf.FloorToInt(transform.position.y);
        int roundZ = Mathf.FloorToInt(transform.position.z);
        Debug.Log(roundX);
        if (roundX % gridOffset != 0)
        {
            if (roundX % gridOffset >= gridOffset / 2) roundX += (gridOffset - (roundX % gridOffset));
            else roundX -= (roundX % gridOffset);
        }

        if (roundY % gridOffset != 0)
        {
            if (roundY % gridOffset >= gridOffset / 2) roundY += (gridOffset - (roundY % gridOffset));
            else roundY -= (roundY % gridOffset);
        }

        if (roundZ % gridOffset != 0)
        {
            if (roundZ % gridOffset >= gridOffset / 2) roundZ += (gridOffset - (roundZ % gridOffset));
            else roundZ -= (roundZ % gridOffset);
        }

        transform.position = new Vector3(Mathf.FloorToInt(roundX), Mathf.FloorToInt(roundY), Mathf.FloorToInt(roundZ));
    }

    public static void SnapAllToGrid()
    {
        SnapsToGrid[] transforms = FindObjectsOfType<SnapsToGrid>();
        foreach (SnapsToGrid snap in transforms)
        {
            SnapSelectedToGrid(snap.transform);
        }
    }
}
