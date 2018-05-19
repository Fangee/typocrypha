using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
    
    bool gamePause;
    Color dimCol;
    string pauseKey = "escape";
    GameObject hideChild;
    bool battlePause = false; //saves battlemanager pause state
    bool blockTextboxInput = false;

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
                BattleManagerS.main.setPause(true);
                dimCol = BattleEffects.main.dimmer.color;
                BattleEffects.main.setDim(true);
                blockTextboxInput = DialogueManager.main.block_input;
                DialogueManager.main.block_input = true;
                AudioPlayer.main.pauseSFX();
				hideChild.SetActive (true);
            }
            else
            {
                Time.timeScale = 1;
                BattleManagerS.main.setPause(battlePause);
                BattleEffects.main.dimmer.color = dimCol;
                DialogueManager.main.block_input = blockTextboxInput;
                AudioPlayer.main.unpauseSFX();
				hideChild.SetActive (false);
            }
        }
	}
}
