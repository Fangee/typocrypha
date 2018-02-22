using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ICasterType
{
    INVALID,
    PLAYER,
    ENEMY,
    NPC_ALLY,
}

//Povides OOP structure neccesary for Universal casting
//Anything that can cast
public interface ICaster
{
    Transform Transform { get; }
    CasterStats Stats { get; }
    BuffDebuff BuffDebuff { get; }
    int Curr_hp { get; set; }
    int Curr_shield { get; set; }
    int Curr_stagger { get; set; }
    bool Is_stunned { get; }
    bool Is_dead { get; }
    ICasterType CasterType { get; }
    //Note: cast data actually functions as an input/output variable
    void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false);
}

