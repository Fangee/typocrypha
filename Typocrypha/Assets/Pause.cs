using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
    
    bool gamePause;
    Color dimCol;
    string pauseKey = "escape";
    GameObject[] children;

    bool battlePause; //saves battlemanager pause state

    void Start () {
        gamePause = false;
        children = new GameObject[3];
        children[0] = transform.GetChild(0).gameObject; //music slider
        children[1] = transform.GetChild(1).gameObject; //sound slider
        children[2] = transform.GetChild(2).gameObject; //scroll slider

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
                foreach (GameObject g in children)
                    g.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                BattleManagerS.main.pause = battlePause;
                BattleEffects.main.dimmer.color = dimCol;
                AudioPlayer.main.unpauseSFX();
                foreach (GameObject g in children)
                    g.SetActive(false);
            }
        }
	}
}
