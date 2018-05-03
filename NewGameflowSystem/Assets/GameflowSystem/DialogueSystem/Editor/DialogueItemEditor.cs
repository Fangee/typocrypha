using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* UNUSED
// Custom editor for DialogueItems
[CustomEditor(typeof(DialogueItem))]
[CanEditMultipleObjects]
public class DialogueItemEditor : Editor {
	SerializedProperty dialogue_mode_prop;
	SerializedProperty speaker_name_prop;
	SerializedProperty left_icon_prop;
	SerializedProperty right_icon_prop;
	SerializedProperty icon_side_prop;
	SerializedProperty text_prop;

	void OnEnable() {
		dialogue_mode_prop = serializedObject.FindProperty ("dialogue_mode");
		speaker_name_prop = serializedObject.FindProperty ("speaker_name");
		left_icon_prop = serializedObject.FindProperty ("left_icon");
		right_icon_prop = serializedObject.FindProperty ("right_icon");
		icon_side_prop = serializedObject.FindProperty ("icon_side");
		text_prop = serializedObject.FindProperty ("text");
	}
	
	public override void OnInspectorGUI () {
		serializedObject.Update ();
		EditorGUILayout.PropertyField (dialogue_mode_prop, new GUIContent ("Mode"));
		EditorGUILayout.PropertyField (speaker_name_prop, new GUIContent("Speaker"));
		EditorGUILayout.PropertyField (left_icon_prop, new GUIContent("Left Icon"));
		EditorGUILayout.PropertyField (right_icon_prop, new GUIContent("Right Icon"));
		EditorGUILayout.PropertyField (icon_side_prop, new GUIContent("Icon Side"));
		text_prop.stringValue = EditorGUILayout.TextArea (text_prop.stringValue, GUILayout.MinHeight (128));
		serializedObject.ApplyModifiedProperties();
	}
}
*/