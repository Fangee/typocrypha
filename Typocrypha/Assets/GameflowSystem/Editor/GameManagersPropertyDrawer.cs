using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Gameflow.GameManagers))]
public class GameManagersDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        Rect UIrect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));

        EditorGUI.LabelField(UIrect, new GUIContent("Managers"), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("dialogManager"));
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("characterManager"));
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("waveManager"));
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("backgroundManager"));
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("textEventManager"));
        UIrect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("playerDataManager"));

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 7;
    }
}
