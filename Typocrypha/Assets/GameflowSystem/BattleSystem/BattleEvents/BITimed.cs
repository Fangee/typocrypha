using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BITimed : BattleInterruptTrigger{
    public float seconds;
    private float curr_time;
    private bool finished = false;
    private bool started = false;
    public override bool checkTrigger(BattleField state)
    {
        if (seconds == 0)
            return true;
        if(!finished)
        {
            if (!started)
                StartCoroutine(timer());
            return false;
        }
        return true;
    }
    IEnumerator timer()
    {
        while (!finished)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => BattleManagerS.main.pause);
            curr_time += Time.deltaTime;
            if (curr_time > seconds)
                finished = true;
        }
    }
}
