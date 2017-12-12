using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// represents a scene in the game: either a cutscene or a battle scene
public class GameScene {
	
}

// represents the intro scene
public class IntroScene : GameScene {
	// constructs a new intro scene
	public IntroScene() {
	}
}

// represents a cutscene
public class CutScene : GameScene {
	public string[] npcs; // array of npc names (TEMP: should be npc objects)
	public string[] whos_talking; // who is saying each line of dialogue
	public string[] dialogue; // lines of dialogue for this scene
	public string[] npc_sprites; // name of sprite files for each talking scene
	public string[] music_tracks; // names of tracks for each dialogue sequence

	// constructs a new cutscene
	public CutScene(string[] i_npcs, string[] i_whos_talking, 
		string[] i_dialogue, string[] i_npc_sprites, string[] i_music_tracks) {
		npcs = i_npcs;
		whos_talking = i_whos_talking;
		dialogue = i_dialogue;
		npc_sprites = i_npc_sprites;
		music_tracks = i_music_tracks;
	}

}

// represents a battle scene
public class BattleScene : GameScene {
	public EnemyStats[] enemy_stats; // stats for all enemies
	public string[] music_tracks; // music tracks for battle

	public BattleScene(EnemyStats[] i_enemy_stats, string[] i_music_tracks) {
		enemy_stats = i_enemy_stats;
		music_tracks = i_music_tracks;
	}
}