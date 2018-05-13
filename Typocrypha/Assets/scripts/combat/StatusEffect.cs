using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A class that processes an input key
public abstract class StatusEffect {
    public bool isNormal = false;
    public abstract string processKey(char key);
    public abstract IEnumerator keyGraphics(char key, Image image, Text text);
    //Resets all aspects of the key from given data (most things are actually reset in the StatusNormal constructor)
    //The only things you really need to add here are if you change the key text
    public virtual void reset(char key, Image image, Text text) { }
    //return true if status should be reset to normal
    public virtual bool update() { return false; }
}

public class StatusNormal : StatusEffect
{
	public StatusNormal(Sprite sp, Image image, Text text)
    {
        image.sprite = sp;
        text.color = Color.white;
        image.color = Color.gray;
        isNormal = true;
    }
    public override IEnumerator keyGraphics(char key, Image image, Text text)
    {
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
	Popper player_popper;
	public StatusFreeze(Sprite[] sp, Popper popp, char c){
		frozen_keys = sp;
		player_popper = popp;
	}
    public override IEnumerator keyGraphics(char key, Image image, Text text)
    {
        image.color = Color.white;
        text.color = Color.white;
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
        --freezeNum;
        if (freezeNum == 0)
        {
            AudioPlayer.main.playSFX("sfx_status_frozen_key_break");
            player_popper.spawnText("<color=lime>" + key + "</color> <color=aqua>THAWED</color>", 1.0f, new Vector3(0.0f, -0.5f, 0.0f));
        }
        else
        {
            AudioPlayer.main.playSFX("sfx_status_frozen_key_hit");
            player_popper.spawnText("<color=red>" + key + "</color> <color=aqua>FROZEN</color>", 1.0f, new Vector3(0.0f, -0.5f, 0.0f));
        }
        return "";
    }
    public override bool update()
    {
        return freezeNum <= 0;
    }
}

public class StatusBurn : StatusEffect
{
    const int dmg = 10;
    Player p;
	Popper player_popper;
	public StatusBurn(Player p, Popper popp, char c)
    {
        this.p = p;
		this.player_popper = popp;
    }
    public override IEnumerator keyGraphics(char key, Image image, Text text)
    {
        text.color = Color.red;
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
		player_popper.spawnText ("<color=red>"+key+"</color> <color=orange>BURN</color>", 1.0f, new Vector3(0.0f,0.0f,0.0f));
		player_popper.spawnText (dmg+"", 1.0f, new Vector3(0.0f,-1.0f,0.0f));
		AudioPlayer.main.playSFX ("sfx_spell_hit");
        return key.ToString();
    }
}

public class StatusShock : StatusEffect
{
    char swapped;
	char original;
	Popper player_popper;
	public StatusShock(char swap, char origin, Popper popp)
    {
        swapped = swap;
		this.original = origin;
		this.player_popper = popp;
    }
    public override IEnumerator keyGraphics(char key, Image image, Text text)
    {
        text.color = Color.yellow;
        image.color = Color.yellow;
        text.text = swapped.ToString();
        yield break;
    }
    public override string processKey(char key)
    {
		AudioPlayer.main.playSFX("sfx_botch");
		player_popper.spawnText ("<color=red>" + this.original + "</color> <color=yellow><===></color> <color=lime>" + swapped + "</color>", 1.0f, new Vector3 (0.0f, -0.5f, 0.0f));
        return swapped.ToString();
    }

    public override void reset(char key, Image image, Text text)
    {
        base.reset(key, image, text);
        text.text = key.ToString();
    }
}



