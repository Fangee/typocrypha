using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueItemConditionalJump))]
[CanEditMultipleObjects]
public class DialogueItemConditionalJumpEditor : Editor
{
    SerializedProperty conditionKey;
    SerializedProperty conditionCases;
    SerializedProperty targetsProp;
    void OnEnable()
    {
        conditionKey = serializedObject.FindProperty("conditionKey");
        conditionCases = serializedObject.FindProperty("conditionCases");
        targetsProp = serializedObject.FindProperty("targets");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(conditionKey);
        EditorGUILayout.PropertyField(conditionCases, true);
        EditorGUILayout.PropertyField(targetsProp, true);
        serializedObject.ApplyModifiedProperties();
    }
}
