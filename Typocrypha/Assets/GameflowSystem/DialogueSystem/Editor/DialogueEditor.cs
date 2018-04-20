using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
// Memory problem: deleted lines of dialogue aren't actually deleted

/* UNUSED
// Custom editor for Dialogue
[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor {
	// SerializedProperties allow for permanent editing of serialized files
	SerializedProperty count_prop; 
	SerializedProperty lines_prop;
	List<AnimBool> box_fade = new List<AnimBool> (); // Animation variables

	void OnEnable() {
		// Allow us to modify properties
		count_prop = serializedObject.FindProperty ("count");
		lines_prop = serializedObject.FindProperty ("lines");
	}

	public override void OnInspectorGUI () {
		serializedObject.Update (); // Needs to be called at beginning of edit
		EditorGUILayout.PropertyField (count_prop, new GUIContent("Line Count"));
		EditorGUILayout.Space ();
		// Go through array of lines of dialogue
		for (int i = 0; i < count_prop.intValue; ++i) {
			SerializedProperty sp = lines_prop.GetArrayElementAtIndex (i); // Get ith dialogue line
			// Create new (empty) dialogue item if empty
			if (sp.objectReferenceValue == null) {
				DialogueItem new_item = ScriptableObject.CreateInstance<DialogueItem> ();
				AssetDatabase.AddObjectToAsset (new_item, AssetDatabase.GetAssetPath(target));
				sp.objectReferenceValue = new_item;
			}
			// Create animation variable (for style)
			if (box_fade.Count <= i) {
				AnimBool abool = new AnimBool ();
				abool.valueChanged.AddListener (Repaint);
				box_fade.Add (abool);
			}
			createDialogueItemMenu (sp, i); // Create submenu for each line of dialogue
		}
		serializedObject.ApplyModifiedProperties (); // Needs to be called at end of edit
	}

	void createDialogueItemMenu(SerializedProperty sp, int i) {
		GUILayout.BeginHorizontal (); // Draws editor elements left to right
		EditorGUILayout.PropertyField (sp, new GUIContent ("Line " + i), GUILayout.ExpandWidth(false));
		box_fade [i].target = EditorGUILayout.ToggleLeft ("", box_fade[i].target);
		GUILayout.EndHorizontal ();

		SerializedObject item = new SerializedObject (sp.objectReferenceValue);
		item.Update ();
		EditorGUI.indentLevel++;
		if (EditorGUILayout.BeginFadeGroup (box_fade [i].faded)) { // Contains everything in a fading toggle submenu
			// These calls draw the fields, and allow editing
			EditorGUILayout.PropertyField (item.FindProperty ("dialogue_mode"), new GUIContent ("Mode"));
			EditorGUILayout.PropertyField (item.FindProperty ("dialogue_type"), new GUIContent ("Type"));
			EditorGUILayout.PropertyField (item.FindProperty ("speaker_name"), new GUIContent ("Speaker"));
			EditorGUILayout.PropertyField (item.FindProperty ("left_icon"), new GUIContent ("Left Icon"));
			EditorGUILayout.PropertyField (item.FindProperty ("right_icon"), new GUIContent ("Right Icon"));
			EditorGUILayout.PropertyField (item.FindProperty ("icon_side"), new GUIContent ("Icon Side"));
			// Manually applies changes to the 'text' field
			item.FindProperty ("text").stringValue = EditorGUILayout.TextArea (item.FindProperty ("text").stringValue, GUILayout.MinHeight (128));
			// Show input submenu with more options if this line requires user input
			if (item.FindProperty ("dialogue_type").enumValueIndex == (int)DialogueType.INPUT) {
				EditorGUILayout.Space ();
				EditorGUILayout.PropertyField (item.FindProperty ("input_display"), new GUIContent ("Input Display"));
				EditorGUILayout.PropertyField (item.FindProperty ("input_count"), new GUIContent ("Input Count"));
				EditorGUI.indentLevel++;
				int c = item.FindProperty ("input_count").intValue;
				for (int j = 0; j < c; ++j) {
					EditorGUILayout.PropertyField (item.FindProperty ("input_options").GetArrayElementAtIndex(j), new GUIContent ("Option " + j));
					EditorGUILayout.PropertyField (item.FindProperty ("input_branches").GetArrayElementAtIndex(j), new GUIContent ("Branch " + j));
					EditorGUILayout.Space ();
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space ();
		}
		EditorGUILayout.EndFadeGroup ();
		EditorGUI.indentLevel--;
		item.ApplyModifiedProperties ();
	}
}
*/
