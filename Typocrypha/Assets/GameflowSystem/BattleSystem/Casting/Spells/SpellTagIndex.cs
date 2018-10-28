using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SpellTagIndex
{
    private static readonly string path = "ScriptableObjects/SpellTags";
    private static readonly Dictionary<string, SpellTag> allTags = build();
    private static Dictionary<string, SpellTag> build()
    {
        List<SpellTag> tags = AssetUtils.GetAssetList<SpellTag>(path); 
        Dictionary<string, SpellTag> _allTags = new Dictionary<string, SpellTag>();
        foreach (SpellTag tag in tags)
        {
            _allTags.Add(tag.name, tag);
        }
        return _allTags;
    }
    public static SpellTag getTagFromString(string key)
    {
#if DEBUG
        if (!allTags.ContainsKey(key))
            throw new System.ArgumentOutOfRangeException("key: " + key + " is not a valid SpellTag");
#endif
        SpellTag tag = allTags[key];
#if UNITY_EDITOR
        if (tag.name != key)
        {
            allTags.Remove(key);
            allTags.Add(tag.name, tag);
            throw new System.Exception("key: " + key + " is no longer valid due to rename. It should now be: " + tag.name);
        }
#endif
        return tag;
    }
}

