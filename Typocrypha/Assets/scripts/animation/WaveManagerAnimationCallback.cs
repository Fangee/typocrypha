using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerAnimationCallback : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //GameObject.Find("/Managers/WaveManager").GetComponent<TypocryphaGameflow.WaveManager>().startCombat();
        TypocryphaGameflow.GameflowManager.main.managers.waveManager.startCombat();
    }

}
