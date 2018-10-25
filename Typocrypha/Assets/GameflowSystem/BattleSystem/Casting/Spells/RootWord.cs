using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

[CreateAssetMenu(fileName = "RootWord", menuName = "Spell Word/Root")]
public class RootWord : SpellWord {
    public override WordType Type { get { return WordType.Root; } }
    [SerializeField] private List<RootSpellEffect> _effects = new List<RootSpellEffect>();
    public ReorderableSOList<RootSpellEffect> effects = null;
    public SpellTag.TagDict tags = new SpellTag.TagDict();

    public void initList()
    {
        effects = new ReorderableSOList<RootSpellEffect>(_effects, true, true, new GUIContent("Effects", "TODO: tooltip"));
        effects.processAddedItem = (item) =>
        {
            item.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(item, this);
        };
    }
}

#region GUI
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
#endregion
