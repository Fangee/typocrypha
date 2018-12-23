using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpellTag))]
public class SpellTagInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SpellTag tag = target as SpellTag;
        GUILayout.Label("Spell Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
        if (GUI.changed)
            EditorUtility.SetDirty(tag);
    }
}
