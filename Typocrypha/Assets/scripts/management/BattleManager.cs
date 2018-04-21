using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages battle sequences
public class BattleManager : MonoBehaviour {
	// statically linked fields
	public static BattleManager main = null; // static instance accessible globally (try not to use this)
    public Player player;
	public SpellDictionary spellDict; // spell dictionary object
    public SpellEffects spellEffects;
    public TrackTyping trackTyping;
	public GameObject enemy_prefab; // prefab for enemy object
    public GameObject ally_prefab; //prefab for ally object
    public GameObject ally_left; // left ally UI
    public GameObject ally_right; // right ally UI
    private DisplayAlly ally_display_left;
    private DisplayAlly ally_display_right;
    public ChatDatabase chat; //Database containing chat lines
	public EnemyChargeBars charge_bars; // creates and mananges charge bars
	public EnemyStaggerBars stagger_bars; // creates and manages stagger bars
	public EnemyHealthBars health_bars; // creates and manages enemy health bars
	public CooldownList cooldown_list; // creates and manages player's cooldowns
	public GameObject target_ret; // contains targetting sprites
	public GameObject target_floor; // holds the enemy floor panels
	public GameObject dialogue_box; // text box for dialogue
	public GameObject battle_bg_prefab; // prefab of battle background

	[HideInInspector] public bool pause; // is battle paused?
	[HideInInspector] public BattleField battle_field; // Encapsulates battle info

	TargetReticule target_ret_scr; // TargetReticule script ref
	TargetFloor target_floor_scr;  // TargetFloor script ref
	BattleScene curr_battle; // current battle scene

	const float enemy_spacing = 6f; // horizontal space between enemies
	const float enemy_y_offset = 0.5f; // offset of enemy from y axis
	const float reticule_y_offset = 1.5f; // offset of target reticule
	const int undim_layer = -1; // layer of enemy when enemy sprite is shown
	const int dim_layer = -5;   // layer of enemy when enemy sprite is dimmed

	void Awake() {
		if (main == null) main = this;
		pause = false;
		target_ret_scr = target_ret.GetComponent<TargetReticule> ();
		target_floor_scr = target_floor.GetComponent<TargetFloor> ();
		battle_field = new BattleField ();
		battle_field.player_arr[battle_field.player_ind] = player;
	}

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle!");
		pause = true;
		curr_battle = scene;

        //INITIALIZE ENEMIES//

		battle_field.enemy_arr = new Enemy[3];
		battle_field.enemy_count = 0;
		battle_field.curr_dead = 0;
		charge_bars.initChargeBars ();
		stagger_bars.initStaggerBars ();
		health_bars.initHealthBars ();

        //CREATE ALLIES//

        if (scene.ally_stats.Length == 0)
        {
            ally_left.SetActive(false);
            ally_right.SetActive(false);
        }

        for(int i = 0; i < scene.ally_stats.Length; ++i)
        {
            GameObject new_ally = GameObject.Instantiate(ally_prefab, transform);
            new_ally.transform.localScale = new Vector3(1, 1, 1);
            new_ally.transform.localPosition = new Vector3(1, 0, 0);
            Ally a = new_ally.GetComponent<Ally>();
            a.setStats(scene.ally_stats[i]);
            if (i == 1)
            {
                ally_display_right = ally_right.GetComponentInChildren<DisplayAlly>();
                ally_display_right.setAlly(a);
                a.transform.position = ally_display_right.transform.position;
                ally_right.SetActive(true);
                ++i;
            }
            else
            {
                ally_display_left = ally_left.GetComponentInChildren<DisplayAlly>();
                ally_display_left.setAlly(a);
                a.transform.position = ally_display_left.transform.position;
                ally_left.SetActive(true);
            }
            a.Position = i;
			battle_field.player_arr[i] = a;
        }

		//INITIALIZE TARGET UI//

		//battle_field.target_ind = 1;
		//battle_field.target_pos = new Vector2 (battle_field.target_ind * enemy_spacing, reticule_y_offset);
		//target_ret.transform.localPosition = battle_field.target_pos;

		//INITIALIZE OTHER TRACKING VARIABLES//

		battle_field.time_started = DateTime.Now;
		battle_field.num_player_attacks = 0;
		battle_field.last_cast = new List<CastData> ();
		battle_field.last_spell = null;
		battle_field.last_register = new bool[3];

        //FINISH//

		StartCoroutine (finishBattlePrep (scene));
	}

	// finishes up battle start and effects
	IEnumerator finishBattlePrep(BattleScene scene) {
		// play battle transition
		BattleEffects.main.battleTransitionEffect("swirl_in", 1f);
		//BattleEffects.main.pixelateIn (1f);
		yield return new WaitForSeconds (1f);
		BackgroundEffects.main.setPrefabBG (battle_bg_prefab); // set background
		for (int i = 0; i < scene.enemy_stats.Length; i++) createEnemy (i, scene); // create enemies
		// unpause for a split second to allow enemy to initialize
		pause = false;
		yield return new WaitForSeconds (0.1f);
		pause = true;
		BattleEffects.main.battleTransitionEffect("swirl_out", 1f);
		yield return new WaitForSeconds (1f);

		pause = true;
		yield return new WaitForSeconds (1f);
		// show targeting ui
		target_ret.SetActive (true);
		target_floor.SetActive (true);
		target_ret_scr.updateTarget ();
		target_floor_scr.updateFloor ();

		// start battle
		pause = false;
		AudioPlayer.main.playMusic (scene.music_tracks[0]);
		checkInterrupts ();
	}

	// creates the enemy specified at 'i' (0-left, 1-mid, 2-right) by the 'scene'
	void createEnemy(int i, BattleScene scene) {
        if (scene.enemy_stats[i] == null)
            return;
		++battle_field.enemy_count;
		GameObject new_enemy = GameObject.Instantiate (enemy_prefab, transform);
		new_enemy.transform.localScale = new Vector3 (1, 1, 1);
		new_enemy.transform.localPosition = new Vector3 (i * enemy_spacing, enemy_y_offset, 0);
		battle_field.enemy_arr [i] = new_enemy.GetComponent<Enemy> ();
		battle_field.enemy_arr[i].field = battle_field; //Give enemy access to field (for calling spellcasts)
		battle_field.enemy_arr [i].initialize (scene.enemy_stats [i]); //sets enemy stats (AND INITITIALIZES ATTACKING AND AI)
		battle_field.enemy_arr [i].Position = i;      //Log enemy position in field
		battle_field.enemy_arr[i].bars = charge_bars; //Give enemy access to charge_bars
		Vector3 bar_pos = new_enemy.transform.position + new Vector3(-0.5f, -1.0f, 0);
		charge_bars.makeChargeMeter(i, bar_pos);
		stagger_bars.makeStaggerMeter (i, bar_pos);
		health_bars.makeHealthMeter (i, bar_pos);
	}

    //removes all enemies and charge bars
    public void stopBattle()
    {
        pause = true;
		foreach (Enemy enemy in battle_field.enemy_arr)
        {
            if (enemy != null) GameObject.Destroy(enemy.gameObject);
        }
		battle_field.enemy_arr = null;
        cooldown_list.removeAll();
        charge_bars.removeAll();
        stagger_bars.removeAll();
        health_bars.removeAll();
		target_ret.SetActive (false);
		target_floor.SetActive (false);
		BackgroundEffects.main.removePrefabBG (2.0f);
    }

    // check if player switches targets or attacks
    void Update() {
		if (Input.GetKeyDown (KeyCode.BackQuote)) // toggle pause
			pause = !pause;
		if (pause) return;
		int old_ind = battle_field.target_ind;

        //TARGET RETICULE CODE 

		// move target left or right
		if (Input.GetKeyDown (KeyCode.LeftArrow)) --battle_field.target_ind;
		if (Input.GetKeyDown (KeyCode.RightArrow)) ++battle_field.target_ind;

        //toggle enemy info on Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        { 
                target_ret_scr.toggleScouter();
        }

		// fix if target is out of bounds
		if (battle_field.target_ind < 0) battle_field.target_ind = 0;
		if (battle_field.target_ind > 2) battle_field.target_ind = 2;
		// check if target was actually moved
		if (old_ind != battle_field.target_ind) {
			// move and update target reticule and update floor panels
			target_ret_scr.updateTarget (new Vector2(battle_field.target_ind * enemy_spacing, reticule_y_offset));
			target_floor_scr.updateFloor ();
			// play sfx
			AudioPlayer.main.playSFX ("sfx_enemy_select");
		}
        //SPELLBOOK CODE

        // go to next page if down is pressed
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (spellDict.pageDown())
                AudioPlayer.main.playSFX("sfx_spellbook_scroll", 0.3F);
            //else {play sfx_thud (player is on the last page this direction)}
        }
        // go to last page if down is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (spellDict.pageUp())
                AudioPlayer.main.playSFX("sfx_spellbook_scroll", 0.3F);
            //else {play sfx_thud (player is on the last page this direction)}
        }
    }

    //CASTING CODE//---------------------------------------------------------------------------------------------------------------------------------------//

    // attack currently targeted enemy with spell
    public void attackCurrent(string spell) {
        SpellData s;
        string message = "";
        //Send spell, Enemy state, and target index to parser and caster
        CastStatus status = spellDict.parse(spell.ToLower(), out s);
        Pair<bool[], bool[]> targetPattern = null;
        switch (status)
        {
            case CastStatus.SUCCESS:
			++battle_field.num_player_attacks;
			targetPattern = spellDict.getTargetPattern(s, battle_field.enemy_arr, battle_field.target_ind, battle_field.player_arr, battle_field.player_ind);
                message = chat.getLine(player.Stats.ChatDatabaseID);
                preCastEffects(targetPattern, player, s, message);
                StartCoroutine(pauseAttackCurrent(s, player));
                trackTyping.clearBuffer();//Clear the casting buffer
                break;
            case CastStatus.BOTCH:
                //diplay.playBotchEffects
                spellEffects.popp.spawnSprite("popups_invalid", 1.0F, player.transform.position - new Vector3(0, 0.375f, 0));
                trackTyping.clearBuffer();//Clear the casting buffer
                break;
            case CastStatus.FIZZLE:
                //diplay.playBotchEffects
				spellEffects.popp.spawnSprite("popups_invalid", 1.0F, player.transform.position - new Vector3(0, 0.375f, 0));
                trackTyping.clearBuffer();//Clear the casting buffer
                break;
            case CastStatus.ONCOOLDOWN:
                //display.playOnCooldownEffects
				spellEffects.popp.spawnSprite("popups_oncooldown", 1.0F, player.transform.position - new Vector3(0, 0.375f, 0));
                break;
            case CastStatus.COOLDOWNFULL:
                //diplay.playCooldownFullEffects
				spellEffects.popp.spawnSprite("popups_cooldownfull", 1.0F, player.transform.position - new Vector3(0, 0.375f, 0));
                break;
            case CastStatus.ALLYSPELL:
                int allyPos = getAllyPosition(s.root);
			if (allyPos == -1 || battle_field.player_arr[allyPos].Is_stunned || !((Ally)battle_field.player_arr[allyPos]).tryCast())//display.playAllyNotHereEffects
                    break;
			targetPattern = spellDict.getTargetPattern(s, battle_field.enemy_arr, battle_field.target_ind, battle_field.player_arr, allyPos);
			message = chat.getLine(battle_field.player_arr[allyPos].Stats.ChatDatabaseID);
			preCastEffects(targetPattern, battle_field.player_arr[allyPos], s, message);
			StartCoroutine(pauseAttackCurrent(s, battle_field.player_arr[allyPos]));
                trackTyping.clearBuffer();//Clear the casting buffer
                break;
        }
    }
    IEnumerator pauseAttackCurrent(SpellData s, ICaster caster)
    {
        pause = true;

        //BEGIN PAUSE//

        yield return new WaitForSeconds(1f);

        //CASTING//
        spellDict.startCooldown(s, player);
        List<CastData> data;
		data = spellDict.cast(s, battle_field.enemy_arr, battle_field.target_ind, battle_field.player_arr, caster.Position);
        processCast(data, s);

        yield return new WaitForSeconds(1f);

        //END PAUSE//

        postCastEffects();
        updateEnemies();
        pause = false;
    }
 
    //Casts from an enemy position: calls processCast on results
    public void enemyCast(SpellDictionary dict, SpellData s, int position, int target)
    {
        pause = true; // pause battle for attack
        AudioPlayer.main.playSFX("sfx_enemy_cast");
		Pair<bool[], bool[]> targetPattern = spellDict.getTargetPattern(s, battle_field.player_arr, target, battle_field.enemy_arr, position);
		preCastEffects(targetPattern, battle_field.enemy_arr[position], s, chat.getLine(battle_field.enemy_arr[position].Stats.ChatDatabaseID));
		BattleEffects.main.setDim(true, battle_field.enemy_arr[position].GetComponent<SpriteRenderer>());
        StartCoroutine(enemy_pause_cast(dict, s, position, target));
    }

    private IEnumerator enemy_pause_cast(SpellDictionary dict, SpellData s, int position, int target)
    {

		BattleEffects.main.setDim(true, battle_field.enemy_arr[position].GetComponent<SpriteRenderer>());

        yield return new WaitForSeconds(1f);

		battle_field.enemy_arr[position].startSwell();
		List<CastData> data = dict.cast(s, battle_field.player_arr, target, battle_field.enemy_arr, position);
        processCast(data, s);

        yield return new WaitForSeconds(1f);

        postCastEffects();
		battle_field.enemy_arr[position].attack_in_progress = false;
        updateEnemies();
        pause = false; // unpause
    }

    //Method for processing CastData (most effects now happen in SpellEffects.cs)
    //Called by Cast in the SUCCESS CastStatus case, possibly on BOTCH in the future
    private void processCast(List<CastData> data, SpellData s)
    {
		battle_field.last_cast = data;
		battle_field.last_spell = s;
        //Process the data here
        foreach (CastData d in data)
            spellEffects.StartCoroutine(spellEffects.playEffects(d, s));
        //Register unregistered keywords here
        bool [] regData = spellDict.safeRegister(s);
        if (regData[0] || regData[1] || regData[2])
            StartCoroutine(learnSFX());
		battle_field.last_register = regData;
        //Process regData (for register graphics) here. 
        //format is bool [3], where regData[0] is true if s.element is new, regData[1] is true if s.root is new, and regData[2] is true if s.style is new
    }

    private IEnumerator learnSFX()
    {
        yield return new WaitWhile(() => BattleManagerS.main.pause);
        AudioPlayer.main.playSFX("sfx_learn_spell_battle");
    }
    //Effects that happen before any actor casts
    private void preCastEffects(Pair<bool[], bool[]> targetPattern, ICaster caster, SpellData cast, string message)
    {
        BattleEffects.main.setDim(true);
		BattleLog.main.battleLog(cast.ToString(), caster.CasterType, message, caster.Stats.name);
        if (targetPattern != null)
        {
            if (caster.CasterType == ICasterType.ENEMY)
                raiseTargets(targetPattern.second, targetPattern.first);
            else
                raiseTargets(targetPattern.first, targetPattern.second);
        }
        target_ret.SetActive(false); // disable / make target reticule disappear on a cast
    }

    //effects that hafter after any actor casts
    private void postCastEffects()
    {
		BattleLog.main.stopBattleLog();
        for (int i = 0; i < 3; ++i)
        {
			if (battle_field.enemy_arr[i] != null)
				battle_field.enemy_arr[i].enemy_sprite.sortingOrder = dim_layer;
        }
        target_ret.SetActive(true); // enable / make target reticule appear after a cast
        BattleEffects.main.setDim(false);
    }

    //Raises the targets (array val = true) above the dimmer level
    private void raiseTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
				battle_field.enemy_arr[i].enemy_sprite.sortingOrder = undim_layer;
        }
    }
    //Lowers the targets (array val = true) below the dimmer level
    private void lowerTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
				battle_field.enemy_arr[i].enemy_sprite.sortingOrder = dim_layer;
        }
    }

    //returns the position of ally with specified name (if in battle)
    private int getAllyPosition(string name)
    {
		if (battle_field.player_arr[0] != null && battle_field.player_arr[0].Stats.name.ToLower() == name.ToLower())
            return 0;
		if (battle_field.player_arr[2] != null && battle_field.player_arr[2].Stats.name.ToLower() == name.ToLower())
            return 2;
        return -1;
    }


    //Updates death and opacity of enemies after pause in puaseAttackCurrent
    public void updateEnemies()
    {
        bool interrupted = checkInterrupts(); // check for interrupts
        if (interrupted) return;
		for (int i = 0; i < battle_field.enemy_arr.Length; i++)
        {
			if (battle_field.enemy_arr[i] != null)
            {
				if (!battle_field.enemy_arr[i].Is_dead)
                {
					battle_field.enemy_arr[i].updateCondition();
					if (battle_field.enemy_arr[i].Is_dead)
						++battle_field.curr_dead;
                }
            }
        }
		//Update target and floor effects
		target_ret_scr.updateTarget ();
		target_floor_scr.updateFloor ();
		if (battle_field.curr_dead == battle_field.enemy_count) // end battle if all enemies dead
        {
            Debug.Log("you win!");
			stopBattle ();
            StartCoroutine(StateManager.main.nextSceneDelayed(2.0f));
        }
    }

    //INTERRUPT CODE//-------------------------------------------------------------------------------------------------------------------------------------//

    // checks and plays battle interruptions (returns true if an interrupt occured)
    bool checkInterrupts() {
		bool interrupted = false;
		// check for interrupt scenes
		for (int i = 0; i < curr_battle.interrupts.Length; ++i) {
			if (curr_battle.interrupts [i] == null) continue;
			BattleInterrupt binter = curr_battle.interrupts [i];
			BattleInterruptStatus status = binter.checkCondition ();
			// check if condition is fulfilled, and play interrupt if true
			if (status != BattleInterruptStatus.FALSE) {
				Debug.Log ("battle interrupt!");
				interrupted = true;
				// remove battle interrupt if non-repeatable
				if (status != BattleInterruptStatus.REPEAT)
					curr_battle.interrupts [i] = null;
				StartCoroutine (playInterrupt (binter.scene));
			}
			if (interrupted) break;
		}
		return interrupted;
	}

	// plays a battle interrupt
	IEnumerator playInterrupt(CutScene scene) {
		pause = true;
		dialogue_box.SetActive (true);
		CutsceneManager.main.enabled = true;
		CutsceneManager.main.battle_interrupt = true;
		CutsceneManager.main.startCutscene (scene);
		yield return new WaitUntil (() => CutsceneManager.main.at_end);
		CutsceneManager.main.enabled = false;
		dialogue_box.SetActive (false);
		pause = false;
		updateEnemies ();
	}
}
