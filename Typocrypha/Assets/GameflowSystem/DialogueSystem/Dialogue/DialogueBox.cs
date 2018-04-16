using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Manages a single dialogue box
public class DialogueBox : MonoBehaviour {
	public string speaker; // Speaker's name
	public string text; // Text to be displayed
	public float scroll_delay; // Time (in seconds) between showing characters
	public FXText fx_text; // FXText component for displaying text
	public Image left_icon; // Left dialogue speaker's icon
	public Image right_icon; // Light dialogue speaker's icon
	public RectTransform rect_tr; // RectTransform of dialogue box
	[HideInInspector] public Coroutine cr_scroll; // Coroutine that scrolls text
	[HideInInspector] public DialogueItem d_item; // Dialogue item

	DialogueManager dialogue_manager; // Manages all dialogue boxes
	FXTextColor set_color; // Effect that hides text for scrolling
	float text_pad; // Padding between text and dialogue box

	public void dialogueBoxStart() {
		dialogue_manager = FindObjectOfType<DialogueManager> ();
		text_pad = fx_text.rectTransform.offsetMin.y - fx_text.rectTransform.offsetMax.y;
		// Set fields
		left_icon.sprite = d_item.left_icon;
		right_icon.sprite = d_item.right_icon;
		scroll_delay = 0.02f; // DEFAULT TEMP
		// Initialize color effect to hide text
		set_color = fx_text.gameObject.AddComponent<FXTextColor> ();
		set_color.color = fx_text.color;
		set_color.color.a = 0;
		set_color.chars = new int[2]{0,text.Length};
		fx_text.addEffect (set_color);
		// Set text (with speaker name)
		fx_text.text = text;
		// Display appropriate icon
		if (d_item.icon_side == IconSide.LEFT || d_item.icon_side == IconSide.BOTH)
			left_icon.enabled = true;
		if (d_item.icon_side == IconSide.RIGHT || d_item.icon_side == IconSide.BOTH)
			right_icon.enabled = true;
		// Set box height (if chat mode) and start displaying text
		if (d_item.dialogue_mode == DialogueMode.CHAT) setBoxHeight ();
		// Set talking sfx
		AudioPlayer.main.setSFX(AudioPlayer.channel_voice, "speak_boop");
		cr_scroll = StartCoroutine (textScrollCR ());
	}

	// Dumps all remaining text
	public void dumpText() {
		TextEvents.main.StopAllCoroutines ();
		TextEvents.main.reset ();
		StopCoroutine (cr_scroll);
		cr_scroll = null;
		set_color.chars [0] = text.Length;
	}

	// Scrolls text character by character
	IEnumerator textScrollCR() {
		int cnt = 0;
		bool tag = false; // Are we scrolling over a tag?
		while (cnt < text.Length) {
			yield return new WaitWhile (() => DialogueManager.main.pause_scroll);
			checkEvents (cnt);
			tag = tag ? (text [cnt - 1] != '>') || (text[cnt] == '<') : (text[cnt] == '<'); // Skip Unity rich text tags
			if (!tag) yield return new WaitForSeconds (scroll_delay);
			AudioPlayer.main.playSFX (AudioPlayer.channel_voice);
			set_color.chars [0] = ++cnt;
		}
		cr_scroll = null;
	}

	// Checks for and plays text events
	void checkEvents(int start_pos) {
		if (d_item.text_events [start_pos] != null && d_item.text_events [start_pos].Count > 0)
			foreach (TextEvent t in d_item.text_events [start_pos])
				TextEvents.main.playEvent (t.evt, t.opt);
	}

	// Sets dialogue box's height based on text. Also updates dialogue window height
	void setBoxHeight() {
		rect_tr.sizeDelta = new Vector2 (rect_tr.sizeDelta.x, text_pad + fx_text.preferredHeight);
		dialogue_manager.expandWindow (rect_tr.sizeDelta.y);
	}
}
