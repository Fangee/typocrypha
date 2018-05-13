using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// represents a macro substitution function
public delegate string MacroSubDel(string[] opt);

// event class for text macro substitions
public class TextMacros : MonoBehaviour {
	public static TextMacros main = null; // global static ref
	public Dictionary<string, MacroSubDel> macro_map; // for substituting macros
	public Dictionary<string, string> color_map; // for color presets (hex representation)
	public Dictionary<string, Pair<string, string>> character_map; // for character dialogue presets

	void Awake () {
		if (main == null) main = this;
		macro_map = new Dictionary<string, MacroSubDel> {
			{"name", macroNameSub},
			{"NAME", macroNameSub},
			{"pronoun",macroPronoun},
			{"last-cast", macroLastCast},
			{"time", macroTime},
			{"c", macroColor},
			{"t", macroSetTalkSfx},
			{"h", macroHighlightCharacter},
			{"speak", macroSpeaker}
		};
		color_map = new Dictionary<string, string> {
			{ "spell",      "#ff6eff" },
			{ "ui-terms",   "#05abff" },
			{ "evil-eye",   "#ff0042" },
			{ "enemy-talk", "#be2dd4" },
			{ "enemy-name", "#16e00c" },
			{ "tips",       "#ffdb16" },
			{ "whisper",    "#c8c8c8" },
			{ "highlight",  "#ff840c" }
		};
		character_map = new Dictionary<string, Pair<string, string>> {
			{"dahlia", new Pair<string, string>("dahlia", "vo_dahlia") },
			{"illyia", new Pair<string, string>("illyia", "vo_illyia") },
			{"mackey", new Pair<string, string>("mackey", "vo_mackey") },
			{"mc", new Pair<string, string>("_mc_", "vo_mc")}
		};
	}

	// substitutes player's name
	// input: NONE
	string macroNameSub(string[] opt) {
		return PlayerDialogueInfo.main.player_name;
	}

	// substitutes in appropriate pronoun term
	// choice is made based on 'PlayerDialogueInfo' field
	// input: [0]: string: appropriate term for FEMININE pronoun
	// input: [1]: string: appropriate term for INCLUSIVE pronoun
	// input: [2]: string: appropriate term for FIRSTNAME pronoun
	//   NOTE: input string is concatenated after player's name
	// input: [3]: string: appropriate term for MASCULINE pronoun
	string macroPronoun(string[] opt) {
		switch (PlayerDialogueInfo.main.player_pronoun) {
		case Pronoun.FEMININE:  return opt [0];
		case Pronoun.INCLUSIVE: return opt [1];
		case Pronoun.FIRSTNAME: return PlayerDialogueInfo.main.player_name + opt [2];
		case Pronoun.MASCULINE: return opt [3];
		default: return "pronoun";
		}
	}

	// substitutes last cast spell's attributes
	// input: [0]: string, "elem","root","style" : specifies which part of spell to display
	string macroLastCast(string[] opt) {
		/*
		switch (opt [0]) {
		case "elem":  return BattleManager.main.last_spell.element;
		case "root":  return BattleManager.main.last_spell.root;
		case "style": return BattleManager.main.last_spell.style;
		default:      return "error: bad spell substitute macro argument";	
		}
		*/
		return "unimplemented";
	}

	// substitutes with current time
	// input: NONE
	string macroTime(string[] opt) {
		return System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
	}

	// substitutes in appropriate color tag
	// input: [0]: string, color name (must be implemented in Unity rich tags)
	//             if argument is empty, subsitutes the closing tag '</color>'
	string macroColor(string[] opt) {
		if (opt [0] != null && opt[0] != "") {
			if (color_map.ContainsKey(opt[0]))
				return "<color=" + color_map[opt[0]] + ">";
			else return "<color=" + opt [0] + ">";
		} else {
			return "</color>";
		}
	}

	// substitues in 'set-talk-sfx' TextEvent
	// input: [0]: string, name of audio file
	string macroSetTalkSfx(string[] opt) {
		return "[set-talk-sfx=vo_" + opt[0] + "]";
	}

	// substitutes in 'highlight-character' TextEvent.
	// input: [0]: string, name of sprite to highlight
	//        [1]: float, amount to highlight (multiplier to tint)
	string macroHighlightCharacter(string[] opt) {
		return "[highlight-character=" + opt[0] + "," + opt[1] + "]";
	}

	// substitutes combined 'set-talk-sfx' and 'highlight-character'
	// solely highlights given character, and switches to their talk sfx
	// input: [0]: string, name of character (see character map)
	string macroSpeaker(string[] opt) {
		string macro = 
			"[set-talk-sfx=" + character_map[opt[0]].second + "]" +
			"[sole-highlight=" + character_map[opt[0]].first + "]";
		return macro;
	}
}
