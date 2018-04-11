using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// manages pretitle sequence
public class PreTitle : MonoBehaviour {
	public TextScroll text_scroll; // scrolls text character by character
	public Text prompt_text; // prompts player
	public Text input_text; // player's input
	public string target_input; // what player should enter
	public SpriteRenderer dimmer; // dims screen
	public SpriteRenderer screen_dimmer; // dims screen (above everything else)
	public TitleScreen title_screen; // title screen component
	public GameObject caret; // caret to show where to type next

	bool input_ready; // can player input?
	Regex alpha_space; // is character an alphanumeric or a space?
	string buffer; // contains input text
	int count; // keeps track of number of things typed
	float caret_x; // x pos of caret
	float caret_y; // y pos of caret
	Coroutine blink_caret; // blinks caret

	void Awake () {
		input_ready = false;
		alpha_space = new Regex ("[ a-zA-Z]");
		buffer = "";
	}

	void Start () {
		StartCoroutine (sequence ());
		caret_x = caret.GetComponent<RectTransform> ().localPosition.x;
		caret_y = caret.GetComponent<RectTransform> ().localPosition.y;
	}

	void Update() {
		if (!input_ready) return;
		// check key presses
		if (Input.GetKeyDown (KeyCode.Return)) {
			AudioPlayer.main.playSFX ("sfx_enter");
			buffer = buffer.Trim().ToUpper();
			if (buffer.CompareTo (target_input) == 0) { // check correct input
				input_ready = false;
				StopCoroutine (blink_caret);
				caret.SetActive (false);
				StartCoroutine (transitionToTitle ());
			} else {
				buffer = "";
				count = 0;
			}
		} else if (Input.GetKey (KeyCode.Backspace)) {
			if (Input.GetKeyDown (KeyCode.Backspace)) {
				if (count > 0) {
					buffer = buffer.Remove (count - 1, 1);
					count = count - 1;
				}
			}
		} else {
			string in_str = Input.inputString;
			foreach (char c in in_str) {
				AudioPlayer.main.playSFX ("sfx_type_key");
				if (alpha_space.IsMatch(c.ToString())) {
					buffer += c.ToString();
					++count;
					StopCoroutine (blink_caret);
					blink_caret = StartCoroutine (blinkCaret ());
				}
			}
		}
		// update display
		input_text.text = buffer.ToUpper();
		updateCaretPos ();
	}

	// execute pre-title sequence
	IEnumerator sequence() {
		float alpha = 1;
		while (alpha > 0) {
			screen_dimmer.color = new Color (0, 0, 0, alpha);
			alpha -= 0.005f;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitForSeconds (0.5f);
		text_scroll.startPrint ("[set-talk-sfx=]Please type in the following phrase.", prompt_text);
		yield return new WaitWhile(() => text_scroll.is_print);
		yield return new WaitForSeconds (0.5f);
		prompt_text.text = target_input;
		input_ready = true;
		blink_caret = StartCoroutine (blinkCaret ());
	}

	// updates caret's position
	void updateCaretPos() {
		int num_thin = 0;
		int num_med = 0;
		int num_else = 0;
		foreach (char c in input_text.text) {
			if (c.CompareTo (' ') == 0 || c.CompareTo ('I') == 0)
				++num_thin;
			else if (c.CompareTo ('F') == 0 || c.CompareTo ('J') == 0 || 
				c.CompareTo ('E') == 0 || c.CompareTo ('L') == 0 || 
				c.CompareTo ('Z') == 0 || c.CompareTo ('S') == 0)
				++num_med;
			else
				++num_else;
		}
		float new_x = caret_x + 2 + (num_thin * 16) + (num_med * 16) + (num_else * 16); //(num_thin * 9) + (num_med * 21) + (num_else * 25);
		caret.GetComponent<RectTransform>().localPosition = new Vector2(new_x, caret_y);
	}

	// blinks caret
	IEnumerator blinkCaret() {
		caret.SetActive (true);
		while (input_ready) {
			yield return new WaitForSeconds (0.5f);
			caret.SetActive (!caret.activeSelf);
		}
	}

	// transitions to title screen
	IEnumerator transitionToTitle() {
		// flash phrase
		for (int i = 0; i < 4; ++i) {
			yield return new WaitForSeconds (0.25f);
			input_text.gameObject.SetActive (false);
			yield return new WaitForSeconds (0.25f);
			input_text.gameObject.SetActive (true);
		}
		// fade text
		float alpha = 1;
		while (alpha > 0) {
			input_text.color = new Color (1, 1, 1, alpha);
			prompt_text.color = new Color (1, 1, 1, alpha);
			alpha -= 0.01f;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitForSeconds (1f);
		// fade screen to title
		alpha = 1;
		while (alpha > 0) {
			dimmer.color = new Color (0, 0, 0, alpha);
			alpha -= 0.005f;
			yield return new WaitForEndOfFrame ();
		}
		title_screen.startTitle ();
	}
}
