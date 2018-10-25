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

    public void doBaseGUILayout()
    {
        GUILayout.Label(new GUIContent("Description"), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold} );
        description = EditorGUILayout.TextArea(description, new GUIStyle(GUI.skin.textArea) { wordWrap = true }, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
    }
}
