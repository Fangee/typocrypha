using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CharacterData inspector (read-only)
[CustomEditor(typeof(EnemyData))]
public class EnemyDataInspector : Editor
{

    public override void OnInspectorGUI()
    {
        EnemyData data = target as EnemyData;

        GUILayout.Label("Enemy: " + data.name + " (" + data.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), data.displayName);
        data.image = EditorGUILayout.ObjectField(new GUIContent("Sprite"), data.image, typeof(Sprite), false) as Sprite;
        data.AIType = EditorGUILayout.TextField(new GUIContent("AI Type (temp)"), data.AIType);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.stats.doGUILayout();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.tags.doGUILayout("Tags");
        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
