using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Preprocesses all macros and effects in a dialogue item
public class DialogParser : MonoBehaviour {
	public static DialogParser main = null; // Global static reference

	Dictionary<string,FXTextEffect> text_effect_map;
	Stack<FXTextEffect> effect_stack;
	char[] opt_delim = new char[1] { ',' };

	void Awake() {
		if (main == null) main = this;
		else Destroy (this);
		text_effect_map = new Dictionary<string, FXTextEffect> () { 
			{"color", gameObject.AddComponent<FXTextColor>()},
			{"shake", gameObject.AddComponent<FXTextShake>()},
			{"scramble", gameObject.AddComponent<FXTextScramble>()},
			{"wave", gameObject.AddComponent<FXTextWave>()},
			{"pulse", gameObject.AddComponent<FXTextPulse>()},
			{"color-cycle", gameObject.AddComponent<FXTextColorCycle>()}
		};
		effect_stack = new Stack<FXTextEffect> ();
	}

	// Parses dialogue item, returning updated text and modifying fields
	public string parse(DialogItem d_item, DialogBox d_box) {
		//Debug.Log ("parse:" + d_item.text);
		StringBuilder parsed = new StringBuilder(); // Processes string
		string text = substituteMacros(d_item.text);
		d_item.fx_text_effects = new List<FXTextEffect> ();
		d_item.text_events = new List<TextEvent>[text.Length];
		bool tag = false; // Are we parsing a tag?
		int i = 0;
		for (; i < text.Length; ++i) {
			char c = text [i];
			if (c == '|') { // FXTextEffect start tag
				tag = !tag;
				if (tag) parseEffectStart (i + 1, text, parsed, d_box);
			} else if (c == '\\') { // FXTextEffect end tag
				tag = !tag;
				if (tag) parseEffectEnd (i + 1, text, d_item, parsed);
			} else if (c == '[' || c == ']') { // Text Event
				i = parseTextEvent (i, text, d_item, parsed);
			} else if (!tag) {
				if(c == '’'){
					parsed.Append ("'");
				}
				else {
					parsed.Append (c);
				}
			}
		}
		return parsed.ToString ();
	}

	// Parses an effect's starting tag, and adds it to the stack
	// NEEDS TO ALSO PARSE PARAMETERS
	void parseEffectStart(int start_pos, string text, StringBuilder parsed, DialogBox d_box) {
		int end_pos = text.IndexOf ('|', start_pos) - 1;
		string fx_name = text.Substring (start_pos, end_pos - start_pos + 1);
		FXTextEffect text_effect = text_effect_map[fx_name].clone(d_box.fx_text.gameObject);
		text_effect.font = d_box.fx_text.font;
		text_effect.initEffect (); // TEMP; should read options from tag
		text_effect.chars = new int[2] {parsed.Length, -1};
		effect_stack.Push(text_effect);
	}

	// Parses an effect's ending tag, and matches with top of effect stack
	void parseEffectEnd(int start_pos, string text, DialogItem d_item, StringBuilder parsed) {
		int end_pos = text.IndexOf ('\\', start_pos) - 1;
		string fx_name = text.Substring (start_pos, end_pos - start_pos + 1);
		FXTextEffect top = effect_stack.Pop ();
		if (text_effect_map [fx_name].GetType () != top.GetType ())
			Debug.LogException (new System.Exception("Mismatched FXTextEffect tags:" + fx_name));
		top.chars [1] = parsed.Length;
		d_item.fx_text_effects.Add (top);
	}

	// Parses a Text Event
	int parseTextEvent(int start_pos, string text, DialogItem d_item, StringBuilder parsed) {
		int end_pos = text.IndexOf (']', start_pos);
		int eq_pos = text.IndexOf ('=', start_pos);
		string evt;
		string[] opt;
		if (eq_pos == -1 || eq_pos > end_pos) { // if no '='
			evt = text.Substring (start_pos + 1, end_pos - start_pos - 1);
			opt = new string[0]; 
		} else {
			evt = text.Substring (start_pos + 1, eq_pos - start_pos - 1);
			opt = text.Substring (eq_pos + 1, end_pos - eq_pos - 1).Split (opt_delim);
		}
		if (d_item.text_events [parsed.Length] == null)
			d_item.text_events [parsed.Length] = new List<TextEvent> ();
		d_item.text_events [parsed.Length].Add(new TextEvent(evt, opt));
		//Debug.Log ("  text_event:" + evt + ":" + opt.Aggregate("", (acc, next) => acc + "," + next));
		return end_pos;
	}

	// Substiutes macros in (curly braces)
	public string substituteMacros(string text) {
		StringBuilder true_str = new StringBuilder ();
		for (int i = 0; i < text.Length;) {
			if (text [i] == '{') {
				int start_pos = i + 1;
				int end_pos = text.IndexOf ('}', start_pos);
				string[] macro = text.Substring (start_pos, end_pos - start_pos).Split(opt_delim);
				Debug.Log ("  macro:" + macro.Aggregate("", (acc, next) => acc + "," + next));
				string[] opt = macro.Skip (1).Take (macro.Length - 1).ToArray ();
				string sub = TextMacros.main.macro_map [macro[0]] (opt);
				//Debug.Log ("    macro sub:" + sub);
				true_str.Append (sub);
				i = end_pos + 1;
			} else {
				true_str.Append (text [i++]);
			}
		}
		return true_str.ToString ();
	}
}
