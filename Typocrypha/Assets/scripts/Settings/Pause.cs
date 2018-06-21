using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
	public static Pause main = null; // Global static reference
    bool gamePause;
    Color dimCol;
    string pauseKey = "escape";
    GameObject hideChild;
    bool battlePause = false; //saves battlemanager pause state
    bool blockTextboxInput = false;
	public bool block_pause = false;

	public MusicSlider slider_music;
	public SfxSlider slider_sfx;
	public ScrollSlider slider_scroll;

	int pos_menu_h = 0;
	int pos_menu_v = 0;
	int pos_music;
	int pos_sfx;
	int pos_scroll;
	int pos_resolution;

    void Start () {
        gamePause = false;
		hideChild = transform.GetChild(0).gameObject;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(pauseKey) && (block_pause == false))
        {
			gamePause = !gamePause;

            if (gamePause)
            {
                Time.timeScale = 0;
                battlePause = BattleManagerS.main.pause;
                BattleManagerS.main.setPause(true);
                dimCol = BattleEffects.main.dimmer.color;
                BattleEffects.main.setDim(true);
                blockTextboxInput = DialogueManager.main.block_input;
                DialogueManager.main.block_input = true;
				DialogueManager.main.input_field.DeactivateInputField ();
                AudioPlayer.main.pauseSFX();
				hideChild.SetActive (true);
				//Cursor.visible = true;
				//Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                BattleManagerS.main.setPause(battlePause);
                BattleEffects.main.dimmer.color = dimCol;
                DialogueManager.main.block_input = blockTextboxInput;
                AudioPlayer.main.unpauseSFX();
				hideChild.SetActive (false);
				//Cursor.visible = false;
				//Cursor.lockState = CursorLockMode.Locked;
            }
        }

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			switch(pos_menu_v){
			case 0:
				slider_music.slider.value -= 0.1f;
				slider_music.slider.value = Mathf.Max (slider_music.slider.value, 0.0f);
				break;
			case 1:
				slider_sfx.slider.value -= 0.1f;
				slider_sfx.slider.value = Mathf.Max (slider_sfx.slider.value, 0.0f);
				break;
			case 2:
				slider_scroll.slider.value -= 0.1f;
				slider_scroll.slider.value = Mathf.Max (slider_scroll.slider.value, 0.0f);
				break;
			}

		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			switch (pos_menu_v) {
			case 0:
				slider_music.slider.value += 0.1f;
				slider_music.slider.value = Mathf.Min (slider_music.slider.value, 1.0f);
				break;
			case 1:
				slider_sfx.slider.value += 0.1f;
				slider_sfx.slider.value = Mathf.Min (slider_sfx.slider.value, 1.0f);
				break;
			case 2:
				slider_scroll.slider.value += 0.1f;
				slider_scroll.slider.value = Mathf.Min (slider_scroll.slider.value, 1.0f);
				break;
			}
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			--pos_menu_v;
			pos_menu_v = Mathf.Max (pos_menu_v, 0);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			++pos_menu_v;
			pos_menu_v = Mathf.Min (pos_menu_v, 2);
		}
	}
}
