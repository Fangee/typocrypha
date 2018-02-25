using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffects : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public IEnumerator playEffects(CastData d, Transform pos)
    {
        for (int i = 0; i < d.animData.Length; ++i)
        {
            if (d.animData[i] != null)
            {
                AudioPlayer.main.setSFX(AudioPlayer.channel_spell_sfx, d.sfxData[i]);
                AudioPlayer.main.playSFX(AudioPlayer.channel_spell_sfx);
                AnimationPlayer.main.playAnimation(d.animData[i], pos.position, 1);
                yield return new WaitForSeconds(0.333F);
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
