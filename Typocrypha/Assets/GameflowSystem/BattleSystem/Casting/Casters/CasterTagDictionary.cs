using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CasterTagDictionary
{
    private bool showDetails = false;
    //[SerializeField] private CasterTag.TagDict subTags;
    [SerializeField] private CasterTag.TagDict tags = new CasterTag.TagDict();
    [SerializeField] private TagMultiSet allTags = new TagMultiSet();

    #region Dictionary Functions
    public void Add(CasterTag tag)
    {
        if (tags.ContainsKey(tag.name))
        {
            Debug.LogWarning("Cannot Add Duplicate caster tag: " + tag.name);
            return;
        }
        tags.Add(tag.name, tag);
    }
    public void Remove(string tagName)
    {
        tags.Remove(tagName);
        //TODO: Handle SUBTAGS
    }

    public bool ContainsTag(string tagName)
    {
        return tags.ContainsKey(tagName);//|| subTags.ContainsKey(tagName);
    }
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
            Add(t);
            e.Use();
            return;
        }
        #endregion

        GUILayout.BeginHorizontal();
        GUILayout.Label(title + ": " + tags.Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        showDetails = GUILayout.Toggle(showDetails, new GUIContent("Show Details"));
        System.Random rnd = new System.Random();
        if (GUILayout.Button("+"))
            EditorGUIUtility.ShowObjectPicker<CasterTag>(null, false, "", 1);
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        string toDelete = null; // Item to delete; -1 if none chosen
        foreach (var tag in tags)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tag.Key, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic }, GUILayout.Width(240));
            if (GUILayout.Button("-"))
                toDelete = tag.Key;
            EditorGUILayout.EndHorizontal();
            if (showDetails)
                tag.Value.dataGUILayout();
        }
        if (toDelete != null)
            Remove(toDelete);
        EditorGUI.indentLevel--;
    }

    [System.Serializable] private class TagMultiSet : SerializableMultiSet<CasterTag> { }
}
