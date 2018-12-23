using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SpellTag", menuName = "Tag/Spell Tag")]
public class SpellTag : ScriptableObject, System.IComparable<SpellTag>
{
    public string displayName = string.Empty;

    public int CompareTo(SpellTag other)
    {
        return name.CompareTo(other.name);
    }

    [System.Serializable]
    public class TagSet : SerializableSet<SpellTag>
    {
        public bool Contains(string tagName)
        {
            return Contains(SpellTagIndex.getTagFromString(tagName));
        }
#if UNITY_EDITOR
        public void doGUILayout(string title)
        {
            #region Object Picker Message Handling
            Event e = Event.current;
            if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
            {
                SpellTag t = EditorGUIUtility.GetObjectPickerObject() as SpellTag;
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
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<SpellTag>(null, false, "", 1);
            GUILayout.EndHorizontal();
            #endregion

            EditorGUI.indentLevel++;
            SpellTag toDelete = null;
            SpellTag[] tags = Items;
            System.Array.Sort(tags);
            foreach (var tag in tags)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.name + " (" + tag.displayName + ")", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic }, GUILayout.Width(240));
                if (GUILayout.Button("-"))
                    toDelete = tag;
                EditorGUILayout.EndHorizontal();
            }
            if (tags != null)
                Remove(toDelete);
            EditorGUI.indentLevel--;
        }
#endif
    }
}
