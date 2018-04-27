using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// tracks player's typing
public class TrackTyping : MonoBehaviour {
	public Text typed_text; // shows typed text
	public Text entry_ok; // displays 'OK' or 'NO' if player can type or not
	public Dictionary<char, Image> key_map; // map from characters to key images
    public BattleKeyboard battleKeyboard;
	public Transform keyboard; // keyboard transform (holds key images)
	public GameObject key_prefab; // prefab for key image object
	public GameObject spacebar_prefab; // prefab for spacebar image object
	public GameObject popper; // popper

	string buffer; // contains typed text
	int count; // number of characters typed
	string[] rows = { "qwertyuiop", "asdfghjkl", "zxcvbnm", " " };
	float[] row_offsets = { 0f, 24f, 72f, 0 };

    public void clearBuffer() {
        buffer = "";
        count = 0;
    }

	void Start () {
		typed_text.text = "";
		key_map = new Dictionary<char, Image> ();
        clearBuffer();
		createKeyboard ();
		// initialize key colors to gray
		foreach (KeyValuePair<char, Image> pair in key_map)
			pair.Value.color = Color.gray;
        battleKeyboard.image_map = key_map;
	}

	void Update () {
		if ((BattleManagerS.main.pause || BattleManagerS.main.enabled == false) &&
			(!TextEvents.main.is_prompt)) {
			entry_ok.text = "NO";
			return;
		} else entry_ok.text = "OK";
		// check key presses
		if (Input.GetKeyDown (KeyCode.Return)) {
            AudioPlayer.main.playSFX("sfx_enter"); // MIGHT WANT TO BE MOVED
            if (TextEvents.main.is_prompt) {
				TextEvents.main.prompt_input = buffer;
				TextEvents.main.is_prompt = false;
                clearBuffer();
			} else {
                Debug.Log("Player casts " + buffer.ToUpper().Replace(' ', '-'));
                BattleManagerS.main.handleSpellCast (buffer, this); // attack currently targeted enemy
			}				
		} else if (Input.GetKey (KeyCode.Backspace)) {
			if (Input.GetKeyDown (KeyCode.Backspace)) {
				if (count > 0) {
					buffer = buffer.Remove (count - 1, 1);
					count = count - 1;
				}
			}
		} else if (Input.GetKey (KeyCode.KeypadMinus) || Input.GetKey (KeyCode.Minus)){
			Popper[] pop = popper.GetComponents<Popper>();
			AudioPlayer.main.playSFX ("sfx_botch");
			pop[0].spawnText ("USE THE SPACEBAR INSTEAD OF THE MINUS/DASH", 1.0f, new Vector3(0.0f,-1.0f,0.0f));
		}
		else {
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
                    if (key_map.ContainsKey(c)) {
                        if (c == ' ') {//Dont put space in if cast is empty
                            if (count != 0) {
                                buffer += c;
                                count++;
                                AudioPlayer.main.playSFX("sfx_type_key");
                            }
                            StartCoroutine(colorKey(c));
                            continue;
                        } //Returns a string so it can return an empty string or multiple letters
                        string add = battleKeyboard.processKey(c); 
                        buffer += add;
                        count += add.Length;
                        StartCoroutine(battleKeyboard.keyGraphics(c, key_map[c]));
                    }
                }
            }

        }
		// update display
		typed_text.text = buffer.Replace(" ", "-").ToUpper();
		for (int i = 26 - typed_text.text.Length; i > 0; --i) {
			typed_text.text = typed_text.text + "_";
		}
		typed_text.text = ">" + typed_text.text;
	}

	// create visual keyboard keys
	void createKeyboard() {
		for (int i = 0; i < rows.Length; i++) {
			for (int j = 0; j < rows[i].Length; j++) {
				GameObject new_key;
				if (i == 3) {
					new_key = GameObject.Instantiate (spacebar_prefab);
				} else {
					new_key = GameObject.Instantiate (key_prefab);
				}
				new_key.transform.SetParent (keyboard);
				new_key.transform.localScale = new Vector3 (1, 1, 1);
				new_key.transform.localPosition = new Vector3 (row_offsets[i] + j*48, i*-48, 0);
				if (rows [i] [j].ToString () == " ") {
					new_key.GetComponentInChildren<Text> ().text = "SPACE [ - ]";
				} else {
					new_key.GetComponentInChildren<Text> ().text = rows [i] [j].ToString().ToUpper();
				}
				key_map.Add (rows [i] [j], new_key.GetComponent<Image> ());
			}
		}
	}

	// light up key for a short period of time
	IEnumerator colorKey(char c) {
		if (key_map.ContainsKey (c)) {
			key_map [c].color = Color.white;
			yield return new WaitForSeconds (0.1f);
			key_map [c].color = Color.gray;
		}
	}
}
