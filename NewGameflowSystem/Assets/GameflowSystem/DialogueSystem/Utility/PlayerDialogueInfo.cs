using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

// container class for player info needed for cutscenes
public class PlayerDialogueInfo : MonoBehaviour {
	public static PlayerDialogueInfo main = null; // global static ref
	public Sprite[] player_sprites; // all options for player sprites
	public SpriteRenderer player_sprite_r; // where player sprite is rendered

	[HideInInspector] public string player_name { get; set; }
	[HideInInspector] public Pronoun player_pronoun { get; set; }
	[HideInInspector] public Sprite player_sprite { get; set; }

	void Start() {
		if (main == null) main = this;
		player_name = "???";
	}

	// set player's sprite and update image in dialogue box
	public void setSprite(int ind) {
		player_sprite = player_sprites [ind];
		player_sprite_r.sprite = player_sprite;
	}

	// set player's pronoun
	public void setPronoun(int ind) {
		player_pronoun = (Pronoun)ind;
	}
}
