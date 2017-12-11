using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Class containing Player stat data (structs are pass by value)
//Can be used to set player stats or construct a new player with given stats
//Can also be used as a stat buff/debuff modifier
public class PlayerStats
{
    public int max_hp;
    public int max_shield;
    public int attack;
    public int defense;
    public int speed;
    public int accuracy;
    public int evasion;
    public float[] vsElement;
}

//Contains Static referrence to global Player (Player.main)
public class Player : ICaster
{
    //Main player character (Static Global Basically)
    public static Player main = new Player();

    //Constructors

    //Construct player with default stats
    public Player()
    {
        stats = new PlayerStats();
        stats.max_hp = 100;
        stats.max_shield = 100;
        stats.attack = 0;
        stats.defense = 0;
        stats.speed = 1;
        stats.accuracy = 0;
        stats.evasion = 0;
        stats.vsElement = new float[Elements.count];
        for(int i = 0; i < Elements.count; i++)
        {
            stats.vsElement[i] = 1.0F;
        }
    }
    //Construct player with specified stats
    public Player(PlayerStats i_stats)
    {
        stats = i_stats;
    }

    //Fields

    PlayerStats stats;
    int curr_hp = 100;
    int curr_shield = 100;
    bool is_dead = false;
	string last_cast = ""; // last casted spell

    //READ ONLY PROPERTIES

    public int Max_hp
    {
        get
        {
            return stats.max_hp;
        }
    }
    public int Curr_hp
    {
        get
        {
            return curr_hp;
        }
    }
    public int Max_shield
    {
        get
        {
            return stats.max_shield;
        }
    }
    public int Shield
    {
        get
        {
            return curr_shield;
        }
    }
    public int Attack
    {
        get
        {
            return stats.attack;
        }
    }
    public int Defense
    {
        get
        {
            return stats.defense;
        }
    }
    public int Speed
    {
        get
        {
            return stats.speed;
        }
    }
    public int Accuracy
    {
        get
        {
            return stats.accuracy;
        }
    }
    public int Evasion
    {
        get
        {
            return stats.evasion;
        }
    }
    public bool Is_dead
    {
        get
        {
            return is_dead;
        }
    }

    //READ/WRITE PROPERTIES

    public string Last_cast
    {
        get
        {
            return last_cast;
        }
        set
        {
            last_cast = value;
        }
    }

    //Public Methods

    //Restores player's HP and Shields to Maximum
    public void restoreToFull()
    {
        curr_hp = Max_hp;
        curr_shield = Max_shield;
    }
    //Damage player (hits shield first, if shield remains)
    public void damage(int d, int element, ICaster caster, bool reflect = false)
    {
        //Reflect damage to caster if enemy reflects this element
        if (stats.vsElement[element] == Elements.reflect && reflect == false)
        {
            int dRef = d + stats.defense;
            Debug.Log("Palyer reflects " + dRef + " " + Elements.toString(element) + " damage back at enemy");
            caster.damage(dRef, element, this, true);
            return;
        }
        //Absorb damage if enemy absorbs this type
        else if (stats.vsElement[element] == Elements.absorb)
        {
            Debug.Log("Player absorbs for " + d + " " + Elements.toString(element) + " damage");
            curr_hp += d;
            if (curr_hp > stats.max_hp)
                curr_hp = stats.max_hp;
            return;
        }
        float dMod;
        //Stop 
        if (reflect == false || stats.vsElement[element] != Elements.reflect)
            dMod = stats.vsElement[element] * d;
        else
            dMod = d * Elements.reflect_mod; 
        if(curr_shield > 0)
        {
            if (curr_shield - dMod < 0)
            {
                curr_shield = 0;
                curr_hp -= Mathf.FloorToInt(dMod - curr_shield);
            }
            else
                curr_shield -= Mathf.FloorToInt(dMod);
        }
        else
            curr_hp -= Mathf.FloorToInt(dMod);
        Debug.Log("Player" + " was hit for " + dMod + " of " + Elements.toString(element) + " damage");
        if (Curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
			BattleManager.main.pause = true;
			BattleEffects.main.setDim (false, null);
			restoreToFull ();
			BattleManager.main.stopBattle ();
			StateManager.main.revertScene (2f);
            is_dead = true;
        }
       // return Is_dead;
    }
}
