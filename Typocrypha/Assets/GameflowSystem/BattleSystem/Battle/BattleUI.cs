﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public EnemyChargeBars charge_bars; // creates and mananges charge bars
    public EnemyStaggerBars stagger_bars; // creates and manages stagger bars
    public EnemyHealthBars health_bars; // creates and manages enemy health bars
    public GameObject target_ret; // contains targetting sprites
    public GameObject target_floor; // holds the enemy floor panels
                                    //public GameObject dialogue_box; // text box for dialogue
    public GameObject battle_bg_prefab; // prefab of battle background

    TargetReticule target_ret_scr; // TargetReticule script ref
    TargetFloor target_floor_scr;  // TargetFloor script ref

    public const float enemy_spacing = 6f; // horizontal space between enemies
    public const float enemy_y_offset = 0.5f; // offset of enemy from y axis
    public const float reticule_y_offset = 1.5f; // offset of target reticule
    //const int undim_layer = -1; // layer of enemy when enemy sprite is shown
    //const int dim_layer = -5;   // layer of enemy when enemy sprite is dimmed
    private const int initial_target_ind = 1;

    // Use this for initialization
    void Start()
    {
        target_ret_scr = target_ret.GetComponent<TargetReticule>();
        target_floor_scr = target_floor.GetComponent<TargetFloor>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initialize()
    {
        charge_bars.initChargeBars();
        stagger_bars.initStaggerBars();
        health_bars.initHealthBars();
        target_ret_scr.updateTarget(new Vector2(initial_target_ind * enemy_spacing, reticule_y_offset));
        target_ret.transform.localPosition = new Vector2(initial_target_ind * enemy_spacing, reticule_y_offset);
    }

    public void updateUI()
    {
        //Update target and floor effects
        target_ret_scr.updateTarget();
        target_floor_scr.updateFloor();
    }

    public void setTarget(int target_ind)
    {
        target_ret_scr.updateTarget(new Vector2(target_ind * enemy_spacing, reticule_y_offset));
        target_floor_scr.updateFloor();
        // play sfx
        AudioPlayer.main.playSFX("sfx_enemy_select");
    }

    public void toggleScouter()
    {
        target_ret_scr.toggleScouter();
    }

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