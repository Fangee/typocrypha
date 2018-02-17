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
	public string[] whos_talking; // who is saying each line of dialogue
	public string[] dialogue; // lines of dialogue for this scene
	public string[][] npc_sprites; // names of sprite files for each talking scene
	public Vector2[][] npc_pos; // positions of sprites
	public string[] music_tracks; // names of tracks for each dialogue sequence
    public CutsceneEvent[] events;

	// constructs a new cutscene
	public CutScene(string[] i_whos_talking, string[] i_dialogue, 
		string[][] i_npc_sprites, Vector2[][] i_npc_pos, string[] i_music_tracks, CutsceneEvent[] events) {
		whos_talking = i_whos_talking;
		dialogue = i_dialogue;
		npc_sprites = i_npc_sprites;
		npc_pos = i_npc_pos;
		music_tracks = i_music_tracks;
        this.events = events;
	}

}

// represents a battle scene
public class BattleScene : GameScene {
	public EnemyStats[] enemy_stats; // stats for all enemies
    public AllyStats[] ally_stats; // stats for all allies
	public string[] music_tracks; // music tracks for battle
	public BattleInterrupt[] interrupts; // mid battle cutscenes

	public BattleScene(EnemyStats[] i_enemy_stats, AllyStats[] i_ally_stats, string[] i_music_tracks, BattleInterrupt[] i_interrupts) {
		enemy_stats = i_enemy_stats;
        ally_stats = i_ally_stats;
		music_tracks = i_music_tracks;
		interrupts = i_interrupts;
	}
}