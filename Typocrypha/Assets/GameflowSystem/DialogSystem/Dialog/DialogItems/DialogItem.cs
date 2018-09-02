using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconSide {LEFT, RIGHT, BOTH, NONE}; // Side which icon displays
public enum DialogueType {NORMAL, INPUT}; // Type of player interaction with dialouge

// Represents a single line of dialogue, and all it's effects/events/etc
public class DialogItem {
	[HideInInspector] public List<FXTextEffect> fx_text_effects; // List of text effects
	[HideInInspector] public List<TextEvent>[] text_events; // Array of text events at each character index
	public string text; // Dialogue text
    public DialogItem(string text)
    {
        this.text = text;
    }
}




