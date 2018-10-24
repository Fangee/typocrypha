using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    public bool CompletionTriggers = true;
    public float speed = 1f;
    public AnimationClip[] clips;

	// Use this for initialization
	void Start () {
        StartCoroutine(play());
	}

    IEnumerator play()
    {
        while (true)
        {
            foreach (var clip in clips)
            {
                if(CompletionTriggers)
                    yield return new WaitUntilAnimationComplete(AnimationPlayer.main.playAnimation(clip, new Vector2(0, 0), speed));
                else
                    yield return new WaitForSeconds(AnimationPlayer.main.playAnimation(clip, new Vector2(0, 0), speed).time);
            }
        }
    }
}
