using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
// edits by James Iwamasa

// represents an event to be played during text dialogue scrolling
public delegate IEnumerator TextEventDel(string[] opt);

// struct to contain event text
public struct TextEvent {
	public string evt;
	public string[] opt;
	public TextEvent(string evt, string[] opt) {
		this.evt = evt;
		this.opt = opt;
	}
}

// event class for events during text dialogue
public class TextEvents : MonoBehaviour {
	public static TextEvents main = null;
	public Dictionary<string, TextEventDel> text_event_map;
	[HideInInspector] public bool is_prompt; // are we prompting input?
	[HideInInspector] public string prompt_input; // input from prompt

	public SpriteRenderer dimmer; // sprite used to cover screen for fade/dim effects
	public GameObject center_text; // for showing floating text in the center
	public Camera main_camera; // main camera object
	public GameObject dialogue_box; // dialogue box object
	public GameObject glitch_effect; // sprite used for glitch effect
	public GameObject screen_frame; // screenframe object
	public Transform float_text_view; // view for floating text
	public GameObject float_d_box_prefab; // floating text dialogue box prefab
	public Animator train_animator; // animator component for train
	public SpriteRenderer train_sprite; // sprite renderer for train 

	AssetBundle train_bundle; // asset bundle for train sprites

	void Awake() {
		main = this;
		train_bundle = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "train_sprites"));
		text_event_map = new Dictionary<string, TextEventDel> {
			{"screen-shake", screenShake},
			{"block", block},
			{"pause", pause},
			{"fade", fade},
			{"next", next},
			{"next-delay", nextDelay},
			{"center-text-scroll", centerTextScroll},
			{"center-text-fade", centerTextFade},
			{"play-sfx", playSFX},
			{"play-music", playMusic},
			{"play-bgm", playMusic},
			{"stop-music", stopMusic},
			{"stop-bgm", stopMusic},
			{"set-scroll-delay", setScrollDelay},
			{"set-bg", setBG},
			{"hide-text-box", hideTextBox},
			{"set-talk-sfx", setTalkSFX},
			{"highlight-character", highlightCharacter},
			{"sole-highlight", soleHighlight},
            {"sole-highlight-codec", soleHighlightCodec},
			{"remove-character", removeCharacter},
			{"remove-all-character", removeAllCharacter},
            {"evil-eye", evilEye},
            {"prompt", prompt},
			{"glitch", glitch},
			{"set-name", setName},
            {"set-info", setInfo},
			{"frame", frame},
			{"heal-player", healPlayer},
			{"clear-log", clearTextLog},
			{"float-text", floatText},
			{"multi-float-text", multiFloatText},
			{"train-switch", trainSwitch},
			{"train-transition", trainTransition},
			{"train-sign", trainSign}
		};
		is_prompt = false;
	}

	// plays the event 'evt', returning the coroutine created (null if event doesnt exist)
	public Coroutine playEvent(string evt, string[] opt) {
		TextEventDel text_event;
		if (!text_event_map.TryGetValue (evt, out text_event))
			Debug.LogException (new System.Exception("Bad text event parameters:" + evt));
		return StartCoroutine(text_event (opt));
	}

	// resets all the parameters that might have been changed
	public void reset() {
		main_camera.transform.position = new Vector3 (0,0,-10);
		DialogueManager.main.pause_scroll = false;
		DialogueManager.main.block_input = false;
		//foreach (Transform tr in float_text_view)
		//	Destroy (tr.gameObject);
	}

	// finishes up persistent events that might have been skipped (like removing a character)
	public void finishUp(List<TextEvent>[] text_events) {
		foreach (List<TextEvent> text_event_pos in text_events) {
			if (text_event_pos == null) continue;
			foreach (TextEvent text_event in text_event_pos) {
				switch (text_event.evt) {
				case "next":
				case "play-music":
				case "play-bgm":
				case "set-scroll-delay":
				case "set-bg":
				case "remove-character":
				case "remove-all-character":
				case "hide-text-box":
					playEvent (text_event.evt, text_event.opt);
					break;
				default:
					break;
				}
			}
		}
	}

/**************************** TEXT EVENTS *****************************/

	// shakes the screen
	// input: [0]: float, length of shake
	//        [1]: float, intensity of shake
	IEnumerator screenShake(string[] opt) {
		Transform cam_tr = main_camera.transform;
		Vector3 old_cam_pos = cam_tr.position;
		float[] opts = opt.Select((string str) => float.Parse(str)).ToArray();
		float curr_time = 0;
		float wait_time = opts [0] / 4;
		while (curr_time < wait_time) {
			Vector3 new_pos = Random.insideUnitCircle * opts [1];
			new_pos.z = old_cam_pos.z;
			cam_tr.position = new_pos;
			yield return new WaitForSeconds (0.06f);
			curr_time += 0.06f;
		}
		cam_tr.position = new Vector3(0,0,-10);
	}
		
	// blocks or unblocks input
	// input: [t|f], 't' blocks input until 'f' unblocks
	IEnumerator block(string[] opt) {
		if (opt [0].CompareTo ("t") == 0)
			 DialogueManager.main.block_input = true;
		else DialogueManager.main.block_input = false;
		yield return true;
	}

	// causes dialogue and cutscene to pause: it is recommended to also block
	// input: [0]: float, length of pause
	IEnumerator pause(string[] opt) {
		DialogueManager.main.pause_scroll = true; // pause text scroll
		yield return new WaitForSeconds (float.Parse (opt [0]));
		DialogueManager.main.pause_scroll = false;
	}

	// fades the screen in or out
	// input: [0]: [in|out], 'in' re-reveals screen, 'out' hides it
	//        [1]: float, length of fade in seconds
	//        [2]: float, red color component
	//        [3]: float, green color component
	//        [4]: float, blue color component
	IEnumerator fade(string[] opt) {
		float fade_time = float.Parse (opt [1]);
		float r = float.Parse (opt [2]);
		float g = float.Parse (opt [3]);
		float b = float.Parse (opt [4]);
		float alpha;
		float a_step = 1f * Time.deltaTime / fade_time; // amount of change each frame
		if (a_step > 1) a_step = 1;
		alpha = dimmer.color.a;
		if (opt [0].CompareTo ("out") == 0) { // hide screen
			while (alpha < 1f) {
				yield return null;
				alpha += a_step;
				dimmer.color = new Color (r, g, b, alpha);
			}
		} else { // show screen
			while (alpha > 0f) {
				yield return null;;
				alpha -= a_step;
				dimmer.color = new Color (r, g, b, alpha);
			}
		}
	}

	// forces next line of dialogue (SHOULD BE PLACED AT END OF LINE)
	// input: none
	IEnumerator next(string[] opt) {
		DialogueManager.main.forceNextLine ();
		yield return true;
	}

	// forces next line of dialogue after a delay
	// input: [0]: float, delay time in seconds
	IEnumerator nextDelay(string[] opt) {
		Debug.Log ("nextDelay:" + opt[0]);
		yield return new WaitForSeconds (float.Parse (opt [0]));
		DialogueManager.main.forceNextLine ();
	}

	// NON_OPERATIONAL
	// scrolls floating text center aligned in the center of the screen (can also be used to immediately show text)
	// input: [0]: float, delay time in seconds
	//        [1]: float, red color
	//        [2]: float, green color
	//        [3]: float, blue color
	//        [4]: string, text to show
	IEnumerator centerTextScroll(string[] opt) {
		Text txt = center_text.GetComponent<Text> ();
		float delay = float.Parse (opt [0]);
		float r = float.Parse (opt [1]);
		float g = float.Parse (opt [2]);
		float b = float.Parse (opt [3]);
		float a = txt.color.a;
		string txt_str = opt [4];
		txt.color = new Color (r, g, b, a);
		if (delay == 0) { // show text immediately
			txt.text = txt_str;
		} else { // scroll text character by character
			txt.text = "";
			int txt_pos = 0;
			while (txt_pos < txt_str.Length) {
				txt.text += txt_str [txt_pos++];
				yield return new WaitForSeconds (delay);
			}
		}
	}

	// NON_OPERATIONAL
	// fades center text in or out
	// input: [0]: [in|out], 'in' re-reveals screen, 'out' hides it
	//        [1]: float, length of fade in seconds
	IEnumerator centerTextFade(string[] opt) {
		Text txt = center_text.GetComponent<Text> ();
		float fade_time = float.Parse (opt [1]);
		float r = txt.color.r;
		float g = txt.color.g;
		float b = txt.color.b;
		float alpha = txt.color.a;
		float a_step = 1f * 0.017f / fade_time; // amount of change each frame
		if (a_step > 1) a_step = 1;
		if (opt [0].CompareTo ("out") == 0) { // hide text
			while (alpha > 0f) {
				yield return new WaitForSeconds(0.017f);
				alpha -= a_step;
				txt.color = new Color (r, g, b, alpha);
			}
		} else { // show screen
			while (alpha < 1f) {
				yield return new WaitForSeconds(0.017f);
				alpha += a_step;
				txt.color = new Color (r, g, b, alpha);
			}
		}
	}

	// plays the specified sfx
	// input: [0]: string, sfx filename
	IEnumerator playSFX(string[] opt) {
		AudioPlayer.main.playSFX (opt[0]);
		yield return true;
	}

	// plays specified music
	// input: [0]: string, music filename
	IEnumerator playMusic(string[] opt) {
		AudioPlayer.main.playMusic (opt [0]);
		yield return true;
	}

	// stops music
	IEnumerator stopMusic(string[] opt){
		AudioPlayer.main.stopMusic ();
		yield return true;
	}

	// sets scroll delay of main dialogue text scroll
	// input: [0]: float, new delay amount in seconds
	IEnumerator setScrollDelay(string[] opt) {
		DialogueManager.main.setScrollDelay(float.Parse (opt [0]));
		yield return true;
	}
		
	// sets background image from sprite name
	// input: [0]: string, name of image file
	IEnumerator setBG(string[] opt) {
		BackgroundEffects.main.setSpriteBG (opt [0]);
		yield return true;
	}
		
	// hides/shows dialogue box (NOTE: text is STILL GOING when hidden)
	// typically, should block when hiding to avoid skipping reshow event
	// input: [0]: [t|n], hides text box if 't', shows if 'f'
	IEnumerator hideTextBox(string[] opt) {
		dialogue_box.SetActive (opt[0].CompareTo("f") == 0);
		yield return true;
	}
		
	// sets the talking sfx
	// input: [0]: string, name of audio file
	IEnumerator setTalkSFX(string[] opt) {
		AudioPlayer.main.setSFX (3, opt[0]); // put sfx in channel 3
		yield return true;
	}

	// DEPRECATED
	// prompts player to enter something into TYPORCYPHA; resumes on enter
	// input: [0]: string, type of prompt
	IEnumerator prompt(string[] opt) {
		/*
		CutsceneManager.main.enabled = false;
		dialogue_box.SetActive (false);
		track_typing.enabled = true;
		is_prompt = true;
		GameObject display = null; // visual display for prompt (instructions, usually)
		string type = opt [0].Trim ().ToLower ();

		// initialize prompt
		switch (type) {
		case "sprite":
			display = Instantiate (Resources.Load<GameObject> ("prefabs/PlayerSpriteChoice"));
			break;
		case "pronoun":
			display = Instantiate (Resources.Load<GameObject> ("prefabs/PlayerPronounChoice"));
			break;
		}

		while (is_prompt) {
			// wait for uder to press ENTER
			yield return new WaitWhile (() => is_prompt);
			// check result
			string choice = prompt_input.Trim ().ToLower ();
			switch (type) {
			case "name": // set name
				// CHECK NAME PARAMETERS
				PlayerDialogueInfo.main.player_name = prompt_input;
				break;
			case "sprite": // set sprite (choice between 'a' and 'b')
				if (choice.CompareTo ("a") == 0) { PlayerDialogueInfo.main.setSprite (0); } 
				else if (choice.CompareTo ("b") == 0) { PlayerDialogueInfo.main.setSprite (1); } 
				else { 
					// INDICATE BAD INPUT
					is_prompt = true; // keep asking for input
				}
				break;
			case "pronoun": // set pronoun (options go from 1-4)
				if (choice.CompareTo("a") == 0) { PlayerDialogueInfo.main.setPronoun (0); }
				else if (choice.CompareTo("b") == 0) { PlayerDialogueInfo.main.setPronoun (1); }
				else if (choice.CompareTo("c") == 0) { PlayerDialogueInfo.main.setPronoun (2); }
				else if (choice.CompareTo("d") == 0) { PlayerDialogueInfo.main.setPronoun (3); }
				else {
					// INDICATE BAD INPUT
					is_prompt = true; // keep asking for input
				}
				break;
			}
		}

		if (display != null) Destroy (display);
		prompt_input = "";
		track_typing.enabled = false;
		dialogue_box.SetActive (true);
		CutsceneManager.main.enabled = true;
		CutsceneManager.main.forceNextLine ();
		*/
		yield return true;
	}

	// Allows for highlighting a character
	// input: [0]: string, name of sprite to highlight
	//        [1]: float, amount to highlight (multiplier to tint)
	IEnumerator highlightCharacter(string[] opt) {
		DialogueManager.main.highlightCharacter(opt[0], float.Parse(opt[1]));
		yield return true;
	}

	// Highlights one character and unhighlights all others (0.5 greyscale)
	// input: [0]: string, name of sprite to highlight
	IEnumerator soleHighlight(string[] opt) {
		DialogueManager.main.soleHighlight (opt [0]);
		yield return true;
	}

    // Highlights one character and unhighlights all others (0.5 greyscale)
    // input: [0]: string, name of sprite to highlight
    IEnumerator soleHighlightCodec(string[] opt)
    {
        DialogueManager.main.soleHighlightCodec();
        yield return true;
    }

    // Removes a specific character from the scene
    // input: [0]: string, name of sprite to remove
    IEnumerator removeCharacter(string[] opt) {
		DialogueManager.main.removeCharacter (opt [0]);
		yield return true;
	}

	// Removes all characters from a scene
	// input: NONE
	IEnumerator removeAllCharacter(string[] opt) {
		DialogueManager.main.removeAllCharacter ();
		yield return true;
	}

	// NON_OPERATIONAL
    IEnumerator evilEye(string[] opt) {
        //AnimationPlayer.main.playAnimation("Evil_Eye", new Vector3(-5, 0, 0), 2f);
        yield return true;
    }

	// NON_OPERATIONAL
	IEnumerator glitch(string[] opt)
	{
		for(int i = 0; i < 5; i++)
		{
			glitch_effect.SetActive(true);
			yield return new WaitForSeconds (0.05f);
			glitch_effect.SetActive(false);
			yield return new WaitForSeconds (0.05f);
		}
		glitch_effect.SetActive(true);
		yield return new WaitForSeconds (0.15f);
		glitch_effect.SetActive(false);
		
		yield return true;
	}

	// Sets name from last inputed text
	// Input: NONE
	IEnumerator setName(string[] opt) {
		PlayerDialogueInfo.main.player_name = DialogueManager.main.answer;
		yield return true;
	}

    // Sets specified info from last inputed text
    // Input: [0]: string: the key of the info to set (from input if only one arg)
    //        [1]: string: the data to set the info to (optional)
    IEnumerator setInfo(string[] opt)
    {
        if (opt.Length == 1)
            PlayerDialogueInfo.main.setInfo(opt[0], DialogueManager.main.answer);
        else if (opt.Length == 2)
            PlayerDialogueInfo.main.setInfo(opt[0], opt[1]);
        else
            throw new System.Exception("TextEvents.cs: setInfo: Too many args, use 1 or 2");
        yield return true;
    }

    // makes the screenframe come in/out
    // input: [0]: [in|out], 'in' re-reveals screenframe, 'out' hides it
    IEnumerator frame(string[] opt) {
		if (opt [0].CompareTo ("out") == 0) { // hide screenframe
			screen_frame.SetActive(false);
		} else { // show screenframe
			screen_frame.SetActive(true);
		}
		yield return true;
	}

	// restore player to full HP
	// input: N/A
	IEnumerator healPlayer(string[] opt) {
		Debug.Log ("[JohnTypocrypha Voice]: i need healing");
		Player.main.restoreToFull ();
        BattleManagerS.main.battleKeyboard.clearStatus();
		yield return true;
	}

	// clears the chat and AN text logs
	// input: [0]: [chat|an], 'chat' clears chat log, 'an' clears AN log
	IEnumerator clearTextLog(string[] opt){
		if (opt [0].CompareTo ("chat") == 0) {
			Debug.Log ("chat log flushed");
			DialogueManager.main.clearChatLog();
		}
		else if (opt [0].CompareTo ("an") == 0){
			Debug.Log ("an log flushed");
			DialogueManager.main.clearANLog();
		}
		yield return true;
	}

	// scrolls floating text at a specific location
	// input: [0]: float, x-position in world coordinates (bottom left corner)
	//        [1]: float, y-position in world coordinates
	//        [2]: string, text to be displayed
	IEnumerator floatText(string[] opt) {
		// setup dialogue item and dialogue box
		GameObject d_box_obj = Instantiate (float_d_box_prefab, float_text_view);
		string text = opt [2];
		for (int i = 3; i < opt.Length; i++) text += "," + opt [i];
		d_box_obj.GetComponent<FloatText> ().startFloatText (float.Parse(opt[0]), float.Parse(opt[1]), text);
		yield return true;
	}

	// scrolls a bunch of floating text objects at random locations
	// input: [0]: int, number of floating text objs
	//        [1]: string, text to be display
	IEnumerator multiFloatText(string[] opt) {
		int num = int.Parse (opt [0]);
		for (int i = 0; i < num; i++) {
			// setup dialogue item and dialogue box
			GameObject d_box_obj = Instantiate (float_d_box_prefab, float_text_view);
			string text = opt [1];
			for (int j = 3; j < opt.Length; j++) text += "," + opt [j];
			float x = ((Random.value - 0.5f) * 18) - 2;
			float y = (Random.value - 0.5f) * 10;
			d_box_obj.GetComponent<FloatText> ().startFloatText (x, y, text);
			yield return null;
		}
	}

	// plays train transition animation
	// input: [0]: [in|out], 'in' shows train scene, 'out' hides it
	IEnumerator trainTransition(string[] opt) {
		if (opt[0] == "in") 
			train_animator.Play ("train_fade_in");
		else
			train_animator.Play ("train_fade_out");
		yield return true;
	}

	// switches train transition sprite
	// input: [0]: string, name of sprite
	IEnumerator trainSwitch(string[] opt) {
		Sprite spr = train_bundle.LoadAsset<Sprite>(opt[0]);
		train_sprite.sprite = spr;
		yield return true;
	}

	// plays train animation where character wiggles their flag
	// input: [0]: string, character's name
	IEnumerator trainSign(string[] opt) {
		train_animator.Play ("train_" + opt[0] + "_sign");
		yield return true;
	}
}

