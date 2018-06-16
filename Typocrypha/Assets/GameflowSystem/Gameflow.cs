using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the flow of a level (from dialogue to battle to etc...)
public class Gameflow : GameflowItem {
    [HideInInspector] public int curr_item = -1; // Current event number
}
