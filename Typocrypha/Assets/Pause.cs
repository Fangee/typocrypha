using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
    
    bool gamePause;
    Color dimCol;
    string pauseKey = "escape";
    GameObject hideChild;

    bool battlePause; //saves battlemanager pause state

    void Start () {
        gamePause = false;
		hideChild = transform.GetChild(0).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(pauseKey))
        {
            gamePause = !gamePause;

            if (gamePause)
            {
                Time.timeScale = 0;
                battlePause = BattleManagerS.main.pause;
                BattleManagerS.main.pause = true;
                dimCol = BattleEffects.main.dimmer.color;
                BattleEffects.main.setDim(true);
                AudioPlayer.main.pauseSFX();
				hideChild.SetActive (true);
            }
            else
            {
                Time.timeScale = 1;
                BattleManagerS.main.pause = battlePause;
                BattleEffects.main.dimmer.color = dimCol;
                AudioPlayer.main.unpauseSFX();
				hideChild.SetActive (false);
            }
        }
	}
}
