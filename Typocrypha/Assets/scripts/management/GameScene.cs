using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// represents a scene in the game: either a cutscene or a battle scene
public class GameScene {
	public string music_track; // name of music tracked played

	public GameScene(string i_music_track) {
		music_track = i_music_track;
	}
}

// represents the intro scene
public class IntroScene : GameScene {
	// constructs a new intro scene
	public IntroScene(string i_music_track) : base(i_music_track) {
	}
}

// represents a cutscene
public class CutScene : GameScene {
	public string[] npcs; // array of npc names (TEMP: should be npc objects)
	public string[] whos_talking; // who is saying each line of dialogue
	public string[] dialogue; // lines of dialogue for this scene
	public string[] npc_sprites; // name of sprite files for each talking scene

	// constructs a new cutscene
	public CutScene(string i_music_track, string[] i_npcs, string[] i_whos_talking, 
		string[] i_dialogue, string[] i_npc_sprites) : base(i_music_track) {
		npcs = i_npcs;
		whos_talking = i_whos_talking;
		dialogue = i_dialogue;
		npc_sprites = i_npc_sprites;
	}

}

// represents a battle scene
public class BattleScene : GameScene {
	public EnemyStats[] enemy_stats; // stats for all enemies

	public BattleScene(string i_music_track, EnemyStats[] i_enemy_stats) : base(i_music_track) {
		enemy_stats = i_enemy_stats;
	}
}