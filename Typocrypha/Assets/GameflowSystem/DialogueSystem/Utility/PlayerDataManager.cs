using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

// container class for player info needed for cutscenes
public class PlayerDataManager : MonoBehaviour {
	public static PlayerDataManager main = null; // global static ref

	public Sprite[] player_sprites; // all options for player sprites
	public SpriteRenderer player_sprite_r; // where player sprite is rendered
    public Dictionary<string, string> player_data_map;

    public string PlayerName { get { return getData("player-name"); } set {setData("player-name", value);} }
    private const string lastInputKey = "prompt";
    public string LastPlayerInput { get { return getData(lastInputKey); } set { setData(lastInputKey, value); } }
	[HideInInspector] public Pronoun player_pronoun { get; set; }

    private void Awake() {
        player_data_map = new Dictionary<string, string> { };
        player_data_map.Add("player-name", "???");
    }

    void Start() {
		if (main == null) main = this;
    }

    // return an info string from the info map, or throw an exception if one does not exist
    public string getData(string key) {
        if (player_data_map.ContainsKey(key))
            return player_data_map[key];
        Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning undefined");
        return "undefined";
    }

    // set an info string in the info map
    public void setData(string key, string data)
    {
        if (player_data_map.ContainsKey(key))
            player_data_map[key] = data;
        else
            player_data_map.Add(key, data);
    }

	// set player's pronoun
	public void setPronoun(int ind) {
		player_pronoun = (Pronoun)ind;
	}
}
