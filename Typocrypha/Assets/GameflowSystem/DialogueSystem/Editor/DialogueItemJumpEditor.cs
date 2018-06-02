using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueItemJump))]
[CanEditMultipleObjects]
public class DialogueItemEditor : Editor
{
    SerializedProperty targetProp;
    void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(targetProp, true);
        serializedObject.ApplyModifiedProperties();
    }
}