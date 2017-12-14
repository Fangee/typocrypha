using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// loads gameflow file that defines dialgue, battles, etc
public class LoadGameFlow : MonoBehaviour {
	public string file_name; // name of gameflow file
	public bool is_loaded; // is the gameflow done loading?
	public GameScene[] scene_arr; // array of gamescenes
    public EnemyDatabase enemy_data;//Enemy database

	TextAsset text_file; // original text asset
	char[] line_delim = { '\n' };
	char[] col_delim = { '\t' };

	void Start () {
		is_loaded = false;
        EnemyDatabase.main.build();//Build enemy database (so data is ready for cutscene building
        enemy_data = EnemyDatabase.main;
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
		List<string> music_tracks = new List<string>();
		int i = pos;
		for (; i < lines.Length; i++) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("NPC") == 0) { // read in npc
				npcs.Add (cols [1]);
			} else if (cols [0].CompareTo ("DIALOGUE") == 0) { // read in dialogue
				whos_talking.Add (cols[1]);
				dialogue.Add (cols [2]);
				npc_sprites.Add (cols [3].Trim());
				music_tracks.Add (cols [4].Trim());
			} else { // otherwise, scene is done
				break;
			}
		}
		scene_arr [curr_scene] = new CutScene (npcs.ToArray (), whos_talking.ToArray(), 
			dialogue.ToArray (), npc_sprites.ToArray(), music_tracks.ToArray());
		return i;
	}

	// parses a battle scene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseBattle(string[] lines, int pos, int curr_scene) {
		// read in lines of scene
		List<string> music_tracks = new List<string>();
		List<EnemyStats> enemies = new List<EnemyStats>();
		List<BattleInterrupt> interrupts = new List<BattleInterrupt> ();
		int i = pos;
		for (; i < lines.Length; i++) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("MUSIC") == 0) {
				for (int j = 1; j < cols.Length; ++j) {
					if (cols [j].CompareTo ("") != 0) {
						music_tracks.Add (cols [j].Trim ());
					}
				}
			} else if (cols [0].CompareTo ("ENEMY") == 0) { // read in enemy
				EnemyStats new_stats = enemy_data.getData (cols [1]);
				enemies.Add (new_stats);
			} else if (cols[0].CompareTo("INTERRUPT") == 0) { // interrupt scene
				i = parseInterrupt(lines, i, interrupts);
			} else { // otherwise, scene is done
				break;
			}
		}
		scene_arr [curr_scene] = new BattleScene (enemies.ToArray (), music_tracks.ToArray(), 
			interrupts.ToArray());
		return i;
	}

	// parses battlescene interruption
	// returns line number at end of interrupt
	int parseInterrupt(string[] lines, int pos, List<BattleInterrupt> interrupts) {
		bool[] who_speak = { false, false, false, false };
		int who_cond = -1;
		float health_cond = -1;
		// parse first line, which contains the interrupt condition
		string[] first = lines[pos].Split (col_delim);
		// get who condition tracks
		switch (first [1].Trim()) { 
			case "LEFT_HEALTH": who_cond = 0; break;
			case "MIDDLE_HEALTH": who_cond = 1; break;
			case "RIGHT_HEALTH": who_cond = 2; break;
			case "PLAYER_HEALTH": who_cond = 3; break;
		}
		// get percent health condition
		float.TryParse(first[2], out health_cond);
		// get who's talking in scene
		foreach (char c in first[3]) {
			switch (c) {
				case 'L': who_speak [0] = true; break;
				case 'M': who_speak [1] = true; break;
				case 'R': who_speak [2] = true; break;
				case 'P':who_speak [3] = true; break;
			}
		}
		// parse dialogue
		List<string> whos_talking = new List<string> ();
		List<string> dialogue = new List<string> ();
		List<string> npc_sprites = new List<string> ();
		List<string> music_tracks = new List<string>();
		int i = pos + 1;
		for (; i < lines.Length; ++i) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("DIALOGUE") == 0) {
				whos_talking.Add (cols [1]);
				dialogue.Add (cols [2]);
				npc_sprites.Add (cols [3].Trim ());
				music_tracks.Add (cols [4].Trim ());
			} else {
				break;
			}
		}
		CutScene battle_cutscene = new CutScene (null, whos_talking.ToArray(), 
			dialogue.ToArray (), npc_sprites.ToArray(), music_tracks.ToArray());
		BattleInterrupt battle_interrupt = new BattleInterrupt ();
		battle_interrupt.scene = battle_cutscene;
		battle_interrupt.who_speak = who_speak;
		battle_interrupt.who_cond = who_cond;
		battle_interrupt.health_cond = health_cond;
		interrupts.Add (battle_interrupt);
		return i;
	}
}
