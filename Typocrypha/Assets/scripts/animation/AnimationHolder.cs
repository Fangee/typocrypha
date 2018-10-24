using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHolder : MonoBehaviour {

    public AnimationPlayer.CompletionData completionData = new AnimationPlayer.CompletionData();
    public void AnimationComplete()
    {
        completionData.keepPlaying = false;
    }
}
