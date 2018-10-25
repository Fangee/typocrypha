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

[CreateAssetMenu(fileName = "CasterTag", menuName = "Tag/Caster Tag")]
public class CasterTag : ScriptableObject
{
    public string displayName = string.Empty;
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

    [System.Serializable]
    public class Reaction
    {
        public SpellTag reactTo;
        public ReactionType reactionType = ReactionType.NEUTRAL;
    }
    [System.Serializable] public class ReactionDict : SerializableDictionary<string, Reaction>
    {
        public void doGUILayout(string title)
        {
            #region Object Picker Message Handling
            Event e = Event.current;
            if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
            {
                SpellTag t = EditorGUIUtility.GetObjectPickerObject() as SpellTag;
                if (t == null)
                    return;
                Add(t.name, new Reaction() { reactTo = t});
                e.Use();
                return;
            }
            #endregion

            #region Title and Controls
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<SpellTag>(null, false, "", 1);
            GUILayout.EndHorizontal();
            #endregion

            EditorGUI.indentLevel++;
            string toDelete = null; // Item to delete; -1 if none chosen
            string[] keys = new string[Count];
            Keys.CopyTo(keys, 0);
            System.Array.Sort(keys);
            List<string> toReplace = new List<string>();
            foreach (var key in keys)
            {
                if (key != this[key].reactTo.name)
                    toReplace.Add(key);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key, GUILayout.Width(100));
                GUILayout.BeginVertical();
                GUILayout.Space(3);
                this[key].reactionType = (ReactionType)EditorGUILayout.EnumPopup(this[key].reactionType, GUILayout.Width(150), GUILayout.Height(EditorGUIUtility.singleLineHeight - 3));
                GUILayout.EndVertical();
                if (GUILayout.Button("-"))
                    toDelete = key;
                GUILayout.EndHorizontal();
            }
            foreach (var key in toReplace)
            {
                if (key == toDelete)
                    continue;
                Reaction r = this[key];
                Remove(key);
                Add(r.reactTo.name, r);
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
        GUILayout.Label("Caster Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
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


