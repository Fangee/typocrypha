using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Povides OOP structure neccesary for Universal casting
//Anything that can cast
public interface ICaster
{
    CasterStats Stats { get;}

    void damage(int d, int element, ICaster caster, bool reflect = false);
}


