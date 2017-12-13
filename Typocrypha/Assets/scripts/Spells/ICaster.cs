using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Povides OOP structure neccesary for Universal casting
//Anything that can cast
public interface ICaster
{
    CasterStats Stats { get; }
    int Curr_hp { get; set; }
    int Curr_shield { get; set; }
    int Curr_stagger { get; set; }
    bool Is_stunned { get;}

    void damage(int d, int element, ICaster caster, bool crit, bool reflect = false);
}

