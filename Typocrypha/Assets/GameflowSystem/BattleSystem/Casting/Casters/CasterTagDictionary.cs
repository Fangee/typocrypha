using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CasterTagDictionary
{
    private bool showDetails = false;
    //[SerializeField] private CasterTag.TagDict subTags;
    [SerializeField] private TagDict tags = new TagDict();
    [SerializeField] private TagMultiDict allTags = new TagMultiDict();

    #region Dictionary Functions
    public void Add(CasterTag tag)
    {
        if (tags.ContainsKey(tag.name))
        {
            Debug.LogWarning("Cannot Add Duplicate caster tag: " + tag.name);
            return;
        }
        tags.Add(tag.name, tag);
        addWithSubTags(tag);
    }
    private void addWithSubTags(CasterTag tag)
    {
        allTags.Add(tag.name, tag);
        foreach (CasterTag t in tag.subTags)
            addWithSubTags(t);
    }
    public void Remove(string tagName)
    {
        CasterTag tag = tags[tagName];
        tags.Remove(tagName);
        removeWithSubTags(tag);
    }
    private void removeWithSubTags(CasterTag tag)
    {
        allTags.Remove(tag.name);
        foreach (CasterTag t in tag.subTags)
            removeWithSubTags(t);
    }
    public bool ContainsTag(string tagName)
    {
        return allTags.ContainsKey(tagName);
    }
    #endregion

    #region Aggregate Tag Data  (TODO)
    public CasterStats statMod;
    public List<CasterAbility> abilities;
    #endregion

    public void doGUILayout(string title)
    {
        #region Object Picker Message Handling
        Event e = Event.current;
        if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
        {
            CasterTag t = EditorGUIUtility.GetObjectPickerObject() as CasterTag;
            if (t == null)
                return;
            if(allTags.ContainsKey(t.name))
            {
                Debug.LogWarning("Tag: " + t.name + " is already in this dict as a tag or subtag");
                return;
            }
            Add(t);
            e.Use();
            return;
        }
        #endregion

        #region Title and Control
        GUILayout.BeginHorizontal();
        GUILayout.Label(title + ": " + tags.Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        showDetails = GUILayout.Toggle(showDetails, new GUIContent("Show Details"));
        if (GUILayout.Button("+"))
            EditorGUIUtility.ShowObjectPicker<CasterTag>(null, false, "", 1);
        GUILayout.EndHorizontal();
        #endregion

        EditorGUI.indentLevel++;
        string toDelete = null; // Item to delete; -1 if none chosen
        string[] keys = new string[tags.Count];
        tags.Keys.CopyTo(keys, 0);
        System.Array.Sort(keys);
        List<string> toReplace = new List<string>();
        foreach (var key in keys)
        {
            if (key != tags[key].name)
                toReplace.Add(key);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(key, new GUIStyle(GUI.skin.label) { fontStyle = showDetails ? FontStyle.Bold : FontStyle.Italic }, GUILayout.Width(240));
            if (GUILayout.Button("-"))
                toDelete = key;
            EditorGUILayout.EndHorizontal();
            if (showDetails)
                tags[key].dataGUILayout();
        }
        foreach (var key in toReplace)
        {
            if (key == toDelete)
                continue;
            CasterTag t = tags[key];
            Remove(key);
            Add(t);
        }
        toReplace.Clear();
        foreach(var kvp in allTags)
        {
            if (kvp.Key != kvp.Value.name)
                toReplace.Add(kvp.Key);
        }
        foreach (var key in toReplace)
        {
            if (key == toDelete)
                continue;
            CasterTag t = allTags[key];
            int val = allTags.Frequency(key);
            allTags.ClearItem(key);
            allTags.Add(t.name, t, val);
        }
        if (toDelete != null)
            Remove(toDelete);
        EditorGUI.indentLevel--;
    }

    [System.Serializable] private class TagDict : SerializableDictionary<string, CasterTag> { }
    [System.Serializable] private class TagMultiDict : SerializableMultiDictionary<string, CasterTag> { }
}
