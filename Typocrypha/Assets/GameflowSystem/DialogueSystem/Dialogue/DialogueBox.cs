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

	FXTextColor set_color; // Effect that hides text for scrolling
	float text_pad; // Padding between text and dialogue box

	public void dialogueBoxStart() {
		text_pad = fx_text.rectTransform.offsetMin.y - fx_text.rectTransform.offsetMax.y;
		// Initialize color effect to hide text
		set_color = fx_text.gameObject.AddComponent<FXTextColor> ();
		set_color.color = fx_text.color;
		set_color.color.a = 0;
		set_color.chars = new int[2]{0,text.Length};
		fx_text.addEffect (set_color);
		// Set text (with speaker name)
		scroll_delay = 0.02f; // DEFAULT TEMP
		fx_text.text = text;
		// Set sprites
		if (d_item.GetType () == typeof(DialogueItemChat)) {
			DialogueItemChat c_item = (DialogueItemChat)d_item;
			// Set icon
			left_icon.sprite = c_item.left_icon;
			right_icon.sprite = c_item.right_icon;
			// Display appropriate icon
			if (c_item.icon_side == IconSide.LEFT || c_item.icon_side == IconSide.BOTH)
				left_icon.enabled = true;
			if (c_item.icon_side == IconSide.RIGHT || c_item.icon_side == IconSide.BOTH)
				right_icon.enabled = true;
			// Set box height
			setBoxHeight ();
		} else {
			DialogueItemVN v_item = (DialogueItemVN)d_item;
			// Display character sprites
			for (int i = 0; i < v_item.char_sprites.Length; ++i)
				DialogueManager.main.displayCharacter (v_item.char_sprites [i], v_item.char_sprite_pos [i]);
		}
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
		DialogueManager.main.expandWindow (rect_tr.sizeDelta.y);
	}
}
