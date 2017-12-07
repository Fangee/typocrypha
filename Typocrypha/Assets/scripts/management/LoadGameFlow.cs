using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// loads gameflow file that defines dialgue, battles, etc
public class LoadGameFlow : MonoBehaviour {
	public string file_name; // name of gameflow file
	public bool is_loaded; // is the gameflow done loading?
	public GameScene[] scene_arr; // array of gamescenes

	TextAsset text_file; // original text asset
	char[] line_delim = { '\n' };
	char[] col_delim = { '\t' };

	void Start () {
		is_loaded = false;
		parseFile (); // load gameflow file
		is_loaded = true;
	}

	// parses gameflow file which should be a tab-delimited txt file (from excel)
	void parseFile() {
		text_file = Resources.Load<TextAsset> (file_name);
		string[] lines = text_file.text.Split(line_delim);
		int scene_count = 0; // total number of scenes in game
		int.TryParse (lines [0].Split (col_delim) [0], out scene_count);
		scene_arr = new GameScene[scene_count];
		int curr_scene = 0; // current scene being loaded
		for (int i = 1; i < lines.Length; i++) { // go through each line
			string[] cols = lines[i].Split(col_delim);
			if (cols [0].Trim().CompareTo ("START_SCENE") == 0) { // start of a new scene
				if (cols [1].Trim().CompareTo ("INTRO") == 0) { // start of intro
					i = parseIntroScene(lines, i + 1, curr_scene++);
				} else if (cols [1].Trim().CompareTo ("CUTSCENE") == 0) { // start of cutscene
					i = parseCutScene (lines, i + 1, curr_scene++);
				} else if (cols [1].Trim().CompareTo ("BATTLE") == 0) { // start of battle
					i = parseBattle (lines, i + 1, curr_scene++);
				}
			} else { // at end of file
				break;
			}
		}
	}

	// parses intro scene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseIntroScene(string[] lines, int pos, int curr_scene) {
		scene_arr [curr_scene] = new IntroScene ();
		return pos;
	}

	// parses a cutscene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseCutScene(string[] lines, int pos, int curr_scene) {
		// read in lines of scene
		List<string> npcs = new List<string> ();
		List<string> whos_talking = new List<string> ();
		List<string> dialogue = new List<string> ();
		List<string> npc_sprites = new List<string> ();
		int i = pos;
		for (; i < lines.Length; i++) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("NPC") == 0) { // read in npc
				npcs.Add (cols [1]);
			} else if (cols [0].CompareTo ("DIALOGUE") == 0) { // read in dialogue
				whos_talking.Add (cols[1]);
				dialogue.Add (cols [2]);
				npc_sprites.Add (cols [3]);
			} else { // otherwise, scene is done
				break;
			}
		}
		scene_arr [curr_scene] = new CutScene (npcs.ToArray (), whos_talking.ToArray(), 
			dialogue.ToArray (), npc_sprites.ToArray());
		return i;
	}

	// parses a battle scene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseBattle(string[] lines, int pos, int curr_scene) {
		// read in lines of scene
		List<EnemyStats> enemies = new List<EnemyStats>();
		int i = pos;
		for (; i < lines.Length; i++) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("ENEMY") == 0) { // read in enemy
				EnemyStats new_stats = new EnemyStats();
				new_stats.name = cols [1];
				int.TryParse (cols [2], out new_stats.max_hp);
				float.TryParse (cols [3], out new_stats.atk_time);
				enemies.Add (new_stats);
			} else { // otherwise, scene is done
				break;
			}
		}
		scene_arr [curr_scene] = new BattleScene (enemies.ToArray ());
		return i;
	}
}
