using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum ICasterType
    {
        INVALID,
        PLAYER,
        ENEMY,
        ALLY,
    }
    public interface ICaster
    {
        string Name { get; }
        Vector3 WorldPos { get; set; }
        Battlefield.Position FieldPos { get; set; }
        Battlefield.Position TargetPos { get; set; }
        CasterStats Stats { get; }
        int Health { get; set; }
        int Stagger { get; set; }
        bool Stunned { get; }
        bool Dead { get; }
        ICasterType CasterType { get; }
    }
