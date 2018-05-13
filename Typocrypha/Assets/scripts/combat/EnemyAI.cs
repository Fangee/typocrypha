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
        CHARGE_MAX,
        HEALTH_LOW,
        HEALTH_CRITICAL,
        ATTACK_SPECIAL,
    }
    //Condition in which AI is being updated
    public enum Update_Case
    {
        AFTER_CAST,
        UNSTUN,
        WAS_HIT,
    }
    //Current state of this (with associated property)
    protected AI_State state = AI_State.NORMAL;
    public AI_State State { get { return state; } set { state = value; } }
    //Returns the next spell for the enemy to cast and sets the target in out int target
    public abstract SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target);
    //Updates the state of this AI module based on field data
    public abstract void updateState(Enemy[] allies, int position, ICaster[] player_arr, Update_Case flag);

    //Public static methods

    //Returns an appropriate EnemyAI derivitive with specified parameters from ID string (MAYBE IMPROVE)
    public static EnemyAI GetAIFromString(string key, string[] parameters = null, Enemy self = null)
    {
        switch (key)
        {
            case "Attacker1":
                return new AttackerAI1();
            case "HealthLow1":
                return new HealthLowAI1(parameters);
            case "FormChange1":
                return new FormChangeAI1(parameters);
            case "Doppleganger1":
                return new DopplegangerAI1(self);
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
    //Gets a random spell from the array (non-weighted)
    protected static SpellData getSpellRandom(SpellData[] spells)
    {
        var randomIndex = Random.Range(0, spells.Length);
        return spells[randomIndex];
    }
    //Throw when code tries to access a non-existant spell group
    protected class SpellGroupException : System.Exception
    {
        public SpellGroupException(string message) : base(message) { }
    }
    protected class AiStateException : System.Exception
    {
        public AiStateException(string message) : base(message) { }
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

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr, Update_Case flag) { return; }
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

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr, Update_Case flag)
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
//Enemy AI that switches to another enemy when health is below a certain level
public class FormChangeAI1 : EnemyAI
{
    protected EnemyAI baseAI;
    protected string nextForm;
    bool resetHealthandStagger;
    bool resetAttack = true;
    bool hasChanged = false;
    //Params: the class name of the base AI to use (string), the name of the enemydatabase entry to change to (string), 
    //and whether or not to reset health on change (bool)
    public FormChangeAI1(string[] parameters)
    {
        baseAI = GetAIFromString(parameters[0]);
        nextForm = parameters[1];
        bool.TryParse(parameters[2], out resetHealthandStagger);
        if (parameters.Length >= 4)
            bool.TryParse(parameters[3], out resetAttack);
    }
    public override SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target)
    {
        return baseAI.getNextSpell(spells, allies, position, player_arr, out target);
    }

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr, Update_Case flag)
    {
        baseAI.updateState(allies, position, player_arr, flag);
        if(!hasChanged && flag == Update_Case.WAS_HIT)
        {
            allies[position].setStats(EnemyDatabase.main.getData(nextForm), resetHealthandStagger);
            if(resetAttack)
                allies[position].resetAttack();
            allies[position].changeForm();
            hasChanged = true;
        }
    }
}
//Doppleganger Unique AI
public class DopplegangerAI1 : EnemyAI
{
    int numAttacks = 0;
    int form = 1;
    string color = "NONE";
    readonly string[] colors = { "RED", "BLUE", "YELLOW" };
    public DopplegangerAI1(Enemy self)
    {
        state = AI_State.NORMAL;
        self.stagger_time = 2;
    }
    public override SpellData getNextSpell(EnemySpellList spells, Enemy[] allies, int position, ICaster[] player_arr, out int target)
    {
        target = 1;
        if (state == AI_State.NORMAL)
            return getSpellRandom(spells.getSpells());
        else if (state == AI_State.CHARGE_MAX)
            return spells.getSpells("TOO_LONG")[0];
        else if (state == AI_State.ATTACK_SPECIAL)
        {
            return spells.getSpells()[0];
        }
        else
            throw new AiStateException("invalid AI state in Doppelganger AI");
    }

    public override void updateState(Enemy[] allies, int position, ICaster[] player_arr, Update_Case flag)
    {
        Debug.Log("updating doppel AI state: attack in prog = " + allies[position].attack_in_progress + " " + flag.ToString());
        if(form <= 3)
        {
            if (flag == Update_Case.AFTER_CAST)
            {
                numAttacks++;
                if (numAttacks > 3)
                    state = AI_State.CHARGE_MAX;
                else
                    state = AI_State.NORMAL;
            }
            else if(flag == Update_Case.UNSTUN)
            {
                if(form == 1)
                {
                    state = AI_State.NORMAL;
                    allies[position].setStats(EnemyDatabase.main.getData("Doppelganger (YELLOW)"), true);
                    allies[position].stagger_time = 2f;
                    allies[position].changeForm();
                    allies[position].resetAttack();
                }
                else if(form == 2)
                {
                    state = AI_State.NORMAL;
                    allies[position].setStats(EnemyDatabase.main.getData("Doppelganger (RED)"), true);
                    allies[position].stagger_time = 2f;
                    allies[position].changeForm();
                    allies[position].resetAttack();
                }
                else if(form == 3)
                {
                    state = AI_State.ATTACK_SPECIAL;
                    allies[position].setStats(EnemyDatabase.main.getData("Doppelganger (???)"), true);
                    allies[position].stagger_time = 2f;
                    changeToRandomColor(allies[position]);
                    allies[position].resetAttack();
                }
                ++form;
            }
        }
        else if(form == 4)
        {
            if (flag == Update_Case.UNSTUN)
            {
                state = AI_State.NORMAL;
                allies[position].setStats(EnemyDatabase.main.getData("Doppelganger (GRAY)"), true);
                allies[position].stagger_time = 10f;
                allies[position].resetAttack();
                ++form;
            }
            else if (flag == Update_Case.AFTER_CAST)
            {
                changeToRandomColor(allies[position]);
            }
            else
            {
                changeToRandomColor(allies[position]);
                switch (color)
                {
                    case "RED":
                        allies[position].getCurrSpell().element = "agni";
                        break;
                    case "BLUE":
                        allies[position].getCurrSpell().element = "cryo";
                        break;
                    case "YELLOW":
                        allies[position].getCurrSpell().element = "veld";
                        break;
                    default:
                        throw new System.Exception("Doppleganger is invalid color");
                }
            }
        }
        else if(form == 5)
        {
            //if(flag == Update_Case.UNSTUN)
            //{
            //    allies[position].Curr_hp = 0;
            //}
        }
    }

    private void changeToRandomColor(Enemy self)
    {
        var randomIndex = Random.Range(0, colors.Length);
        while(colors[randomIndex] == color)
            randomIndex = Random.Range(0, colors.Length);
        color = colors[randomIndex];
        EnemyStats formStats = EnemyDatabase.main.getData("Doppelganger (" + color + ")");
        ((EnemyStats)self.Stats).sprite_path = formStats.sprite_path;
        self.Stats.vsElement = formStats.vsElement;
        self.Stats.vsElement[0] = 0;
        ((EnemyStats)self.Stats).spells = formStats.spells;
        self.changeForm();
    }
}



