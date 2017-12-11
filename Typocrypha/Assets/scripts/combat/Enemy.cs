using System.Collections;
using UnityEngine;

// simple container for enemy stats (Not a struct anymore cuz structs pass by value in c#)
public class EnemyStats {
    //Sorry for the massive constructor but all the vals are readonly so...
    public EnemyStats(string name, string sprite, int hp, int shield, int stag, int atk, int def, float speed, int acc, int evade, float[] vsElem = null, SpellData[] sp = null)
    {
        this.name = name;
        sprite_path = sprite;
        max_hp = hp;
        max_shield = shield;
        max_stagger = stag;
        attack = atk;
        defense = def;
        this.speed = speed;
        accuracy = acc;
        evasion = evade;
        vsElement = vsElem;
        spells = sp;
    }
	public readonly string name;    // name of enemy
    public readonly string sprite_path; //path of sprite/resource to load at creation
    public readonly int max_hp;     // max health
    public readonly int max_stagger; //max stagger
    //Makes casting easier (irrelevant right now)
    public readonly int max_shield;
    //Spell modifiers (to be used when spellcasting is hooked up)
    public readonly int attack;     //numerical damage boost
    public readonly int defense;    //numerical damage reduction
    public readonly float speed;    //percentage of casting time reduction
    public readonly int accuracy;   //numerical hitchance boost
    public readonly int evasion;    //numerical dodgechance boost
    public readonly float[] vsElement; //elemental weaknesses/resistances
    public readonly SpellData[] spells; // castable spells
}

// defines enemy behaviour
public class Enemy : MonoBehaviour {
    public bool is_dead; // is enemy dead?
    public Enemy[] field; //State of battle scene (for ally-target casting)
    public int position; //index to field (current position)
	public SpriteRenderer enemy_sprite; // this enemy's sprite
    EnemyStats stats; // stats of enemy DO NOT MUTATE

    int curr_spell = 0;
	int curr_hp; // current amount of health
    int curr_shield; //current amount of shield
    int curr_stagger; //current amount of stagger
    float curr_time; // current time (from 0 to atk_time)
    float atk_time; // time it takes to attack
    private Player target = Player.main; //Current target;
    private static SpellDictionary dict; //Dictionary to refer to (set in setStats)

    void Start() {
		is_dead = false;
    }

	public void setStats(EnemyStats i_stats) {
		stats = i_stats;
		curr_hp = stats.max_hp;
        curr_shield = stats.max_shield;
        curr_stagger = stats.max_stagger;
		curr_time = 0;
        if(dict == null)
            dict = GameObject.FindGameObjectWithTag("SpellDictionary").GetComponent<SpellDictionary>();
		enemy_sprite = GetComponent<SpriteRenderer> ();
        enemy_sprite.sprite = Resources.Load<Sprite>(stats.sprite_path);
        //Start Attacking
        StartCoroutine (timer ()); 
	}

    public EnemyStats getStats()
    {
        return stats;
    }


	// returns curr_time/atk_time
	public float getProgress() {
		return curr_time / atk_time;
	}

	// returns name of current spell
	public SpellData getCurrSpell() {
		return stats.spells[curr_spell];
	}

	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator timer() {
        SpellData s = stats.spells[curr_spell];        //Initialize with current spell
        atk_time = dict.getCastingTime(s, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForSeconds (0.1f);
			while (BattleManager.main.pause)
				yield return new WaitForSeconds (0.1f);
			curr_time += 0.1f;
			if (curr_time >= atk_time) {
				BattleManager.main.pause = true; // pause battle for attack
				BattleEffects.main.setDim(true, enemy_sprite);
				yield return new WaitForSeconds (1f);
				attackPlayer (s,target);
				yield return new WaitForSeconds (1f);
                curr_spell++;
                if (curr_spell >= stats.spells.Length)//Reached end of spell list
                    curr_spell = 0;
                s = stats.spells[curr_spell]; //get next spell
                atk_time = dict.getCastingTime(s, stats.speed);//get new casting time
                curr_time = 0;
				BattleEffects.main.setDim(false, enemy_sprite);
				BattleManager.main.pause = false; // unpause
			}
		}
	}

	// attacks player with specified spell
	void attackPlayer(SpellData s, Player target) {
		Debug.Log (stats.name + " casts " + s.ToString());
		StartCoroutine (swell ());
		BattleEffects.main.screenShake (0.5f, 0.1f);
        dict.GetComponent<SpellDictionary>().enemyCast(this, s, field, position, target);
	}

	// be attacked by the player
	public void damage(int d, int element) {
        //Apply elemental weakness/resistances
        int dMod = Mathf.FloorToInt(stats.vsElement[element] * d);
		Debug.Log (stats.name + " was hit for " + dMod + " of " + Elements.toString(element) + " damage");
        //Apply shield
        if (curr_shield > 0)
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
        if(dMod > 0)
            BattleEffects.main.spriteShake(gameObject.transform, 0.5f, 0.1f * Mathf.Log( dMod, 7F));
        //opacity and death are now updated in updateCondition()

    }
    
    //Updates opacity and death(after pause in battlemanager)
    public void updateCondition()
    {
        // make enemy sprite fade as damaged (lazy health rep)
        enemy_sprite.color = new Color(1, 1, 1, (float)curr_hp / stats.max_hp);
        if (curr_hp <= 0)
        { // check if killed
            Debug.Log(stats.name + " has been slain!");
            is_dead = true;
        }

    }

	// cause enemy to swell in size for a short period of time (lazy attack rep)
	IEnumerator swell() {
		transform.localScale = new Vector3 (1.25f, 1.25f, 1.25f);
		yield return new WaitForSeconds (1f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
}
