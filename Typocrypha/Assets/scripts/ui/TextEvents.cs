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
	public static Dictionary<string, TextEventDel> text_event_map;

	void Start() {
		if (main == null) main = this;
		if (text_event_map == null) text_event_map = new Dictionary<string, TextEventDel> {
			{"screen-shake", screenShake}
		};
	}

	// plays the event 'evt', returning the coroutine created (null if event doesnt exist)
	public Coroutine playEvent(string evt, string[] opt) {
		TextEventDel text_event;
		if (!text_event_map.TryGetValue (evt, out text_event)) return null;
		return StartCoroutine(text_event (opt));
	}

	// shakes the screen
	// input: string[2]
	//   string[0]: length of shake
	//   string[1]: intensity of shake
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
}

