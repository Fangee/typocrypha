using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Class containing Player stat data (structs are pass by value)
//Can be used to set player stats or construct a new player with given stats
//Can also be used as a stat buff/debuff modifier with CasterStats.modify;
public class PlayerStats : CasterStats
{
    public PlayerStats() : base("Player", 100, 100, -1, 1F, 0, 1F, 1F, 0, new float[Elements.count])
    {
        for (int i = 0; i < Elements.count; i++)
        {
            vsElement[i] = 1.0F;
        }
    }
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
    }
    //Construct player with specified stats
    public Player(PlayerStats i_stats)
    {
        stats = i_stats;
    }

    //ICaster Poroperties

    PlayerStats stats;
    public CasterStats Stats { get { return stats; } }
    public int Curr_hp
    {
        get
        {
            return curr_hp;
        }
        set
        {
            curr_hp = value;
        }
    }
    public int Curr_shield
    {
        get
        {
            return curr_shield;
        }
        set
        {
            curr_shield = value;
        }
    }
    public int Curr_stagger
    {
        get
        {
            return 0;
        }
        set { return; }
    }
    public bool Is_stunned { get { return false; } }
    public bool Is_dead
    {
        get
        {
            return is_dead;
        }
    }
    public ICasterType CasterType { get {return ICasterType.PLAYER; } }

    //Fields

    int curr_hp = 100;
    int curr_shield = 100;
    bool is_dead = false;
	string last_cast = ""; // last casted spell

    //READ ONLY PROPERTIES

 

    //READ/WRITE PROPERTIES

    public string Last_cast
    {
        get
        {
            return last_cast;
        }
        set
        {
            last_cast = value.Replace(' ', '-').ToUpper();
        }
    }

    //Public Methods

    //Restores player's HP and Shields to Maximum
    public void restoreToFull()
    {
        curr_hp = Stats.max_hp;
        curr_shield = Stats.max_shield;
    }

    //ICaster interface overrides

    //Damage player
    public void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false)
    {
        //Reflect damage to caster if enemy reflects this element (FIX WITH CASTDATA)
        if (stats.vsElement[element] == Elements.reflect && reflect == false)
        {
            int dRef = d;
            Debug.Log("Player reflects " + dRef + " " + Elements.toString(element) + " damage back at enemy");
            caster.damage(data, dRef, element, caster, crit, true);
            return;
        }
        bool damaged = CasterOps.calcDamage(data, d, element, caster, this, crit);

        if (damaged) {
		}
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
