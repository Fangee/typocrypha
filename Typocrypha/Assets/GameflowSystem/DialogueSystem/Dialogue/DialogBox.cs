using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages a single dialogue box
public class DialogBox : MonoBehaviour {
    [HideInInspector] public string speaker; // Speaker's name
    [HideInInspector] public string text; // Text to be displayed
	public float scroll_delay; // Time (in seconds) between showing characters
	public bool talk_sfx = true; // Should talking sfx play?
	public FXText fx_text; // FXText component for displaying text
	public RectTransform rect_tr; // RectTransform of dialogue box
	public bool is_floating; // Is this floating text?
	[HideInInspector] public Coroutine cr_scroll; // Coroutine that scrolls text
    [HideInInspector] public Coroutine cr_init; // Coroutine that inits before scrolling
	[HideInInspector] public DialogItem d_item; // Dialogue item

	FXTextColor set_color; // Effect that hides text for scrolling
	float text_pad; // Padding between text and dialogue box

	// Initializes dialogue box and starts text scroll
	public void dialogueBoxStart(DialogItem lineData) {
        // Remove old text effects
        FXTextEffect[] fx_arr = fx_text.gameObject.GetComponents<FXTextEffect>();
        foreach (FXTextEffect fx in fx_arr)
        {
            fx_text.removeEffect(fx);
            Destroy(fx);
        }
        //Initialize line data and parse text
        d_item = lineData;
        text = DialogueParser.main.parse(lineData, this);
        //Initialize text padding
        text_pad = fx_text.rectTransform.offsetMin.y - fx_text.rectTransform.offsetMax.y;
		// Initialize color effect to hide text
		set_color = fx_text.gameObject.AddComponent<FXTextColor> ();
		set_color.color = fx_text.color;
		set_color.color.a = 0;
		set_color.chars = new int[2]{0,text.Length};
		fx_text.addEffect (set_color);
        fx_text.text = text;
        // Add new text effects
        foreach (FXTextEffect text_effect in lineData.fx_text_effects)
            fx_text.addEffect(text_effect);
        scroll_delay = 0.01f; //TEMPORARY UNTIL HOOKED BACK UP TO SETTINGS' SCROLL SPEED
        // Set talking sfx
        if (talk_sfx) AudioPlayer.main.setSFX(AudioPlayer.channel_voice, "speak_boop");
		cr_init =  StartCoroutine (startTextScrollCR ());
	}

	// Dumps all remaining text
	public IEnumerator dumpText(Ref<bool> finished) {
		TextEvents.main.stopEvents();
		TextEvents.main.reset ();
		yield return TextEvents.main.finishUp (d_item.text_events);
        if (cr_scroll != null)
		 	StopCoroutine (cr_scroll);
		cr_scroll = null;
		set_color.chars [0] = text.Length;
        finished.Obj = true;
	}

	// Starts scrolling text
	IEnumerator startTextScrollCR() {
		yield return new WaitWhile (() => cr_scroll != null);
		cr_scroll = StartCoroutine (textScrollCR ());
        cr_init = null;
	}

	// Scrolls text character by character
	IEnumerator textScrollCR() {
		Debug.Log (text);
		int offset = set_color.chars [0];
		int cnt = set_color.chars[0];
		int sfx_interval = 3; // Play voice effect for every Xth char displayed
		while (cnt < text.Length) {
			yield return StartCoroutine(checkEvents (cnt - offset));
			if (DialogueManager.main.pause_scroll && !is_floating)
			    yield return new WaitWhile (() => DialogueManager.main.pause_scroll); //NEEDS REVAMP, PAUSING CODE OUT OF DATE
			if (text [cnt] == '<') {
				cnt = text.IndexOf ('>', cnt + 1) + 1;
				if (cnt >= text.Length) break;
			}
			set_color.chars [0] = ++cnt;
			if (talk_sfx && cnt % sfx_interval == 0) AudioPlayer.main.playSFX (AudioPlayer.channel_voice);
            //NEEDS REVAMP, DIALOGUE MANAGER IS DEPRECATED
			if (scroll_delay * DialogueManager.main.scroll_scale > 0 && !is_floating)
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
				TextEvents.main.evt_queue.Enqueue(TextEvents.main.playEvent (t.evt, t.opt));
				if (t.evt == "pause")
					yield return new WaitForSeconds (float.Parse(t.opt[0]));
			}
			d_item.text_events [start_pos] = null;
		}
	}

	// Sets dialogue box's height based on text. Also updates dialogue window height
	void setBoxHeight() {
		rect_tr.sizeDelta = new Vector2 (rect_tr.sizeDelta.x, text_pad + fx_text.preferredHeight);
		//DialogueManager.main.expandWindow (rect_tr.sizeDelta.y); (NEEDS REVAMP (IN AN/CHAT VIEW?)
	}
}
