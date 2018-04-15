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

	public InputField input_field; // Input field for dialogue choices
	public GameObject dialogue_box_prefab; // Prefab of dialogue box object
	public float scroll_time; // Time it takes to automatically update window
	public float top_space; // Space between top of window and dialogue
	[HideInInspector] public bool pause_scroll; // Pause text scroll
	[HideInInspector] public bool block_input; // Blocks user input

	int curr_line; // Current line number of dialogue
	float window_height; // Height of dialogue history
	Coroutine slide_scroll_cr; // Coroutine that smoothly adjusts window
	List<DialogueBox> history; // List of all dialogue boxes

	bool input; // Are we waiting for input?
	string answer; // Player's input
	GameObject input_display; // Display image for input

	void Awake() {
		if (main == null) main = this;
		else Destroy (this);
		pause_scroll = false;
		block_input = false;
		curr_line = -1;
		window_height = top_space;
		history = new List<DialogueBox> ();
		input = false;
	}

	void Update() {
		if (block_input) return;
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!nextLine ()) GameflowManager.main.next ();
		}
	}

	// Starts a new dialogue scene
	public void startDialogue(GameObject new_dialogue) {
		curr_line = -1;
		curr_dialogue = new_dialogue;
		nextLine ();
	}

	// Displays next line of dialogue. Returns 'false' if there is no next line.
	bool nextLine() {
		if (history.Count > 0 && history [history.Count - 1].cr_scroll != null) {
			history [history.Count - 1].dumpText ();
		} else {
			if (input) return true;
			if (curr_line >= curr_dialogue.GetComponents<DialogueItem>().Length - 1) return false;
			DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem>()[++curr_line];
			DialogueBox d_box = null;
			if (d_item.dialogue_mode == DialogueMode.VN) {
				VNView.SetActive (true);
				ChatView.SetActive (false);
				d_box = VNDialogueBox;
				VNSpeaker.text = d_item.speaker_name;
			} else if (d_item.dialogue_mode == DialogueMode.CHAT) {
				VNView.SetActive (false);
				ChatView.SetActive (true);
				GameObject d_obj = Instantiate (dialogue_box_prefab, ChatContent);
				d_box = d_obj.GetComponent<DialogueBox> ();
			}
			d_box.d_item = d_item;
			d_box.speaker = d_item.speaker_name;
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
			history.Add (d_box);
			if (d_item.dialogue_type == DialogueType.INPUT) {
				input = true;
				StartCoroutine(showInput(d_item, d_box));
			} else {
				input = false;
			}
			d_box.dialogueBoxStart ();
		}
		return true;
	}

	// Show input options when text is done scrolling
	IEnumerator showInput(DialogueItem d_item, DialogueBox d_box) {
		yield return new WaitUntil (() => d_box.cr_scroll != null);
		yield return new WaitUntil (() => d_box.cr_scroll == null);
		input_field.gameObject.SetActive (true);
		input_field.ActivateInputField ();
		input_display = Instantiate (d_item.input_display, transform);
	}

	// Called when input field is submitted
	public void submitInput() {
		answer = input_field.text.Trim().ToLower();
		// CHECK IF CORRECT INPUT
		input_field.gameObject.SetActive (false);
		input_field.text = "";
		Destroy (input_display);
		DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem>()[curr_line];
		if (d_item.input_options.Length > 0) { // If set number of choices
			int i = 0;
			for (; i < d_item.input_options.Length; ++i)
				if (d_item.input_options [i].Trim().ToLower().CompareTo (answer) == 0) break;
			if (i < d_item.input_options.Length) { // Option was found, so branch
				curr_dialogue = d_item.input_branches [i].gameObject;
				curr_line = -1;
			} else { // If not found, try again
				curr_line--;
			}
		}
		input = false;
		forceNextLine ();
	}

	// Forces next line
	public void forceNextLine() {
		if (history.Count > 0 && history [history.Count - 1].cr_scroll != null) {
			history [history.Count - 1].dumpText ();
			nextLine ();
		} else {
			nextLine ();
		}
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
}
