using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CreateAssetMenu(fileName = "CasterTag", menuName = "Caster Tag")]
public class CasterTag : ScriptableObject
{
    public CasterStats statMods;
    public ReactionDict tagReactions;
    public TagSet subTags;
    public AbilitySet abilities;

    public void dataGUILayout()
    {
        EditorGUI.indentLevel++;
        statMods.doModifierDataGUILayout();
        EditorGUI.indentLevel--;
    }




    [System.Serializable] public class ReactionDict : SerializableDictionary<SpellTag, Elements.vsElement> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
    [System.Serializable] public class TagDict : SerializableDictionary2<string, CasterTag> { }
    [System.Serializable] public class AbilitySet : SerializableSet<CasterAbility> { }
}

#region Caster Tag Inspector GUI
// CharacterData inspector (read-only)
[CustomEditor(typeof(CasterTag))]
public class CasterTagInspector : Editor
{

    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;

        GUILayout.Label("Caster Tag: " + tag.name);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter});
        tag.statMods.doGUILayout(true);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //data.tags.doGUILayout("Tags");
    }
}
#endregion

[System.Serializable]
public class CasterTagDictionary
{
    private bool showDetails = false;
    [SerializeField] private CasterTag.TagDict subTags;
    [SerializeField] private CasterTag.TagDict tags;

    public CasterTagDictionary()
    {
        tags = new CasterTag.TagDict();
        subTags = new CasterTag.TagDict();
    }

    #region Dictionary Functions
    public void Add(CasterTag tag)
    {
        if (tags.ContainsKey(tag.name))
        {
            Debug.LogWarning("Cannot Add Duplicate caster tag: " + tag.name);
            return;
        }
        tags.Add(tag.name, tag);
        //TODO: Handle SUBTAGS
    }
    public void Remove(string tagName)
    {
        tags.Remove(tagName);
        //TODO: Handle SUBTAGS
    }

    public bool ContainsTag(string tagName)
    {
        return tags.ContainsKey(tagName) || subTags.ContainsKey(tagName);
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
        if (GUILayout.Button("+"))
            EditorGUIUtility.ShowObjectPicker<CasterTag>(null, false, "", 1);
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        string toDelete = null; // Item to delete; -1 if none chosen
        foreach (string tag in tags)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tag, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic }, GUILayout.Width(240));
            if (GUILayout.Button("-"))
                toDelete = tag;
            EditorGUILayout.EndHorizontal();
            if (showDetails)
                tags[tag].dataGUILayout();
        }
        if (toDelete != null)
            Remove(toDelete);
        EditorGUI.indentLevel--;
    }
}

public class SpellTag
{
    string keyword;

}

public class CasterAbility
{

}

