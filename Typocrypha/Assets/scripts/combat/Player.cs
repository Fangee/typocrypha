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
    public float[] vsElem;
}

//Contains Static referrence to global Player (Player.main)
public class Player
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
        stats.vsElem = new float[Elements.count];
        for(int i = 0; i < Elements.count; i++)
        {
            stats.vsElem[i] = 1.0F;
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
    public bool damage(int d, int element)
    {
        int dMod = Mathf.FloorToInt(stats.vsElem[element] * d);
        if(curr_shield > 0)
        {
            if (curr_shield - dMod < 0)
            {
                curr_shield = 0;
                curr_hp -= (dMod - curr_shield);
            }
            else
                curr_shield -= dMod;
        }
        else
            curr_hp -= dMod;
        Debug.Log("Player" + " was hit for " + dMod + " of " + Elements.toString(element) + " damage");
        if (Curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
            is_dead = true;
        }
        return Is_dead;
    }
}
