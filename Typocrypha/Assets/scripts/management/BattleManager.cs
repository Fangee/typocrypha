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
	public GameObject enemy_prefab; // prefab for enemy object
    public GameObject ally_prefab; //prefab for ally object
    public GameObject ally_left; // left ally UI
    public GameObject ally_right; // right ally UI
    private DisplayAlly ally_display_left;
    private DisplayAlly ally_display_right;
    public ChatDatabase chat; //Database containing chat lines
    public Color playerColor;
    public Color enemyColor;
    public Color allyColor;
    public Color clarkeColor;
    public GameObject battleLogCast; //Casting log object and Associated reference to store
    private Image castBox;
    public Text logCastText;
    public Text logCastInfo;
    public GameObject battleLogTalk; //talk log object and Associated reference to store
    private Image talkBox;
    public Text logTalkText;
    public Text logTalkInfo;
	public EnemyChargeBars charge_bars; // creates and mananges charge bars
	public EnemyStaggerBars stagger_bars; // creates and manages stagger bars
	public EnemyHealthBars health_bars; // creates and manages enemy health bars
	public CooldownList cooldown_list; // creates and manages player's cooldowns
	public GameObject target_ret; // contains targetting sprites
	public GameObject target_floor; // holds the enemy floor panels
	public GameObject dialogue_box; // text box for dialogue
	public GameObject battle_bg_prefab; // prefab of battle background

	// publically accessible fields
	[HideInInspector] public bool pause; // is battle paused?
	[HideInInspector] public Enemy[] enemy_arr; // array of Enemy components (size 3)
	[HideInInspector] public int target_ind; // index of currently targeted enemy
	[HideInInspector] public ICaster[] player_arr = { null, null, null }; // array of Player and allies (size 3)
	[HideInInspector] public int player_ind = 1;
	[HideInInspector] public int enemy_count; // number of enemies in battle
    [HideInInspector] public int curr_dead;
	[HideInInspector] public Vector2 target_pos; // position of target ret
	[HideInInspector] public DateTime time_started; // time battle started
	[HideInInspector] public List<CastData> last_cast; // last performed cast action
	[HideInInspector] public SpellData last_spell; // last performed spell
	[HideInInspector] public bool[] last_register; // last spell register status
	[HideInInspector] public int num_player_attacks; // number of player attacks from beginning of battle

	TargetReticule target_ret_scr; // TargetReticule script ref
	TargetFloor target_floor_scr;  // TargetFloor script ref
	BattleScene curr_battle; // current battle scene

	const float enemy_spacing = 6f; // horizontal space between enemies
	const float enemy_y_offset = 0.9f; // offset of enemy from y axis
	const float reticule_y_offset = 1.7f; // offset of target reticule
	const int undim_layer = -1; // layer of enemy when enemy sprite is shown
	const int dim_layer = -5;   // layer of enemy when enemy sprite is dimmed

	void Awake() {
		if (main == null) main = this;
		pause = false;
		target_ret_scr = target_ret.GetComponent<TargetReticule> ();
		target_floor_scr = target_floor.GetComponent<TargetFloor> ();
        player_arr[player_ind] = player;
	}

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle!");
		pause = true;
		curr_battle = scene;

        //INITIALIZE ENEMIES//

		enemy_arr = new Enemy[3];
		enemy_count = 0;
        curr_dead = 0;
		charge_bars.initChargeBars ();
		stagger_bars.initStaggerBars ();
		health_bars.initHealthBars ();
		//for (int i = 0; i < scene.enemy_stats.Length; i++) createEnemy (i, scene);

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
            player_arr[i] = a;
        }

        //INITIALIZE BATTLE LOG STUFF//

        castBox = battleLogCast.GetComponent<Image>();
        talkBox = battleLogTalk.GetComponent<Image>();

		//INITIALIZE TARGET UI//

		target_ind = 1;
		target_pos = new Vector2 (target_ind * enemy_spacing, reticule_y_offset);
		target_ret.transform.localPosition = target_pos;

		//INITIALIZE OTHER TRACKING VARIABLES//

		time_started = DateTime.Now;
		num_player_attacks = 0;
		last_cast = new List<CastData> ();
		last_spell = null;
		last_register = new bool[3];

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

		/*
		// spawn in enemies one by one
		for (int i = 0; i < scene.enemy_stats.Length; i++) {
			createEnemy (i, scene);
			// unpause for a split second to allow enemy to initialize
			pause = false;
			yield return new WaitForSeconds (0.1f);
			pause = true;
			yield return new WaitForSeconds (0.5f);
		}
		*/

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
        ++enemy_count;
		GameObject new_enemy = GameObject.Instantiate (enemy_prefab, transform);
		new_enemy.transform.localScale = new Vector3 (1, 1, 1);
		new_enemy.transform.localPosition = new Vector3 (i * enemy_spacing, enemy_y_offset, 0);
		enemy_arr [i] = new_enemy.GetComponent<Enemy> ();
		enemy_arr[i].field = this; //Give enemy access to field (for calling spellcasts)
		enemy_arr [i].initialize (scene.enemy_stats [i]); //sets enemy stats (AND INITITIALIZES ATTACKING AND AI)
		enemy_arr [i].Position = i;      //Log enemy position in field
		enemy_arr[i].bars = charge_bars; //Give enemy access to charge_bars
		Vector3 bar_pos = new_enemy.transform.position + new Vector3(0, -1.0f, 0);
		charge_bars.makeChargeMeter(i, bar_pos);
		stagger_bars.makeStaggerMeter (i, bar_pos);
		health_bars.makeHealthMeter (i, bar_pos);
	}

    //removes all enemies and charge bars
    public void stopBattle()
    {
        pause = true;
        foreach (Enemy enemy in enemy_arr)
        {
            if (enemy != null) GameObject.Destroy(enemy.gameObject);
        }
        enemy_arr = null;
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
		int old_ind = target_ind;

        //TARGET RETICULE CODE 

		// move target left or right
		if (Input.GetKeyDown (KeyCode.LeftArrow)) --target_ind;
		if (Input.GetKeyDown (KeyCode.RightArrow)) ++target_ind;
		// fix if target is out of bounds
		if (target_ind < 0) target_ind = 0;
		if (target_ind > 2) target_ind = 2;
		// check if target was actually moved
		if (old_ind != target_ind) {
			// move and update target reticule and update floor panels
			target_pos = new Vector2 (target_ind * enemy_spacing, reticule_y_offset);
			target_ret_scr.updateTarget ();
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
                ++num_player_attacks;
                targetPattern = spellDict.getTargetPattern(s, enemy_arr, target_ind, player_arr, player_ind);
                message = chat.getLine(player.Stats.ChatDatabaseID);
                preCastEffects(targetPattern, player, s, message);
                StartCoroutine(pauseAttackCurrent(s, player));
                break;
            case CastStatus.BOTCH:
                //diplay.playBotchEffects
                break;
            case CastStatus.FIZZLE:
                //diplay.playBotchEffects
                break;
            case CastStatus.ONCOOLDOWN:
                //display.playOnCooldownEffects
                break;
            case CastStatus.COOLDOWNFULL:
                //diplay.playCooldownFullEffects
                break;
            case CastStatus.ALLYSPELL:
                int allyPos = getAllyPosition(s.root);
                if (allyPos == -1 || player_arr[allyPos].Is_stunned || !((Ally)player_arr[allyPos]).tryCast())//display.playAllyNotHereEffects
                    break;
                targetPattern = spellDict.getTargetPattern(s, enemy_arr, target_ind, player_arr, allyPos);
                message = chat.getLine(player_arr[allyPos].Stats.ChatDatabaseID);
                preCastEffects(targetPattern, player_arr[allyPos], s, message);
                StartCoroutine(pauseAttackCurrent(s, player_arr[allyPos]));
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
        data = spellDict.cast(s, enemy_arr, target_ind, player_arr, caster.Position);
        processCast(data, s);

        yield return new WaitForSeconds(1f);

        //END PAUSE//

        postCastEffects();
        pause = false;
        updateEnemies();
    }
 
    //Casts from an enemy position: calls processCast on results
    public void enemyCast(SpellDictionary dict, SpellData s, int position, int target)
    {
        pause = true; // pause battle for attack
        AudioPlayer.main.playSFX("sfx_enemy_cast");
        Pair<bool[], bool[]> targetPattern = spellDict.getTargetPattern(s, player_arr, target, enemy_arr, position);
        preCastEffects(targetPattern, enemy_arr[position], s, chat.getLine(enemy_arr[position].Stats.ChatDatabaseID));
        BattleEffects.main.setDim(true, enemy_arr[position].GetComponent<SpriteRenderer>());
        StartCoroutine(enemy_pause_cast(dict, s, position, target));
    }

    private IEnumerator enemy_pause_cast(SpellDictionary dict, SpellData s, int position, int target)
    {

        BattleEffects.main.setDim(true, enemy_arr[position].GetComponent<SpriteRenderer>());

        yield return new WaitForSeconds(1f);

        enemy_arr[position].startSwell();
        List<CastData> data = dict.cast(s, player_arr, target, enemy_arr, position);
        processCast(data, s);

        yield return new WaitForSeconds(1f);

        postCastEffects();
        //BattleEffects.main.setDim(false, enemy_arr[position].GetComponent<SpriteRenderer>());
        pause = false; // unpause
        enemy_arr[position].attack_in_progress = false;
        updateEnemies();
    }

    //Method for processing CastData (most effects now happen in SpellEffects.cs)
    //Called by Cast in the SUCCESS CastStatus case, possibly on BOTCH in the future
    private void processCast(List<CastData> data, SpellData s)
    {
		last_cast = data;
		last_spell = s;
        //Process the data here
        foreach (CastData d in data)
            spellEffects.StartCoroutine(spellEffects.playEffects(d, s));
        //Register unregistered keywords here
        bool [] regData = spellDict.safeRegister(s);
        if (regData[0] || regData[1] || regData[2])
            StartCoroutine(learnSFX());
		last_register = regData;
        //Process regData (for register graphics) here. 
        //format is bool [3], where regData[0] is true if s.element is new, regData[1] is true if s.root is new, and regData[2] is true if s.style is new
    }

    private IEnumerator learnSFX()
    {
        yield return new WaitWhile(() => BattleManager.main.pause);
        AudioPlayer.main.playSFX("sfx_learn_spell_battle");
    }
    //Effects that happen before any actor casts
    private void preCastEffects(Pair<bool[], bool[]> targetPattern, ICaster caster, SpellData cast, string message)
    {
        BattleEffects.main.setDim(true);
        battleLog(cast.ToString(), caster.CasterType, message, caster.Stats.name);
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
        stopBattleLog();
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_arr[i] != null)
                enemy_arr[i].enemy_sprite.sortingOrder = dim_layer;
        }
        target_ret.SetActive(true); // enable / make target reticule appear after a cast
        BattleEffects.main.setDim(false);
    }
    //Enable battle log UI state (call anywhere that the battlemanager pauses to cast)
    private void battleLog(string cast, ICasterType caster, string talk, string speaker)
    {
        battleLogCast.SetActive(true);
        battleLogTalk.SetActive(true);
        logCastText.text = "> " + cast;
        logTalkText.text = talk;
        logTalkInfo.text = speaker;
        if (caster == ICasterType.ENEMY)
        {
            castBox.color = enemyColor;
            talkBox.color = enemyColor;
            logCastInfo.text = "ENEMY  CAST";
        }
        else if (caster == ICasterType.PLAYER)
        {
            castBox.color = playerColor;
            talkBox.color = playerColor;
            logCastInfo.text = "PLAYER CAST";
        }
        else if (caster == ICasterType.NPC_ALLY)
        {
            castBox.color = allyColor;
            talkBox.color = allyColor;
            logCastInfo.text = "ALLY   CAST";
        }
        else //caster == IcasterType.INVALID (clarke is speaking)
        {
            castBox.color = clarkeColor;
            talkBox.color = clarkeColor;
            logCastInfo.text = "ERROR  CAST";
        }
    }
    //Stop battle log UI (call after every pause to cast
    private void stopBattleLog()
    {
        battleLogCast.SetActive(false);
        battleLogTalk.SetActive(false);
    }

    //Raises the targets (array val = true) above the dimmer level
    private void raiseTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
                enemy_arr[i].enemy_sprite.sortingOrder = undim_layer;
        }
    }
    //Lowers the targets (array val = true) below the dimmer level
    private void lowerTargets(bool[] enemy_r, bool[] player_r)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (enemy_r[i])
                enemy_arr[i].enemy_sprite.sortingOrder = dim_layer;
        }
    }

    //returns the position of ally with specified name (if in battle)
    private int getAllyPosition(string name)
    {
        if (player_arr[0] != null && player_arr[0].Stats.name.ToLower() == name.ToLower())
            return 0;
        if (player_arr[2] != null && player_arr[2].Stats.name.ToLower() == name.ToLower())
            return 2;
        return -1;
    }


    //Updates death and opacity of enemies after pause in puaseAttackCurrent
    public void updateEnemies()
    {
        bool interrupted = checkInterrupts(); // check for interrupts
        if (interrupted) return;
        for (int i = 0; i < enemy_arr.Length; i++)
        {
            if (enemy_arr[i] != null)
            {
                if (!enemy_arr[i].Is_dead)
                {
                    enemy_arr[i].updateCondition();
                    if (enemy_arr[i].Is_dead)
                        ++curr_dead;
                }
            }
        }
		//Update target and floor effects
		target_ret_scr.updateTarget ();
		target_floor_scr.updateFloor ();
        if (curr_dead == enemy_count) // end battle if all enemies dead
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
