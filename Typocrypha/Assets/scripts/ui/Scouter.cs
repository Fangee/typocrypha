using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scouter : MonoBehaviour {

    TextMesh left; //buff/debuff
    TextMesh right; //damage modifiers
    SpriteRenderer brackets;
    float alpha = 0f;
    float aDelta = .33f;

	void Awake () {
        left = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        right = transform.GetChild(1).gameObject.GetComponent<TextMesh>();
        brackets = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
	}

    //Show scouter
    public void Show() {
        updateInfo();
        StopAllCoroutines();
        StartCoroutine(fadeIn());
    }

    //Hide scouter
    public void Hide() {
        StopAllCoroutines();
        StartCoroutine(fadeOut());
    }

    //Update the info on the enemy
    public void updateInfo() {
        if (left == null) return; //not sure why left/right are null at start of battle, but this stops it from doing garbage
        //Targeted enemy
        Enemy foe = BattleManager.main.enemy_arr[BattleManager.main.target_ind];
        //Check if enemy exists
        if (foe == null || foe.Is_dead)
        {
            //You're not looking at anything you absolute moron
            left.text = "??\n??\n??\n??\n??";
            right.text = "??\n??\n??\n??\n??\n??";
        }
        else
        {
            //Get those numbers
            left.text = 
                stylize(foe.BuffDebuff.attack)      + "\n" +
                stylize(foe.BuffDebuff.defense)     + "\n" +
                stylize(foe.BuffDebuff.speed)       + "\n" +
                stylize(foe.BuffDebuff.accuracy)    + "\n" +
                stylize(foe.BuffDebuff.evasion);
            right.text = 
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.@null])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.fire])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.ice])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.volt])) + "\n" + "\n";
           
        }
    }

    //deals with signs
    private string stylize(float f) {
        if (f == 0) return "--";
        if (f > 0) return "+" + f;
        return f.ToString();
    }

    //shorten the modifier strings
    private string abbreviate(Elements.vsElement modifier) {
        switch (modifier)
        {
            case Elements.vsElement.REPEL:
                return "RP";
            case Elements.vsElement.DRAIN:
                return "DR";
            case Elements.vsElement.BLOCK:
                return "BL";
            case Elements.vsElement.RESIST:
                return "RS";
            case Elements.vsElement.NEUTRAL:
                return "--";
            case Elements.vsElement.WEAK:
                return "WK";
            case Elements.vsElement.SUPERWEAK:
                return "SW";
        }
        return "UNKNOWN MODIFIER";
    }

    private IEnumerator fadeIn() {
        while (alpha < 1)
        {
            alpha += aDelta;
            left.color = new Color(255f, 255f, 255f, alpha);
            right.color = new Color(255f, 255f, 255f, alpha);
            brackets.color = new Color(255f, 255f, 255f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator fadeOut() {
        while (alpha > 0)
        {
            alpha -= aDelta;
            left.color = new Color(255f, 255f, 255f, alpha);
            right.color = new Color(255f, 255f, 255f, alpha);
            brackets.color = new Color(255f, 255f, 255f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
}
