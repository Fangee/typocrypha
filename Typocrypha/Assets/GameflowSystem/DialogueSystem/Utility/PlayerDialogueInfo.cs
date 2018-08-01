using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

// container class for player info needed for cutscenes
public class PlayerDialogueInfo : MonoBehaviour {
	public static PlayerDialogueInfo main = null; // global static ref
    public const string defaultPromptKey = "prompt";
	public Sprite[] player_sprites; // all options for player sprites
	public SpriteRenderer player_sprite_r; // where player sprite is rendered
    public Dictionary<string, string> player_info_map;

	[HideInInspector] public string player_name { get; set; }
	[HideInInspector] public Pronoun player_pronoun { get; set; }
	[HideInInspector] public Sprite player_sprite { get; set; }

    private void Awake() {
        player_info_map = new Dictionary<string, string> { };
    }

    void Start() {
		if (main == null) main = this;
		player_name = "???";

    }

    // return an info string from the info map, or throw an exception if one does not exist
    public string getInfo(string key) {
        if (player_info_map.ContainsKey(key))
            return player_info_map[key];
        Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning undefined");
        return "undefined";
    }

    // set an info string in the info map
    public void setInfo(string key, string data)
    {
        if (player_info_map.ContainsKey(key))
            player_info_map[key] = data;
        else
            player_info_map.Add(key, data);
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
