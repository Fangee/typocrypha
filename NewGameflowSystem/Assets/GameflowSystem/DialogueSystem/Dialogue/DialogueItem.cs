using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconSide {LEFT, RIGHT, BOTH, NONE}; // Side which icon displays
public enum DialogueMode {VN, CHAT}; // Style of dialogue presentation
public enum DialogueType {NORMAL, INPUT}; // Type of player interaction with dialouge

// Represents a single line of dialogue, and all it's effects/events/etc
public class DialogueItem : MonoBehaviour {
	[HideInInspector] public List<FXTextEffect> fx_text_effects; // List of text effects
	[HideInInspector] public List<TextEvent>[] text_events; // Array of text events at each character index
	public DialogueMode dialogue_mode; // Display style of dialogue
	public DialogueType dialogue_type; // Type of player interaction with dialogue
	public string speaker_name; // Label of who is speaking
	public Sprite left_icon; // Icon for left speaker icon
	public Sprite right_icon; // Icon for right speaker icon
	public IconSide icon_side; // Side where icon shows
	public string text; // Dialogue text

	// Input fields
	public GameObject input_display; // What to show on screen during choice
	// These fields are for input sections where the choices are limited
	// If the input section is free response, arrays are empty
	public string[] input_options; // Possible strings user can type in
	public Dialogue[] input_branches; // Dialogue to transition to after choice is made
}
