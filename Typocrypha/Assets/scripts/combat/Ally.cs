﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// simple container for Ally stats (Not a struct anymore cuz structs pass by value in c#)
//public class AllyStats : CasterStats
//{
//    //Sorry for the massive constructor but all the vals are readonly so...
//    public AllyStats(string name, string chat, int hp, int shield, int stag, float atk, float def, float speed, float acc, int evade, float[] vsElem = null)
//        : base(name, chat, hp, shield, stag, atk, def, speed, acc, evade, vsElem)
//    {
//        spell = new SpellData(name.ToLower());
//    }
//    public SpellData spell; // castable spell
//    public AllyStats clone()
//    {
//        return new AllyStats(name, ChatDatabaseID, max_hp, max_shield, max_stagger, attack, defense, speed, accuracy, evasion, vsElement);
//    }
//}

//public class Ally : MonoBehaviour, ICaster {

//    private const float stagger_mult_constant = 1F;//Amount to multiply max_stagger by when calculating stagger time
//    private const float stagger_add_constant = 5F;//Amount to add when calculating stagger time

//    //ICASTER STUFF//
//    public Transform Transform { get { return transform; } }
//    private int position; //position in battle field
//    public int Position { get { return position; } set { position = value; } }
//    private AllyStats stats;
//    public CasterStats Stats
//    {
//        get
//        {
//            return stats;
//        }
//    }

//    private BuffDebuff buffDebuff = new BuffDebuff();
//    public BuffDebuff BuffDebuff { get { return buffDebuff; } }

//    private float gauge_value;
//    public int Curr_hp { get { return Mathf.CeilToInt(gauge_value); } set { gauge_value = Mathf.Clamp(value, 0, stats.max_hp); } }
//    private int curr_stagger;
//    public int Curr_stagger { get { return curr_stagger; } set { curr_stagger = value; } }

//    bool is_stunned = false;
//    public bool Is_stunned { get { return is_stunned; } }
//    private bool is_dead = false;
//    public bool Is_dead { get { return is_dead; } }

//    public ICasterType CasterType { get { return ICasterType.NPC_ALLY; } }

//    private int _target = 1;
//    public int TargetPosition
//    {
//        get
//        {
//            return _target;
//        }

//        set
//        {
//            _target = value;
//        }
//    }

//    public void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false)
//    {
//        //Apply repel and return if applicable
//        if (CasterOps.calcRepel(data, d, element, caster, this, crit, reflect))
//            return;
//        CasterOps.calcDamage(data, d, element, caster, this, crit);
//        //Apply stun if applicable
//        if (curr_stagger <= 0 && is_stunned == false)
//        {
//            data.isStun = true;
//            stun();
//        }
//    }

//    public void setStats(AllyStats stats)
//    {
//        this.stats = stats;
//        gauge_value = stats.max_hp /2;
//        stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
//        Curr_stagger = stats.max_stagger;
//        StartCoroutine(charge());
//    }

//    public bool tryCast()
//    {
//        if(getPercent() >= 0.75)
//        {
//            gauge_value = stats.max_hp / 2;
//            return true;
//        }
//        return false;
//    }

//    //END ICASTER STUFF//

//    float stagger_time; //The time an enemy's stun will last
//    float curr_stagger_time = 0; // current time staggered

//    IEnumerator charge()
//    {
//        while (!is_dead)
//        {
   
//            yield return new WaitForEndOfFrame();
//            yield return new WaitWhile(() => BattleManagerS.main.battlePause);
//            while (is_stunned)//Stop attack loop from continuing while the enemy is stunned
//            {
//                yield return new WaitForEndOfFrame();
//                //BattleEffects.main.spriteShake(gameObject.transform, Time.deltaTime * 2, 0.05f);
//                yield return new WaitWhile(() => BattleManagerS.main.battlePause);
//                curr_stagger_time += Time.deltaTime;
//                if (curr_stagger_time >= stagger_time)//End stun if time up
//                {
//                    unStun();
//                    curr_stagger_time = 0F;
//                }
//            }
//            if(gauge_value < stats.max_hp)
//                gauge_value += 5 * (Time.deltaTime * stats.speed);
//        }
//    }

//    //Apply stun condition to ally
//    private void stun()
//    {
//        is_stunned = true;
//    }
//    //Un-stun ally
//    private void unStun()
//    {
//        is_stunned = false;
//        Curr_stagger = stats.max_stagger;
//    }

//    //Gets charge bar percentage full
//    public float getPercent()
//    {    
//        return Mathf.Clamp01(gauge_value / stats.max_hp); 
//    }

//    // Use this for initialization
//    void Start () {
		
//	}
	
//	// Update is called once per frame
//	void Update () {
		
//	}
//}
