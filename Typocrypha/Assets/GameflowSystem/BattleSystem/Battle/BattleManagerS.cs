using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerS : MonoBehaviour {
    public static BattleManagerS main = null;
    public BattleField field = new BattleField();
    private BattleWave Wave { get { return waves[curr_wave]; } }
    private int curr_wave = -1;
    private BattleWave[] waves;

    private void Awake()
    {
        if (main == null)
            main = this;
        else Destroy(this);
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
            victoryScreen();
            return;
        }
        createEnemies(Wave);
        waveTransition(Wave.Title);
        nextWave();
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



    private void createEnemies(BattleWave wave)
    {
        
    }
    // creates the enemy specified at 'i' (0-left, 1-mid, 2-right) by the 'scene'
    private void createEnemy(int i, string name)
    {
        if (scene.enemy_stats[i] == null)
            return;
        ++battle_field.enemy_count;
        GameObject new_enemy = GameObject.Instantiate(enemy_prefab, transform);
        new_enemy.transform.localScale = new Vector3(1, 1, 1);
        new_enemy.transform.localPosition = new Vector3(i * enemy_spacing, enemy_y_offset, 0);
        battle_field.enemy_arr[i] = new_enemy.GetComponent<Enemy>();
        battle_field.enemy_arr[i].field = this; //Give enemy access to field (for calling spellcasts)
        battle_field.enemy_arr[i].initialize(scene.enemy_stats[i]); //sets enemy stats (AND INITITIALIZES ATTACKING AND AI)
        battle_field.enemy_arr[i].Position = i;      //Log enemy position in field
        battle_field.enemy_arr[i].bars = charge_bars; //Give enemy access to charge_bars
        Vector3 bar_pos = new_enemy.transform.position + new Vector3(-0.5f, -1.0f, 0);
        charge_bars.makeChargeMeter(i, bar_pos);
        stagger_bars.makeStaggerMeter(i, bar_pos);
        health_bars.makeHealthMeter(i, bar_pos);
    }
    private void waveTransition(string Title)
    {

    }
}
