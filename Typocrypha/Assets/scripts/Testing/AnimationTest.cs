using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    public float speed = 1f;
    public AnimationClip[] clips;

	// Use this for initialization
	void Start () {
        StartCoroutine(play());
	}

    IEnumerator play()
    {
        foreach(var clip in clips)
        {
            yield return new WaitForSeconds(AnimationPlayer.main.playAnimation(clip, new Vector2(0, 0), speed));
        }

    }
}
