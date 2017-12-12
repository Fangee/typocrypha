using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// simple container for enemy stats (Not a struct anymore cuz structs pass by value in c#)
public class EnemyStats : CasterStats {
    //Sorry for the massive constructor but all the vals are readonly so...
    public EnemyStats(string name, string sprite, int hp, int shield, int stag, int atk, int def, float speed, int acc, int evade, float[] vsElem = null, SpellData[] sp = null)
        : base(name, hp, shield, stag, atk, def, speed, acc, evade, vsElem)
    {
        sprite_path = sprite;
        spells = sp;
    }
    public readonly string sprite_path; //path of sprite/resource to load at creation
    public readonly SpellData[] spells; // castable spells
}

// defines enemy behaviour
public class Enemy : MonoBehaviour, ICaster {

    //Const fields//

    private const float stagger_mult_constant = 1F;//Amount to multiply max_stagger by when calculating stagger time
    private const float stagger_add_constant = 5F;//Amount to add when calculating stagger time

    //Public fields//

    public bool is_dead; // is enemy dead?
    public bool is_stunned; // is the enemy stunned?
    public Enemy[] allies; //State of battle scene (for ally-target casting)
    public ICaster[] targets;//State of the player and their allies (for target-casting)

    public int position; //index to field (current position)
    public EnemyChargeBars bars;
	public SpriteRenderer enemy_sprite; // this enemy's sprite

    public CasterStats Stats
    {
        get
        {
            return stats;
        }
    }

    //Private fields
    private int selected = 1;//Which target in targets is selected (index to targets)
    EnemyStats stats; // stats of enemy DO NOT MUTATE

    int curr_spell = 0;
	int curr_hp; // current amount of health
    int curr_shield; //current amount of shield
    int curr_stagger; //current amount of stagger
    float stagger_time; //The time an enemy's stun will last
    float curr_time; // current time (from 0 to atk_time)
    float atk_time; // time it takes to attack
    private static SpellDictionary dict; //Dictionary to refer to (set in setStats)

    void Start() {
		is_dead = false;
    }

	public void setStats(EnemyStats i_stats) {
		stats = i_stats;
		curr_hp = stats.max_hp;
        curr_shield = stats.max_shield;
        stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
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
    //Main attack AI
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
				AudioPlayer.main.playSFX (1, SFXType.SPELL, "magic_sound"); 
				yield return new WaitForSeconds (1.5f);
				attackPlayer (s);
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

	// pause battle, attack player with specified spell
	void attackPlayer(SpellData s) {
		Debug.Log (stats.name + " casts " + s.ToString());
		StartCoroutine (swell ());
		BattleEffects.main.screenShake (0.5f, 0.1f);
        dict.GetComponent<SpellDictionary>().NPC_Cast(s,targets, selected, allies, position);
	}

	// be attacked by the player
	public void damage(int d, int element, ICaster caster, bool reflect = false) {
        //Reflect damage to caster if enemy reflects this element
        if(stats.vsElement[element] == Elements.reflect  && reflect == false)
        {
            Debug.Log("Enemy reflects " + d + " " + Elements.toString(element) + " damage back at player");
            caster.damage(d, element, this, true);
            return;
        }

        bool damaged = CasterOps.calcDamage(d, element, caster, this, ref curr_hp, ref curr_shield, ref curr_stagger, is_stunned);
        //Apply stun if applicable
        if (curr_stagger <= 0 && is_stunned == false)
            stun();
        //Apply shake and sfx if hit
		if (damaged) { 
			BattleEffects.main.spriteShake (gameObject.transform, 0.5f, 0.1f);
		}
        //opacity and death are now updated in updateCondition()
    }
    //Apply stun condition to enemy
    private void stun()
    {
		AudioPlayer.main.playSFX (2, SFXType.BATTLE, "sfx_stagger");
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.5F, 0);
        is_stunned = true;
    }
    //Un-stun enemy
    private void unStun()
    {
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
        is_stunned = false;
        curr_stagger = stats.max_stagger;
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
			StopAllCoroutines ();
        }

    }

	// cause enemy to swell in size for a short period of time (lazy attack rep)
	IEnumerator swell() {
		transform.localScale = new Vector3 (1.25f, 1.25f, 1.25f);
		yield return new WaitForSeconds (1f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
}
