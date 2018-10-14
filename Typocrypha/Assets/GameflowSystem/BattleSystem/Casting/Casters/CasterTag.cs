using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public enum ReactionType
{
    ANY = -100,
    WEAK = -1,
    NEUTRAL,
    RESIST,
    BLOCK,
    DODGE,
    DRAIN,
    REPEL,
}

[CreateAssetMenu(fileName = "CasterTag", menuName = "Caster Tag")]
public class CasterTag : ScriptableObject
{
    public CasterStats statMods;
    public ReactionDict reactions;
    public TagSet subTags;
    public AbilitySet abilities;

    public void dataGUILayout()
    {
        EditorGUI.indentLevel++;
        statMods.doModifierDataGUILayout();
        if (subTags.Count > 0)
        {
            EditorGUILayout.LabelField("Subtags: ", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(240));
            foreach (var tag in subTags)
                tag.doSubtagNames();
        }
        EditorGUI.indentLevel--;
    }
    private void doSubtagNames()
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(name, new GUIStyle(GUI.skin.label), GUILayout.Width(240));
        foreach (var tag in subTags)
            tag.doSubtagNames();
        EditorGUI.indentLevel--;
    }

    [System.Serializable] public class ReactionDict : SerializableDictionary<string, ReactionType>
    {
        private string addField;
        public void doGUILayout(string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(100));
            addField = EditorGUILayout.TextField(addField, GUILayout.Width(100));
            if (GUILayout.Button("+") && !string.IsNullOrEmpty(addField))
            {
                Add(addField, ReactionType.NEUTRAL);
                addField = string.Empty;
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            string toDelete = null; // Item to delete; -1 if none chosen
            string[] keys = new string[Count];
            Keys.CopyTo(keys, 0);
            System.Array.Sort(keys);
            foreach (var key in keys)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key, GUILayout.Width(100));
                this[key] = (ReactionType)EditorGUILayout.EnumPopup(this[key]);
                if (GUILayout.Button("-"))
                    toDelete = key;
                GUILayout.EndHorizontal();
            }
            if (toDelete != null)
                Remove(toDelete);
            EditorGUI.indentLevel--;
        }
    }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag>
    {
        private bool showDetails = false;
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

            #region Title and Controls
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            showDetails = GUILayout.Toggle(showDetails, new GUIContent("Show Details"));
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<CasterTag>(null, false, "", 1);
            GUILayout.EndHorizontal();
            #endregion

            EditorGUI.indentLevel++;
            CasterTag toDelete = null; // Item to delete; -1 if none chosen
            foreach (var tag in this)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.name, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic }, GUILayout.Width(240));
                if (GUILayout.Button("-"))
                    toDelete = tag;
                EditorGUILayout.EndHorizontal();
                if (showDetails)
                    tag.dataGUILayout();
            }
            if (toDelete != null)
                Remove(toDelete);
            EditorGUI.indentLevel--;
        }
    }
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
        if(tag.statMods != null)
            tag.statMods.doGUILayout(true);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.reactions.doGUILayout("Reactions");
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (tag.subTags != null)
        {
            tag.subTags.doGUILayout("SubTags");
            if (tag.subTags.Contains(tag))
                tag.subTags.Remove(tag);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(tag);
    }
}
#endregion

public class CasterAbility
{

}


