using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages game flow (from dialogue to battle to etc...)
// Different scenes are store as child objects to 'GameflowManager' object
public class GameflowManager : Gameflow {
	public static GameflowManager main = null; // Global static ref
	public GameObject player_ui; // the Typocrypha UI 
	//public GameObject screenframe_vn;
	//public GameObject screenframe_battle;
    private Gameflow curr_gameflow;

	void Awake() {
        curr_gameflow = this;
		if (main == null) main = this;
		else Destroy (this);
	}

    private void Start() {
        StartCoroutine(waitForLoad());
    }

    // waits for files to load
    IEnumerator waitForLoad() {
        yield return new WaitUntil(() => AudioPlayer.main.ready);
        AudioPlayer.main.stopAll(); 
        yield return new WaitUntil(() => EnemyDatabase.main.is_loaded);
        yield return new WaitUntil(() => AllyDatabase.main.is_loaded);
        yield return new WaitUntil(() => AnimationPlayer.main.ready);
        Debug.Log("done loading assets");
        gameflowStart();
    }

    public void gameflowStart() {
		Debug.Log ("gameflowStart");
		curr_item = -1;
		if (transform.childCount != 0) next (); // Start immediately
	}

	// Go to next item
	public void next() {
        if(++(curr_gameflow.curr_item) >= curr_gameflow.transform.childCount) {//Go back to last gameflow object if at the end of nested gameflow
            Debug.Log("returning to gameflow: " + curr_gameflow.transform.parent.name);
            curr_gameflow = curr_gameflow.transform.parent.GetComponent<Gameflow>();
            if (curr_gameflow == null) throw new System.NotImplementedException("Reached end of main gameflow: currently unhadled");
            next();
            return;
        }
        GameflowItem item = curr_gameflow.transform.GetChild(curr_gameflow.curr_item).gameObject.GetComponent<GameflowItem>();
		if (item.gameObject.activeInHierarchy == false) { // Don't read disabled items
			next ();
			return;
		}
        if (item.GetType() == typeof(Dialogue)) {
			Debug.Log ("starting dialogue: " + item.name);
			player_ui.SetActive (false);
			//screenframe_vn.SetActive (true);
			//screenframe_battle.SetActive (false);
            BattleManagerS.main.setEnabled(false);
            DialogueManager.main.setEnabled(true);
			DialogueManager.main.startDialogue (curr_gameflow.transform.GetChild(curr_gameflow.curr_item).gameObject);
		} else if(item.GetType() == typeof(Battle)) {
            Debug.Log("starting battle: " + item.name);
			player_ui.SetActive (true);
			//screenframe_vn.SetActive (false);
			//screenframe_battle.SetActive (true);
            DialogueManager.main.setEnabled(false);
            BattleManagerS.main.setEnabled(true);
            BattleManagerS.main.startBattle(curr_gameflow.transform.GetChild(curr_gameflow.curr_item).gameObject);
        } else if (item.GetType() == typeof(Gameflow)) {
            Debug.Log("starting gameflow: " + item.name);
            curr_gameflow = (Gameflow)item;
            next();
        }
	}
    // Jump to item
    public void jump(GameObject targetGameFlowItem, bool goToNext = true) {
        Debug.Log("Gameflow: Jumping to " + targetGameFlowItem.name);
        curr_gameflow = targetGameFlowItem.transform.parent.GetComponent<Gameflow>();
        curr_gameflow.curr_item = targetGameFlowItem.transform.GetSiblingIndex() - 1;
        if (goToNext) next();
    }
}

