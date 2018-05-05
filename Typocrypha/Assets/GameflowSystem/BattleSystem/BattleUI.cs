using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public EnemyChargeBars charge_bars; // creates and mananges charge bars
    public EnemyStaggerBars stagger_bars; // creates and manages stagger bars
    public EnemyHealthBars health_bars; // creates and manages enemy health bars
    public BattleLog battle_log;
    public GameObject target_ret; // contains targetting sprites
    public GameObject target_floor; // holds the enemy floor panels
                                    //public GameObject dialogue_box; // text box for dialogue
    public GameObject battle_bg_prefab; // prefab of battle background

    TargetReticule target_ret_scr; // TargetReticule script ref
    TargetFloor target_floor_scr;  // TargetFloor script ref

    public const float enemy_spacing = 6f; // horizontal space between enemies
    public const float enemy_y_offset = 0.5f; // offset of enemy from y axis
    public const float reticule_y_offset = 2.5f; // offset of target reticule

    private const int initial_target_ind = 1;

    // configure script refs
    void Start()
    {
        target_ret_scr = target_ret.GetComponent<TargetReticule>();
        target_floor_scr = target_floor.GetComponent<TargetFloor>();
    }
    //Initialize the battle bg
    public void initBg()
    {
        //Set background
        BackgroundEffects.main.setPrefabBG(battle_bg_prefab);
    }
    //Initialize the targeting and target floor UI
    public void initTarget()
    {
		//Show targeting UI
		target_ret.SetActive(true);
		target_floor.SetActive (true);
		//Update UI
		target_ret_scr.updateTarget(new Vector2((initial_target_ind - 1) * enemy_spacing, reticule_y_offset));
		target_floor_scr.updateFloor();
    }
    //Start the UI for a new wave
    public void startWave()
    {
        charge_bars.removeAll();
        stagger_bars.removeAll();
        health_bars.removeAll();
        charge_bars.initChargeBars();
        stagger_bars.initStaggerBars();
        health_bars.initHealthBars();
    }
    //Update targeting UI floor UI, and charge bars
    public void updateUI()
    {
        //Update target and floor effects
        target_ret_scr.updateTarget();
        target_floor_scr.updateFloor();
		charge_bars.updateChargeBars ();
    }
    //Sets the target from specified index
    public void setTarget(int target_ind)
    {
		target_ret_scr.updateTarget(new Vector2((target_ind - 1) * enemy_spacing, reticule_y_offset));
        target_floor_scr.updateFloor();
        // play sfx
        AudioPlayer.main.playSFX("sfx_enemy_select");
    }
    //Toggles if the scouter is on or not
    public void toggleScouter()
    {
        target_ret_scr.toggleScouter();
    }
    //Clears all enemy UI and battle bg
    public void clear()
    {
        charge_bars.removeAll();
        stagger_bars.removeAll();
        health_bars.removeAll();
        target_ret.SetActive(false);
        target_floor.SetActive(false);
        BackgroundEffects.main.removePrefabBG(2.0f);
    }
}