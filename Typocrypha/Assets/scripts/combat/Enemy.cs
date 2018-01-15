﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// simple container for enemy stats (Not a struct anymore cuz structs pass by value in c#)
public class EnemyStats : CasterStats {
    //Sorry for the massive constructor but all the vals are readonly so...
    public EnemyStats(string name, string sprite, int hp, int shield, int stag, float atk, float def, float speed, float acc, int evade, float[] vsElem = null, SpellData[] sp = null)
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

    //ICaster Properties//

    public CasterStats Stats
    {
        get
        {
            return stats;
        }
    }
    public BuffDebuff BuffDebuff { get { return buffDebuff; } }
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
    public int Curr_shield { get { return curr_shield; } set { curr_shield = value; } }
    public int Curr_stagger { get { return curr_stagger; } set { curr_stagger = value; } }
    public bool Is_stunned { get { return is_stunned; } }
    public bool Is_dead { get { return is_dead; } }
    public ICasterType CasterType
    {
        get
        {
            return ICasterType.ENEMY;
        }
    }


    //Public fields//

    bool is_dead; // is enemy dead?
    public bool attack_in_progress = false;

    public BattleManager field;
    public int position; //index to field (current position)
    public EnemyChargeBars bars;
	public SpriteRenderer enemy_sprite; // this enemy's sprite

    EnemyStats stats; // stats of enemy DO NOT MUTATE
    BuffDebuff buffDebuff = new BuffDebuff(); // buff/debuff state

    bool is_stunned; // is the enemy stunned?
    int curr_spell = 0;
	int curr_hp; // current amount of health
    int curr_shield; //current amount of shield
    int curr_stagger; //current amount of stagger
    float stagger_time; //The time an enemy's stun will last
	float curr_stagger_time; // current time staggered
    float curr_time; // current time (from 0 to atk_time)
    float atk_time; // time it takes to attack
    private static SpellDictionary dict; //Dictionary to refer to (set in setStats)

    void Start() {
		is_dead = false;
    }

	public void setStats(EnemyStats i_stats) {
		stats = i_stats;
		Curr_hp = stats.max_hp;
        Curr_shield = stats.max_shield;
        stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
        Curr_stagger = stats.max_stagger;
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

	// returns progress of stagger bar
	public float getStagger() {
		if (is_stunned) return curr_stagger_time / stagger_time;
		else            return ((float)curr_stagger/(float)stats.max_stagger);
	}
    //Main attack AI
	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator timer() {
		Vector3 original_pos = transform.position;
        curr_stagger_time = 0F;
        SpellData s = stats.spells[curr_spell];        //Initialize with current spell
        atk_time = dict.getCastingTime(s, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForEndOfFrame ();
			yield return new WaitWhile (() => BattleManager.main.pause);
            while(is_stunned)//Stop attack loop from continuing while the enemy is stunned
            {
				yield return new WaitForEndOfFrame();
				BattleEffects.main.spriteShake(gameObject.transform, Time.deltaTime * 2, 0.05f);
				yield return new WaitWhile (() => BattleManager.main.pause);
				curr_stagger_time += Time.deltaTime;
                if (curr_stagger_time >= stagger_time)//End stun if time up
                {
                    unStun();
                    curr_stagger_time = 0F;
                }
            }
			curr_time += Time.deltaTime;
			if (curr_time >= atk_time) {
                attack_in_progress = true;
                fullBarFX(); // notify player of full bar
                yield return new WaitForSeconds(1f);
                yield return new WaitWhile(() => BattleManager.main.pause);
                if(!is_stunned)
                    attackPlayer (s);
                curr_spell++;
                if (curr_spell >= stats.spells.Length)//Reached end of spell list
                    curr_spell = 0;
                s = stats.spells[curr_spell]; //get next spell
                atk_time = dict.getCastingTime(s, stats.speed);//get new casting time
                curr_time = 0;
                //resetBarFX(); // stop full bar effects (now in barFlash)
            }
		}
	}

    // effects when enemy is ready to attack
    void fullBarFX()
    {
        //graphic
        StartCoroutine(barFlash());
        //sound
        AudioPlayer.main.playSFX(1, SFXType.BATTLE, "enemy_attack_ready");
    }

    IEnumerator barFlash()
    {
        for (int i = 0; i < 60; i++)
        {
            bars.Charge_bars[position].setColor(Random.value, Random.value, 0.1F);

            yield return new WaitForEndOfFrame();
        }
        bars.Charge_bars[position].setColor(1F, 0.1F, 0.1F);

        yield return new WaitWhile(() => attack_in_progress);

        resetBarFX();
    }

    // terminate effects started by fullBarFX()
    void resetBarFX()
    {
        bars.Charge_bars[position].setColor(0, 0.9F, 0);
    }

    // pause battle, attack player with specified spell
    void attackPlayer(SpellData s) {
        Debug.Log(stats.name + " casts " + s.ToString());
        field.enemyCast(dict, s, position);
	}

	// be attacked by the player
	public void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false) {
        //Apply reflect and return if applicable
        if (CasterOps.calcReflect(data,d,element,caster,this,crit,reflect))
            return;
        CasterOps.calcDamage(data, d, element, caster, this, crit, Is_stunned);
        //Apply stun if applicable
        if (curr_stagger <= 0 && is_stunned == false)
        {
            data.isStun = true;
            stun();
        }   
        //opacity and death are now updated in updateCondition()
    }
    //Apply stun condition to enemy
    private void stun()
    {
        is_stunned = true;
    }
    //Un-stun enemy
    private void unStun()
    {
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
        is_stunned = false;
        Curr_stagger = stats.max_stagger;
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

    //Starts swell from outside class (used in battlemanager.cs)
    public void startSwell()
    {
        StartCoroutine(swell());
    }

	// cause enemy to swell in size for a short period of time (lazy attack rep)
	IEnumerator swell() {
		transform.localScale = new Vector3 (1.25f, 1.25f, 1.25f);
		yield return new WaitForSeconds (1f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
}
