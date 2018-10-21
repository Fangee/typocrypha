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
    public override WordType Type { get { return WordType.Modifier; } }
    public Direction direction = Direction.Left;
}
