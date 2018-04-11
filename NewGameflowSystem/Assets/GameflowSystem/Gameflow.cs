using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the flow of a level (from dialogue to battle to etc...)
public class Gameflow : ScriptableObject {
	public int count = 0; // Number of events
	public GameflowItem[] items = new GameflowItem[64]; // List of events
}
