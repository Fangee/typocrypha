using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Struct containing Player stat data
//Can be used to set player stats or construct a new player with given stats
//Can also be used as a stat buff/debuff modifier
public struct PlayerStats
{
    public int max_hp;
    public int max_shield;
    public int attack;
    public int defense;
    public int speed;
    public int accuracy;
    public int evasion;
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
        stats.max_hp = 100;
        stats.max_shield = 100;
        stats.attack = 0;
        stats.defense = 0;
        stats.speed = 2;
        stats.accuracy = 0;
        stats.evasion = 0;
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
    public bool damage(int d, string type)
    {
        if(curr_shield > 0)
        {
            if (curr_shield - d < 0)
            {
                curr_shield = 0;
                curr_hp -= (d - curr_shield);
            }
            else
                curr_shield -= d;
        }
        else
            curr_hp -= d;
        Debug.Log("Player" + " was hit for " + d + " of " + type + " damage");
        if (Curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
            is_dead = true;
        }
        return Is_dead;
    }
}
