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
	public float scroll_delay = 0.01f; // Time (in seconds) between showing characters
	public bool talk_sfx = true; // Should talking sfx play?
	public FXText fx_text; // FXText component for displaying text
	public Image left_icon; // Left dialogue speaker's icon
	public Image right_icon; // Light dialogue speaker's icon
	public RectTransform rect_tr; // RectTransform of dialogue box
	public bool is_floating; // Is this floating text?
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
		scroll_delay = 0.01f; // DEFAULT TEMP
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
			// Add text with speaker's name, and offset text display
			int offset = 0;
			if (d_item.speaker_name != null && d_item.speaker_name.Length != 0) {
				text = d_item.speaker_name + "\n" + text;
				offset += d_item.speaker_name.Length + 1;
			}
			fx_text.text = text;
			set_color.chars [0] = offset;
			set_color.chars [1] += offset;
			// Set box height
			setBoxHeight ();
		} else if (d_item.GetType () == typeof(DialogueItemVN)) {
			DialogueItemVN v_item = (DialogueItemVN)d_item;
			// Display character sprites
			for (int i = 0; i < v_item.char_sprites.Length; ++i)
				DialogueManager.main.displayCharacter (v_item.char_sprites [i], v_item.char_sprite_pos [i]);
			fx_text.text = text;
		} else if (d_item.GetType () == typeof(DialogueItemAN)) {
			fx_text.text = text;
			setBoxHeight ();
		} else {
			fx_text.text = text;
		}
		// Set talking sfx
		if (talk_sfx) AudioPlayer.main.setSFX(AudioPlayer.channel_voice, "speak_boop");
		cr_scroll = StartCoroutine (textScrollCR ());
	}

	// Dumps all remaining text
	public void dumpText() {
		TextEvents.main.StopAllCoroutines ();
		TextEvents.main.reset ();
		TextEvents.main.finishUp (d_item.text_events);
        if(cr_scroll != null)
		    StopCoroutine (cr_scroll);
		cr_scroll = null;
		set_color.chars [0] = text.Length;
	}

	// Scrolls text character by character
	IEnumerator textScrollCR() {
		Debug.Log (text);
		int offset = set_color.chars [0];
		int cnt = set_color.chars[0];
		int sfx_interval = 3; // Play voice effect for every Xth char displayed
		while (cnt < text.Length) {
			StartCoroutine(checkEvents (cnt - offset));
			if (DialogueManager.main.pause_scroll && !is_floating)
			    yield return new WaitWhile (() => DialogueManager.main.pause_scroll);
			if (text [cnt] == '<') {
				cnt = text.IndexOf ('>', cnt + 1) + 1;
				if (cnt >= text.Length) break;
			}
			set_color.chars [0] = ++cnt;
			if (talk_sfx && cnt % sfx_interval == 0) AudioPlayer.main.playSFX (AudioPlayer.channel_voice);
            if (scroll_delay * DialogueManager.main.scroll_scale > 0)
                yield return new WaitForSeconds(scroll_delay * DialogueManager.main.scroll_scale);
		}
		yield return StartCoroutine(checkEvents (cnt - offset)); // Play events at end of text
		cr_scroll = null;
	}

	// Checks for and plays text events
	IEnumerator checkEvents(int start_pos) {
		if (start_pos >= d_item.text_events.Length)
			yield break;
		List<TextEvent> evt_list = d_item.text_events [start_pos];
		if (evt_list != null && evt_list.Count > 0) {
			foreach (TextEvent t in evt_list) {
				TextEvents.main.playEvent (t.evt, t.opt);
				if (t.evt == "pause")
					yield return new WaitForSeconds (float.Parse(t.opt[0]));
			}
			d_item.text_events [start_pos] = null;
		}
	}

	// Sets dialogue box's height based on text. Also updates dialogue window height
	void setBoxHeight() {
		rect_tr.sizeDelta = new Vector2 (rect_tr.sizeDelta.x, text_pad + fx_text.preferredHeight);
		DialogueManager.main.expandWindow (rect_tr.sizeDelta.y);
	}
}
