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
            a.position = i;
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
		// pixelate in background
		BattleEffects.main.pixelateIn (1f);
		yield return new WaitForSeconds (1f);
		BackgroundEffects.main.setPrefabBG (battle_bg_prefab); // set background
		for (int i = 0; i < scene.enemy_stats.Length; i++) createEnemy (i, scene); // create enemies
		// unpause for a split second to allow enemy to initialize
		pause = false;
		yield return new WaitForSeconds (0.1f);
		pause = true;
		BattleEffects.main.pixelateOut (1f);
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
		enemy_arr [i].position = i;      //Log enemy position in field
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

        //show enemy info on Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            target_ret_scr.showScouter();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            target_ret_scr.hideScouter();
        }

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
		++num_player_attacks;
        //Can attack dead enemies now, just wont cast spell at them
		StartCoroutine (pauseAttackCurrent (spell));
    }

	// pause for player attack, play animations, unpause
	IEnumerator pauseAttackCurrent(string spell){

        pause = true;

        //SPELL PARSING//

        SpellData s;
        //Send spell, Enemy state, and target index to parser and caster
        CastStatus status = spellDict.parse(spell.ToLower(), out s);
        
        //DIMMING AND BATTLELOG//

        BattleEffects.main.setDim(true);
        Pair<bool[], bool[]> targetPattern = null;
        int allyPos = getAllyPosition(s.root);
        if (status == CastStatus.ALLYSPELL)
        {
            if(allyPos != -1)
            {
                battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.NPC_ALLY, chat.getLine(player_arr[allyPos].Stats.ChatDatabaseID), Utility.String.FirstLetterToUpperCase(s.root));
                targetPattern = spellDict.getTargetPattern(s, enemy_arr, target_ind, player_arr, allyPos);
            }
            else
            {
                battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, s.root + " isn't here right now!", "Clarke");
            }
        }          
        else if (status == CastStatus.SUCCESS)
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.PLAYER, chat.getLine(player_arr[player_ind].Stats.ChatDatabaseID), player_arr[player_ind].Stats.name);
            targetPattern = spellDict.getTargetPattern(s, enemy_arr, target_ind, player_arr, player_ind);
        }
        else if (status == CastStatus.BOTCH)
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, chat.getLine("botch"), "Clarke");
        }
        else if (status == CastStatus.FIZZLE)
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, chat.getLine("fizzle"), "Clarke");
        }
        else if (status == CastStatus.ONCOOLDOWN)
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, chat.getLine("oncooldown"), "Clarke");
        }
        else if (status == CastStatus.COOLDOWNFULL)
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, chat.getLine("cooldownlistfull"), "Clarke");
        }
        else
        {
            battleLog(spell.ToUpper().Replace(' ', '-'), ICasterType.INVALID, "SOMETHING HAS GONE HORRIBLY WRONG", "GOD");
        }
        if(targetPattern != null)
            raiseTargets(targetPattern.first, targetPattern.second);

        //BEGIN PAUSE//

        yield return new WaitForSeconds (1f);

        //CASTING//

        //Set last_cast
        ((Player)player_arr[player_ind]).Last_cast = s.ToString();
        //Cast/Botch/Cooldown/Fizzle, with associated effects and processing
        playerCast(spellDict, s, status);

        yield return new WaitForSeconds (1f);

        //END PAUSE//

        if (targetPattern != null)
            lowerTargets(targetPattern.first, targetPattern.second);
        stopBattleLog();
		BattleEffects.main.setDim (false);
		pause = false;
		updateEnemies();
	}

    //Casts from an ally position at target enemy_arr[target]: calls processCast on results
    public void NPC_Cast(SpellDictionary dict, SpellData s, int position, int target)
    {
        if(player_arr[position].Is_stunned)
        {
            Debug.Log(s.root + " cannot assist you because they are stunned!");
        }
        else if (((Ally)player_arr[position]).tryCast())
        {
            dict.startCooldown(s, (Player)player_arr[player_ind]);
            List<CastData> data = dict.cast(s, enemy_arr, target, player_arr, position);
            processCast(data, s);
        }
        else
        {
            Debug.Log(s.root + " is not ready to assist you yet!");
        }
    }

    //Casts from an enemy position: calls processCast on results
    public void enemyCast(SpellDictionary dict, SpellData s, int position, int target)
    {
        pause = true; // pause battle for attack
        StartCoroutine(enemy_pause_cast(dict, s, position, target));
        AudioPlayer.main.playSFX("sfx_enemy_cast");
    }

    //Does the pausing for enemyCast (also does the actual cast calling)
    private IEnumerator enemy_pause_cast(SpellDictionary dict, SpellData s, int position, int target)
    {
        Pair<bool[], bool[]>  targetPattern = spellDict.getTargetPattern(s, player_arr, target, enemy_arr, position);
        BattleEffects.main.setDim(true, enemy_arr[position].GetComponent<SpriteRenderer>());
        raiseTargets(targetPattern.second, targetPattern.first);
        battleLog(s.ToString(), ICasterType.ENEMY, chat.getLine(enemy_arr[position].Stats.ChatDatabaseID), enemy_arr[position].Stats.name);

        yield return new WaitForSeconds(1f);

        enemy_arr[position].startSwell();
        List<CastData> data = dict.cast(s, player_arr, target, enemy_arr, position);
        processCast(data, s);

        yield return new WaitForSeconds(1f);

        stopBattleLog();
        lowerTargets(targetPattern.second, targetPattern.first);
        BattleEffects.main.setDim(false, enemy_arr[position].GetComponent<SpriteRenderer>());
        pause = false; // unpause
        enemy_arr[position].attack_in_progress = false;
        updateEnemies();
    }

    //Cast/Botch/Cooldown/Fizzle, with associated effects and processing
    //all animation and attack effects should be processed here
    //ONLY CALL FOR A PLAYER CAST (NOTE: NPC casts are routed through here as well)
    //Pre: CastStatus is generated by dict.Parse()
    private void playerCast(SpellDictionary dict, SpellData s, CastStatus status)
    {
        List<CastData> data;
        switch (status)//Switched based on caststatus
        {
            case CastStatus.SUCCESS:
                //Player Cast
                dict.startCooldown(s, (Player)player_arr[player_ind]);
                data = dict.cast(s, enemy_arr, target_ind, player_arr, player_ind);
                processCast(data, s);
                break;
            case CastStatus.BOTCH:
                data = dict.botch(s, enemy_arr, target_ind, player_arr, player_ind);
                Debug.Log("Botched cast: " + s.ToString());
                //Process the data here
                break;
            case CastStatus.ALLYSPELL:
                //NPC cast if appropriate
                if (player_arr[0].Stats.name.ToLower() == s.root)
                    NPC_Cast(dict, s, 0, target_ind);
                else if (player_arr[2].Stats.name.ToLower() == s.root)
                    NPC_Cast(dict, s, 2, target_ind);
                else
                {
                    Debug.Log(s.root + " isn't here!");
                }
                break;
            case CastStatus.ONCOOLDOWN:
                //Handle effects
                Debug.Log("Cast failed: " + s.root.ToUpper() + " is on cooldown for " + dict.getTimeLeft(s) + " seconds");
                break;
            case CastStatus.COOLDOWNFULL:
                //Handle effects
                Debug.Log("Cast failed: cooldownList is full!");
                break;
            case CastStatus.FIZZLE:
                //Handle effects
                break;
        }
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

    //Returns true if ally with specified name is in the battle
    private bool allyIsPresent(string name)
    {
        if (player_arr[0] != null && player_arr[0].Stats.name.ToLower() == name.ToLower())
            return true;
        if (player_arr[2] != null && player_arr[2].Stats.name.ToLower() == name.ToLower())
            return true;
        return false;
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
    //Enable battle log UI state (call anywhere that the battlemanager pauses to cast)
    private void battleLog(string cast, ICasterType caster, string talk, string speaker)
    {
        battleLogCast.SetActive(true);
        battleLogTalk.SetActive(true);
        logCastText.text = "> " + cast;
        logTalkText.text = talk;
        logTalkInfo.text = speaker;
        if(caster == ICasterType.ENEMY)
        {
            castBox.color = enemyColor;
            talkBox.color = enemyColor;
            logCastInfo.text = "ENEMY  CAST";
        }
        else if(caster == ICasterType.PLAYER)
        {
            castBox.color = playerColor;
            talkBox.color = playerColor;
            logCastInfo.text = "PLAYER CAST";
        }
        else if(caster == ICasterType.NPC_ALLY)
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
		target_ret.SetActive (false); // disable / make target reticule disappear on a cast
    }
    //Stop battle log UI (call after every pause to cast
    private void stopBattleLog()
    {
        battleLogCast.SetActive(false);
        battleLogTalk.SetActive(false);
		target_ret.SetActive (true); // enable / make target reticule appear after a cast
        //hide scouter when pause ends if player not holding shift
        if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            target_ret_scr.hideScouter();
        }
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
