using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages game flow (from dialogue to battle to etc...)
public class GameflowManager : MonoBehaviour {
	public static GameflowManager main = null; // Global static ref
	public Gameflow gameflow; // Current list of events to play
	int curr_item; // Current event number

	void Awake() {
		if (main == null) main = this;
		else Destroy (this);
	}

	void Start() {
		curr_item = -1;
		if (gameflow != null) next (); // Start immediately
	}

	// Go to next item
	public void next() {
		GameflowItem item = gameflow.items [++curr_item];
		if (item.GetType() == typeof(Dialogue)) {
			Debug.Log ("starting dialogue");
			DialogueManager.main.startDialogue ((Dialogue)gameflow.items [curr_item]);
		}
	}
}

