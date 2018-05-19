using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages game flow (from dialogue to battle to etc...)
// Different scenes are store as child objects to 'GameflowManager' object
public class GameflowManager : MonoBehaviour {
	public static GameflowManager main = null; // Global static ref
	public GameObject player_ui; // the Typocrypha UI 
	public int curr_item; // Current event number

	void Awake() {
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
		GameflowItem item = transform.GetChild(++curr_item).gameObject.GetComponent<GameflowItem>();
		if (item.gameObject.activeInHierarchy == false) { // Don't read disabled items
			next ();
			return;
		}
		if (item.GetType() == typeof(Dialogue)) {
			Debug.Log ("starting dialogue: " + item.name);
			player_ui.SetActive (false);
            BattleManagerS.main.setEnabled(false);
            DialogueManager.main.setEnabled(true);
			DialogueManager.main.startDialogue (transform.GetChild(curr_item).gameObject);
		} else if(item.GetType() == typeof(Battle)) {
            Debug.Log("starting battle: " + item.name);
			player_ui.SetActive (true);
            DialogueManager.main.setEnabled(false);
            BattleManagerS.main.setEnabled(true);
            BattleManagerS.main.startBattle(transform.GetChild(curr_item).gameObject);
        }
	}
}

