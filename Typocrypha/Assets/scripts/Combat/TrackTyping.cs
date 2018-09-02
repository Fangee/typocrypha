﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// tracks player's typing
public class TrackTyping : MonoBehaviour {
	public Text typed_text; // shows typed text
	public Text entry_ok; // displays 'OK' or 'NO' if player can type or not
	public Dictionary<char, Image> key_image_map; // map from characters to key images
    public Dictionary<char, Text> key_text_map; // map from characters to key text
    public BattleKeyboard battleKeyboard;
	public Transform keyboard; // keyboard transform (holds key images)
	public GameObject key_prefab; // prefab for key image object
	public GameObject spacebar_prefab; // prefab for spacebar image object
	public GameObject popper; // popper

    string last_buffer = "";
	string buffer = ""; // contains typed text
	int count; // number of characters typed
	int deleteCounter = 0; // tracks how long the player holds down backspace to clear buffer
	string[] rows = { "qwertyuiop", "asdfghjkl", "zxcvbnm", " " };
	float[] row_offsets = { 0f, 24f, 72f, 0 };

    public void clearBuffer() {
        buffer = "";
        count = 0;
    }
    public void setBuffer(string newBuffer)
    {
        buffer = newBuffer;
        count = buffer.Length;
    }
    public void revertBuffer()
    {
        setBuffer(last_buffer);
    }
	public string getBuffer(){
		return last_buffer;
	}
    public void updateDisplay()
    {
        // update display
        typed_text.text = buffer.Replace(" ", "-").ToUpper();
        for (int i = 26 - typed_text.text.Length; i > 0; --i)
        {
            typed_text.text = typed_text.text + "_";
        }
        typed_text.text = ">" + typed_text.text;
    }

	void Start () {
		typed_text.text = "";
		key_image_map = new Dictionary<char, Image> ();
        key_text_map = new Dictionary<char, Text>();
        clearBuffer();
		createKeyboard ();
		// initialize key colors to gray
		foreach (KeyValuePair<char, Image> pair in key_image_map)
			pair.Value.color = Color.gray;
        battleKeyboard.image_map = key_image_map;
        battleKeyboard.text_map = key_text_map;
        battleKeyboard.initialize();
    }

	void Update () {
		if (((BattleManagerS.main.battlePause && !BattleManagerS.main.frenzyCastActive) || BattleManagerS.main.enabled == false) &&
			(!TextEvents.main.is_prompt)) {
			entry_ok.text = "NO";
			return;
		} else entry_ok.text = "OK";
		// check key presses
		if (Input.GetKeyDown (KeyCode.Return)) {
            if (TextEvents.main.is_prompt) {
                AudioPlayer.main.playSFX("sfx_enter"); // MIGHT WANT TO BE MOVED
                TextEvents.main.prompt_input = buffer;
				TextEvents.main.is_prompt = false;
                clearBuffer();
			} else {
                Debug.Log("Player casts " + buffer.ToUpper().Replace(' ', '-'));
                if(buffer != string.Empty)
                    last_buffer = buffer;
                BattleManagerS.main.handleSpellCast (buffer, this); // attack currently targeted enemy
			}				
		} else if (Input.GetKey (KeyCode.Backspace)) {
			if (Input.GetKeyDown (KeyCode.Backspace)) {
				if (count > 0) {
					AudioPlayer.main.playSFX ("sfx_backspace");
					buffer = buffer.Remove (count - 1, 1);
					count = count - 1;
				}
			} else {
				if (count > 0) {
					if (deleteCounter < 15) {
						++deleteCounter;
					} else {
						AudioPlayer.main.playSFX ("sfx_backspace");
						buffer = "";
						count = 0;
					}
				}
			}
		} else if (Input.GetKey (KeyCode.KeypadMinus) || Input.GetKey (KeyCode.Minus)){
			Popper[] pop = popper.GetComponents<Popper>();
			AudioPlayer.main.playSFX ("sfx_botch");
			pop[0].spawnText ("<color=red>USE THE SPACEBAR INSTEAD OF THE MINUS/DASH</color>", 1.0f, new Vector3(0.0f,-1.0f,0.0f));
		}
		else {
			deleteCounter = 0;
            string in_str = Input.inputString;
            if (TextEvents.main.is_prompt) {
                buffer += in_str;
                count += in_str.Length;
                // highlight pressed keys
                foreach (char c in in_str)
                    StartCoroutine(colorKey(c));
            } else {
                // process status conditions on keys and apply appropriate graphical effects
                foreach (char c in in_str) {
                    char cLower = char.ToLower(c);
                    if (key_image_map.ContainsKey(cLower)) {
                        if (cLower == ' ') {//Dont put space in if cast is empty
                            if (count != 0) {
                                buffer += cLower;
                                count++;
                                AudioPlayer.main.playSFX("sfx_type_key");
                            }
                            StartCoroutine(colorKey(cLower));
                            continue;
                        } //Returns a string so it can return an empty string or multiple letters
                        string add = battleKeyboard.processKey(cLower); 
                        buffer += add;
                        count += add.Length;
                        StartCoroutine(battleKeyboard.keyGraphics(cLower, key_image_map[cLower], key_text_map[cLower]));
                    }
                }
            }

        }
        updateDisplay();
	}

	// create visual keyboard keys
	void createKeyboard() {
		for (int i = 0; i < rows.Length; i++) {
			for (int j = 0; j < rows[i].Length; j++) {
				GameObject new_key;
				if (i == 3) {
					//return;
					new_key = GameObject.Instantiate (spacebar_prefab);
				} else {
					new_key = GameObject.Instantiate (key_prefab);
				}
				new_key.transform.SetParent (keyboard);
				new_key.transform.localScale = new Vector3 (1, 1, 1);
				new_key.transform.localPosition = new Vector3 (row_offsets[i] + j*48, i*-48, 0);
				if (rows [i] [j].ToString () == " ") {
					new_key.GetComponentInChildren<Text> ().text = "";//"SPACE [ - ]";
				} else {
					new_key.GetComponentInChildren<Text> ().text = rows [i] [j].ToString().ToUpper();
				}
                key_text_map.Add(rows[i][j], new_key.GetComponentInChildren<Text>());
                key_image_map.Add (rows [i] [j], new_key.GetComponent<Image> ());
			}
		}
	}

	// light up key for a short period of time
	IEnumerator colorKey(char c) {
		if (key_image_map.ContainsKey (c)) {
			key_image_map [c].color = Color.white;
			yield return new WaitForSeconds (0.1f);
			key_image_map [c].color = Color.gray;
		}
	}
}
