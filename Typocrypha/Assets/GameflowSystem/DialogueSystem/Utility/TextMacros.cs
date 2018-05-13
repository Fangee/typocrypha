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
			{"NAME", macroNameSub},
			{"pronoun",macroPronoun},
			{"last-cast", macroLastCast},
			{"time", macroTime}
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
		switch (opt [0]) {
		case "elem":  return BattleManagerS.main.field.last_spell.element.ToUpper();
		case "root":  return BattleManagerS.main.field.last_spell.root.ToUpper();
		case "style": return BattleManagerS.main.field.last_spell.style.ToUpper();
		default:      return "error: bad spell substitute macro argument";	
		}
		//return "unimplemented";
	}

	// substitutes with current time
	// input: NONE
	string macroTime(string[] opt) {
		return System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
	}
}
