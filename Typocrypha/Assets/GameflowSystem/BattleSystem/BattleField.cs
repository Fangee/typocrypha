﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Encapsulates battle info
public class BattleField {

    //Properties

    public ICaster Player { get { return player_arr[player_ind]; } }
    public bool Pause { get { return callback.battlePause; } set { callback.battlePause = value; } }

    public enum FieldPosition
    {
        NONE = -2,
        ANY = -1,
        LEFT,
        MIDDLE,
        RIGHT,
        ALLYLEFT,
        PLAYER,
        ALLYRIGHT,
    }
    public ICaster getCasterFromFieldPosition(FieldPosition pos)
    {
        int index = (int)pos;
        ICaster target = null;
        if (index > 0)
        {
            if (index < 3)
                target = enemy_arr[index];
            else
                target = player_arr[index - 3];
        }
        return target;
    }
    public static bool isEnemy(FieldPosition pos)
    {
        return (pos == FieldPosition.LEFT || pos == FieldPosition.MIDDLE || pos == FieldPosition.RIGHT);
    }

    //BattleField data

	[HideInInspector] public Enemy[] enemy_arr = new Enemy[3]; // array of Enemy components (size 3)
	[HideInInspector] public int target_ind = 1; // index of currently targeted enemy
	[HideInInspector] public ICaster[] player_arr = { null, null, null }; // array of Player and allies (size 3)
	[HideInInspector] public int player_ind = 1;
	[HideInInspector] public int enemy_count = 0; // number of enemies in battle
	[HideInInspector] public int curr_dead = 0;

    //Interrupt stuff

	[HideInInspector] public System.DateTime time_started; // time battle started
    [HideInInspector] public FieldPosition lastCaster = FieldPosition.NONE;
    [HideInInspector] public List<CastData> last_enemy_cast; // last performed cast action
    [HideInInspector] public List<CastData> last_player_cast;
	[HideInInspector] public SpellData last_enemy_spell; // last performed spell
    [HideInInspector] public SpellData last_player_spell; // last performed spell
    [HideInInspector] public bool[] last_register; // last spell register status
	[HideInInspector] public int num_player_attacks; // number of player attacks from beginning of battle

    //CALLBACK FUNCTIONS

    private BattleManagerS callback;

    public BattleField(BattleManagerS callback)
    {
        this.callback = callback;
    }
    public void addSceneToQueue(GameObject interruptScene)
    {
        callback.addSceneToQueue(interruptScene);
    }
    public void update()
    {
        callback.updateEnemies();
    }
    public void updateScouterInfo()
    {
        callback.uiManager.updateScourterInfo();
    }
    public void breakThirdEye()
    {
        if(callback.thirdEyeActive)
            callback.stopThirdEye(true);
    }
}
