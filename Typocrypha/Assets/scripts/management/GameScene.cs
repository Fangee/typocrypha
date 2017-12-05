using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// represents a scene in the game: either a cutscene or a battle scene
public class GameScene {}

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

	// constructs a new cutscene
	public CutScene(string[] i_npcs, string[] i_whos_talking, 
		            string[] i_dialogue, string[] i_npc_sprites) {
		npcs = i_npcs;
		whos_talking = i_whos_talking;
		dialogue = i_dialogue;
		npc_sprites = i_npc_sprites;
	}

}

// represents a battle scene
public class BattleScene : GameScene {
	public string[] enemies; // array of enemy names (TEMP: should be enemy objects)

	public BattleScene(string[] i_enemies) {
		enemies = i_enemies;
	}

	// BATTLE STUFF
}