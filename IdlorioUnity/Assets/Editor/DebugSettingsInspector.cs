using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugSettings))]
public class DebugSettingsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DebugSettings settings = (DebugSettings)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < settings._debugToggles.Length; ++i)
        {
            settings._debugToggles[i] = EditorGUILayout.ToggleLeft(((DebugKit)i).ToString(), settings._debugToggles[i]);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
}
