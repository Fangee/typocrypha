using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CasterTag))]
public class CasterTagInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;
        EditorGUILayout.LabelField("Caster Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (tag.statMods != null)
            tag.statMods.doGUILayout(true);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.reactions.doGUILayout("Reactions");
        EditorGUILayout.Space(); // EditorGUIUtility.singleLineHeight / 2
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (tag.subTags != null)
        {
            tag.subTags.doGUILayout("SubTags");
            if (tag.subTags.Contains(tag))
                tag.subTags.Remove(tag);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(tag);
    }
}