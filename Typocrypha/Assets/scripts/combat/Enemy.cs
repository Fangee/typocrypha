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
    public bool is_dead; // is enemy dead?
    public Enemy[] field; //State of battle scene (for ally-target casting)
    public int position; //index to field (current position)
	public SpriteRenderer enemy_sprite; // this enemy's sprite
    EnemyStats stats; // stats of enemy
    SpellData[] spells; // castable spells
    int curr_spell = 0;
	int curr_hp; // current amount of health
	float curr_time; // current time (from 0 to atk_time)
    private Player target = Player.main; //Current target;
    private static SpellDictionary dict; //Dictionary to refer to (set in setStats)

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
        if(dict == null)
            dict = GameObject.FindGameObjectWithTag("SpellDictionary").GetComponent<SpellDictionary>();
        stats.speed = (1.1F) + Random.Range(0,0.75F);
        stats.attack = 1;
        stats.defense = 1;
		enemy_sprite = GetComponent<SpriteRenderer> ();
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

	// returns curr_time/atk_time
	public float getProgress() {
		return curr_time / stats.atk_time;
	}

	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator timer() {
        SpellData s = spells[curr_spell];        //Initialize with current spell
        stats.atk_time = dict.getCastingTime(s, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForSeconds (0.1f);
			while (BattleManager.main.pause)
				yield return new WaitForSeconds (0.1f);
			curr_time += 0.1f;
			if (curr_time >= stats.atk_time) {
				BattleManager.main.pause = true; // pause battle for attack
				BattleEffects.main.toggleDim(enemy_sprite);
				yield return new WaitForSeconds (1f);
				attackPlayer (s,target);
				yield return new WaitForSeconds (1f);
				BattleEffects.main.toggleDim(enemy_sprite);
                curr_spell++;
                if (curr_spell >= spells.Length)//Reached end of spell list
                    curr_spell = 0;
                s = spells[curr_spell]; //get next spell
                stats.atk_time = dict.getCastingTime(s, stats.speed);//get new casting time
                curr_time = 0;
				BattleManager.main.pause = false; // unpause
			}
		}
	}

	// attacks player with specified spell
	void attackPlayer(SpellData s, Player target) {
		Debug.Log (stats.name + " casts " + s.ToString());
		StartCoroutine (swell ());
		BattleEffects.main.screenShake (0.5f);
        dict.GetComponent<SpellDictionary>().enemyCast(this, s, field, position, target);
	}

	// be attacked by the player
	public void damage(int d) {
		Debug.Log (stats.name + " was hit for " + d);
		curr_hp -= d;
		// make enemy sprite fade as damaged (lazy health rep)
		enemy_sprite.color = new Color(1, 1, 1, (float)curr_hp/stats.max_hp);
		if (curr_hp <= 0) { // check if killed
			Debug.Log (stats.name + " has been slain!");
			is_dead = true;
            BattleEffects.main.toggleDim(enemy_sprite);
            GameObject.Destroy (gameObject);
		}
	}

	// cause enemy to swell in size for a short period of time (lazy attack rep)
	IEnumerator swell() {
		transform.localScale = new Vector3 (1.25f, 1.25f, 1.25f);
		yield return new WaitForSeconds (1f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
}
