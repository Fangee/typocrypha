using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages cutscenes
public class CutsceneManager : MonoBehaviour {
	public StateManager state_manager; // manages global state/scenes
	public Text display_text; // text where dialogue will be displayed
	public Text name_text; // text where name will be displayed

	int curr_line; // current line of dialogue
	CutScene scene; // cutscene object

	// start cutscene
	public void startCutscene(CutScene i_scene) {
		scene = i_scene;
		curr_line = -1; // reset dialogue
		Debug.Log("NPCs in cutscene:");
		foreach (string npc in scene.npcs)
			Debug.Log ("  " + npc);
		nextLine ();
	}

	void Update () {
		// go to next line if space is pressed
		if (Input.GetKeyDown (KeyCode.Space)) {
			// if dialogue over, go to next scene
			if (!nextLine ()) state_manager.nextScene ();
		}
	}

	// displays next line of text; returns false if at end
	bool nextLine() {
		if (curr_line >= scene.dialogue.Length - 1)
			return false;
		display_text.text = scene.dialogue [++curr_line];
		name_text.text = scene.whos_talking [curr_line];
		return true;
	}
}
