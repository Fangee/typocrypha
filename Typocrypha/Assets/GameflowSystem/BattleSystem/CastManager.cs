using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastManager : MonoBehaviour
{

    public SpellDictionary spellDict;
    public CooldownList cooldown;
    public SpellBook spellBook;
    public SpellEffects spellEffects;
    public BattleUI uiManager;
    public ChatDatabase chat; //Database containing chat lines

    [HideInInspector] public BattleField Field { get { return field; } set { field = value; } }
    private BattleField field;

    //CASTING CODE//---------------------------------------------------------------------------------------------------------------------------------------//

    // attack currently targeted enemy with spell
    public bool attackCurrent(string spell, TrackTyping callback)
    {
        SpellData s;
        string message = "";
        //Send spell, Enemy state, and target index to parser and caster
        CastStatus status = spellDict.parse(spell.ToLower(), out s);
        Pair<bool[], bool[]> targetPattern = null;
        callback.clearBuffer();
        switch (status)
        {
            case CastStatus.SUCCESS:
                ++field.num_player_attacks;
                targetPattern = spellDict.getTargetPattern(s, field.enemy_arr, field.target_ind, field.player_arr, field.player_ind);
                message = chat.getLine(field.Player.Stats.ChatDatabaseID);
                preCastEffects(targetPattern, field.Player, s, message);
                AudioPlayer.main.playSFX("sfx_enter");
				AudioPlayer.main.playSFX ("sfx_player_cast");
				AudioPlayer.main.playSFX ("sfx_cast", 0.35f);
				AnimationPlayer.main.playAnimation("anim_spell_empower", field.Player.Transform.position, 2f);
                StartCoroutine(pauseAttackCurrent(s, field.Player));
                return true; //Clear the casting buffer
            case CastStatus.BOTCH:
                //diplay.playBotchEffects
                spellEffects.popp.spawnSprite("popups_invalid", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return true; //Clear the casting buffer
            case CastStatus.FIZZLE:
                //diplay.playBotchEffects
                spellEffects.popp.spawnSprite("popups_invalid", 1.0F, field.Player.Transform.transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return true; //Clear the casting buffer
            case CastStatus.ONCOOLDOWN:
                //display.playOnCooldownEffects
                spellEffects.popp.spawnSprite("popups_oncooldown", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return false;
            case CastStatus.COOLDOWNFULL:
                //diplay.playCooldownFullEffects
                spellEffects.popp.spawnSprite("popups_cooldownfull", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return false;
            case CastStatus.ALLYSPELL:
                int allyPos = getAllyPosition(s.root);
                if (allyPos == -1 || field.player_arr[allyPos].Is_stunned || !((Ally)field.player_arr[allyPos]).tryCast())//display.playAllyNotHereEffects
                    break;
                targetPattern = spellDict.getTargetPattern(s, field.enemy_arr, field.target_ind, field.player_arr, allyPos);
                message = chat.getLine(field.player_arr[allyPos].Stats.ChatDatabaseID);
                preCastEffects(targetPattern, field.player_arr[allyPos], s, message);
                StartCoroutine(pauseAttackCurrent(s, field.player_arr[allyPos]));
                return true; //Clear the casting buffer
            default:
                return false;
        }
        return false;
    }

    IEnumerator pauseAttackCurrent(SpellData s, ICaster caster)
    {
        field.Pause = true;

		uiManager.setEnabledGauges (false);

        //BEGIN pause//

        yield return new WaitForSeconds(1f);

        //CASTING//
        startCooldown(s, field.Player);
        List<CastData> data;
        List<Transform> noTargetPositions;
        data = spellDict.cast(s, field.enemy_arr, field.target_ind, field.player_arr, caster.Position, out noTargetPositions);
        processCast(data, s, noTargetPositions, BattleField.FieldPosition.PLAYER);

        yield return new WaitForSeconds(1.1f);

        //END pause//

		uiManager.setEnabledGauges (true);

        postCastEffects();
        field.lastCaster = BattleField.FieldPosition.PLAYER;
        //Updates field.Pause if necessary
        field.update();
    }

    //Casts from an enemy position: calls processCast on results
    public void enemyCast(SpellDictionary dict, SpellData s, int position, int target)
    {
        field.breakThirdEye();
        field.Pause = true; // parent.pause battle for attack
        AudioPlayer.main.playSFX("sfx_enemy_cast");
		AnimationPlayer.main.playAnimation("anim_spell_empower", field.enemy_arr[position].Transform.position, 2f);
        Pair<bool[], bool[]> targetPattern = spellDict.getTargetPattern(s, field.player_arr, target, field.enemy_arr, position);
        preCastEffects(targetPattern, field.enemy_arr[position], s, chat.getLine(field.enemy_arr[position].Stats.ChatDatabaseID));
		BattleEffects.main.setDim(true, field.enemy_arr[position].enemy_sprite);
        StartCoroutine(enemy_pause_cast(dict, s, position, target));
    }

    private IEnumerator enemy_pause_cast(SpellDictionary dict, SpellData s, int position, int target)
    {
        uiManager.setEnabledGauges (false);

		BattleEffects.main.setDim(true, field.enemy_arr[position].enemy_sprite);

        yield return new WaitForSeconds(1f);

        field.enemy_arr[position].startSwell();
        List<Transform> noTargetPositions;
        List<CastData> data = dict.cast(s, field.player_arr, target, field.enemy_arr, position, out noTargetPositions);
        processCast(data, s, noTargetPositions, (BattleField.FieldPosition)position);

        yield return new WaitForSeconds(1f);

		uiManager.setEnabledGauges (true);

        postCastEffects();

        field.enemy_arr[position].attack_in_progress = false;
        field.lastCaster = (BattleField.FieldPosition)position;
        field.update();
    }

    //Method for processing CastData (most effects now happen in SpellEffects.cs)
    //Called by Cast in the SUCCESS CastStatus case, possibly on BOTCH in the future
    private void processCast(List<CastData> data, SpellData s, List<Transform> noTargetPositions, BattleField.FieldPosition casterPos)
    {
        if (casterPos == BattleField.FieldPosition.PLAYER)
        {
            field.last_player_cast = data;
            field.last_player_spell = s;
        }
        else if (BattleField.isEnemy(casterPos))
        {
            field.last_enemy_cast = data;
            field.last_enemy_spell = s;
        }
		uiManager.battle_log.stop ();
        float delay = 0;
        foreach(Transform t in noTargetPositions)
        {
            spellEffects.StartCoroutine(spellEffects.noTargetEffects(t, delay));
            //delay += 0.1f;
        }
        if (noTargetPositions.Count > 0 && data.Count == 0)
            AudioPlayer.main.playSFX("sfx_miss");
        //Process the data here
        foreach (CastData d in data)
        {
            spellEffects.StartCoroutine(spellEffects.playEffects(d, s, delay));
            delay += 0.1f;
            if (!d.isHit)
                continue;
            //Learn intel if applicable
            if (d.Target.CasterType == ICasterType.ENEMY)
                EnemyIntel.main.learnIntel(d.Target.Stats.name, d.element);
            else if (d.Caster.CasterType == ICasterType.ENEMY && d.Target.CasterType == ICasterType.PLAYER && d.repel)
                EnemyIntel.main.learnIntel(d.Caster.Stats.name, d.element);
        }
        //Register unregistered keywords here
        bool[] regData = spellDict.safeRegister(spellBook, s);
        if (regData[0] || regData[1] || regData[2])
            StartCoroutine(learnSFX());
        field.last_register = regData;
        //Process regData (for register graphics) here. 
        //format is bool [3], where regData[0] is true if s.element is new, regData[1] is true if s.root is new, and regData[2] is true if s.style is new
    }
    private IEnumerator learnSFX()
    {
        yield return new WaitWhile(() => field.Pause);
        AudioPlayer.main.playSFX("sfx_learn_spell_battle");
    }
    //Effects that happen before any actor casts
    private void preCastEffects(Pair<bool[], bool[]> targetPattern, ICaster caster, SpellData cast, string message)
    {
        BattleEffects.main.setDim(true);
		uiManager.battle_log.log(cast, caster.CasterType, message, caster.Stats.name, caster.Transform.position);
        if (targetPattern != null)
        {
            if (caster.CasterType == ICasterType.ENEMY)
                raiseTargets(targetPattern.second, targetPattern.first);
            else
                raiseTargets(targetPattern.first, targetPattern.second);
        }
        uiManager.target_ret.SetActive(false); // disable / make target reticule disappear on a cast
    }

    //effects that hafter after any actor casts
    private void postCastEffects()
    {
        //uiManager.battle_log.stop();
        for (int i = 0; i < 3; ++i)
        {
            if (field.enemy_arr[i] != null)
                field.enemy_arr[i].enemy_sprite.sortingOrder = BattleEffects.dim_layer;
        }
        uiManager.target_ret.SetActive(true); // enable / make target reticule appear after a cast
        BattleEffects.main.setDim(false);
    }

    //Raises the targets (array val = true) above the dimmer level
    private void raiseTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
                field.enemy_arr[i].enemy_sprite.sortingOrder = BattleEffects.undim_layer;
        }
    }
    //Lowers the targets (array val = true) below the dimmer level
    private void lowerTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
                field.enemy_arr[i].enemy_sprite.sortingOrder = BattleEffects.dim_layer;
        }
    }

    //returns the position of ally with specified name (if in battle)
    private int getAllyPosition(string name)
    {
        if (field.player_arr[0] != null && field.player_arr[0].Stats.name.ToLower() == name.ToLower())
            return 0;
        if (field.player_arr[2] != null && field.player_arr[2].Stats.name.ToLower() == name.ToLower())
            return 2;
        return -1;
    }

    //Starts cooldown of spell
    private void startCooldown(SpellData data, ICaster castingPlayer)
    {
        float cooldownTime = spellDict.getCastingTime(data, castingPlayer.Stats.speed);
        spellDict.getSpell(data).startCooldown(cooldown, data.root, cooldownTime);
    }

    //Utility

    //Returns valid spelldata if the string is a valid spell, or null if not
    public SpellData isValidSpell(string spell)
    {
        SpellData s = null;
        CastStatus status = spellDict.parse(spell, out s);
        if (status == CastStatus.SUCCESS || status == CastStatus.ONCOOLDOWN || status == CastStatus.COOLDOWNFULL)
            return s;
        return null;
    }
    //Play a spell animation without really doing anything
    public IEnumerator playSpellEffects(SpellData s, CastData dummyValues)
    {
        dummyValues.animData = spellDict.getAnimData(s);
        dummyValues.sfxData = spellDict.getSfxData(s);
        dummyValues.element = Elements.fromString(s.element);
        dummyValues.damageInflicted = spellDict.getSpell(s).power * 2;
        yield return StartCoroutine(spellEffects.playEffects(dummyValues, s));
    }

    //SpellBook management

    //Moves page up in member spellBook
    public bool pageUp()
    {
        return spellBook.previousPage();
    }
    //Moves page down in member spellBook
    public bool pageDown()
    {
        return spellBook.nextPage();
    }
}
