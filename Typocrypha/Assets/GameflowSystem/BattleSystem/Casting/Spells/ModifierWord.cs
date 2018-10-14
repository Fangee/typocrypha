using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Spell Word/Modifier")]
public class ModifierWord : SpellWord {

    public enum Direction
    {
        Left,
        Right,
        Bidirectional,
    }
    public Direction direction = Direction.Left;
}
