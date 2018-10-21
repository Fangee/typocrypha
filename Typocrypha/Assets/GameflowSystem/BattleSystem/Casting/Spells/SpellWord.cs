using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class SpellWord : ScriptableObject {

    public enum WordType
    {
        Root,
        Modifier,
    }

    public abstract WordType Type { get; }
    public string description;          //Spell's description (in spellbook)
    public string animationID;          //Spell's animation ID 
    public string sfxID;                //Spell's sfx ID

    public void doBaseGUILayout()
    {
        animationID = EditorGUILayout.TextField(new GUIContent("Animation ID"), animationID);
        sfxID = EditorGUILayout.TextField(new GUIContent("Sfx ID"), sfxID);
        GUILayout.Label(new GUIContent("Description"), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold} );
        description = EditorGUILayout.TextArea(description, new GUIStyle(GUI.skin.textArea) { wordWrap = true }, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
    }

    [System.Serializable] public class SpellTagSet : SerializableSet<string>
    {
        private string addField;
        public void doGUILayout(string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + Count, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(100));
            addField = EditorGUILayout.TextField(addField, GUILayout.Width(100));
            if (GUILayout.Button("+") && !string.IsNullOrEmpty(addField))
            {
                Add(addField);
                addField = string.Empty;
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            string toDelete = null; // Item to delete; -1 if none chosen
            foreach (var item in this)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item, GUILayout.Width(204));
                if (GUILayout.Button("-"))
                    toDelete = item;
                GUILayout.EndHorizontal();
            }
            if (toDelete != null)
                Remove(toDelete);
            EditorGUI.indentLevel--;
        }
    }
}
