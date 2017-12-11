using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    //Const fields//

    private const float stagger_mult_constant = 1F;//Amount to multiply max_stagger by when calculating stagger time
    private const float stagger_add_constant = 5F;//Amount to add when calculating stagger time

    //Public fields//

    public bool is_dead; // is enemy dead?
    public bool is_stunned; // is the enemy stunned?
    public Enemy[] field; //State of battle scene (for ally-target casting)
    public int position; //index to field (current position)
    public EnemyChargeBars bars;
	public SpriteRenderer enemy_sprite; // this enemy's sprite

    //Private fields

    EnemyStats stats; // stats of enemy DO NOT MUTATE
    int curr_spell = 0;
	int curr_hp; // current amount of health
    int curr_shield; //current amount of shield
    int curr_stagger = 0; //current amount of stagger
    float stagger_time; //The time an enemy's stun will last
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
        stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
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
        float curr_stagger_time = 0F;
        SpellData s = stats.spells[curr_spell];        //Initialize with current spell
        atk_time = dict.getCastingTime(s, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForSeconds (0.1f);
			while (BattleManager.main.pause)
				yield return new WaitForSeconds (0.1f);
            while(is_stunned)//Stop attack loop from continuing while the enemy is stunned
            {
                if (curr_stagger_time >= stagger_time)//End stun if time up
                {
                    unStun();
                    curr_stagger_time = 0F;
                }
                else//Wait longer
                {
                    yield return new WaitForSeconds(0.1f);
                    if (!BattleManager.main.pause)
                    {
                        curr_stagger_time += 0.1F;
                        BattleEffects.main.spriteShake(gameObject.transform, 0.05f, 0.05f);
                    }
                }
            }
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
        int staggerDamage = 0;
        //Apply elemental weakness/resistances
        float dMod = stats.vsElement[element] * d;
        //Calculate stagger damage (UNFINISHED add crit, mods, etc)
        if (stats.vsElement[element] > 1)//If enemy is weak
            staggerDamage++;
        //Apply stun damage mod
        if (is_stunned)
            dMod *= (1.25F + (0.25F * staggerDamage));
		Debug.Log (stats.name + " was hit for " + dMod + " of " + Elements.toString(element) + " damage");
        //Apply shield
        if (curr_shield > 0)
        {
            if (curr_shield - dMod < 0)//Shield breaks
            {
                curr_shield = 0;
                curr_hp -= Mathf.FloorToInt(dMod - curr_shield);
                if (staggerDamage >= 1)
                    curr_stagger++;
            }
            else
                curr_shield -= Mathf.FloorToInt(dMod);
        }
        else
        {
            curr_hp -= Mathf.FloorToInt(dMod);
            if (staggerDamage >= 1)
                curr_stagger++;
        }
        //Apply stun if applicable
        if (curr_stagger >= stats.max_stagger)
            stun();
        //Apply shake if hit
        if(dMod > 0)
            BattleEffects.main.spriteShake(gameObject.transform, 0.5f, 0.1f * Mathf.Log( dMod, 7F));
        //opacity and death are now updated in updateCondition()

    }
    
    private void stun()
    {
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.5F, 0);
        is_stunned = true;
    }

    private void unStun()
    {
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
        is_stunned = false;
        curr_stagger = 0;
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
