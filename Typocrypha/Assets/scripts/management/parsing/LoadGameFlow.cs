using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// loads gameflow file that defines dialgue, battles, etc
public class LoadGameFlow : MonoBehaviour {
	public string file_name; // name of gameflow file
	public bool is_loaded; // is the gameflow done loading?
	public GameScene[] scene_arr; // array of gamescenes
    public EnemyDatabase enemy_data;//Enemy database
    public AllyDatabase ally_data;//Ally database
    public SpellDictionary spellDictionary;

	public List<GameScene> scene_list; // list of gamescenes (converted to array)
	TextAsset text_file; // original text asset
	char[] line_delim = { '\n' };
	char[] col_delim = { '\t' };
	char[] coord_delim = { ',' };
	Dictionary<string, Vector2> npc_sprite_pos; // maps position macros

	void Awake() {
		npc_sprite_pos = new Dictionary<string, Vector2> { 
			{"LEFT", new Vector2(-5, 1) }, 
			{"CENTER", new Vector2(0, 1) },
			{"RIGHT", new Vector2(5, 1) } 
		};
	}

    private const string system_name = "CLARKE";
    private const string system_sprite = "system";
    private const string register_track = "STOP";

	void Start () {
		is_loaded = false;
		EnemyDatabase.main.build();//Build enemy database (so data is ready for cutscene building)
        enemy_data = EnemyDatabase.main;
        AllyDatabase.main.build();
        ally_data = AllyDatabase.main;
        parseFile (); // load gameflow file
		is_loaded = true;
	}

	// parses gameflow file which should be a tab-delimited txt file (from excel)
	void parseFile() {
		text_file = Resources.Load<TextAsset> (file_name);
		string[] lines = substituteLinks(text_file.text.Split(line_delim));
		int scene_count = 0; // total number of scenes in game
		int.TryParse (lines [0].Split (col_delim) [0], out scene_count);
		scene_list = new List<GameScene> ();
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
		scene_arr = scene_list.ToArray ();
	}

	// substitutes in links to other spreadsheets
	// supports nested links, however, looping links will cause an infinite loop
	string[] substituteLinks(string[] lines) {
		int n = lines.Length;
		List<string> res_lines = new List<string> ();
		for (int i = 0; i < n; ++i) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].Trim ().CompareTo ("") == 0) continue;
			if (cols [0].Trim ().CompareTo ("LINK") == 0) { // substitute link
				TextAsset link_file = Resources.Load<TextAsset>(cols[1].Trim());
				string[] link_lines = substituteLinks(link_file.text.Split (line_delim));
				foreach (string link_line in link_lines)
					res_lines.Add (link_line);
			} else {
				res_lines.Add (lines [i]);
			}
		}
		foreach (string s in res_lines) Debug.Log ("substitue:" + s);
		return res_lines.ToArray ();
	}

	// parses intro scene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseIntroScene(string[] lines, int pos, int curr_scene) {
		scene_list.Add (new IntroScene ());
		return pos;
	}

	// parses a cutscene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseCutScene(string[] lines, int pos, int curr_scene) {
		// read in lines of scene
		List<string> whos_talking = new List<string> ();
		List<string> dialogue = new List<string> ();
		List<List<string>> npc_sprites = new List<List<string>> ();
		List<List<Vector2>> npc_pos = new List<List<Vector2>> ();
		List<string> music_tracks = new List<string>();
        List<CutsceneEvent> events = new List<CutsceneEvent>();
		int i = pos;
		for (; i < lines.Length; i++) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("DIALOGUE") == 0) { // read in dialogue
				whos_talking.Add (cols[1]);
				dialogue.Add (cols [2]);
				music_tracks.Add (cols [3].Trim());
				var npc_pair = parseNPCSprites (cols);
				npc_sprites.Add (npc_pair.first);
				npc_pos.Add (npc_pair.second);
                events.Add(null);
			} else if(cols [0].CompareTo ("REGISTER") == 0) { // read in register event
                //Add hard-coded attributes
                whos_talking.Add(system_name);
                music_tracks.Add(register_track);
                List<string> NPC = new List<string>();
                NPC.Add(system_sprite);
                npc_sprites.Add(NPC);
                List<Vector2> NPC_POS = new List<Vector2>();
                NPC_POS.Add(new Vector2(0, 1));
                npc_pos.Add(NPC_POS);
                //generate text and bake registration event
                List<string> words = new List<string>();
                for(int j = 1; cols[j].Trim().CompareTo("END") != 0; j++) {
                    words.Add(cols[j].Trim());
                }
                events.Add(new RegisterSpellEvent(words, spellDictionary));
                if (words.Count < 1) {
                    dialogue.Add("No new words");
                    continue;
                } else if(words.Count == 1) {
                    dialogue.Add("Keyword " + words[0] + " was registered");
                    continue;
                } else if(words.Count == 2) {
                    dialogue.Add("Keywords " + words[0] + " and " + words[1] + " were registered");
                    continue;
                }
                string wordlist = "Keywords ";
                for(int j = 0; j < words.Count; j++) {
                    if(j == words.Count - 1) {
                        wordlist += " and " + words[j];
                        break;
                    }
                    wordlist += words[j] + ", ";
                }
                wordlist += " were registered";
                dialogue.Add(wordlist);
            } else { // otherwise, scene is done
				break;
			}
		}
		string[][] npc_sprites_arr = npc_sprites.Select (a => a.ToArray ()).ToArray ();
		Vector2[][] npc_pos_arr = npc_pos.Select (a => a.ToArray ()).ToArray ();
		scene_list.Add(new CutScene (whos_talking.ToArray(), dialogue.ToArray (), 
			npc_sprites_arr, npc_pos_arr, music_tracks.ToArray(), events.ToArray()));
		return i;
	}

	// parses list of sprites to display for a line of dialogue
	Pair<List<string>, List<Vector2>> parseNPCSprites(string[] line) {
		Pair<List<string>, List<Vector2>> npc_pair = new Pair<List<string>, List<Vector2>> (); 
		npc_pair.first = new List<string> (); // list of sprite names
		npc_pair.second = new List<Vector2> (); // list of location Vector2s
		for (int i = 4;;i+=2) {
			string speaker = line [i].Trim();
			if (speaker.CompareTo ("END_DIALOGUE") == 0) break;
			npc_pair.first.Add (speaker); // add name of sprite
			string loc = line [i + 1].Trim ().Replace("\"", "");
			if (loc.Contains (",")) { // parse absolute location
				string[] coords = loc.Split (coord_delim);
				float x, y;
				float.TryParse (coords [0], out x);
				float.TryParse (coords [1], out y);
				npc_pair.second.Add (new Vector2 (x, y));
			} else { // preset macro location (i.e. "LEFT" "RIGHT" etc)
				npc_pair.second.Add (npc_sprite_pos[loc]);
			}
		}
		return npc_pair;
	}

	// parses a battle scene; pos is the line number of the scene in file
	// returns line number at the end of this scene in file
	int parseBattle(string[] lines, int pos, int curr_scene) {
		// read in lines of scene
		List<string> music_tracks = new List<string>();
		List<EnemyStats> enemies = new List<EnemyStats>();
        List<AllyStats> allies = new List<AllyStats>();
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
			} else if(cols [0].CompareTo ("ALLY") == 0) {
                AllyStats new_stats = ally_data.getData(cols[1]);
                allies.Add(new_stats);
            } else if (cols[0].CompareTo("INTERRUPT") == 0) { // interrupt scene
				i = parseInterrupt(lines, i, interrupts);
			} else { // otherwise, scene is done
				break;
			}
		}
		scene_list.Add(new BattleScene (enemies.ToArray (), allies.ToArray(), music_tracks.ToArray(), 
			interrupts.ToArray()));
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
				case 'P': who_speak [3] = true; break;
			}
		}
		// parse dialogue
		List<string> whos_talking = new List<string> ();
		List<string> dialogue = new List<string> ();
		List<List<string>> npc_sprites = new List<List<string>> ();
		List<List<Vector2>> npc_pos = new List<List<Vector2>> ();
		List<string> music_tracks = new List<string>();
        List<CutsceneEvent> events = new List<CutsceneEvent>();
		int i = pos + 1;
		for (; i < lines.Length; ++i) {
			string[] cols = lines [i].Split (col_delim);
			if (cols [0].CompareTo ("DIALOGUE") == 0) {
				whos_talking.Add (cols [1]);
				dialogue.Add (cols [2]);
				music_tracks.Add (cols [3].Trim ());
				var npc_pair = parseNPCSprites (cols);
				npc_sprites.Add (npc_pair.first);
				npc_pos.Add (npc_pair.second);
                events.Add(null);
			} else if (cols[0].CompareTo("REGISTER") == 0)
            { // read in register event
                //Add hard-coded attributes
                whos_talking.Add(system_name);
                music_tracks.Add(register_track);
                List<string> NPC = new List<string>();
                NPC.Add(system_sprite);
                npc_sprites.Add(NPC);
                List<Vector2> NPC_POS = new List<Vector2>();
                NPC_POS.Add(new Vector2(0, 1));
                npc_pos.Add(NPC_POS);
                //generate text and bake registration event
                List<string> words = new List<string>();
                for (int j = 1; cols[j].Trim().CompareTo("END") != 0; j++)
                {
                    words.Add(cols[j].Trim());
                }
                events.Add(new RegisterSpellEvent(words, spellDictionary));
                if (words.Count < 1)
                {
                    dialogue.Add("No new words");
                    continue;
                }
                else if (words.Count == 1)
                {
                    dialogue.Add("Keyword " + words[0] + " was registered");
                    continue;
                }
                else if (words.Count == 2)
                {
                    dialogue.Add("Keywords " + words[0] + " and " + words[1] + " were registered");
                    continue;
                }
                string wordlist = "Keywords ";
                for (int j = 0; j < words.Count; j++)
                {
                    if (j == words.Count - 1)
                    {
                        wordlist += " and " + words[j];
                        break;
                    }
                    wordlist += words[j] + ", ";
                }
                wordlist += " were registered";
                dialogue.Add(wordlist);
            }
            else {
				break;
			}
		}
		string[][] npc_sprites_arr = npc_sprites.Select (a => a.ToArray ()).ToArray ();
		Vector2[][] npc_pos_arr = npc_pos.Select (a => a.ToArray ()).ToArray ();
		CutScene battle_cutscene = new CutScene (whos_talking.ToArray(), dialogue.ToArray (), 
			npc_sprites_arr, npc_pos_arr, music_tracks.ToArray(), events.ToArray());
		BattleInterrupt battle_interrupt = new BattleInterrupt ();
		battle_interrupt.scene = battle_cutscene;
		battle_interrupt.who_speak = who_speak;
		battle_interrupt.who_cond = who_cond;
		battle_interrupt.health_cond = health_cond;
		interrupts.Add (battle_interrupt);
		return i;
	}
}
