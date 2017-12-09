using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that manages simple timers for non-monobehaivior classes
//To start a timer, call newTimer(float finish_time, ref float curr_time, ref bool is_finished)
//The state of the timer is linked to the calling class by ref float curr_time and ref bool is_finished
public class Timer : MonoBehaviour {

	public void newTimer(float finish_time, Ref<float> curr_time, Ref<bool> is_finished)
    {
        StartCoroutine(timer(finish_time, curr_time, is_finished));
    }

    // keep track of time, and attack whenever curr_time = atk_time
    IEnumerator timer(float finish_time, Ref<float> curr_time, Ref<bool> is_finished)
    {
        is_finished.Obj = false;
        while(curr_time.Obj < finish_time)
        {
            yield return new WaitForSeconds(0.1f);
            curr_time.Obj += 0.1f;
        }
        is_finished.Obj = true;
        curr_time.Obj = 0;
    }

}

