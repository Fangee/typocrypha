using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages cutscenes
public class CutsceneManager : MonoBehaviour {
	public static CutsceneManager main = null; // static global ref
	public Text display_text; // text where dialogue will be displayed
	public Text name_text; // text where name will be displayed
	public SpriteRenderer sprite_holder; // where speaking character sprite will go
	public TextScroll text_scroll; // displays text character by character
	public bool at_end; // is dialogue at end of current scene?
	public bool battle_interrupt; // is the current scene a battle cutscene?

	int curr_line; // current line of dialogue
	CutScene scene; // cutscene object

	void Awake() {
		if (main == null) main = this;
	}

	// start cutscene
	public void startCutscene(CutScene i_scene) {
		at_end = false;
		scene = i_scene;
		curr_line = 0; // reset dialogue
		nextLine ();
	}

	void Update () {
		// go to next line if space is pressed
		if (Input.GetKeyDown (KeyCode.Space)) {
			// if dialogue over, go to next scene
			if (!nextLine ()) {
				at_end = true;
				if (!battle_interrupt)
					StateManager.main.nextScene ();
			}
		}
	}

	// displays next line of text; returns false if at end
	bool nextLine() {
		// check if dialogue is being printed
		if (!text_scroll.is_print) {
			if (curr_line >= scene.dialogue.Length) return false;
			name_text.text = scene.whos_talking [curr_line]; // show name of speaker
			clearNPCSprite(); // clear old sprites
			for (int i = 0; i < scene.npc_sprites[curr_line].Length; ++i) {
				Sprite npc_sprite = // set sprite of current speaker
					Resources.Load<Sprite>("sprites/" + scene.npc_sprites[curr_line][i].Trim());
				displayNPCSprite (npc_sprite, scene.npc_pos[curr_line][i]);
			}
			if (curr_line != 0) // play advance dialogue sfx
				AudioPlayer.main.playSFX(0, SFXType.UI, "sfx_next_textbox"); 
			AudioPlayer.main.playMusic(MusicType.CUTSCENE, scene.music_tracks[curr_line]);
			text_scroll.startPrint (scene.dialogue [curr_line], display_text, "speak_boop");
			++curr_line;
		} else { // dump if dialogue already started
			text_scroll.dump ();
		}
		return true;
	}

	// displays a sprite at given position; sprite becomes a child object of 'sprite_holder'
	void displayNPCSprite(Sprite npc_sprite, Vector2 pos) {
		GameObject display = new GameObject (); // make a new sprite holder
		display.transform.SetParent (sprite_holder.gameObject.transform);
		display.name = "NPCSprite" + display.transform.GetSiblingIndex ();
		Debug.Log (pos.x + " " + pos.y);
		display.transform.position = pos;
		SpriteRenderer sprite_r = display.AddComponent<SpriteRenderer> ();
		sprite_r.sprite = npc_sprite;
	}

	// clears all currently displayed npc sprites
	void clearNPCSprite() {
		foreach (Transform child in sprite_holder.gameObject.transform)
			GameObject.Destroy (child.gameObject);
	}
}
