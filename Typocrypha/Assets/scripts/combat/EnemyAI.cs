using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class For enemy AI.
//Enemy AI has two abstract methods: getNextSpell(), and updateState()
//getNextSpell() returns the next spell for the enemy to cast and sets the target in out int target
//updateState updates the state component of the AI module
//AI modules can be generic (with optional parameters in constructor) or unique (for bosses, etc)
public abstract class EnemyAI
{
    //All possible AI states
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
    //Current state of this (with associated property)
    protected AI_State state = AI_State.NORMAL;
    public AI_State State { get { return state; } set { state = value; } }
    //Returns the next spell for the enemy to cast and sets the target in out int target
    public abstract SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target);
    //Updates the state of this AI module based on field data
    public abstract void updateState(Enemy[] allies, int position, ICaster[] player_arr);

    //Public static methods

    //Returns an appropriate EnemyAI derivitive with specified parameters from ID string (MAYBE IMPROVE)
    public static EnemyAI GetAIFromString(string key, string[] parameters = null)
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

    //Helper static methods (protected)

    //Gets the current spell in the array, and increments the referenced index (current), and sets the index to 0 if the end is reached
    protected static SpellData getSpellSequential(SpellData[] spells, ref int current)
    {
        SpellData ret = spells[current];
        current++;
        if (current >= spells.Length)
            current = 0;
        return ret;
    }
    //Throw when code tries to access a non-existant spell group
    protected class SpellGroupException : System.Exception
    {
        public SpellGroupException(string message) : base(message)
        {

        }
    }
}
//Enemy AI for basic attackers (might be legacy)
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
//Enemy AI that switches to a new spell group when health is low and/or critical
//Uses AttackerAI1 as a backend
public class HealthLowAI1 : EnemyAI
{
    protected AttackerAI1 nrmlAtk = new AttackerAI1();
    int currentSpell = 0;
    int lowThresh = 50;
    int criticalThresh = 10;
    public HealthLowAI1(string[] parameters = null)
    {
        if(parameters != null)
        {
            int.TryParse(parameters[0], out lowThresh);
            if(parameters.Length > 1)
                int.TryParse(parameters[1], out criticalThresh);
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


