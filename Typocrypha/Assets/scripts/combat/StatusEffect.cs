using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A class that processes an input key
public abstract class StatusEffect {
    public bool isNormal = false;
    public abstract string processKey(char key);
    public abstract IEnumerator keyGraphics(char key, Image image);
    //return true if status should be reset to normal
    public virtual bool update() { return false; }
}

public class StatusNormal : StatusEffect
{
	Sprite normal_key;
	public StatusNormal(Sprite sp)
    {
		normal_key = sp;
        isNormal = true;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
		image.sprite = normal_key;
        image.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        image.color = Color.gray;
    }

    public override string processKey(char key)
    {
		AudioPlayer.main.playSFX ("sfx_type_key");
        return key.ToString();
    }
}

public class StatusFreeze : StatusEffect
{
	int freezeNum = Random.Range(1,5);
	Sprite[] frozen_keys;
	public StatusFreeze(Sprite[] sp){
		this.frozen_keys = sp;
	}
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.white;
		switch (freezeNum) {
		case 1:
			image.sprite = frozen_keys[3]; 
			break;
		case 2:
			image.sprite = frozen_keys[2]; 
			break;
		case 3:
			image.sprite = frozen_keys[1]; 
			break;
		case 4:
			image.sprite = frozen_keys[0]; 
			break;
		}
        yield break;
    }

    public override string processKey(char key)
    {
        return "";
    }
    public override bool update()
    {
        --freezeNum;
        if(freezeNum == 0)
            AudioPlayer.main.playSFX("sfx_status_frozen_key_break");
        else
            AudioPlayer.main.playSFX("sfx_status_frozen_key_hit");
        return freezeNum <= 0;
    }
}

public class StatusBurn : StatusEffect
{
    int dmg = 2;
    Player p;
    public StatusBurn(Player p)
    {
        this.p = p;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.red;
        yield break;
    }

    public override string processKey(char key)
    {
		if (p.Curr_hp - dmg <= 0) {
			p.Curr_hp = 1;
		} else {
			p.Curr_hp -= dmg;
		}
		AudioPlayer.main.playSFX ("sfx_spell_hit");
        return key.ToString();
    }
}

public class StatusShock : StatusEffect
{
    char swapped;
    public StatusShock(char swap)
    {
        swapped = swap;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.yellow;
        yield break;
    }

    public override string processKey(char key)
    {
        return swapped.ToString();
    }
}



