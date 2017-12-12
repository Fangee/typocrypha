using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages cutscenes
public class CutsceneManager : MonoBehaviour {
	public static CutsceneManager main = null; // static gloval ref
	public Text display_text; // text where dialogue will be displayed
	public Text name_text; // text where name will be displayed
	public SpriteRenderer sprite_holder; // where speaking character sprite will go
	public TextScroll text_scroll; // displays text character by character

	int curr_line; // current line of dialogue
	CutScene scene; // cutscene object

	void Awake() {
		if (main == null) main = this;
	}

	// start cutscene
	public void startCutscene(CutScene i_scene) {
		scene = i_scene;
		curr_line = 0; // reset dialogue
		nextLine ();
	}

	void Update () {
		// go to next line if space is pressed
		if (Input.GetKeyDown (KeyCode.Space)) {
			// if dialogue over, go to next scene
			if (!nextLine ()) StateManager.main.nextScene ();
		}
	}

	// displays next line of text; returns false if at end
	bool nextLine() {
		// check if dialogue is being printed
		if (!text_scroll.is_print) {
			if (curr_line >= scene.dialogue.Length) return false;
			name_text.text = scene.whos_talking [curr_line]; // show name of speaker
			Sprite npc_sprite = // set sprite of current speaker
				Resources.Load<Sprite>("sprites/" + scene.npc_sprites[curr_line].Trim());
			sprite_holder.sprite = npc_sprite;
			if (curr_line != 0) // play advance dialogue sfx
				AudioPlayer.main.playSFX(0, SFXType.UI, "menu_boop"); 
			AudioPlayer.main.playMusic(MusicType.CUTSCENE, scene.music_tracks[curr_line]);
			text_scroll.startPrint (scene.dialogue [curr_line], display_text, "speak_boop");
			++curr_line;
		} else { // dump if dialogue already started
			text_scroll.dump ();
		}
		return true;
	}
}
