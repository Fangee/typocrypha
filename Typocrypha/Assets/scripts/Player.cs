using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Player
{
    public static int max_hp = 100;
    public static int curr_hp = 100;
    public static int max_shield;
    public static int shield;
    public static int attack = 0;
    public static int defense = 0;
    public static int speed = 0;
    public static int accuracy = 0;
    public static int evasion = 0;
    public static bool is_dead = false;
	public static string last_cast = ""; // last casted spell

    public static bool damage(int d, string type)
    {
        Debug.Log("Player" + " was hit for " + d + " of " + type + " damage");
        curr_hp -= d; 
        if (curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
            is_dead = true;
        }
        return is_dead;
    }
}
