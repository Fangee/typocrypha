using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// plays out the intro sequence
public class IntroSequence : MonoBehaviour {
	public IntroScene intro_scene; // intro scene object
	public GameObject player_ui; // the player's ui
	public Vector2 player_ui_pos; // where ui should be for main game
	public GameObject intro_text; // general use text display
	public TextScroll text_scroll; // scrolls text character by character

	// starts intro sequence
	public void startIntro(IntroScene scene) {
        player_ui.transform.position = new Vector3(0, -1000, 0);
		Debug.Log ("intro start");
		intro_scene = scene;
		StartCoroutine (introSequence ());
	}

	// intro sequence
	IEnumerator introSequence() {
		// show and delete starting text
		intro_text.SetActive(true);
		text_scroll.delay = (0.1f);
		text_scroll.startPrint ("[set-talk-sfx=]PRESENT DAY         \nANOTHER TIME         \nOUR WORLD",
			intro_text.GetComponent<Text>());
		yield return new WaitWhile (() => text_scroll.is_print);
		yield return new WaitForSeconds (2.0f);
		text_scroll.delay = (0.05f);
		text_scroll.startDelete ("PRESENT DAY\nANOTHER TIME\nOUR WORLD", intro_text.GetComponent<Text>(), "null");
		yield return new WaitWhile (() => text_scroll.is_print);
		yield return new WaitForSeconds (2.0f);
		// move Typocrypha (UI) into view
		yield return StartCoroutine(slideUIBack ());
		// wait and go to next scene
		yield return new WaitForSeconds (2.0f);
		StateManager.main.nextScene ();
	}

	// slide player's ui into place
	IEnumerator slideUIBack() {
		float dist = Vector2.Distance (player_ui.transform.position, player_ui_pos);
		while (dist > 0.01) {
			player_ui.transform.position = 
				Vector2.MoveTowards (player_ui.transform.position, player_ui_pos, dist/10);
			dist = Vector2.Distance (player_ui.transform.position, player_ui_pos);
			yield return new WaitForEndOfFrame ();
		}
		player_ui.transform.position = player_ui_pos;
	}
}
