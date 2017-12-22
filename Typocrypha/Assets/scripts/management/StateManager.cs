using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages the game state (cutscene/battle/etc)
public class StateManager : MonoBehaviour {
	public static StateManager main = null; // static global reference
	public LoadGameFlow load_game_flow; // loads the gameflow file
	public SpellDictionary spell_dictionary; // manages spells
	public TrackTyping track_typing; // tracks player's typing
	public GameObject dialogue_box; // text box for dialogue
	public bool loaded; // are all the assets (databases, AssetBundles, etc) loaded?
	public bool ready; // is main scene ready (all components are ready)?
	public bool start_immediate; // should main scene start when loaded? (useful for skipping title for debug)

	GameScene[] scene_arr; // array of gamescenes (loaded by load_game_flow)
	int curr_scene; // current index of game scene

	void Awake() {
		if (main == null) main = this;
		loaded = false;
		ready = false;
	}

	void Start () {
		StartCoroutine (waitForLoad ());
	}

	// waits for files to load
	IEnumerator waitForLoad() {
		yield return new WaitUntil (() => spell_dictionary.is_loaded);
		yield return new WaitUntil (() => EnemyDatabase.main.is_loaded);
		yield return new WaitUntil (() => load_game_flow.is_loaded);
		scene_arr = load_game_flow.scene_arr;
		yield return new WaitUntil (() => AudioPlayer.main.ready);
		yield return new WaitUntil (() => AnimationPlayer.main.ready);
		loaded = true;
		Debug.Log ("done loading assets");
	}

	// update is called after all Start functions have finished
	void Update() {
		if (!ready && loaded) {
			Debug.Log ("statemanager is ready");
			ready = true;
			if (start_immediate) startFirstScene ();
		}
	}

	// starts game from first scene
	public void startFirstScene() {
		curr_scene = -1;
		nextScene ();
	}

	// transition to next scene; returns false if at end
	public bool nextScene() {
		AudioPlayer.main.stopAll (); // stop all playing audio
		if (curr_scene >= scene_arr.Length - 1) {
			track_typing.enabled = false;
			CutsceneManager.main.enabled = false;
			BattleManager.main.enabled = false;
			dialogue_box.SetActive (false);
			Debug.Log ("game over");
			return false;
		}
		GameScene next_scene = scene_arr [++curr_scene];
		string scene_type = next_scene.GetType ().ToString ();
		switch(scene_type) {
		case "IntroScene":
			Debug.Log ("Intro! (please wait to change)");
			track_typing.enabled = true;
			CutsceneManager.main.enabled = false;
			BattleManager.main.enabled = false;
			dialogue_box.SetActive (false);
			StartCoroutine (nextSceneDelayed (3.0f)); // change after waiting for a bit
			break;
		case "CutScene":
			track_typing.enabled = false;
			CutsceneManager.main.enabled = true;
			CutsceneManager.main.battle_interrupt = false;
			BattleManager.main.enabled = false;
			dialogue_box.SetActive (true);
			CutsceneManager.main.startCutscene ((CutScene)next_scene);
			break;
		case "BattleScene":
			track_typing.enabled = true;
			CutsceneManager.main.enabled = false;
			BattleManager.main.enabled = true;
			dialogue_box.SetActive (false);
			BattleManager.main.startBattle ((BattleScene)next_scene);
			break;
		}
		return true;
	}

	// goes back to beginning of current scene (after seconds)
	public void revertScene(float seconds) {
		--curr_scene;
		StartCoroutine (nextSceneDelayed (seconds));
	}

	// go to next scene after a delay
	public IEnumerator nextSceneDelayed(float seconds) {
		yield return new WaitForSeconds (seconds);
		nextScene ();
	}
}
