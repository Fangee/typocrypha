using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerS : MonoBehaviour {
    public static BattleManagerS main = null;
    public BattleField field = new BattleField();
    private int curr_wave = -1;
    private Wave[] waves;

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
        waves = new_battle.GetComponents<Wave>();
        nextWave();
    }
    public void nextWave()
    {
        if(++curr_wave >= waves.Length)
        {
            victoryScreen();
            return;
        }
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
}
