using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffects : MonoBehaviour {
    public Popper popp; //holds popper script component
	public EnemyHealthBars enemy_hp_bars; // holds enemy HP bars component
	public Animator caster_cutin_animator; // animates player cut-in

    #region Animations
    public AnimationClip reflectAnim;
    public AnimationClip drainAnim;
    public AnimationClip blockAnim;
    #endregion

    #region UI Offset Constants
    const float POP_TIMER = 1.5f; //pop-ups last this many seconds long
    Vector3 DMGNUM_OFFSET = new Vector3(0, 0.375f, 0); //where the damage number should be
    Vector3 UNDER_OFFSET = new Vector3(0, -0.75f, 0); //where something under the damage num should be
	Vector3 UNDER_OFFSET_2 = new Vector3(1.5f, -1.5f, 0); //where something under the damage num should be
    Vector3 OVER_OFFSET = new Vector3(0, 1.5f, 0); //where something over the damage num should be
    Vector3 ELEM_OFFSET = new Vector3(-1.65f, 1.6f, 0);
	Vector3 BACK_OFFSET = new Vector3(-1.65f, 1.4f, 0);
	Vector3 TEXT_OFFSET = new Vector3(0.0f, -0.25f, 0);
    #endregion

    const float shakeIntensityMod = 0.1f; //The amount to scale each shake by (per keyword)

    public IEnumerator playEffects(CastResults d, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
		//caster_cutin_animator.Play ("anim_wave_banner_image");
        if (d.isHit == false)//Spell misses
        {
            Debug.Log(d.caster.Name + " missed " + d.target.Name + "!");
            //Process miss graphics
            popp.spawnSprite("popup_miss", POP_TIMER, d.target.WorldPos + UNDER_OFFSET);
            AudioPlayer.main.playSFX("sfx_spell_miss");
            //BattleEffects.main.spriteShift(d.target.WorldPos, 0.3f, 0.1f); // sprite moves to the right as a dodge
            yield break;
        }

		//Process repel
		if (d.repel)
		{
			//spawnElementPopup(d.element, ReactionType.REPEL, d.caster.WorldPos);
            yield return new WaitUntilAnimationComplete(AnimationPlayer.main.playAnimation(reflectAnim, d.caster.WorldPos, 2f));
			
		}
        float shakeIntensity = 0;
        //Process hit graphics (if not a repelled attack)
        foreach (var data in d.animationData)
        {
            AudioPlayer.main.setSFX(AudioPlayer.channel_spell_sfx, data.sfx);
            AudioPlayer.main.playSFX(AudioPlayer.channel_spell_sfx);
            if (d.target.CasterType == ICasterType.PLAYER)
                BattleEffects.main.screenShake(0.15f + shakeIntensity / 8, 0.05f + shakeIntensity);
            shakeIntensity += shakeIntensityMod;
            if (d.repel)
                popp.spawnText(d.wordName.ToUpper() + "!", POP_TIMER - 1, d.caster.WorldPos, Color.black, new Color(1, 111f / 255f, 1));
            else
                popp.spawnText(d.wordName.ToUpper() + "!", POP_TIMER - 1, d.caster.WorldPos, new Color(1, 111f / 255f, 1), Color.white);
            yield return new WaitUntilAnimationComplete(AnimationPlayer.main.playAnimation(data.animation, d.target.WorldPos, 1));
        }
        if (d.isCrit && d.reaction != ReactionType.BLOCK) {//Spell is crit
			if (d.target.CasterType == ICasterType.ENEMY)
            {
				AudioPlayer.main.playSFX ("sfx_enemy_weakcrit_dmg");
				if (!d.isStun && !d.target.Stunned) popp.spawnText ("<size=48>-1<size=24>SHIELD</size></size>", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.cyan, Color.white);
				if (d.isStun) popp.spawnText ("BREAK!", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.cyan, Color.white);
				if (d.target.Stunned) popp.spawnText ("BONUS DMG!", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.red, Color.white);
			}
			else if (d.target.CasterType == ICasterType.PLAYER || d.target.CasterType == ICasterType.ALLY)
				AudioPlayer.main.playSFX ("sfx_party_weakcrit_dmg");
		}
        else if ((d.reaction == ReactionType.WEAK) && d.damageInflicted > 0)
        {
			if (d.target.CasterType == ICasterType.ENEMY) {
				AudioPlayer.main.playSFX ("sfx_enemy_weakcrit_dmg");
				if (!d.isStun && !d.target.Stunned) popp.spawnText ("<size=48>-1<size=24>SHIELD</size></size>", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.cyan, Color.white);
				if (d.isStun) popp.spawnText ("BREAK!", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.cyan, Color.white);

			}
			else if (d.target.CasterType == ICasterType.PLAYER || d.target.CasterType == ICasterType.ALLY)
				AudioPlayer.main.playSFX ("sfx_party_weakcrit_dmg");
		}
        if (d.isStun)
        {
            //Process stun graphics
            Debug.Log(d.caster.Name + " stuns " + d.target.Name);
            AudioPlayer.main.playSFX("sfx_stagger");
        }

        //Process elemental wk/resist/drain/repel graphics
		//if (!((d.reaction == ReactionType.WEAK || d.reaction == ReactionType.SUPERWEAK) && d.damageInflicted <= 0)) {
		//	spawnElementPopup(d.element, d.reaction, d.target.WorldPos);
		//}

		//Play block/reflect/drain animations if necessary
		if (d.reaction == ReactionType.BLOCK)
            yield return new WaitUntilAnimationComplete(AnimationPlayer.main.playAnimation(blockAnim, d.target.WorldPos, 2f));
		else if (d.reaction == ReactionType.DRAIN)
            yield return new WaitUntilAnimationComplete(AnimationPlayer.main.playAnimation(drainAnim, d.target.WorldPos, 2f));
        //Spawn damage number and some other gubs
        spawnDamagePopup(d, shakeIntensity);
    }
    public IEnumerator notargetEffects(Vector3 pos, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        popp.spawnText("No target!", POP_TIMER, pos, Color.red, Color.white);
    }

    //Spawns elemental popup with proper icon
    private void spawnElementPopup(int element, ReactionType reaction, Vector3 pos)
    {
        switch (reaction)
        {
            case ReactionType.REPEL:
                popp.spawnSprite("popup_reflect", POP_TIMER, pos + OVER_OFFSET);
				popp.spawnText ("REPEL", POP_TIMER, pos + OVER_OFFSET + TEXT_OFFSET, Color.magenta, Color.white);
				AudioPlayer.main.playSFX ("sfx_spell_reflect");
                break;
            case ReactionType.DRAIN:
                popp.spawnSprite("popup_absorb", POP_TIMER, pos + OVER_OFFSET);
				popp.spawnText ("DRAIN", POP_TIMER, pos + OVER_OFFSET + TEXT_OFFSET, Color.green, Color.white);
				AudioPlayer.main.playSFX ("sfx_spell_drain");
                break;
            case ReactionType.BLOCK:
                popp.spawnSprite("popup_nullify", POP_TIMER, pos + OVER_OFFSET);
				popp.spawnText ("BLOCK", POP_TIMER, pos + OVER_OFFSET + TEXT_OFFSET, Color.gray, Color.white);
				AudioPlayer.main.playSFX ("sfx_spell_block");
                break;
            case ReactionType.RESIST:
                popp.spawnSprite("popup_resistant", POP_TIMER, pos + OVER_OFFSET);
				popp.spawnText ("<size=36>RESIST</size>", POP_TIMER, pos + OVER_OFFSET + TEXT_OFFSET, Color.yellow, Color.white);
				AudioPlayer.main.playSFX ("sfx_spell_resist");
                break;
            case ReactionType.WEAK:
				popp.spawnSprite("ui_elem_popup", POP_TIMER, pos + OVER_OFFSET);
				popp.spawnSprite("popup_weak_backing", POP_TIMER, pos + BACK_OFFSET);
				popp.spawnText ("<size=36>WEAK!</size>", POP_TIMER, pos + OVER_OFFSET + TEXT_OFFSET, new Color(255f/255f, 35f/255f, 25f/255f), new Color(255f/255f, 100f/255f, 85f/255f));
                break;
        }
        if (reaction != ReactionType.NEUTRAL)
        {
            switch (element)
            {
                case 0:
					popp.spawnSprite("popup_slash", POP_TIMER, pos + ELEM_OFFSET + TEXT_OFFSET);
                    break;
                case 1:
					popp.spawnSprite("popup_fire", POP_TIMER, pos + ELEM_OFFSET + TEXT_OFFSET);
                    break;
                case 2:
					popp.spawnSprite("popup_ice", POP_TIMER, pos + ELEM_OFFSET + TEXT_OFFSET);
                    break;
                case 3:
					popp.spawnSprite("popup_bolt", POP_TIMER, pos + ELEM_OFFSET + TEXT_OFFSET);
                    break;
            }
        }
    }
    //Spawns damage popup with lots of complicated stuff (clean up)
    private void spawnDamagePopup(CastResults d, float shakeIntensity)
    {
        //Process damage graphics
        if (d.damageInflicted > 0)
        {
            //BattleEffects.main.spriteShake(d.target.Transform, 0.3f, 0.1f);
            AudioPlayer.main.playSFX("sfx_spell_hit");
            Color dmgNumColor = Color.white;
            Color dmgNumColorTop = new Color(255f / 255f, 100f / 255f, 220f / 255f);
            Color dmgNumColorBottom = Color.white;
            if (d.target.CasterType == ICasterType.PLAYER)
            {
                if (d.repel)
                    dmgNumColor = new Color(255, 0, 255);//new Color(220, 86, 249);
                BattleEffects.main.screenShake(0.15f + shakeIntensity / 2, shakeIntensity + 0.3f);
                spawnDamageOverlay(d.tags);
            }
            else if (d.target.CasterType == ICasterType.ENEMY)
            {
                // Gradually lower enemy HP gauge displays
                //BattleManagerS.main.uiManager.setEnabledGauges(true);
                //StartCoroutine(enemy_hp_bars.gradualUpdateDamage(d.target.Position, d.damageInflicted));
                //if (d.isStun)
                //{
                //    Enemy enemyObj = (Enemy)d.target;
                //    ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
                //    emitOverride.startLifetime = 10f;
                //    enemyObj.enemy_particle_sys.Emit(emitOverride, 10);
                //}
                if (!d.isStun && d.target.Stunned) popp.spawnText("<size=24>DAMAGE BONUS!</size>", POP_TIMER, d.target.WorldPos + UNDER_OFFSET_2, Color.red, Color.white);
            }
            // Set damage text size based on amount of damage ratios
            string sizeTagOpen = "<size=";
            string sizeTagClose = "</size>";
            int sizeValueMin = 36;
            int sizeValueMax = 72;
            int sizeValueDiff = sizeValueMax - sizeValueMin;
            float sizeRatio = (float)d.damageInflicted / (float)d.target.Stats.maxHP;
            Debug.Log("damage ratio to max hp: " + sizeRatio);
            int sizeValueCurr = sizeValueMin + Mathf.RoundToInt(((float)sizeValueDiff) * sizeRatio);
            string damageText = sizeTagOpen + sizeValueCurr + ">" + d.damageInflicted.ToString() + sizeTagClose;
            popp.spawnText("-" + damageText + "<size=24>HP</size>", POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET, dmgNumColorBottom, dmgNumColorTop);
            if (sizeRatio > 0.5) AudioPlayer.main.playSFX("sfx_fire_hit");
            if ((d.target.Health <= 0) && (d.damageInflicted > (d.target.Stats.maxHP)))
            {
                Vector3 ko_offset = new Vector3(0.5f, -0.5f, 0);
                popp.spawnText("<size=28>OVERKILL!</size>", POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET + ko_offset, dmgNumColorTop, dmgNumColorBottom);
            }
            else if (d.target.Health <= 0)
            {
                Vector3 ko_offset = new Vector3(0.5f, -0.5f, 0);
                popp.spawnText("<size=28>K.O.</size>", POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET + ko_offset, dmgNumColorTop, dmgNumColorBottom);
            }
            else if (d.isCrit && d.reaction != ReactionType.BLOCK)
            {
                Vector3 ko_offset = new Vector3(0.5f, -0.5f, 0);
                popp.spawnText("<size=28>CRITICAL!</size>", POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET + ko_offset, Color.yellow, Color.white);
            }

        }
        else if (d.damageInflicted < 0)
        {
            string heal = "+" + (-1 * (d.damageInflicted)).ToString() + "<size=32>HP</size>";
            AudioPlayer.main.playSFX("sfx_heal");
            popp.spawnText(heal, POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET, new Color(27f / 255f, 195f / 255f, 43f / 255f));
            if (d.target.CasterType == ICasterType.PLAYER)
            {
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage_heal");
            }
        }
        else
        {
            if (d.reaction == ReactionType.BLOCK)
            {
                popp.spawnText("NULL", POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET, Color.gray);
            }
            else
            {
                popp.spawnText(d.damageInflicted.ToString(), POP_TIMER, d.target.WorldPos + DMGNUM_OFFSET);
            }
        }
    }
    //Plays screen overlay
    private void spawnDamageOverlay(SpellTag.TagSet tags)
    {
        throw new System.Exception();
        int element = 0;
        switch (element)
        {
            case -1:
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage_heal");
                break;
            case 0:
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage");
                break;
            case 1:
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage_fire");
                break;
            case 2:
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage_ice");
                break;
            case 3:
                BattleEffects.main.flashDamageOverlay(1.0f, "anim_overlay_damage_volt");
                break;
        }
    }
}
