using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// edits by James Iwamasa

// represents an event to be played during text dialogue scrolling
public delegate IEnumerator TextEventDel(string[] opt);

// event class for events during text dialogue
public class TextEvents : MonoBehaviour {
	public static TextEvents main = null;
	public Dictionary<string, TextEventDel> text_event_map;
	public SpriteRenderer dimmer; // sprite used to cover screen for fade/dim effects
	public TextScroll text_scroll; // main text scroller for normal dialogue

	void Start() {
		if (main == null) main = this;
		text_event_map = new Dictionary<string, TextEventDel> {
			{"screen-shake", screenShake},
			{"block", block},
			{"pause", pause},
			{"fade", fade},
			{"next", next}
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
	IEnumerator fade(string[] opt) {
		float fade_time = float.Parse (opt [1]);
		float alpha;
		float a_step = 1f * 0.017f / fade_time; // amount of change each frame
		if (opt [0].CompareTo ("out") == 0) { // hide screen
			alpha = 0;
			while (alpha < 1f) {
				yield return new WaitForSeconds(0.017f);
				alpha += a_step;
				dimmer.color = new Color (0, 0, 0, alpha);
			}
		} else { // show screen
			alpha = 1;
			while (alpha > 0f) {
				yield return new WaitForSeconds(0.017f);
				alpha -= a_step;
				dimmer.color = new Color (0, 0, 0, alpha);
			}
		}
	}

	// forces next line of dialogue (SHOULD BE PLACED AT END OF LINE)
	// input: none
	IEnumerator next(string[] opt) {
		CutsceneManager.main.forceNextLine ();
		yield return true;
	}
}

