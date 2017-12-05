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
	public int curr_line; // index of current dialogue line

	// constructs a new cutscene
	public CutScene(string[] i_npcs, string[] i_whos_talking, string[] i_dialogue) {
		npcs = i_npcs;
		whos_talking = i_whos_talking;
		dialogue = i_dialogue;
		curr_line = 0;
	}

	// returns next line of dialogue (null if at end)
	public string nextDialogueLine() {
		if (curr_line >= dialogue.Length) {
			return null;
		} else {
			return dialogue [curr_line++];
		}
	}

	// returns who is saying the current line
	public string currSpeaker() {
		return whos_talking [curr_line];
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