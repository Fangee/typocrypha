using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages effects on the dialogue box
public class DialogueBoxEffects : MonoBehaviour {
	public SpriteRenderer spacebar_icon; // icon for prompting spacebar
	public TextScroll text_scroll; // text scroll script for the dialogue box
	public Color gray; // graying out color for spacebar icon

	void Update() {
		if (text_scroll.is_print)
			spacebar_icon.color = gray;
		else
			spacebar_icon.color = Color.white;
	}
}
