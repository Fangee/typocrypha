using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerS : MonoBehaviour {
    public static BattleManagerS main = null;
    public BattleUI uiManager;
    public CastManager castManager;
    public EnemyDatabase enemyData;
    public AllyDatabase allyData;
    public GameObject enemy_prefab; // prefab for enemy object
    public GameObject ally_prefab; //prefab for ally object
    public BattleField field = new BattleField();
    [HideInInspector] public bool pause;

    private BattleWave Wave { get { return waves[curr_wave]; } }
    private int curr_wave = -1;
    private BattleWave[] waves;

    //const int undim_layer = -1; // layer of enemy when enemy sprite is shown
    //const int dim_layer = -5;   // layer of enemy when enemy sprite is dimmed

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

    public void setEnabled(bool e)
    {
        enabled = e;
    }

    // start battle scene
    public void startBattle(GameObject new_battle)
    {
        curr_wave = -1;
        waves = new_battle.GetComponents<BattleWave>();
        nextWave();
    }
    public void nextWave()
    {
        if(++curr_wave >= waves.Length)
        {
            Debug.Log("Encounter over: going to victory screen");
            victoryScreen();
            return;
        }
        Debug.Log("starting wave: " + Wave.Title);
        uiManager.initialize();
        createEnemies(Wave);
        waveTransition(Wave.Title);
        //nextWave();
        //Initialize next wave and do transition here
    }
    public void victoryScreen()
    {
        //Transition to victoryScreen
        endBattle();
    }
    public void endBattle()
    {
        GameflowManager.main.next();
    }

    public void handleSpellCast(string spell, TrackTyping callback)
    {
        castManager.attackCurrent(spell, callback);
    }
    public void playerDeath()
    {
        Debug.Log("The player has died!");
    }

    private void createEnemies(BattleWave wave)
    {
        if (wave.Enemy1 != string.Empty)
            createEnemy(0, wave.Enemy1);
        if (wave.Enemy2 != string.Empty)
            createEnemy(1, wave.Enemy2);
        if (wave.Enemy3 != string.Empty)
            createEnemy(2, wave.Enemy3);
    }
    // creates the enemy specified at 'i' (0-left, 1-mid, 2-right) by the 'scene'
    private void createEnemy(int i, string name)
    {
        ++field.enemy_count;
        Enemy enemy = Instantiate(enemy_prefab, transform).GetComponent<Enemy>(); ;
        enemy.transform.localScale = new Vector3(1, 1, 1);
        enemy.transform.localPosition = new Vector3(i * BattleUI.enemy_spacing, BattleUI.enemy_y_offset, 0);
        enemy.field = field; 
        enemy.castManager = castManager;
        enemy.initialize(enemyData.getData(name)); //sets enemy stats (AND INITITIALIZES ATTACKING AND AI)
        enemy.Position = i;      //Log enemy position in field
        enemy.bars = uiManager.charge_bars; //Give enemy access to charge_bars
        Vector3 bar_pos = enemy.transform.position + new Vector3(-0.5f, -1.0f, 0);
        uiManager.charge_bars.makeChargeMeter(i, bar_pos);
        uiManager.stagger_bars.makeStaggerMeter(i, bar_pos);
        uiManager.health_bars.makeHealthMeter(i, bar_pos);
        field.enemy_arr[i] = enemy;
    }
    private void waveTransition(string Title)
    {
        uiManager.clear();
    }
}
