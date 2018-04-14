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

	void Start() {
		curr_item = -1;
		if (transform.childCount != 0) next (); // Start immediately
	}

	// Go to next item
	public void next() {
		GameflowItem item = transform.GetChild(++curr_item).gameObject.GetComponent<GameflowItem>();
		if (item.GetType() == typeof(Dialogue)) {
			Debug.Log ("starting dialogue");
			DialogueManager.main.startDialogue (transform.GetChild(curr_item).gameObject);
		}
	}
}

