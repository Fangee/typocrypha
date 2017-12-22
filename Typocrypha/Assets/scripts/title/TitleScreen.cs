using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// manages title screen input and animations
public class TitleScreen : MonoBehaviour {
	public GameObject title_menu; // holds all of the menu buttons
	public Button new_file_button; // starts a new game
	public Button load_file_button; // loads a saved game
	public Button option_button; // accesses otions
	public Button quit_button; // exits game
	public TextScroll text_scroll; // scrolls text
	public SpriteRenderer title_sprite; // title screen sprite

	void Awake () {
		new_file_button.onClick.AddListener (transitionToStart);
	}

	// starts title screen music/ui/animations/etc
	public void startTitle() {
		title_menu.SetActive (true);
	}

	// called to transition to MainScene (new game) when start is pressed
	void transitionToStart() {
		AsyncOperation load_op = SceneManager.LoadSceneAsync ("MainScene", LoadSceneMode.Additive);
		StartCoroutine (loadMainScene (load_op));
	}

	// spawns loading screen, waits for main scene to load, and then transitions
	IEnumerator loadMainScene(AsyncOperation load_op) {
		Debug.Log ("loading main scene...");
		new_file_button.interactable = false;
		new_file_button.gameObject.GetComponentInChildren<Text>().text = "Loading...";
		yield return new WaitUntil (() => load_op.isDone);
		GameObject.Find ("Main Camera").GetComponent<AudioListener>().enabled = false; // to avoid multiple listeners
		StartCoroutine (fadeAndStart ());
	}

	// fades title screen to transparent and start main scene
	IEnumerator fadeAndStart() {
		float alpha = 1;
		while (alpha > 0) {
			title_sprite.color = new Color (1, 1, 1, alpha);
			alpha -= 0.005f;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitUntil (() => StateManager.main.ready);
		StateManager.main.startFirstScene ();
		SceneManager.UnloadSceneAsync ("TitleScene");
	}
}
