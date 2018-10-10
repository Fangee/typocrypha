using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu] // attribute allows creation of EnemyData assets in unity editor
public class EnemyData : ScriptableObject
{
	// Enemy sprite
	public Sprite image;

    public CasterStats stats;
		
	// AI settings
	public string AIType;
	public string[] AIParameters;
	
	// Spells
	public SpellMap spells;

    // Tags
    public CasterTagDictionary tags;
    //public CasterTagDictionary tags;

    // Serializable SpellMap Dictionary
    [System.Serializable] public class SpellMap : SerializableDictionary<string, SpellData[]> {}
}

#region GUI
// CharacterData inspector (read-only)
[CustomEditor(typeof(EnemyData))]
public class EnemyDataInspector : Editor
{

    public override void OnInspectorGUI()
    {
        EnemyData data = target as EnemyData;

        GUILayout.Label("Enemy: " + data.name);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.image = EditorGUILayout.ObjectField(data.image, typeof(Sprite), false) as Sprite;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.stats.doGUILayout();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.tags.doGUILayout("Tags");

        if(GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
#endregion
