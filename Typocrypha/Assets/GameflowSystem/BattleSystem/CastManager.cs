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

    [HideInInspector] public Battlefield Field { get { return field; } set { field = value; } }
    private Battlefield field;

    //CASTING CODE//---------------------------------------------------------------------------------------------------------------------------------------//

    //Pre: spell.isValid() = true
    public List<CastData> cast(SpellData spell, Battlefield field, ICaster caster, out List<Transform> noTargetPositions)
    {
        noTargetPositions = new List<Transform>();
        Spell s = spellDict.getRoot(spell.root);
        Spell c = Spell.createSpellFromType(s.type);
        s.copyInto(c);
        int wordCount = 1;
        string[] animData = { null, s.animationID, null };
        string[] sfxData = { null, s.sfxID, null };
        ElementMod e = null;
        StyleMod st = null;
        if (!string.IsNullOrEmpty(spell.element))
        {
            e = spellDict.getElementMod(spell.element);
            sfxData[2] = e.sfxID;
            animData[2] = e.animationID;
            ++wordCount;
        }
        if (!string.IsNullOrEmpty(spell.style))
        {
            st = spellDict.getStyleMod(spell.style);
            sfxData[0] = st.sfxID;
            animData[0] = st.animationID;
            ++wordCount;
        }
        c.Modify(e, st);
        List<ICaster> toCastAt = c.target(field, caster);
        List<CastData> data = new List<CastData>();
        foreach (ICaster target in toCastAt)
        {
            if (target == null)
                continue;
            if (target.Is_dead)
            {
                noTargetPositions.Add(target.Transform);
                continue;
            }
            CastData castData = c.cast(target, caster);
            animData.CopyTo(castData.animData, 0);
            sfxData.CopyTo(castData.sfxData, 0);
            castData.wordCount = wordCount;
            if (castData.repel == true)
                castData.setLocationData(caster, target);
            else
                castData.setLocationData(target, caster);
            data.Add(castData);
        }
        return data;
    }


    // attack currently targeted enemy with spell
    public bool attackCurrent(string spell, TrackTyping callback)
    {
        SpellData s;
        string message = "";
        //Send spell, Enemy state, and target index to parser and caster
        CastParser.ParseResults status = CastParser.parse(spell.ToLower().Split(' '), spellDict, out s);
        Pair<bool[], bool[]> targetPattern = null;
        callback.clearBuffer();
        if(status == CastParser.ParseResults.Valid)
        {
            if (cooldown.isFull())
            {
                //diplay.playCooldownFullEffects
                spellEffects.popp.spawnSprite("popups_cooldownfull", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return false;
            }
            else if (cooldown.isOnCooldown(s))
            {
                //display.playOnCooldownEffects
                spellEffects.popp.spawnSprite("popups_oncooldown", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
                AudioPlayer.main.playSFX("sfx_enter_bad");
                return false;
            }
            targetPattern = spellDict.getTargetPattern(s, field, field.Player);
            message = chat.getLine(field.Player.Stats.ChatDatabaseID);
            preCastEffects(targetPattern, field.Player, s, message);
            AudioPlayer.main.playSFX("sfx_enter");
            AudioPlayer.main.playSFX("sfx_player_cast");
            AudioPlayer.main.playSFX("sfx_cast", 0.35f);
            AnimationPlayer.main.playAnimation("anim_spell_empower", field.Player.Transform.position, 2f);
            StartCoroutine(pauseAttackCurrent(s, field.Player));
            return true; //Clear the casting buffer
        }
        //diplay.playBotchEffects
        spellEffects.popp.spawnSprite("popups_invalid", 1.0F, field.Player.Transform.position - new Vector3(0, 0.375f, 0));
        AudioPlayer.main.playSFX("sfx_enter_bad");
        return true; //Clear the casting buffer
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
        data = cast(s, field, caster, out noTargetPositions);
        processCast(data, s, noTargetPositions);

        yield return new WaitForSeconds(1.1f);

        //END pause//

		uiManager.setEnabledGauges (true);

        postCastEffects();
        field.lastCaster = null;
        //Updates field.Pause if necessary
        field.update();
    }

    //Casts from an enemy position: calls processCast on results
    public void enemyCast(SpellDictionary dict, SpellData s, Enemy enemy)
    {
        field.breakThirdEye();
        field.Pause = true; // parent.pause battle for attack
        AudioPlayer.main.playSFX("sfx_enemy_cast");
		AnimationPlayer.main.playAnimation("anim_spell_empower", enemy.Transform.position, 2f);
        Pair<bool[], bool[]> targetPattern = spellDict.getTargetPattern(s, field, enemy);
        preCastEffects(targetPattern, enemy, s, chat.getLine(enemy.Stats.ChatDatabaseID));
		BattleEffects.main.setDim(true, enemy.enemy_sprite);
        StartCoroutine(enemy_pause_cast(dict, s, enemy));
    }

    private IEnumerator enemy_pause_cast(SpellDictionary dict, SpellData s, Enemy enemy)
    {
        uiManager.setEnabledGauges (false);

		BattleEffects.main.setDim(true, enemy.enemy_sprite);

        yield return new WaitForSeconds(1f);

        enemy.startSwell();
        List<Transform> noTargetPositions;
        List<CastData> data = cast(s, field, enemy, out noTargetPositions);
        processCast(data, s, noTargetPositions);

        yield return new WaitForSeconds(1f);

		uiManager.setEnabledGauges (true);

        postCastEffects();

        enemy.attack_in_progress = false;
        field.lastCaster = enemy;
        field.update();
    }

    //Method for processing CastData (most effects now happen in SpellEffects.cs)
    //Called by Cast in the SUCCESS CastStatus case, possibly on BOTCH in the future
    private void processCast(List<CastData> data, SpellData s, List<Transform> noTargetPositions)
    {
        field.lastCast = data;
        field.lastSpell = s;
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
        bool[] regData = spellBook.safeRegister(spellDict, s);
        if (regData[0] || regData[1] || regData[2])
            StartCoroutine(learnSFX());
        field.lastRegister = regData;
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
            if (field.enemies[i] != null)
                field.enemies[i].enemy_sprite.sortingOrder = BattleEffects.dim_layer;
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
                field.enemies[i].enemy_sprite.sortingOrder = BattleEffects.undim_layer;
        }
    }
    //Lowers the targets (array val = true) below the dimmer level
    private void lowerTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
                field.enemies[i].enemy_sprite.sortingOrder = BattleEffects.dim_layer;
        }
    }

    //returns the position of ally with specified name (if in battle)
    private int getAllyPosition(string name)
    {
        if (field.allies[0] != null && field.allies[0].Stats.name.ToLower() == name.ToLower())
            return 0;
        if (field.allies[2] != null && field.allies[2].Stats.name.ToLower() == name.ToLower())
            return 2;
        return -1;
    }

    //Starts cooldown of spell
    private void startCooldown(SpellData data, ICaster castingPlayer)
    {
        float cooldownTime = data.getCastingTime(spellDict, castingPlayer.Stats.speed);
        cooldown.add(data, cooldownTime);
    }

    //Utility

    //Play a spell animation without really doing anything
    public IEnumerator playSpellEffects(SpellData s, CastData dummyValues)
    {
        dummyValues.animData = spellDict.getAnimData(s);
        dummyValues.sfxData = spellDict.getSfxData(s);
        dummyValues.element = Elements.fromString(s.element);
        dummyValues.damageInflicted = spellDict.getRoot(s).power * 2;
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
