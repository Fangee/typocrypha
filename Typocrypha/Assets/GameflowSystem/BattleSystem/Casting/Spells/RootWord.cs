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
    public SpellTag.TagSet tags = new SpellTag.TagSet();

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
