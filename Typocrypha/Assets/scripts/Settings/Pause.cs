using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Pause : MonoBehaviour {
	public static Pause main = null; // Global static reference
	bool gamePause;
    Color dimCol;
    string pauseKey = "escape";
    GameObject hideChild;
    bool battlePause = false; //saves battlemanager pause state
    bool blockTextboxInput = false;
	public bool block_pause = false;

	public SetResolution resolution_script;

	public MusicSlider slider_music;
	public SfxSlider slider_sfx;
	public ScrollSlider slider_scroll;

	public Sprite[] images_toggles;
	public Sprite[] images_buttons;
	public Image[] settings_toggles;
	public Image[] settings_buttons;
	public Text[] texts_toggles;
	public Image[] toggle_arrows;
	public GameObject select_instructions;

	int pos_menu_h = 0;
	int pos_menu_v = 0;
	int pos_music;
	int pos_sfx;
	int pos_scroll;
	int pos_resolution;
	int pos_screenmode;
	int[] selected_res;
	Dictionary<int, int[]> resolution_map;

    void Start () {
		if (main == null) main = this;
        gamePause = false;
        Time.timeScale = 1;
        hideChild = transform.GetChild(0).gameObject;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		resolution_map = new Dictionary<int, int[]> {
			{9, new int[]{1920, 1080}},
			{8, new int[]{1280, 720}},
			{7, new int[]{1152, 648}},
			{6, new int[]{1024, 576}},
			{5, new int[]{960, 540}},
			{4, new int[]{896, 504}},
			{3, new int[]{800, 450}},
			{2, new int[]{768, 432}},
			{1, new int[]{640, 360}},
			{0, new int[]{256, 144}}
		};
		pos_resolution = 9;
		if (Screen.fullScreen)
			pos_screenmode = 1;
		else
			pos_screenmode = 0;
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
                AudioPlayer.main.pauseSFX();
				hideChild.SetActive (true);
				//Cursor.visible = true;
				//Cursor.lockState = CursorLockMode.None;
				AudioPlayer.main.playSFX("sfx_scanner_open");
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
				AudioPlayer.main.playSFX("sfx_scanner_close");
            }
        }

		if (Input.GetKeyDown(KeyCode.LeftArrow) && gamePause) {
			switch(pos_menu_v){
			case 0:
				if(slider_music.slider.value != 0) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_music.slider.value -= 0.1f;
				slider_music.slider.value = Mathf.Max (slider_music.slider.value, 0.0f);
				break;
			case 1:
				if(slider_sfx.slider.value != 0) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_sfx.slider.value -= 0.1f;
				slider_sfx.slider.value = Mathf.Max (slider_sfx.slider.value, 0.0f);
				break;
			case 2:
				if(slider_scroll.slider.value != 0) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_scroll.slider.value -= 0.1f;
				slider_scroll.slider.value = Mathf.Max (slider_scroll.slider.value, 0.0f);
				break;
			case 3:
				if (pos_resolution != 0)
					AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				--pos_resolution;
				pos_resolution = Mathf.Max (pos_resolution, 0);
				break;
			case 4:
				if (pos_screenmode != 0)
					AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				pos_screenmode = 0;
				break;
			case 5:
				if(pos_menu_h != 0) AudioPlayer.main.playSFX ("sfx_enemy_select");
				pos_menu_h = 0;
				break;
			}

		}
		if (Input.GetKeyDown (KeyCode.RightArrow) && gamePause) {
			switch (pos_menu_v) {
			case 0:
				if(slider_music.slider.value != 1) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_music.slider.value += 0.1f;
				slider_music.slider.value = Mathf.Min (slider_music.slider.value, 1.0f);
				break;
			case 1:
				if(slider_sfx.slider.value != 1) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_sfx.slider.value += 0.1f;
				slider_sfx.slider.value = Mathf.Min (slider_sfx.slider.value, 1.0f);
				break;
			case 2:
				if(slider_scroll.slider.value != 1) AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				slider_scroll.slider.value += 0.1f;
				slider_scroll.slider.value = Mathf.Min (slider_scroll.slider.value, 1.0f);
				break;
			case 3:
				if (pos_resolution != 9)
					AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				++pos_resolution;
				pos_resolution = Mathf.Min (pos_resolution, 9);
				break;
			case 4:
				if (pos_screenmode != 1)
					AudioPlayer.main.playSFX ("sfx_type_key");
				else AudioPlayer.main.playSFX ("sfx_backspace");
				pos_screenmode = 1;
				break;
			case 5:
				if(pos_menu_h != 1) AudioPlayer.main.playSFX ("sfx_enemy_select");
				pos_menu_h = 1;
				break;
			}
		}
			
		if (Input.GetKeyDown (KeyCode.UpArrow) && gamePause) {
			if(pos_menu_v != 0) AudioPlayer.main.playSFX ("sfx_enemy_select");
			--pos_menu_v;
			pos_menu_v = Mathf.Max (pos_menu_v, 0);
			pos_menu_h = 0;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) && gamePause) {
			if(pos_menu_v != 5) AudioPlayer.main.playSFX ("sfx_enemy_select");
			++pos_menu_v;
			pos_menu_v = Mathf.Min (pos_menu_v, 5);
		}

		if ((Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.Return)) && gamePause) {
			if (pos_menu_v == 5) {
				if (pos_menu_h == 0) {
					AudioPlayer.main.playSFX ("sfx_enter");
					resolution_script.SetFull (Convert.ToBoolean(pos_screenmode));
					selected_res = resolution_map [pos_resolution];
					resolution_script.SetRes (selected_res [0], selected_res [1]);
					resolution_script.ApplySettings ();
				} else {
					//Application.Quit;
					AudioPlayer.main.playSFX ("sfx_enter");
					ExitGame();
				}
			}
		}

		// UPDATE GRAPHICS BASED ON NEW VALUES

		if (pos_menu_v == 5) {
			select_instructions.SetActive (true);
		} else {
			select_instructions.SetActive (false);
		}

		string newvalue;
		newvalue = Mathf.RoundToInt(slider_music.slider.value * 100) + "%";
		texts_toggles [0].text = newvalue;
		newvalue = Mathf.RoundToInt(slider_sfx.slider.value * 100) + "%";
		texts_toggles [1].text = newvalue;
		newvalue = Mathf.RoundToInt(slider_scroll.slider.value * 100) + "%";
		texts_toggles [2].text = newvalue;
		selected_res = resolution_map[pos_resolution];
		texts_toggles [3].text = selected_res [0] + " x " + selected_res [1];
		if (pos_screenmode == 1)
			texts_toggles [4].text = "FULLSCREEN";
		else
			texts_toggles [4].text = "WINDOWED";

		int y = 0;
		foreach (Image toggleimg in settings_toggles){
			if (y == pos_menu_v) {
				toggleimg.sprite = images_toggles [1];
			} else {
				toggleimg.sprite = images_toggles [0];
			}
			++y;
		}
		int x = 0;
		foreach (Image buttonimg in settings_buttons){
			if (y == pos_menu_v) {
				if(x == pos_menu_h) buttonimg.sprite = images_buttons [1];
				else buttonimg.sprite = images_buttons [0];
			} else {
				buttonimg.sprite = images_buttons [0];
			}
			++x;
		}

		int isRightArrow = 0;
		foreach (Image arrowimg in toggle_arrows) {
			Vector3 new_arrow_pos;
			Vector3 go_away = new Vector3 (arrowimg.rectTransform.localPosition.x, 42069, arrowimg.rectTransform.localPosition.z);
			switch (pos_menu_v) {
			case 0:
				new_arrow_pos = new Vector3 (arrowimg.rectTransform.localPosition.x, 268, arrowimg.rectTransform.localPosition.z);
				arrowimg.rectTransform.localPosition = new_arrow_pos;
				if (isRightArrow == 1) {
					if (slider_music.slider.value == 1)
						arrowimg.rectTransform.localPosition = go_away;
				} else {
					if (slider_music.slider.value == 0)
						arrowimg.rectTransform.localPosition = go_away;
				}
				break;
			case 1:
				new_arrow_pos = new Vector3 (arrowimg.rectTransform.localPosition.x, 188, arrowimg.rectTransform.localPosition.z);
				arrowimg.rectTransform.localPosition = new_arrow_pos;
				if (isRightArrow == 1) {
					if (slider_sfx.slider.value == 1)
						arrowimg.rectTransform.localPosition = go_away;
				} else {
					if (slider_sfx.slider.value == 0)
						arrowimg.rectTransform.localPosition = go_away;
				}
				break;
			case 2:
				new_arrow_pos = new Vector3 (arrowimg.rectTransform.localPosition.x, 108, arrowimg.rectTransform.localPosition.z);
				arrowimg.rectTransform.localPosition = new_arrow_pos;
				if (isRightArrow == 1) {
					if (slider_scroll.slider.value == 1)
						arrowimg.rectTransform.localPosition = go_away;
				} else {
					if (slider_scroll.slider.value == 0)
						arrowimg.rectTransform.localPosition = go_away;
				}
				break;
			case 3:
				new_arrow_pos = new Vector3 (arrowimg.rectTransform.localPosition.x, 28, arrowimg.rectTransform.localPosition.z);
				arrowimg.rectTransform.localPosition = new_arrow_pos;
				if (isRightArrow == 1) {
					if (pos_resolution == 9)
						arrowimg.rectTransform.localPosition = go_away;
				} else {
					if (pos_resolution == 0)
						arrowimg.rectTransform.localPosition = go_away;
				}
				break;
			case 4:
				new_arrow_pos = new Vector3 (arrowimg.rectTransform.localPosition.x, -52, arrowimg.rectTransform.localPosition.z);
				arrowimg.rectTransform.localPosition = new_arrow_pos;
				if (isRightArrow == 1) {
					if (pos_screenmode == 1)
						arrowimg.rectTransform.localPosition = go_away;
				} else {
					if (pos_screenmode == 0)
						arrowimg.rectTransform.localPosition = go_away;
				}
				break;
			case 5:
				arrowimg.rectTransform.localPosition = go_away;
				break;
			}
			++isRightArrow;
		}
	}

	void ExitGame(){
		Application.Quit ();
	}

	public bool isPaused(){
		return gamePause;
	}
}
