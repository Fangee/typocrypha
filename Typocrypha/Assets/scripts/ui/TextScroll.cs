using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// displays text character by character
public class TextScroll : MonoBehaviour {
	public float delay; // time between displaying characters
	public bool is_print; // is currently printing

	string in_text; // current text being printed out
	string out_buffer; // buffer for what will be shown
	Stack<Pair<string, string>> tag_stack; // stores currently used tags
	Text out_text; // where text gets outputted
	Coroutine curr; // current printing coroutine

	void Start() {
		is_print = false;
		tag_stack = new Stack<Pair<string, string>> ();
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

	// start deleting string in Text display
	public void startDelete(string in_txt, Text out_txt, string delete_sfx) {
		AudioPlayer.main.setSFX (3, SFXType.SPEAKING, delete_sfx); // put sfx in channel 3
		in_text = in_txt;
		out_text = out_txt;
		out_text.text = in_text;
		is_print = true;
		curr = StartCoroutine (deleteScrollText ());
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
		out_buffer = "";
		while (text_pos < in_text.Length) {
			if (in_text [text_pos].CompareTo ('<') == 0) { // check if tag
				if (in_text [text_pos + 1].CompareTo ('/') == 0) { // check if end tag (pop)
					string end_tag = tag_stack.Pop().second; // remove tag from stack
					out_buffer += end_tag;                   // place end tag directly into buffer
					text_pos += end_tag.Length;              // move text_pos over after tag
				} else { // otherwise, is a start tag (push)
					Pair<string, string> tag = new Pair<string, string>();
					int fstart_pos, fend_pos, estart_pos, eend_pos; // mark the '<' and '>' for the start and end respectively
					// get starting tag
					fstart_pos = text_pos;
					fend_pos = in_text.IndexOf ('>', fstart_pos + 1);
					tag.first = in_text.Substring (fstart_pos, fend_pos - fstart_pos + 1);
					// get ending tag
					estart_pos = in_text.IndexOf ('<', fend_pos + 1);
					eend_pos = in_text.IndexOf ('>', estart_pos + 1);
					tag.second = in_text.Substring (estart_pos, eend_pos - estart_pos + 1);
					tag_stack.Push (tag);    // add tag to tag stack
					text_pos = fend_pos + 1; // set new text_pos at end of start tag
					out_buffer += tag.first; // add start tag to out_buffer (end tag is added later)
					//Debug.Log("new tag:" + tag.first + ":" + tag.second);
				}
			}
			if (text_pos >= in_text.Length) break;
			AudioPlayer.main.playSFX (3); // play speaking sfx
			out_buffer += in_text [text_pos++];
			out_text.text = out_buffer + tag_stack.Aggregate ("", (acc, next) => acc + next.second); // functional programming trick to build end tags
			yield return new WaitForSeconds (delay);
		}
		is_print = false;
	}

	// deletes characters currently in buffer one by one
	IEnumerator deleteScrollText() {
		int text_pos = in_text.Length - 2;
		while (text_pos >= 0) {
			out_text.text = out_text.text.Substring(0, text_pos--);
			yield return new WaitForSeconds (delay);
		}
		is_print = false;
	}
}
