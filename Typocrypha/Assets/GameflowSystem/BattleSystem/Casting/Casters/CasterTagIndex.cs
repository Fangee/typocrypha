using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CasterTagIndex
{
    private static readonly string path = "ScriptableObjects/CasterTags";
    private static readonly Dictionary<string, CasterTag> allTags = build();
    private static Dictionary<string, CasterTag> build()
    {
        List<CasterTag> tags = AssetUtils.GetAssetList<CasterTag>(path);
        Dictionary<string, CasterTag> _allTags = new Dictionary<string, CasterTag>();
        foreach (CasterTag tag in tags)
        {
            _allTags.Add(tag.name, tag);
        }
        return _allTags;
    }
    public static CasterTag getTagFromString(string key)
    {
#if DEBUG
        if (!allTags.ContainsKey(key))
            throw new System.ArgumentOutOfRangeException("key: " + key + " is not a valid CasterTag");
#endif
        CasterTag tag = allTags[key];
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
