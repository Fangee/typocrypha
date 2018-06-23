using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages dialogue boxes
public class DialogueManager : MonoBehaviour {
	public static DialogueManager main = null; // Global static reference
	public GameObject curr_dialogue; // Current dialogue to be run

	public GameObject ChatView; // Chat view hiearchy
	public RectTransform ChatContent; // Content of chat scroll view (contains dialogue boxes)
	public Scrollbar scroll_bar; // Scroll bar of chat dialogue window

	public GameObject VNView; // Visual Novel view hiearchy
	public DialogueBox VNDialogueBox; // Visual novel style dialogue box
	public FXText VNSpeaker; // Text that contains speaker's name for VN style
	public SpriteRenderer VNMCSprite; // Holds mc's sprite
	public SpriteRenderer VNCodecSprite; // Holds codec call sprites (right side)

	public GameObject ANView; // Audio Novel view hiearchy
	public RectTransform ANContent; // Content of scroll view

	public RectTransform player_ui; // the Typocrypha UI

	public InputField input_field; // Input field for dialogue choices
	public GameObject dialogue_box_prefab; // Prefab of dialogue box object
	public GameObject an_dialouge_box_prefab; // Prefab of audio novel dialogue 
	public GameObject chr_spr_prefab; // Prefab of character sprite display
	public float scroll_time; // Time it takes to automatically update window
	public float scroll_delay = 0.01f; // Time it takes for text to scroll 
    public float scroll_scale; // Scroll delay multiplier
	public float top_space; // Space between top of window and dialogue

	public GameObject input_display_choices; // Game object displaying dialogue choices
	public GameObject input_display_C; // Dialogue choice C box

	public GameObject spacebar_icon_vn; // Spacebar icon VN view
	public Animator animator_spacebar_vn; // Spacebar icon key animator

    public GameObject spacebar_icon_an; // Spacebar icon AN view
    public Animator animator_spacebar_an; // Spacebar icon key animator

    public GameObject spacebar_icon_chat; // Spacebar icon CHAT view
    public Animator animator_spacebar_chat; // Spacebar icon key animator

    [HideInInspector] public bool pause_scroll; // Pause text scroll
	[HideInInspector] public bool block_input; // Blocks user input
	[HideInInspector] public bool is_dump; // Is text being dumped?
	[HideInInspector] public string answer; // Player's input

	int curr_line; // Current line number of dialogue
	float window_height; // Height of dialogue history
	float default_window_height; // Default Height of dialogue history (to reset)
	Coroutine slide_scroll_cr; // Coroutine that smoothly adjusts window
	Coroutine dump_cr; // Coroutine that dumps text
	List<DialogueBox> history; // List of all dialogue boxes
	List<GameObject> chr_spr_list; // List of character sprite holders
	string stringEdit = "";

	bool input; // Are we waiting for input?
	GameObject input_display; // Display image for input

    private GameObject curr_spacebar_icon; // Spacebar icon VN view ref
    private Animator curr_spacebar_animator; // Spacebar icon key animator ref

    private bool isInterrupt = false; //is this dialogue event a oneshot (interrupts, etc)

	void Awake() {
		if (main == null) main = this;
		else Destroy (this);
		pause_scroll = false;
		block_input = false;
		curr_line = -1;
		window_height = top_space;
		default_window_height = top_space;
		history = new List<DialogueBox> ();
		chr_spr_list = new List<GameObject> ();
        curr_spacebar_icon = spacebar_icon_an;
        curr_spacebar_animator = animator_spacebar_an;
		input = false;
        scroll_scale = 1f;
	}

	void Update() {
		if (Pause.main.isPaused ()) {
            if (input_field.enabled) {
                input_field.enabled = false;
            }
            return;
        }
        input_field.enabled = true;
        if (history.Count > 0 && history [history.Count - 1].cr_scroll == null && !input) {
			curr_spacebar_icon.SetActive (true);
		}
		if (block_input) {
			curr_spacebar_icon.SetActive (true);
			if (Input.GetKeyDown (KeyCode.Space)) AudioPlayer.main.playSFX ("sfx_botch");
            if (curr_spacebar_icon.activeInHierarchy)
                curr_spacebar_animator.Play ("anim_key_spacebar_no");
			return;
		} else {
			if (history.Count > 0 && history [history.Count - 1].cr_scroll != null){
				curr_spacebar_icon.SetActive (false);
			}
			if (curr_spacebar_icon.activeInHierarchy) {
                curr_spacebar_animator.Play ("anim_key_spacebar");
			} 
		}
		if (isInterrupt)
			player_ui.localScale = new Vector3(0,0,0);
		if (is_dump)
			return;
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!nextLine ()) {
				if (isInterrupt) {
					if (!BattleManagerS.main.playSceneFromQueue ()) {
						player_ui.localScale = new Vector3 (1, 1, 1);
						BattleManagerS.main.setPause (false);
						BattleManagerS.main.postInterrupt ();
						isInterrupt = false;
						setEnabled (false);
					}
				} else {
					GameflowManager.main.next ();
				}
			}
			if (!block_input) {
				if (!input)
					AudioPlayer.main.playSFX ("sfx_type_key");
			}
		}
	}

    public void setEnabled(bool e) {
        enabled = e;
        ChatView.SetActive(e);
        VNView.SetActive(e);
        ANView.SetActive(e);
    }

	// Starts a new dialogue scene
	public void startDialogue(GameObject new_dialogue) {
		clearChatLog ();
		clearANLog ();
		curr_line = -1;
		curr_dialogue = new_dialogue;
		nextLine ();
	}

    // Starts a new dialogue scene
    public void startInterrupt(GameObject new_dialogue_interrupt) {
        isInterrupt = true;
        startDialogue(new_dialogue_interrupt);
    }

    // Displays next line of dialogue. Returns 'false' if there is no next line.
    bool nextLine() {
		if (history.Count > 0 && history [history.Count - 1].cr_scroll != null) {
			is_dump = true; // Flag is set back to false by 'dumpText()'
			dump_cr = StartCoroutine (history [history.Count - 1].dumpText ());
		} else {
			//if (!input && Input.GetKeyDown (KeyCode.Space)) AudioPlayer.main.playSFX ("sfx_advance_text");
			//curr_spacebar_icon.SetActive (false);
			if (input) return true;
			if (curr_line >= curr_dialogue.GetComponents<DialogueItem>().Length - 1) return false;
			// Create dialogue box
			DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem>()[++curr_line];
			DialogueBox d_box = null;
            if (d_item is DialogueItemJumpBase) {
                GameflowManager.main.jump(((DialogueItemJumpBase)d_item).evaluateTarget().gameObject, false);
                return false;
            } else if (d_item.GetType() == typeof(DialogueItemVN)) {
                Sprite mc_sprite = ((DialogueItemVN)d_item).mc_sprite;
                Sprite codec_sprite = ((DialogueItemVN)d_item).codec_sprite;
                if (mc_sprite != null)
                    VNMCSprite.sprite = mc_sprite;
                if (codec_sprite != null)
                    VNCodecSprite.sprite = codec_sprite;
                VNView.SetActive(true);
                ChatView.SetActive(false);
                ANView.SetActive(false);
                curr_spacebar_icon = spacebar_icon_vn;
                curr_spacebar_animator = animator_spacebar_vn;
                d_box = VNDialogueBox;
                VNSpeaker.text = DialogueParser.main.substituteMacros(d_item.speaker_name);
            } else if (d_item.GetType() == typeof(DialogueItemChat)) {
                VNView.SetActive(false);
                ChatView.SetActive(true);
                ANView.SetActive(false);
                curr_spacebar_icon = spacebar_icon_chat;
                curr_spacebar_animator = animator_spacebar_chat;
                //clearLog (ChatView);
                GameObject d_obj = Instantiate(dialogue_box_prefab, ChatContent);
                d_box = d_obj.GetComponent<DialogueBox>();
            } else if (d_item.GetType() == typeof(DialogueItemAN)) {
                VNView.SetActive(false);
                ChatView.SetActive(false);
                ANView.SetActive(true);
                curr_spacebar_icon = spacebar_icon_an;
                curr_spacebar_animator = animator_spacebar_an;
                //clearLog (ANView);
                GameObject d_obj = Instantiate(an_dialouge_box_prefab, ANContent);
                d_box = d_obj.GetComponent<DialogueBox>();
            }
			d_box.d_item = d_item;
			d_box.speaker = DialogueParser.main.substituteMacros(d_item.speaker_name);
			// Remove old text effects
			FXTextEffect[] fx_arr = d_box.fx_text.gameObject.GetComponents<FXTextEffect>();
			foreach (FXTextEffect fx in fx_arr) {
				d_box.fx_text.removeEffect (fx);
				Destroy (fx);
			}
			d_box.text = DialogueParser.main.parse (d_item, d_box);
			// Add new text effects
			foreach(FXTextEffect text_effect in d_item.fx_text_effects)
				d_box.fx_text.addEffect (text_effect);
			// Add dialogue box to history (only really works for Chat items)
			history.Add (d_box);
			// Start scroll
			d_box.scroll_delay = scroll_delay;
			Coroutine d_box_init = d_box.dialogueBoxStart ();
			// Prompt input if necessary
			if (d_item.dialogue_type == DialogueType.INPUT) {
				input = true;
				StartCoroutine(showInput(d_item, d_box, d_box_init));
			} else {
				input = false;
			}
		}
		return true;
	}

	// Show input options when text is done scrolling
	IEnumerator showInput(DialogueItem d_item, DialogueBox d_box, Coroutine d_box_init) {
		yield return d_box_init; // Wait for d_box to finish setting up
		yield return new WaitUntil (() => d_box.cr_scroll == null); // Wait for scroll to end
		input_field.gameObject.SetActive (true);
		input_field.ActivateInputField ();
        //spacebar_icon_vn.SetActive(true);
        curr_spacebar_animator.Play("anim_key_spacebar_no");
		if (d_item.input_display != null)
			input_display = Instantiate (d_item.input_display, transform);
		if (d_item.input_options.Length > 0) {
			switch (d_item.input_options.Length) {
			case 2:
				input_display_C.SetActive (false);
				break;
			case 3:
				input_display_C.SetActive (true);
				break;
			}
			populateChoices ();
			input_display_choices.SetActive (true);
		}
	}

	// Called when input field is submitted
	public void submitInput() {
		answer = input_field.text;
		if (Input.GetKey (KeyCode.Escape) || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
			input_field.ActivateInputField ();
			//input_field.text = answer;
			//input_field.ForceLabelUpdate ();
			return;
		}
		// CHECK IF CORRECT INPUT
		input_field.gameObject.SetActive (false);
		input_field.text = "";
		Destroy (input_display);
		DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem>()[curr_line];
		if (d_item.input_options.Length > 0) { // If set number of choices
			int i = 0;
			string choiceStrings = "";
			string answerString = "";
			for (; i < d_item.input_options.Length; ++i) {
				choiceStrings = d_item.input_options [i].Trim ().ToLower().Replace (".", string.Empty).Replace ("?", string.Empty).Replace ("!", string.Empty);
				answerString = answer.Trim ().ToLower ().Replace (".", string.Empty).Replace ("?", string.Empty).Replace ("!", string.Empty);
				if (choiceStrings.CompareTo(answerString) == 0) {
					break;
				} else {
					string comparisonLetter = "";
					switch (i) {
					case 0:
						comparisonLetter = "A";
						break;
					case 1:
						comparisonLetter = "B";
						break;
					case 2:
						comparisonLetter = "C";
						break;
					}
					if (comparisonLetter.Trim ().ToLower ().CompareTo (answer.Trim ().ToLower ()) == 0)
						break;
				}
			}
			if (i < d_item.input_options.Length) { // Option was found, so branch
                AudioPlayer.main.playSFX("sfx_enter");
                GameflowManager.main.jump(d_item.input_branches[i].gameObject);
			} else { // If not found, try again
				curr_line--;
				clearANLog ();
				AudioPlayer.main.playSFX ("sfx_botch");
			}
		} 
		else { 
			/*if (answer.Trim ().ToLower () == "") { // if answer field left blank, we try again
				curr_line--;
				clearANLog ();
				AudioPlayer.main.playSFX ("sfx_botch");
			} 
			else {*/ 
				// Check/compare with possible free response answers
				//AudioPlayer.main.playSFX ("sfx_enter");
			if (d_item.input_answers.Length > 0) {
				int i = 0;
				string checkStrings = "";
				string answerString = "";
				for (; i < d_item.input_answers.Length; ++i) {
					checkStrings = d_item.input_answers [i].Trim ().ToLower ().Replace (".", string.Empty).Replace ("?", string.Empty).Replace ("!", string.Empty);
					answerString = answer.Trim ().ToLower ().Replace (".", string.Empty).Replace ("?", string.Empty).Replace ("!", string.Empty);
					if (checkStrings.CompareTo (answerString) == 0 || checkStrings.CompareTo ("") == 0) {
						break;
					}
				}
				if (i < d_item.input_answers.Length) { // Option was found, so branch
                    AudioPlayer.main.playSFX("sfx_enter");
                    GameflowManager.main.jump(d_item.input_branches[i].gameObject);
				} else { // If not found, try again
					curr_line--;
					clearANLog ();
					AudioPlayer.main.playSFX ("sfx_botch");
				}
			} else {
				curr_spacebar_animator.Play("anim_key_spacebar");
				AudioPlayer.main.playSFX ("sfx_enter");
			}
			//}
		}
		input_display_choices.SetActive (false);
		input = false;
		forceNextLine ();
	}

	// Forces next line
	public void forceNextLine() {
		history [history.Count - 1].StopAllCoroutines();
		history [history.Count - 1].cr_scroll = null;
		if (!nextLine () && !isInterrupt) GameflowManager.main.next ();
	}

	// Changes scroll delay of currently scrolling dialogue
	public void setScrollDelay(float delay) {
		history [history.Count - 1].scroll_delay = delay;
	}

	// Expands height of chat window
	public void expandWindow(float box_height) {
		float expand = box_height + ChatContent.GetComponent<VerticalLayoutGroup> ().spacing;
		window_height += expand;
		ChatContent.sizeDelta = new Vector2 (ChatContent.sizeDelta.x, window_height);
		stopSlideScroll ();
		slide_scroll_cr = StartCoroutine (slideScrollCR());
	}

	// Resets height of chat window
	public void resetWindowSize(){
		window_height = default_window_height;
		ChatContent.sizeDelta = new Vector2 (ChatContent.sizeDelta.x, window_height);
		stopSlideScroll ();
	}

	// Stops slide adjustment of window
	public void stopSlideScroll() {
		if (slide_scroll_cr != null) StopCoroutine (slide_scroll_cr);
	}

	// Coroutine that smoothly slides scroll bar to bottom
	IEnumerator slideScrollCR() {
		yield return new WaitUntil (() => scroll_bar.value > Mathf.Epsilon);
		float vel = 0;
		while (scroll_bar.value > Mathf.Epsilon) {
			scroll_bar.value = Mathf.SmoothDamp (scroll_bar.value, 0, ref vel, scroll_time);
			yield return null;
		}
	}

	// Displays new character
	public void displayCharacter(Sprite spr, Vector2 pos) {
		GameObject new_chr = Instantiate (chr_spr_prefab, transform);
		chr_spr_list.Add (new_chr);
		new_chr.transform.position = pos;
		new_chr.transform.localScale = Vector3.one;
		new_chr.GetComponent<SpriteRenderer> ().sprite = spr;
	}

	// Highlight's a character
	public void highlightCharacter(string spr_name, float amt) {
		foreach(GameObject chr_spr in chr_spr_list) {
			SpriteRenderer spr_r = chr_spr.GetComponent<SpriteRenderer> ();
			if (spr_r.sprite.name.Contains(spr_name)) {
				spr_r.color = new Color(amt, amt, amt, 1);
				break;
			}
		}
	}

	// Highlights given character, and dims rest (0.25 greyscale)
	public void soleHighlight(string spr_name) {
        Color dimColor = new Color(0.25f, 0.25f, 0.25f, 1);
        foreach (GameObject chr_spr in chr_spr_list) {
			SpriteRenderer spr_r = chr_spr.GetComponent<SpriteRenderer> ();
			if (spr_r.sprite.name.Contains (spr_name)) {
				spr_r.color = new Color (1, 1, 1, 1);
			} else {
				spr_r.color = dimColor;
			}
		}
        if (VNMCSprite.sprite.name.Contains(spr_name.Trim('_'))) VNMCSprite.color = new Color(1, 1, 1, 1);
        else VNMCSprite.color = dimColor;
        if (VNCodecSprite.sprite.name.Contains(spr_name.Trim('_'))) VNCodecSprite.color = new Color(1, 1, 1, 1);
        else VNCodecSprite.color = dimColor;
    }

    // Highlights given character, and dims rest (0.25 greyscale)
    public void soleHighlightCodec()
    {
        Color dimColor = new Color(0.25f, 0.25f, 0.25f, 1);
        foreach (GameObject chr_spr in chr_spr_list)
        {
            SpriteRenderer spr_r = chr_spr.GetComponent<SpriteRenderer>();
            spr_r.color =  dimColor;
        }
        VNMCSprite.color = dimColor;
        VNCodecSprite.color = new Color(1, 1, 1, 1);
    }

    // Finds first character with specified sprite, and removes it
    public void removeCharacter(Sprite spr) {
		foreach(GameObject chr_spr in chr_spr_list) {
			if (chr_spr.GetComponent<SpriteRenderer> ().sprite == spr) {
				chr_spr_list.Remove (chr_spr);
				Destroy (chr_spr);
				break;
			}
		}
	}

	// Finds first character with specified sprite name, and removes it
	public void removeCharacter(string spr_name) {
		foreach(GameObject chr_spr in chr_spr_list) {
			if (chr_spr.GetComponent<SpriteRenderer> ().sprite.name.Contains(spr_name)) {
				chr_spr_list.Remove (chr_spr);
				Destroy (chr_spr);
				break;
			}
		}
	}

	// Removes all character from scene
	public void removeAllCharacter() {
		foreach (GameObject chr_spr in chr_spr_list) {
			Destroy (chr_spr);
		}
		chr_spr_list.Clear ();
	}

	// Clears the given chat log of all objects in its hierarchy (general function)
	public void clearLog(GameObject textView, int offset){
		Transform content = textView.transform.GetChild(0).GetChild(0);
		int skipSpacer = 0; // skip the Spacer object in the content hierarchy
		//VerticalLayoutGroup layout = content.GetComponents<VerticalLayoutGroup>();
		Transform[] contentArray = content.GetComponentsInChildren<Transform>();
		int skipCurrentBox = 0; // tracks when to stop going through log as to not delete current textbox
		int currentBoxPosition = contentArray.Length - (offset*2); // when to stop deleting items
		//Debug.Log ("current box position = " + currentBoxPosition);
		foreach (Transform tr in content.transform) {
			if (skipSpacer > 0 && skipCurrentBox < currentBoxPosition) {
				Destroy (tr.gameObject);
				skipCurrentBox += offset;
			}
			else {
				++skipSpacer;
				++skipCurrentBox;
			}
			//Debug.Log ("skip current box = " + skipCurrentBox);
		}
		resetWindowSize();
	}

	// Pops off the latest item in the given chat log of all objects in its hierarchy (general function)
	// BUGGED
	public void popLog(GameObject textView, int offset){
		//Debug.Log ("popping log");
		resetWindowSize();
		Transform content = textView.transform.GetChild(0).GetChild(0);
		int skipSpacer = 0; // skip the Spacer object in the content hierarchy
		//VerticalLayoutGroup layout = content.GetComponents<VerticalLayoutGroup>();
		Transform[] contentArray = content.GetComponentsInChildren<Transform>();
		int skipCurrentBox = 0; // tracks when to stop going through log as to not delete current textbox
		int minBoxPosition = contentArray.Length - (offset*3)-1; // when to stop deleting items
		int maxBoxPosition = contentArray.Length - (offset*2); // when to stop deleting items
		//Debug.Log ("current box position = " + currentBoxPosition);
		foreach (Transform tr in content.transform) {
			if ((skipSpacer > 0) || ((minBoxPosition < skipCurrentBox) && (skipCurrentBox < maxBoxPosition))) {
				Destroy (tr.gameObject);
				skipCurrentBox += offset;
			}
			else {
				++skipSpacer;
				++skipCurrentBox;
			}
			//Debug.Log ("skip current box = " + skipCurrentBox);
		}
	}

	// Clears the Chat view log
	public void clearChatLog(){
		clearLog (ChatView, 4); // A chatview dialogue box has 4 items (the parent and 3 children (text, L/R icons))
	}

	// Clears the AN view log
	public void clearANLog(){
		clearLog (ANView, 2); // An AN view dialogue box has 2 items (the parent and 1 child (text))
	}

	// Fill the choice display boxes with current options text
	public void populateChoices(){
		Text[] choiceText = input_display_choices.GetComponentsInChildren<Text>();
		DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem> () [curr_line];
		int j = 0;
		for (int i = 1; (i < choiceText.Length) && (j < d_item.input_options.Length); i = i + 2) {
			choiceText [i].text = d_item.input_options[j];
			++j;
		}
	}
}
