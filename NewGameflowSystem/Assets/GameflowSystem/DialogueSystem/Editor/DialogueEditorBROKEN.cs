using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;

// BROKEN DONT USE
/*
// Editor window for implementing dialogue
public class DialogueEditorBROKEN : Editor {
	Dialogue curr_dialogue; // Current dialogue being edited
	List<AnimBool> box_fade = new List<AnimBool>();
	Vector2 scroll_pos = Vector2.zero;

	// Add menu item for the dialogue editor
	[MenuItem ("Window/Dialogue Editor")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(DialogueEditor));
	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();
		scroll_pos = EditorGUILayout.BeginScrollView (scroll_pos, GUILayout.ExpandWidth(true));
		// View for base options
		EditorGUILayout.BeginVertical();
		curr_dialogue = (Dialogue)EditorGUILayout.ObjectField ("Dialogue", curr_dialogue, typeof(Dialogue), false);
		curr_dialogue = (Dialogue)target;
		if (curr_dialogue != null) {
			if (GUILayout.Button ("New Line", GUILayout.ExpandWidth (false)))
				createDialogueItem ();
			// Update AnimBools for fading effect
			while (box_fade.Count < curr_dialogue.lines.Count()) {
				AnimBool abool = new AnimBool ();
				abool.valueChanged.AddListener (Repaint);
				box_fade.Add (abool);
			}
		}
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();
		// View for lines of dialogue 
		if (curr_dialogue != null) {
			for (int i = 0; i < curr_dialogue.lines.Count(); ++i)
				createDialogueItemMenu (i);
		}
		EditorGUILayout.EndScrollView ();
		serializedObject.ApplyModifiedProperties ();
	}

	void OnHiearchyChange() {
		curr_dialogue = null;
	}

	// Create a new dialogue item
	void createDialogueItem() {
		DialogueItem d_item = ScriptableObject.CreateInstance<DialogueItem> ();
		string path = AssetDatabase.GetAssetPath (curr_dialogue);
		string parent = path.Substring (0, path.LastIndexOf('/'));
		string child = path.Substring (path.LastIndexOf ('/') + 1, path.LastIndexOf('.') - path.LastIndexOf ('/') - 1);
		string new_path = parent + "/" + child + "Items";
		Debug.Log (parent + "---" + child);
		if (!Directory.Exists(new_path))
			AssetDatabase.CreateFolder (parent, child + "Items");
		AssetDatabase.CreateAsset (d_item, new_path + "/item");
		AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (d_item));
		string guid = AssetDatabase.AssetPathToGUID (AssetDatabase.GetAssetPath (d_item));
		Debug.Log (AssetDatabase.AssetPathToGUID (AssetDatabase.GetAssetPath(d_item)));
		AssetDatabase.MoveAsset (new_path + "/item", new_path + "/item" + guid);
		curr_dialogue.lines[curr_dialogue.count++] = d_item;

		//AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (curr_dialogue));

	}

	// Removes a dialogue item (at index i of list)
	void RemoveDialogueItem(int i) {
		AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath(curr_dialogue.lines [i]));
		curr_dialogue.lines[i] = null;
		Repaint ();
	}

	// Create menu for a single dialogue item
	void createDialogueItemMenu(int i) {
		if (curr_dialogue.lines [i] == null) {
			RemoveDialogueItem (i);
			return;
		}
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Line:" + i, GUILayout.ExpandWidth(false));
		box_fade [i].target = EditorGUILayout.ToggleLeft ("", box_fade[i].target, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal ();

		if (EditorGUILayout.BeginFadeGroup (box_fade [i].faded)) {
			EditorGUI.indentLevel++;
			curr_dialogue.lines [i].speaker_name = EditorGUILayout.TextField ("Speaker Name", curr_dialogue.lines [i].speaker_name, GUILayout.MaxWidth (256));
			curr_dialogue.lines [i].left_icon = (Sprite)EditorGUILayout.ObjectField ("Left Icon", curr_dialogue.lines [i].left_icon, typeof(Sprite), false, GUILayout.ExpandWidth (false));
			curr_dialogue.lines [i].right_icon = (Sprite)EditorGUILayout.ObjectField ("Right Icon", curr_dialogue.lines [i].right_icon, typeof(Sprite), false, GUILayout.ExpandWidth (false));
			curr_dialogue.lines [i].icon_side = (IconSide)EditorGUILayout.EnumPopup ("Icon side", curr_dialogue.lines [i].icon_side, GUILayout.ExpandWidth (false));
			curr_dialogue.lines [i].text = EditorGUILayout.TextArea (curr_dialogue.lines [i].text, GUILayout.MinHeight (128), GUILayout.MaxWidth(256));
			EditorGUI.indentLevel--;
		} 
		EditorGUILayout.EndFadeGroup ();
		if (GUILayout.Button ("Delete", GUILayout.ExpandWidth (false)))
			RemoveDialogueItem (i);
		EditorGUILayout.Space ();
	}
}
*/
