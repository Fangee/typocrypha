using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI
{
    public enum AI_State
    {
        INVALID = -1,
        NORMAL,
        STAGE_1,
        STAGE_2,
        STAGE_3,
        HEALTH_LOW,
        HEALTH_CRITICAL,
    }
    protected AI_State state = AI_State.NORMAL;
    public AI_State State { get { return state; } set { state = value; } }
    public abstract SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target);
    public abstract void updateState(Enemy[] allies, int position, ICaster[] player_arr);

    //Static methods

    public static EnemyAI GetAIFromString(string key, int[] parameters = null)
    {
        switch (key)
        {
            case "Attacker1":
                return new AttackerAI1();
            case "HealthLow1":
                return new HealthLowAI1(parameters);
            default:
                throw new System.NotImplementedException(key + " is not an AI type!");
        }
    }
    protected static SpellData getSpellSequential(SpellData[] spells, ref int current)
    {
        SpellData ret = spells[current];
        current++;
        if (current >= spells.Length)
            current = 0;
        return ret;
    }

    protected class SpellGroupException : System.Exception
    {
        public SpellGroupException(string message) : base(message)
        {

        }
    }
}

public class AttackerAI1 : EnemyAI
{
    int currentSpell = 0;
    public override SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target)
    {
        target = 1;
        return getSpellSequential(spells.getSpells("DEFAULT"), ref currentSpell);
    }

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr) { return; }
}

public class HealthLowAI1 : EnemyAI
{
    protected AttackerAI1 nrmlAtk = new AttackerAI1();
    int currentSpell = 0;
    int lowThresh = 50;
    int criticalThresh = 10;
    public HealthLowAI1(int[] parameters = null)
    {
        if(parameters != null)
        {
            lowThresh = parameters[0];
            criticalThresh = parameters[1];
        }
    }

    public override SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target)
    {
        target = 1;
        if (state == AI_State.NORMAL)
            return nrmlAtk.getNextSpell(spells, allies, position, player_arr, out target);
        else if (state == AI_State.HEALTH_LOW)
            return getSpellSequential(spells.getSpells("HEALTH_LOW"), ref currentSpell);
        else if (state == AI_State.HEALTH_CRITICAL)
        {
            if (spells.hasGroup("HEALTH_CRITICAL"))
                return getSpellSequential(spells.getSpells("HEALTH_CRITICAL"), ref currentSpell);
            return getSpellSequential(spells.getSpells("HEALTH_LOW"), ref currentSpell);
        }
        else
            throw new SpellGroupException("Invalid group");

    }

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr)
    {
        Enemy self = allies[position];
        int curr_hp = Mathf.CeilToInt(((float)self.Curr_hp / self.Stats.max_hp) * 100);
        if (curr_hp < criticalThresh)
            state = AI_State.HEALTH_CRITICAL;
        else if (curr_hp < lowThresh)
            state = AI_State.HEALTH_LOW;
        else
            state = AI_State.NORMAL;
    }
}


