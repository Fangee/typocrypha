using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SpellTag", menuName = "Tag/Spell Tag")]
public class SpellTag : ScriptableObject
{
    public string displayName = string.Empty;
    [System.Serializable] public class TagDict : SerializableDictionary<string, SpellTag>
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
                Add(t.name, t);
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
            string toDelete = string.Empty;
            string[] keys = new string[Count];
            Keys.CopyTo(keys, 0);
            System.Array.Sort(keys);
            List<string> toReplace = new List<string>();
            foreach (string key in keys)
            {
                if (key != this[key].name)
                    toReplace.Add(key);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic }, GUILayout.Width(240));
                if (GUILayout.Button("-"))
                    toDelete = key;
                EditorGUILayout.EndHorizontal();
            }
            foreach (var key in toReplace)
            {
                if (key == toDelete)
                    continue;
                SpellTag s = this[key];
                Remove(key);
                Add(s.name, s);
            }
            if (!string.IsNullOrEmpty(toDelete))
                Remove(toDelete);
            EditorGUI.indentLevel--;
        }
    }
}

#region Caster Tag Inspector GUI
// CharacterData inspector (read-only)
[CustomEditor(typeof(SpellTag))]
public class SpellTagInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        SpellTag tag = target as SpellTag;
        GUILayout.Label("Spell Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
        if (GUI.changed)
            EditorUtility.SetDirty(tag);
    }
}
#endregion
