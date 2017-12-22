using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// manages title screen input and animations
public class TitleScreen : MonoBehaviour {
	public Button start_button; // start button (starting a new game)
	public SpriteRenderer title_screen; // title screen sprite

	// Use this for initialization
	void Start () {
		start_button.onClick.AddListener (transitionStart);
	}
	
	// called to transition to MainScene (new game) when start is pressed
	void transitionStart() {
		AsyncOperation load_op = SceneManager.LoadSceneAsync ("MainScene", LoadSceneMode.Additive);
		StartCoroutine (loadMainScene (load_op));
	}

	// spawns loading screen, waits for main scene to load, and then transitions
	IEnumerator loadMainScene(AsyncOperation load_op) {
		Debug.Log ("loading main scene...");
		start_button.interactable = false;
		start_button.gameObject.GetComponentInChildren<Text>().text = "Loading...";
		yield return new WaitUntil (() => load_op.isDone);
		GameObject.Find ("Main Camera").GetComponent<AudioListener>().enabled = false; // to avoid multiple listeners
		StartCoroutine (fadeAndStart ());
	}

	// fades title screen to transparent and start main scene
	IEnumerator fadeAndStart() {
		float alpha = 1;
		while (alpha > 0) {
			title_screen.color = new Color (1, 1, 1, alpha);
			alpha -= 0.005f;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitUntil (() => StateManager.main.ready);
		StateManager.main.startFirstScene ();
		SceneManager.UnloadSceneAsync ("TitleScene");
	}
}
