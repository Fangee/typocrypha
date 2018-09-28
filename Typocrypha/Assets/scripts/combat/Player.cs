using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Class containing Player stat data (structs are pass by value)
//Can be used to set player stats or construct a new player with given stats
//Can also be used as a stat buff/debuff modifier with CasterStats.modify;
public class PlayerStats : CasterStats
{
    public PlayerStats() : base("Player", "ignore this", 200, 0, -1, 1F, 0.1F, 1F, 1F, 4, new float[Elements.count])
    {
        for (int i = 0; i < Elements.count; i++)
        {
            vsElement[i] = 1.0F;
        }
        chatID = "player_1";
    }
    private string chatID;
    public override string ChatDatabaseID { get { return chatID; } }
}

//Contains Static referrence to global Player (Player.main)
public class Player : MonoBehaviour, ICaster
{
    //Main player character (Static Global Basically)
    public static Player main = null;

    public BattleKeyboard status_manager;

    void Awake()
    {
        if (main == null)
            main = this;
    }

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
    public Transform Transform { get { return transform; } }
    private int position = 1; //position in battle field
    public int Position { get { return position; } set { position = value; } }
    private int _target = 1;
    public int TargetPosition { get { return _target; } set { _target = value; } }
    PlayerStats stats;
    public CasterStats Stats { get { return stats; } }
    BuffDebuff buffDebuff = new BuffDebuff();
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
	int curr_hp = 200;
    bool is_dead = false;
	string last_cast = ""; // last casted spell

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

    //Restores player's HP and Shields to Maximum
    public void restoreToFull()
    {
        curr_hp = Stats.max_hp;
        is_dead = false;
    }

    //Damage player
    public void damage(CastData data, int baseDmg, int element, ICaster caster, bool crit, bool reflect = false)
    {
        //Apply repel and return if applicable
        if (CasterOps.calcRepel(data, baseDmg, element, caster, this, crit, reflect))
            return;
        bool damaged = CasterOps.calcDamage(data, baseDmg, element, caster, this, crit);

        if (damaged && element != Elements.@null) {
            status_manager.inflictCondition(this, element, 0, data.damageInflicted);
		}
        if (Curr_hp <= 0)
        { // check if killed
            Debug.Log("Player" + " has been slain!");
            is_dead = true;
        }
       // return Is_dead;
    }
}
