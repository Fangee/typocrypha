using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagerS : MonoBehaviour {
    public static BattleManagerS main = null;
    public Player player;
    public TrackTyping trackTyping;
    public BattleKeyboard battleKeyboard;
    public BattleUI uiManager;
    public CastManager castManager;
    public BattleEventManager globalEvents;
    public EnemyDatabase enemyData;
    public AllyDatabase allyData;
    public GameObject enemy_prefab; // prefab for enemy object
    public GameObject ally_prefab; //prefab for ally object
    public BattleField field;
	public GameObject typocrypha_object;
	public Animator typocrypha_animator; // animator for typocrypha object
    public Text debugThirdEyeCharge;
    [HideInInspector] public bool pause = true;

    private bool thirdEyeActive = false;
    private Coroutine thirdEyeCr = null;
    private const float maxThirdEyeCharge = 10.5f;
    private float currThirdEyeCharge = maxThirdEyeCharge;
    private BattleWave Wave { get { return waves[curr_wave]; } }
    private int curr_wave = -1;
    private bool wave_started = false;
    private BattleWave[] waves;
    private GameObject curr_battle;
    private List<GameObject> sceneQueue = new List<GameObject>();

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            //Database code (possibly subject to becoming more object-y)
            EnemyDatabase.main.build();
            enemyData = EnemyDatabase.main;
            AllyDatabase.main.build();
            allyData = AllyDatabase.main;
        }
        else Destroy(this);
    }

    private void Start()
    {
        field = new BattleField(this);
        castManager.Field = field;
        field.player_arr[1] = player;
    }

    // check if player switches targets or attacks
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // toggle pause
            setPause(!pause);
        if (pause) return;
        int old_ind = field.target_ind;

        //TARGET RETICULE CODE 

        // move target left or right
        if (Input.GetKeyDown(KeyCode.LeftArrow)) --field.target_ind;
        if (Input.GetKeyDown(KeyCode.RightArrow)) ++field.target_ind;

        // third eye stuff
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            startThirdEye();
        }
        if((Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && thirdEyeActive)
        {
            stopThirdEye();
        }
        if(!thirdEyeActive)
            currThirdEyeCharge = Mathf.Min(currThirdEyeCharge + 0.03f, maxThirdEyeCharge);
        debugThirdEyeCharge.text = ((currThirdEyeCharge / maxThirdEyeCharge) * 10).ToString();
        // fix if target is out of bounds
        if (field.target_ind < 0) field.target_ind = 0;
        if (field.target_ind > 2) field.target_ind = 2;
        // check if target was actually moved
        if (old_ind != field.target_ind)
        {
            uiManager.setTarget(field.target_ind);
        }
        //SPELLBOOK CODE

        // go to next page if down is pressed
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (castManager.pageDown())
                AudioPlayer.main.playSFX("sfx_spellbook_scroll", 0.3F);
            //else {play sfx_thud (player is on the last page this direction)}
        }
        // go to last page if down is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (castManager.pageUp())
                AudioPlayer.main.playSFX("sfx_spellbook_scroll", 0.3F);
            //else {play sfx_thud (player is on the last page this direction)}
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            trackTyping.revertBuffer();
        }
    }

    public void setEnabled(bool e)
    {
        enabled = e;
        trackTyping.enabled = e;
    }
    public void setPause(bool p)
    {
        pause = p;
        trackTyping.enabled = !p;
    }

    public void startThirdEye()
    {
        if (currThirdEyeCharge < 1)
            return; //Play not working sfx here later
        thirdEyeActive = true;
        uiManager.showScouter();
        trackTyping.enabled = false;
        startSlowMo();
        thirdEyeCr = StartCoroutine(thirdEye());
    }
    public void stopThirdEye()
    {
        if (thirdEyeCr != null)
            StopCoroutine(thirdEyeCr);
        stopSlowMo();
        trackTyping.enabled = true;
        uiManager.hideScouter();
        thirdEyeActive = false;
    }
    private IEnumerator thirdEye()
    {
        currThirdEyeCharge = Mathf.Max(currThirdEyeCharge - 0.5f, 0);
        while (currThirdEyeCharge > 0)
        {
            yield return new WaitForFixedUpdate();
            currThirdEyeCharge -= Time.fixedUnscaledDeltaTime;
        }
        stopThirdEye();
    }
    private void startSlowMo()
    {
        Time.timeScale = 0.15f;
        Time.fixedDeltaTime = 0.0005f;
        AudioPlayer.main.playSFX("sfx_slowmo_2");
        BattleEffects.main.setDim(true);
    }
    private void stopSlowMo()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        AudioPlayer.main.playSFX("sfx_speedup3");
        BattleEffects.main.setDim(false);
    }

    //MAIN BATTLE FLOW-----------------------------------------------------------------------//

    // start battle scene
    public void startBattle(GameObject new_battle)
    {
        //Reset player stats and status
        player.restoreToFull();
        currThirdEyeCharge = maxThirdEyeCharge;
        battleKeyboard.clearStatus();
        castManager.cooldown.removeAll();
        trackTyping.clearBuffer();
        trackTyping.updateDisplay();
        //Reset BattleManager curr variables
        curr_battle = new_battle;
        curr_wave = -1;
        waves = new_battle.GetComponents<BattleWave>();
        //Reset target
		field.target_ind = 1;
        StartCoroutine(finishBattlePrep());
    }
    // finishes up battle start and effects
    IEnumerator finishBattlePrep()
    {
        // play battle transition
		pause = true;
		typocrypha_object.SetActive (false);
        BattleEffects.main.battleTransitionEffect("swirl_in", 1f);
        yield return new WaitForSeconds(1f);
		if (waves[0].Background != string.Empty) {
			uiManager.initBg(waves[0].Background);
		} 
		else {
			uiManager.initBg();
		}
        BattleEffects.main.battleTransitionEffect("swirl_out", 1f);
        yield return new WaitForSeconds(1.5f);
		typocrypha_object.SetActive (true);
		typocrypha_animator.Play ("anim_typocrypha_entrance");
        AudioPlayer.main.playSFX("sfx_bootup_2");
		yield return new WaitForSeconds(0.5f);
        nextWave();
    }
    // go to next wave (also starts first wave for real)
    public void nextWave()
    {
        if (++curr_wave >= waves.Length)
        {
            Debug.Log("Encounter over: going to victory screen");
            victoryScreen();
            return;
        }
        StartCoroutine(waveTransition());
    }
    private IEnumerator waveTransition()
    {
        //Moved from next wave coroutine
        yield return new WaitForSeconds(1f);
        setPause(true);
        resetInterruptData();
        Debug.Log("starting wave: " + Wave.Title);
        //Delete old enemies
        foreach (Transform tr in transform)
        {
            Destroy(tr.gameObject);
        }
        //Clear UI for new wave
        uiManager.startWave();
		//Create enemies and wait until all enemies spawn in
		yield return StartCoroutine(createEnemies(Wave));
        //update Target Ret
        if (curr_wave == 0)
            uiManager.initTarget();
        //Play Wave transition and wait for the animation to finish (plays the SFX too)
        yield return StartCoroutine(uiManager.waveTransition(Wave.Title, curr_wave + 1, waves.Length));
        //Play music if applicable (BUG HERE WHERE LOADING MUSIC CAUSES LAG)
        if (Wave.Music != string.Empty)
            AudioPlayer.main.playMusic(Wave.Music);
        if (checkInterrupts() == false)
            setPause(false);
        wave_started = true;
    }
    // show victory screen after all waves are done
    public void victoryScreen()
    {
        //Transition to victoryScreen
        endBattle();
    }
    // end the battle and transition to the next GameflowItem
    public void endBattle()
    {
        pause = true;
        foreach (Enemy enemy in field.enemy_arr)
        {
            if (enemy != null) GameObject.Destroy(enemy.gameObject);
        }
		foreach (Transform tr in transform) 
		{
			Destroy (tr.gameObject);
		}
        uiManager.clear();
        GameflowManager.main.next();

    }

    //END MAIN BATTLE FLOW-------------------------------------------------------------------//

    //Handles a spellcast (by calling the castmanager) and clears callback's buffer if necessary
    public void handleSpellCast(string spell, TrackTyping callback)
    {
        castManager.attackCurrent(spell, callback);
    }
    //Handle player death
    public void playerDeath()
    {
        Debug.Log("The player has died!");
        StopAllCoroutines();
        castManager.StopAllCoroutines();
        uiManager.StopAllCoroutines();
        foreach (Enemy enemy in field.enemy_arr)
        {
            if (enemy != null) GameObject.Destroy(enemy.gameObject);
        }
        foreach(BattleWave w in waves)
        {
            foreach(BattleInterruptTrigger  e in  w.events)
            {
                e.HasTriggered = false;
            }
        }
        field.curr_dead = 0;
        field.enemy_count = 0;
        resetInterruptData();
        uiManager.clear();
        startBattle(curr_battle);
    }
    //Update Enemies and Check for death
    public void updateEnemies()
    {
        for (int i = 0; i < field.enemy_arr.Length; i++)
        {
            if (field.enemy_arr[i] != null && !field.enemy_arr[i].Is_dead)
            {
                field.enemy_arr[i].updateCondition();
                if (field.enemy_arr[i].Is_dead)
                    ++field.curr_dead;
            }
        }
        uiManager.updateUI();
        if (player.Is_dead)
        {
            playerDeath();
        }
        else if (field.curr_dead >= field.enemy_count && wave_started) // next wave if all enemies dead
        {
            Debug.Log("Wave: " + Wave.Title + " complete!");
            wave_started = false;
            nextWave();
        }
        else if (!checkInterrupts())
            setPause(false);
    }
    //Add Scene to trigger queue
    public void addSceneToQueue(GameObject interruptScene)
    {
        sceneQueue.Add(interruptScene);
    }
    //Play Dialogue scene from queue
    public bool playSceneFromQueue()
    {
        if (sceneQueue.Count > 0)
        {
            setPause(true);
            DialogueManager.main.setEnabled(true);
            DialogueManager.main.startInterrupt(sceneQueue[0]);
            sceneQueue.RemoveAt(0);
            return true;
        }
        return false;
    }

    //Create all enemies for this wave
	private IEnumerator createEnemies(BattleWave wave){
		if (wave.Enemy1 != string.Empty) {
			createEnemy (0, wave.Enemy1);
            AudioPlayer.main.playSFX ("sfx_blight_hit");
			AnimationPlayer.main.playAnimation("anim_element_reflect", field.enemy_arr[0].Transform.position, 2f);
			yield return new WaitForSeconds(0.4f);
		}
		if (wave.Enemy2 != string.Empty) {
			createEnemy (1, wave.Enemy2);
			AudioPlayer.main.playSFX ("sfx_blight_hit");
			AnimationPlayer.main.playAnimation("anim_element_reflect", field.enemy_arr[1].Transform.position, 2f);
			yield return new WaitForSeconds(0.4f);
		}
		if (wave.Enemy3 != string.Empty) {
			createEnemy (2, wave.Enemy3);
			AudioPlayer.main.playSFX ("sfx_blight_hit");
			AnimationPlayer.main.playAnimation("anim_element_reflect", field.enemy_arr[2].Transform.position, 2f);
			yield return new WaitForSeconds(0.4f);
		}
	}
    // creates the enemy specified at 'i' (0-left, 1-mid, 2-right) by the 'scene'
    private void createEnemy(int i, string name)
    {
        ++field.enemy_count;
        Enemy enemy = Instantiate(enemy_prefab, transform).GetComponent<Enemy>(); ;
        enemy.transform.localScale = new Vector3(1, 1, 1);
		Vector3 enemy_pos = new Vector3(i * BattleUI.enemy_spacing, BattleUI.enemy_y_offset, 0);
		enemy.transform.localPosition = enemy_pos;
        enemy.field = field; 
        enemy.castManager = castManager;
        enemy.initialize(enemyData.getData(name)); //sets enemy stats (AND INITITIALIZES ATTACKING AND AI)
        enemy.Position = i;      //Log enemy position in field
        enemy.bars = uiManager.charge_bars; //Give enemy access to charge_bars
        Vector3 bar_pos = enemy.transform.position + new Vector3(0, -1.75f, 0);
        uiManager.charge_bars.makeChargeMeter(i, bar_pos);
        uiManager.stagger_bars.makeStaggerMeter(i, bar_pos);
        uiManager.health_bars.makeHealthMeter(i, bar_pos);
        field.enemy_arr[i] = enemy;
		enemy.enemy_animator.Play ("enemy_spawn_in");
        uiManager.updateUI();
    }

    private bool checkInterrupts()
    {
        foreach (BattleEventTrigger e in Wave.events)
        {
            if (!e.HasTriggered && e.checkTrigger(field) && e.onTrigger(field))
            {

            }
        }
        if (!globalEvents.HasTriggered && globalEvents.checkTrigger(field) && globalEvents.onTrigger(field))
        {
        }
        //Play a scene if any are in the queue
        return playSceneFromQueue();
    }

    private void resetInterruptData()
    {

        field.time_started = System.DateTime.Now; // time battle started
        if (field.last_enemy_cast != null)
            field.last_enemy_cast.Clear(); // last performed cast action
        else
            field.last_enemy_cast = new List<CastData>();
        field.last_enemy_spell = new SpellData(""); // last performed spell
        if (field.last_player_cast != null)
            field.last_player_cast.Clear(); // last performed cast action
        else
            field.last_player_cast = new List<CastData>();
        field.last_enemy_spell = new SpellData(""); // last performed spell
        field.last_register = new bool[3] { false, false, false }; // last spell register status
        field.num_player_attacks = 0; // number of player attacks from beginning of battle
    }

}
