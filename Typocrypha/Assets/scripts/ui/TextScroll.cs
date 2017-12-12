using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// displays text character by character
public class TextScroll : MonoBehaviour {
	public float delay; // time between displaying characters
	public bool is_print; // is currently printing

	string in_text; // current text being printed out
	Text out_text; // where text gets outputted
	Coroutine curr; // current printing coroutine

	void Start() {
		is_print = false;
	}

	// start printing string to Text display
	public void startPrint(string in_txt, Text out_txt, string speak_sfx) {
		AudioPlayer.main.setSFX (3, SFXType.SPEAKING, speak_sfx); // put sfx in channel 3
		in_text = in_txt;
		out_text = out_txt;
		out_text.text = "";
		is_print = true;
		curr = StartCoroutine (scrollText ());
	}

	// dump rest of string to output
	public void dump() {
		StopCoroutine (curr);
		is_print = false;
		out_text.text = in_text;
	}

	// print characters to Text object one by one
	IEnumerator scrollText() {
		int text_pos = 0;
		while (text_pos < in_text.Length) {
			AudioPlayer.main.playSFX (3); // play speaking sfx
			out_text.text += in_text [text_pos++];
			yield return new WaitForSeconds (delay);
		}
		is_print = false;
	}
}
