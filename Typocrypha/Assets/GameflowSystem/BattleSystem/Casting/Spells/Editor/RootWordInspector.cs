using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

// CharacterData inspector (read-only)
[CustomEditor(typeof(RootWord))]
public class RootWordInspector : Editor
{
    public override void OnInspectorGUI()
    {
        RootWord data = target as RootWord;
        if (data.effects == null)
            data.initList();

        GUILayout.Label("Root Word: " + data.name);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //Do animation and Description GUI
        data.doBaseGUILayout();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.effects.doLayoutList();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.tags.doGUILayout("Tags");

        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
