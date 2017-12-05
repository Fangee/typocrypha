using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages battle sequences
public class BattleManager : MonoBehaviour {
	public StateManager state_manager; // manages global state/scenes

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle! (wait for transition)");
		Debug.Log ("Enemies in battle:");
		foreach (string enemy in scene.enemies)
			Debug.Log ("  " + enemy);
	}

	void Update () {
		
	}
}
