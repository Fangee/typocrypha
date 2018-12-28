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
        float togglewidth = EditorGUIUtility.labelWidth - 4;
        float refWidth = EditorGUIUtility.currentViewWidth - (EditorGUIUtility.labelWidth + 22);
        EnemyData data = target as EnemyData;
        // Info GUI
        GUILayout.Label("Enemy: " + data.name + " (" + data.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // Display Data GUI
        data.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), data.displayName);
        data.image = EditorGUILayout.ObjectField(new GUIContent("Sprite"), data.image, typeof(Sprite), false) as Sprite;
        EditorGUILayout.BeginHorizontal();
        if(data.overrideSpawnAnim = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Anim"), data.overrideSpawnAnim, GUILayout.Width(togglewidth)))
            data.spawnAnim = EditorGUILayout.ObjectField(GUIContent.none, data.spawnAnim, typeof(AnimationClip), false, GUILayout.Width(refWidth)) as AnimationClip;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (data.overrideSpawnSfx = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Sfx"), data.overrideSpawnSfx, GUILayout.Width(togglewidth)))
            data.spawnSfx = EditorGUILayout.ObjectField(GUIContent.none, data.spawnSfx, typeof(AudioClip), false, GUILayout.Width(refWidth)) as AudioClip;
        EditorGUILayout.EndHorizontal();       
        data.deathAnim = (ATB2.Enemy.DeathAnimation)EditorGUILayout.EnumPopup(new GUIContent("Death Animation"), data.deathAnim);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // AI GUI
        data.AIType = EditorGUILayout.TextField(new GUIContent("AI Type (temp)"), data.AIType);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // Stat GUI
        data.stats.doGUILayout();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // Tag GUI
        data.tags.doGUILayout("Tags");
        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
