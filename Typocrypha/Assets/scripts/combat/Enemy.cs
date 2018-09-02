using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// simple container for enemy stats (Not a struct anymore cuz structs pass by value in c#)
public class EnemyStats : CasterStats {
    //Sorry for the massive constructor but all the vals are readonly so...
    public EnemyStats(string name, string chat, string sprite, int hp, int shield, int stag, float atk, float def, float speed, float acc, int evade, float[] vsElem, EnemySpellList sp, string ai_type, string[] ai_params)
        : base(name, chat, hp, shield, stag, atk, def, speed, acc, evade, vsElem)
    {
        sprite_path = sprite;
        this.ai_type = ai_type;
        this.ai_params = ai_params;
        spells = sp;
    }
    public string sprite_path; //path of sprite/resource to load at creation
    public string ai_type; //type of enemy AI (will be same as name if unique)
    public string[] ai_params;
    public EnemySpellList spells; // castable spells
    public EnemyStats clone()
    {
        return new EnemyStats(name, ChatDatabaseID, sprite_path, max_hp, max_shield, max_stagger, attack, defense, speed, accuracy, evasion, vsElement, spells, ai_type, ai_params);
    }
}

// defines enemy behaviour
public class Enemy : MonoBehaviour, ICaster {

    //Const fields//

    private const float stagger_mult_constant = 1.2F;//Amount to multiply max_stagger by when calculating stagger time
    private const float stagger_add_constant = 7.5F;//Amount to add when calculating stagger time
	private const int enemy_sprite_layer = -5;//Layer of enemy sprite in sorting order

    //ICaster Properties//

    public Transform Transform { get { return transform; } }
    private int position; //position in battle field
    public int Position { get { return position; } set { position = value; } }
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
	public bool Is_done { get { return is_done; } set { is_done = value; } }
    public ICasterType CasterType
    {
        get
        {
            return ICasterType.ENEMY;
        }
    }

    //Public fields//

    public bool attack_in_progress = false;

    public BattleField field; // the field the enemy is located in (battle state)
    public CastManager castManager;
    public EnemyChargeBars bars;
	public SpriteRenderer enemy_sprite; // this enemy's sprite
	public ChangeAnimatedSprite change_sprite; // for changing the sprite
	public Animator enemy_animator; // this enemy's animator
    public EnemyAI AI = null;
    public static AssetBundle sprite_bundle = null; 
	public ParticleSystem enemy_particle_sys; // Sprite particle system
	public Material material_default; // Sprite default material
	public Material material_pixelate; // Sprite pixelation material
	public Material material_glitch; // Sprite glitchout material
	public Material material_slice; // Sprite slicing material
	public Material material_wavy; // Sprite waving material

    //Private fields//

    EnemyStats stats; // stats of enemy
    BuffDebuff buffDebuff = new BuffDebuff(); // buff/debuff state

    bool is_dead; // is enemy dead?
	bool is_done; // is enemy done dying (playing death animation)?
    bool is_stunned; // is the enemy stunned?
    bool was_hit = false; //the enemy was hit in the pause and needs state update
    int target; //Position in player_arr (BattleManager.cs) that this enemy is currently targeting
    SpellData curr_spell = null; //A reference to the current spell
	int curr_hp; // current amount of health
    int curr_shield; //current amount of shield
    int curr_stagger; //current amount of stagger
    public float stagger_time; //The time an enemy's stun will last
	float curr_stagger_time; // current time staggered
    float curr_time; // current time (from 0 to atk_time)
    float atk_time; // time it takes to attack
	Coroutine attack_cr;
    Coroutine form_change_cr = null;

    //Methods//
    void Awake()
    {
		enemy_particle_sys.Stop ();
        if(sprite_bundle == null)
            sprite_bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "enemy_sprites"));
		is_dead = false;
		is_done = false;
    }
    void Start() {
		
    }
    //Initializes enemy stats (and starts attacking routing)
	public void initialize(EnemyStats i_stats) {
        //Set stats
		stats = i_stats;
        //Initialize combat values
		Curr_hp = stats.max_hp;
        Curr_shield = stats.max_shield;
        stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
        Curr_stagger = stats.max_stagger;
		curr_time = 0;
        //Get sprite components
        enemy_sprite.sprite = sprite_bundle.LoadAsset<Sprite>(stats.sprite_path);
		enemy_sprite.sortingOrder = enemy_sprite_layer;
        //Get AI module
        AI = EnemyAI.GetAIFromString(stats.ai_type, stats.ai_params, this);
        //Start Attacking
        attack_cr = StartCoroutine (attackRoutine ()); 
	}
    public void dummy()
    {
        is_dead = true;
        enemy_sprite.enabled = false;
        enemy_animator.enabled = false;
    }
    //AI Helper/Utility functions

    //Sets enemy stats without change
    public void setStats(EnemyStats i_stats, bool resetCurrent = false)
    {
        //Set stats
        stats = i_stats;
        //Initialize combat values
        if (resetCurrent)
        {
            Curr_hp = stats.max_hp;
            Curr_shield = stats.max_shield;
            Curr_stagger = stats.max_stagger;
            stagger_time = (stats.max_stagger * stagger_mult_constant) + stagger_add_constant;
        }
        field.updateScouterInfo();
    }
    //Changes plays the form change animation
    public void changeForm()
    {
        if(form_change_cr != null)
            StopCoroutine(form_change_cr);
        form_change_cr = StartCoroutine(formChangeGraphics());
        Debug.Log("changing form");

    }
    IEnumerator formChangeGraphics()
    {
        yield return new WaitWhile(() => BattleManagerS.main.battlePause);
		enemy_animator.Play("enemy_pixelate_in");
		yield return new WaitForSeconds (1f);
		change_sprite.changeSprite(sprite_bundle.LoadAsset<Sprite>(stats.sprite_path));
		enemy_sprite.sortingOrder = enemy_sprite_layer;
    }
    //Ends and restarts the attacking Coroutine
    public void resetAttack()
    {
        curr_time = 0;
		StopCoroutine(attack_cr);
        attack_cr = StartCoroutine(attackRoutine());
    }
    //Resets the AI object
    public void resetAI()
    {
        AI = EnemyAI.GetAIFromString(stats.ai_type, stats.ai_params, this);
    }

	// returns curr_time/atk_time
	public float getAtkProgress() {
		return curr_time / atk_time;
	}

	// returns data of current spell
	public SpellData getCurrSpell() {
		return curr_spell;
	}

	// returns progress of stagger bar
	public float getStagger() {
		if (is_stunned) return curr_stagger_time / stagger_time;
		else            return ((float)curr_stagger/(float)stats.max_stagger);
	}
    //Main attack AI
	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator attackRoutine() {
		Vector3 original_pos = transform.position;
        curr_stagger_time = 0F;
		curr_spell = AI.getNextSpell(stats.spells, field.enemy_arr, position, field.player_arr, out target).clone();   //Initialize with current spell
        atk_time = castManager.spellDict.getCastingTime(curr_spell, stats.speed);   //Get casting time
		while (!is_dead) {
			yield return new WaitForEndOfFrame ();
			yield return new WaitWhile (() => BattleManagerS.main.battlePause);
            while(is_stunned)//Stop attack loop from continuing while the enemy is stunned
            {
				yield return new WaitForEndOfFrame();
				transform.position = (Vector3)original_pos + (Vector3)(Random.insideUnitCircle * 0.05f);
				enemy_sprite.material = material_glitch;
				yield return new WaitWhile (() => BattleManagerS.main.battlePause);
				curr_stagger_time += Time.deltaTime;
                if (curr_stagger_time >= stagger_time)//End stun if time up
                {
                    unStun();
                    curr_stagger_time = 0F;
					enemy_sprite.material = material_pixelate;
                }
            }
			transform.position = original_pos;
			curr_time += Time.deltaTime;
			if (curr_time >= atk_time) {
                attack_in_progress = true;
				enemy_animator.Play("enemy_swell_cast");
                fullBarFX(); // notify player of full bar
                yield return new WaitForSeconds(1f);
                yield return new WaitWhile(() => BattleManagerS.main.battlePause);
                //Only attack if stunned, else perfect stagger
                if (is_stunned)
                    attack_in_progress = false;
                else
                    attackPlayer (curr_spell);
                //Update state (will move to make it so that state change can interrupt cast)
				AI.updateState(field.enemy_arr, position, field.player_arr, EnemyAI.Update_Case.AFTER_CAST);
                //Get next spell from AI
				curr_spell = AI.getNextSpell(stats.spells, field.enemy_arr, position, field.player_arr, out target).clone();
                atk_time = castManager.spellDict.getCastingTime(curr_spell, stats.speed);//get new casting time
                curr_time = 0;
				enemy_animator.Play("enemy_idle");
            }
		}
	}

    // effects when enemy is ready to attack
    void fullBarFX()
    {
        //graphic
        StartCoroutine(barFlash());
        //sound
        AudioPlayer.main.playSFX("enemy_attack_ready");
    }

    IEnumerator barFlash()
    {
        for (int i = 0; i < 30; i++)
        {
			bars.Charge_bars[position].setColor(Random.value, 0.3f, Random.value);

			yield return new WaitForSeconds(0.0166f); // the framerate for 60fps; see Battle Effects cs
        }
        bars.Charge_bars[position].setColor(1F, 0.1F, 0.1F);

        yield return new WaitWhile(() => attack_in_progress);

        resetBarFX();
    }

    // terminate effects started by fullBarFX()
    void resetBarFX()
    {
		bars.Charge_bars[position].setColor(13.0f/255.0f, 207.0f/255.0f, 223.0f/255.0f);
    }

    // pause battle, attack player with specified spell
    void attackPlayer(SpellData s) {
        Debug.Log(stats.name + " casts " + s.ToString());
        castManager.enemyCast(castManager.spellDict, s, position, target);
	}

	// be attacked by the player
	public void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false) {
        //Apply repel and return if applicable
        if (CasterOps.calcRepel(data,d,element,caster,this,crit,reflect))
            return;
        CasterOps.calcDamage(data, d, element, caster, this, crit, Is_stunned);
        //log stun stun actually happens in update condition
        data.isStun = curr_stagger <= 0 && is_stunned == false;
        //opacity and death are now updated in updateCondition()
        was_hit = true;
    }
    //Apply stun condition to enemy
    private void stun()
    {
        is_stunned = true;
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.5F, 0);
    }
    //Un-stun enemy
    private void unStun()
    {
        bars.Charge_bars[position].gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(13.0f/255.0f, 207.0f/255.0f, 223.0f/255.0f);
        is_stunned = false;
        Curr_stagger = stats.max_stagger;
		AI.updateState(field.enemy_arr, position, field.player_arr, EnemyAI.Update_Case.UNSTUN);
    }

    //Updates opacity and death (after pause in battlemanager)
    public void updateCondition()
    {
        //Update AI if hit (may drop hp to zero)
        if (was_hit)
        {
			AI.updateState(field.enemy_arr, position, field.player_arr, EnemyAI.Update_Case.WAS_HIT);
            if (curr_stagger <= 0 && is_stunned == false)
                stun();
            was_hit = false;
        }
        if (curr_hp <= 0)
        { // check if killed
            Debug.Log(stats.name + " has been slain!");
			AudioPlayer.main.playSFX ("sfx_enemy_death"); // enemy death noise placeholder
            deathAnimation(1);
			//enemy_animator.SetTrigger("death");
			is_dead = true;
			StopAllCoroutines();
        }
    }

    private void deathAnimation(int chooseDeath)
    {
        switch (chooseDeath)
        {
            case 0:
                enemy_sprite.material = material_default;
                enemy_animator.Play("enemy_death");
                break;
            case 1:
                enemy_sprite.material = material_default;
                enemy_animator.Play("enemy_death_launcher");
                AudioPlayer.main.playSFX("sfx_death_flyaway");
                break;
            case 2:
                enemy_animator.Play("enemy_death_slice");
                break;
            case 3:
                enemy_animator.Play("enemy_death_wavy");
                break;
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
//Spell list that contains enemies spells in a useful way (for AI)
//currently mostly just a dictionary, but may change
public class EnemySpellList
{
    public EnemySpellList(Dictionary<string, SpellData[]> spells)
    {
        groups = spells;
    }
    private Dictionary<string, SpellData[]> groups;
    public SpellData[] getSpells(string group = "DEFAULT")
    {
        return groups[group];
    }
    public bool hasGroup(string group)
    {
        return groups.ContainsKey(group);
    }
}

