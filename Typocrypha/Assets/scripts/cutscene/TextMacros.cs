using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// represents a macro substitution function
public delegate string MacroSubDel(string[] opt);

// event class for text macro substitions
public class TextMacros : MonoBehaviour {
	public static TextMacros main = null; // global static ref
	public Dictionary<string, MacroSubDel> macro_map; // for substituting macros

	void Awake () {
		if (main == null) main = this;
		macro_map = new Dictionary<string, MacroSubDel> {
			{"name", macroNameSub},
			{"last-cast", macroLastCast}
		};
	}

	// substitutes player's name
	// input: NONE
	string macroNameSub(string[] opt) {
		return PlayerDialogueInfo.main.player_name;
	}

	// substitutes last cast spell's attributes
	// input: [0]: string, "elem","root","style" : specifies which part of spell to display
	string macroLastCast(string[] opt) {
		switch (opt [0]) {
		case "elem":  return BattleManager.main.last_spell.element;
		case "root":  return BattleManager.main.last_spell.root;
		case "style": return BattleManager.main.last_spell.style;
		default:      return "error: bad spell substitute macro argument";	
		}
	}
}
