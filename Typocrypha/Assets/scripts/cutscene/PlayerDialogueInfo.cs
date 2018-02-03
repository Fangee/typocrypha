using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { MASCULINE, FEMININE, INCLUSIVE, NAME };

// container class for player info needed for cutscenes
public class PlayerDialogueInfo : MonoBehaviour {
	public static PlayerDialogueInfo main = null; // global static ref
	public string player_name { get; set; }
	public Pronoun player_pronoun { get; set; }
	public Sprite player_sprite { get; set; }

	void Start() {
		if (main == null) main = this;
	}
}
