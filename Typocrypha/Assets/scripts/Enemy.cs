using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simple container for enemy stats
public struct EnemyStats {
	public string name;    // name of enemy
    public int max_hp;     // max health
	public float atk_time; // time it takes to attack
                           // also will eventually have other stats
    //Makes casting easier (irrelevant right now)
    public int max_shield;
    public int shield;
    //Spell modifiers (to be used when spellcasting is hooked up)
    public int attack;     //numerical damage boost
    public int defense;    //numerical damage reduction
    public float speed;    //percentage of casting time reduction
    public int accuracy;   //numerical hitchance boost
    public int evasion;    //numerical dodgechance boost
    //ADD elemental weaknesses/resistances
}

// defines enemy behaviour
public class Enemy : MonoBehaviour {
    private SpellDictionary dict;
    public bool is_dead; // is enemy dead?
    public Enemy[] field; //State of battle scene (for ally-target casting)
    public int position; //index to field (current position)
    EnemyStats stats; // stats of enemy
    SpellData[] spells; // castable spells
    int curr_spell = 0;
	int curr_hp; // current amount of health
	float curr_time; // current time (from 0 to atk_time)

	void Start() {
		is_dead = false;
       
    }

	public void setStats(EnemyStats i_stats) {
		stats = i_stats;
		curr_hp = stats.max_hp;
		curr_time = 0;
        SpellData[] sp = { new SpellData("sword") };
        //DEFAULT until other enemy stats are added to scenes or can be loaded somehow (Maybe lets build a database in a seperate excel?)
        setSpells(sp);
        dict = GameObject.FindGameObjectWithTag("SpellDictionary").GetComponent<SpellDictionary>();
        stats.speed = 1;
        stats.attack = 1;
        stats.defense = 1;
        //Start Attacking
        StartCoroutine (timer ()); 
	}

    public EnemyStats getStats()
    {
        return stats;
    }

    public void setSpells(SpellData[] spells)
    {
        this.spells = spells;
    }

	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator timer() {
        SpellData s = spells[curr_spell];        //Initialize with currnet spell
        stats.atk_time = dict.getCastingTime(s, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForSeconds (0.1f);
			curr_time += 0.1f;
			if (curr_time >= stats.atk_time) {
				attackPlayer (s);
                curr_spell++;
                if (curr_spell >= spells.Length)//Reached end of spell list
                    curr_spell = 0;
                s = spells[curr_spell]; //get next spell
                stats.atk_time = dict.getCastingTime(s, stats.speed);//get new casting time
                curr_time = 0;
			}
		}
	}

	// attacks player with specified spell
	// NOTE: once we have a spell class/etc, we might want to replace parameter with that
	void attackPlayer(SpellData s) {
		Debug.Log (stats.name + " casts " + s.ToString());
        dict.GetComponent<SpellDictionary>().enemyCast(this, s, field, position);
	}

	// be attacked by the player
	// NOTE: once we have a spell class/etc, we might want to replace parameter with that
	public void damage(int d) {
		Debug.Log (stats.name + " was hit for " + d);
		curr_hp -= d; // TEMP: hard coded for now
		if (curr_hp <= 0) { // check if killed
			Debug.Log (stats.name + " has been slain!");
			is_dead = true;
			GameObject.Destroy (gameObject);
		}
	}
}
