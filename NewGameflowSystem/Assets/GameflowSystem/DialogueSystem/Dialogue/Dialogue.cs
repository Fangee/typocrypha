using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Represents multiple lines of dialogue
[CreateAssetMenu]
public class Dialogue : GameflowItem {
	public int count = 0;
	public DialogueItem[] lines = new DialogueItem[256]; // Lines of dialogue
}
