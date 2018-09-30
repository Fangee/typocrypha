using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Casting
{
    public enum ICasterType
    {
        INVALID,
        PLAYER,
        ENEMY,
        NPC_ALLY,
    }
    public interface ICaster
    {
        Battlefield.Position Position { get; set; }
        Battlefield.Position TargetPosition { get; set; }
        CasterStats Stats { get; }
        int Health { get; set; }
        int Stagger { get; set; }
        bool Stunned { get; }
        bool Dead { get; }
        ICasterType CasterType { get; }
    }

}
