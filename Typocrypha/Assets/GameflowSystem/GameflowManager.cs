using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages game flow (from dialogue to battle to etc...)
// Different scenes are store as child objects to 'GameflowManager' object
public class GameflowManager : MonoBehaviour {
	public static GameflowManager main = null; // Global static ref
	int curr_item; // Current event number

	void Awake() {
		if (main == null) main = this;
		else Destroy (this);
	}

	public void gameflowStart() {
		curr_item = -1;
		if (transform.childCount != 0) next (); // Start immediately
	}

	// Go to next item
	public void next() {
		Debug.Log ("gameflow next");
		GameflowItem item = transform.GetChild(++curr_item).gameObject.GetComponent<GameflowItem>();
		if (item.gameObject.activeInHierarchy == false) { // Don't read disabled items
			next ();
			return;
		}
		if (item.GetType() == typeof(Dialogue)) {
			Debug.Log ("starting dialogue");
            BattleManagerS.main.setEnabled(false);
            DialogueManager.main.setEnabled(true);
			DialogueManager.main.startDialogue (transform.GetChild(curr_item).gameObject);
		} else if(item.GetType() == typeof(Battle)) {
            Debug.Log("starting battle");
            DialogueManager.main.setEnabled(false);
            BattleManagerS.main.setEnabled(true);
            BattleManagerS.main.startBattle(transform.GetChild(curr_item).gameObject);
        }
	}
}

