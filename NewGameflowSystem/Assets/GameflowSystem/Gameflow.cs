using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the flow of a level (from dialogue to battle to etc...)
[CreateAssetMenu]
public class Gameflow : ScriptableObject {
	public GameflowItem[] items; // List of events
}
