using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// manages title screen input and animations
public class TitleScreen : MonoBehaviour {
    public GameObject title_menu; // contains menu sprites
    public SpriteRenderer highlight_sprite;
    public int num_elements;
	public TextScroll text_scroll; // scrolls text
	public SpriteRenderer title_sprite; // title screen sprite

    private bool isActive = false;
    private int index = 0;
    private const float y_offset = 0.62f;

	void Awake () {
	}

    private void Update()
    {
        if (!isActive)
            return;
        // go to next item if down is pressed
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(index + 1 < num_elements)
            {
                ++index;
                Vector3 move = new Vector3(0, -y_offset, 0);
                highlight_sprite.transform.position += move;
                AudioPlayer.main.playSFX("sfx_enemy_select");
            }
        }
        // go to last item if up is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index - 1 >= 0)
            {
                --index;
                Vector3 move = new Vector3(0, y_offset, 0);
                highlight_sprite.transform.position += move;
                AudioPlayer.main.playSFX("sfx_enemy_select");
            }
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            switch (index)
            {
                case 0: //continue button
                    throw new System.NotImplementedException("continue functionality is not implemented (see TitleScreen.cs)");
                case 1: //load game button
                    throw new System.NotImplementedException("load game functionality is not implemented (see TitleScreen.cs)");
                case 2: //new game button
                    Debug.Log("starting new game");
                    transitionToStart();
                    break;
                case 3: //settings button
                    throw new System.NotImplementedException("settings functionality is not implemented (see TitleScreen.cs)");
                case 4:
                    Application.Quit();
                    break;
                default:
                    throw new System.NotImplementedException("no such menu index: " + index);

            }
            AudioPlayer.main.playSFX("sfx_enter");
            isActive = false;
        }
    }

    // starts title screen music/ui/animations/etc
    public void startTitle() {
		title_menu.SetActive (true);
        isActive = true;
	}

	// called to transition to MainScene (new game) when start is pressed
	void transitionToStart() {
		AsyncOperation load_op = SceneManager.LoadSceneAsync ("MainScene", LoadSceneMode.Additive);
		StartCoroutine (loadMainScene (load_op));
	}

	// spawns loading screen, waits for main scene to load, and then transitions
	IEnumerator loadMainScene(AsyncOperation load_op) {
		Debug.Log ("loading main scene...");
		//new_file_button.interactable = false;
		//new_file_button.gameObject.GetComponentInChildren<Text>().text = "Loading...";
		yield return new WaitUntil (() => load_op.isDone);
		GameObject.Find ("Main Camera").GetComponent<AudioListener>().enabled = false; // to avoid multiple listeners
		StartCoroutine (fadeAndStart ());
	}

	// fades title screen to transparent and start main scene
	IEnumerator fadeAndStart() {
		float alpha = 1;
		while (alpha > 0) {
			title_sprite.color = new Color (1, 1, 1, alpha);
			alpha -= 0.01f;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitUntil (() => StateManager.main.ready);
		StateManager.main.startFirstScene ();
		SceneManager.UnloadSceneAsync ("TitleScene");
	}
}
