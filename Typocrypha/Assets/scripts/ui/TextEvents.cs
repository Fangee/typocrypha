using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
// edits by James Iwamasa

// represents an event to be played during text dialogue scrolling
public delegate IEnumerator TextEventDel(string[] opt);

// event class for events during text dialogue
public class TextEvents : MonoBehaviour {
	public static TextEvents main = null;
	public Dictionary<string, TextEventDel> text_event_map;
	public SpriteRenderer dimmer; // sprite used to cover screen for fade/dim effects
	public TextScroll text_scroll; // main text scroller for normal dialogue
	public GameObject center_text; // for showing floating text in the center

	void Start() {
		if (main == null) main = this;
		text_event_map = new Dictionary<string, TextEventDel> {
			{"screen-shake", screenShake},
			{"block", block},
			{"pause", pause},
			{"fade", fade},
			{"next", next},
			{"center-text-scroll", centerTextScroll},
			{"center-text-fade", centerTextFade},
			{"play-sfx",playSFX}
		};
	}

	// plays the event 'evt', returning the coroutine created (null if event doesnt exist)
	public Coroutine playEvent(string evt, string[] opt) {
		TextEventDel text_event;
		if (!text_event_map.TryGetValue (evt, out text_event)) return null;
		return StartCoroutine(text_event (opt));
	}

	// shakes the screen
	// input: [0]: float, length of shake
	//        [1]: float, intensity of shake
	IEnumerator screenShake(string[] opt) {
		Transform cam_tr = Camera.current.transform;
		Vector3 old_cam_pos = cam_tr.position;
		float[] opts = opt.Select((string str) => float.Parse(str)).ToArray();
		float curr_time = 0;
		float wait_time = opts [0] / 4;
		while (curr_time < wait_time) {
			cam_tr.position = Random.insideUnitCircle * opts[1];
			for (int i = 0; i < 4; ++i) yield return new WaitForEndOfFrame();
			curr_time += Time.deltaTime;
		}
		cam_tr.position = old_cam_pos;
	}

	// blocks or unblocks input
	// input: [t|f], 't' blocks input until 'f' unblocks
	IEnumerator block(string[] opt) {
		if (opt[0].CompareTo("t") == 0) CutsceneManager.main.enabled = false;
		else 		                    CutsceneManager.main.enabled = true;
		yield return true;
	}

	// causes dialogue and cutscene to pause: it is recommended to also block
	// input: [0]: float, length of pause
	IEnumerator pause(string[] opt) {
		text_scroll.pause_print = true; // pause text scroll
		yield return new WaitForSeconds (float.Parse (opt [0]));
		text_scroll.pause_print = false;
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
		float a_step = 1f * 0.017f / fade_time; // amount of change each frame
		if (a_step > 1) a_step = 1;
		if (opt [0].CompareTo ("out") == 0) { // hide screen
			alpha = 0;
			while (alpha < 1f) {
				yield return new WaitForSeconds(0.017f);
				alpha += a_step;
				dimmer.color = new Color (r, g, b, alpha);
			}
		} else { // show screen
			alpha = 1;
			while (alpha > 0f) {
				yield return new WaitForSeconds(0.017f);
				alpha -= a_step;
				dimmer.color = new Color (r, g, b, alpha);
			}
		}
	}

	// forces next line of dialogue (SHOULD BE PLACED AT END OF LINE)
	// input: none
	IEnumerator next(string[] opt) {
		CutsceneManager.main.forceNextLine ();
		yield return true;
	}

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

	// fades center text in or out
	// input: [0]: [in|out], 'in' re-reveals screen, 'out' hides it
	//        [1]: float, length of fade in seconds
	IEnumerator centerTextFade(string[] opt) {
		Text txt = center_text.GetComponent<Text> ();
		Debug.Log ("text fade:" + opt[0] + ":" + txt.color.a);
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
	// input: [0]: string, type
	//        [1]: string, sfx filename
	IEnumerator playSFX(string[] opt) {
		for (int i = 0; i < (int)SFXType.size; ++i) {
			if (((SFXType)i).ToString ().CompareTo (opt [0]) == 0) {
				AudioPlayer.main.playSFX (4, (SFXType)i, opt[1]);
				break;
			}
		}
		yield return true;
	}
}

