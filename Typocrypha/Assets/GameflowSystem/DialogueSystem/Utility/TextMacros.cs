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
    public Dictionary<char, char> translate_map;

	void Awake () {
		if (main == null) main = this;
		macro_map = new Dictionary<string, MacroSubDel> {
			{"name", macroNameSub},
			{"NAME", macroNameSub},
			{"pronoun",macroPronoun},
            {"info", macroInfo},
            {"i", macroInfo},
            {"v", macroInfo},
            {"last-cast", macroLastCast},
            {"last-cast-enemy", macroLastCastEnemy},
            {"time", macroTime},
			{"c", macroColor},
			{"t", macroSetTalkSfx},
			{"h", macroHighlightCharacter},
            {"hc", macroHighlightCodec},
            {"speak", macroSpeaker},
            {"tl", macroTranslate},
            {"translate", macroTranslate},
			{"languageTL", macroTranslatedLanguage},
			{"rc", macroRemoveCharacter}
        };
		color_map = new Dictionary<string, string> {
			{ "spell",      "#ff6eff" },
			{ "ui-terms",   "#05abff" },
			{ "evil-eye",   "#ff0042" },
			{ "enemy-talk", "#974dfe" },
			{ "enemy-name", "#16e00c" },
			{ "tips",       "#ffdb16" },
			{ "whisper",    "#c8c8c8" },
			{ "highlight",  "#ff840c" },
			{ "mc",         "#d043e2" },
			{ "illyia",     "#c70126" },
			{ "dahlia",     "#8097e0" },
			{ "doppel",     "#E0015A" }
		};
		character_map = new Dictionary<string, Pair<string, string>> {
			{"dahlia", new Pair<string, string>("dahlia", "vo_dahlia") },
			{"illyia", new Pair<string, string>("illyia", "vo_illyia") },
			{"mackey", new Pair<string, string>("mackey", "vo_mackey") },
			{"mc", new Pair<string, string>("_mc_", "vo_mc")},
			{"doppelganger", new Pair<string, string>("doppelganger", "vo_doppelganger")},
			{"clarke", new Pair<string, string>("clarke", "vo_clarke")},
			{"iris", new Pair<string, string>("iris", "vo_iris")},
			{"cat", new Pair<string, string>("cat", "vo_cat")},
			{"cat_person", new Pair<string, string>("cat_person", "vo_cat_person")},
			{"evil_eye", new Pair<string, string>("evil_eye", "vo_evil_eye")},
			{"marcel", new Pair<string, string>("marcel", "vo_marcel")}
		};
        translate_map = new Dictionary<char, char> {
            {'a', 'e' }, {'b', 'd' }, {'c', 'k' }, {'d', 'b' }, {'e', 'i' },
            {'f', 'h' }, {'g', 'h' }, {'h', 'f' }, {'i', 'a' }, {'j', 'z' },
            {'k', 'x' }, {'l', 't' }, {'m', 'l' }, {'n', 'v' }, {'o', 'u' },
            {'p', 'g' }, {'q', 'n' }, {'r', 'w' }, {'s', 'y' }, {'t', 'm' },
            {'u', 'o' }, {'v', 'r' }, {'w', 'p' }, {'x', 'c' }, {'y', 's' },
            {'z', 'j' }, {'.', ',' }, {',', '.' }
        };
	}

	// substitutes player's name
	// input: NONE
	string macroNameSub(string[] opt) {
		return PlayerDataManager.main.PlayerName;
	}

	// substitutes in appropriate pronoun term
	// choice is made based on 'PlayerDialogueInfo' field
	// input: [0]: string: appropriate term for FEMININE pronoun
	// input: [1]: string: appropriate term for INCLUSIVE pronoun
	// input: [2]: string: appropriate term for FIRSTNAME pronoun
	//   NOTE: input string is concatenated after player's name
	// input: [3]: string: appropriate term for MASCULINE pronoun
	string macroPronoun(string[] opt) {
		switch (PlayerDataManager.main.player_pronoun) {
		case Pronoun.FEMININE:  return opt [0];
		case Pronoun.INCLUSIVE: return opt [1];
		case Pronoun.FIRSTNAME: return PlayerDataManager.main.PlayerName + opt [2];
		case Pronoun.MASCULINE: return opt [3];
		default: return "pronoun";
		}
	}

    // substitutes in info from the PlayerDialogueInfo map
    // input: [0]: string: info key
    string macroInfo(string[] opt) {
        return PlayerDataManager.main.getData(opt[0]);
    }

	// substitutes last cast spell's attributes
	// input: [0]: string, "elem","root","style" : specifies which part of spell to display (or "all" for whole spell)
	string macroLastCast(string[] opt) {
        string ret = string.Empty;
		switch (opt [0]) {
		    case "elem":  ret = BattleManagerS.main.field.last_player_spell.element.ToUpper(); break;
		    case "root":  ret =  BattleManagerS.main.field.last_player_spell.root.ToUpper(); break;
		    case "style": ret = BattleManagerS.main.field.last_player_spell.style.ToUpper(); break;
            case "all": ret = BattleManagerS.main.field.last_player_spell.ToString(); break;
            default:      return "error: bad spell substitute macro argument";	
		}
		return "<color=" + color_map["spell"] + ">" + ret + "</color>";
	}

    // substitutes last cast spell's attributes
    // input: [0]: string, "elem","root","style" : specifies which part of spell to display (or "all" for whole spell)
    string macroLastCastEnemy(string[] opt) {
        string ret = string.Empty;
        switch (opt[0])
        {
            case "elem": ret = BattleManagerS.main.field.last_enemy_spell.element.ToUpper(); break;
            case "root": ret = BattleManagerS.main.field.last_enemy_spell.root.ToUpper(); break;
            case "style": ret = BattleManagerS.main.field.last_enemy_spell.style.ToUpper(); break;
            case "all": ret = BattleManagerS.main.field.last_enemy_spell.ToString(); break;
            default: return "error: bad spell substitute macro argument";
        }
        return "<color=" + color_map["spell"] + ">" + ret + "</color>";
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

    // substitutes in 'highlight-codec' TextEvent.
    string macroHighlightCodec(string[] opt)
    {
        return "[sole-highlight-codec]";
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

    string macroTranslate(string[] opt) {
        char[] op = opt[0].ToCharArray(); //char array is faster than StringBuilder here because mutations are simple
        for(int i = 0; i < op.Length; ++i) {
            if (translate_map.ContainsKey(op[i]) && char.IsLower(op[i]))
                op[i] = translate_map[op[i]];
            else if (translate_map.ContainsKey(char.ToLower(op[i])))
                op[i] = char.ToUpper(translate_map[char.ToLower(op[i])]);
        }
        return new string (op);
    }

	string macroTranslatedLanguage(string[] opt) {
		return "Ihsuik";
	}

	// substitutes 'remove-character'
	// input: [0]: string, name of character sprite (spr_vn_ omitted)
	string macroRemoveCharacter(string[] opt) {
		string macro = 
			"[remove-character=spr_vn_" + opt[0] + "]";
		return macro;
	}
}
