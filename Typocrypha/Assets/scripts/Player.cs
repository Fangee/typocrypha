using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contains Static referrence to global Player (Player.main)
public class Player
{
    //Main player character (Static Global Basically
    public static Player main = new Player();

    int max_hp = 100;
    int curr_hp = 100;
    int max_shield = 100;
    int shield = 100;
    int attack = 0;
    int defense = 0;
    int speed = 0;
    int accuracy = 0;
    int evasion = 0;
    bool is_dead = false;
	string last_cast = ""; // last casted spell

    //READ ONLY PROPERTIES

    public int Max_hp
    {
        get
        {
            return max_hp;
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
            return max_shield;
        }
    }
    public int Shield
    {
        get
        {
            return shield;
        }
    }
    public int Attack
    {
        get
        {
            return attack;
        }
    }
    public int Defense
    {
        get
        {
            return defense;
        }
    }
    public int Speed
    {
        get
        {
            return speed;
        }
    }
    public int Accuracy
    {
        get
        {
            return accuracy;
        }
    }
    public int Evasion
    {
        get
        {
            return evasion;
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

    public bool damage(int d, string type)
    {
        Debug.Log("Player" + " was hit for " + d + " of " + type + " damage");
        curr_hp -= d; 
        if (Curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
            is_dead = true;
        }
        return Is_dead;
    }
}
