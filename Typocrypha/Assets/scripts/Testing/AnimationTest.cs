using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    public bool CompletionTriggers = true;
    public float speed = 1f;
    public List<SpellAnimationData> clips;
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
                if (CompletionTriggers)
                 {
                    AnimationPlayer.CompletionData data = null;
                    foreach (var anim in clip.animations)
                        data = AnimationPlayer.main.playAnimation(anim, new Vector2(0, 0), speed);
                    if(data != null)
                        yield return new WaitUntilAnimationComplete(data);
                }  
                else
                {
                    AnimationPlayer.CompletionData data = null;
                    foreach (var anim in clip.animations)
                        data = AnimationPlayer.main.playAnimation(anim, new Vector2(0, 0), speed);
                    if (data != null)
                        yield return new WaitForSeconds(data.time);
                }
            }
        }
    }
}
