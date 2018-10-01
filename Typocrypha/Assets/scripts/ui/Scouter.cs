using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scouter : MonoBehaviour {

	public Image inputDisableImage;
    public BattleManagerS field;

    TextMesh left; //buff/debuff
    TextMesh right; //damage modifiers
    SpriteRenderer brackets;
    float alpha = 0f;
    float aDelta = .33f;
    bool isActive = false;

	void Awake () {
        left = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        right = transform.GetChild(1).gameObject.GetComponent<TextMesh>();
        brackets = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
	}

    //Show scouter
    public void show() {
        updateInfo();
        StopAllCoroutines();
        StartCoroutine(fadeIn());
        isActive = true;
    }

    //Hide scouter
    public void hide() {
        if (!isActive)
            return;
        StopAllCoroutines();
        StartCoroutine(fadeOut());
        isActive = false;
    }
    //Hide scouter
    public void hideImmediate()
    {
        if (!isActive)
            return;
        StopAllCoroutines();
        left.color = new Color(255f, 255f, 255f, 0);
        right.color = new Color(255f, 255f, 255f, 0);
        brackets.color = new Color(255f, 255f, 255f, 0);
		inputDisableImage.color = new Color(255f, 255f, 255f, 0);
        isActive = false;
    }

    public bool toggle()
    {
        if (isActive)
            hide();
        else
            show();
        return isActive;
    }

    //Update the info on the enemy
    public void updateInfo() {
        if (left == null) return; //not sure why left/right are null at start of battle, but this stops it from doing garbage
        //Targeted enemy
        //Enemy foe = field.field.enemies[field.field.Player.TargetPosition];
        ////Check if enemy exists
        //if (foe == null || foe.Is_dead)
        //{
        //    //You're not looking at anything you absolute moron
        //    left.text = "..\n..\n..\n..\n..";
        //    right.text = "..\n..\n..\n..\n..\n..";
        //}
        //else
        //{
        //    //Get those numbers
        //    left.text = 
        //        displayBuff(foe.BuffDebuff.attack)      + "\n" +
        //        displayBuff(foe.BuffDebuff.defense)     + "\n" +
        //        displayBuff(foe.BuffDebuff.speed)       + "\n" +
        //        displayBuff(foe.BuffDebuff.accuracy)    + "\n" +
        //        displayBuff(foe.BuffDebuff.evasion);
        //    right.text = 
        //        displayMod(foe, Elements.@null)   + "\n" +
        //        displayMod(foe, Elements.fire)    + "\n" +
        //        displayMod(foe, Elements.ice)     + "\n" +
        //        displayMod(foe, Elements.volt)    + "\n" + "\n";
           
        //}
    }

    //deals with signs
    private string displayBuff(float f) {
        if (f == 0) return "--";
        if (f > 0) return "+" + f;
        return f.ToString();
    }

    //shorten the modifier strings
    private string displayMod(ATB2.Enemy foe, int element) {

        if (EnemyIntel.main.hasIntel(foe.name, element))
        {
            Elements.vsElement modifier = Elements.vsElement.ANY;//Elements.getLevel(foe.Stats.vsElement[element]);
            switch (modifier)
            {
                case Elements.vsElement.REPEL:
                    return "Rp";
                case Elements.vsElement.DRAIN:
                    return "Dr";
                case Elements.vsElement.BLOCK:
                    return "Bl";
                case Elements.vsElement.RESIST:
                    return "Rs";
                case Elements.vsElement.NEUTRAL:
                    return "--";
                case Elements.vsElement.WEAK:
                    return "Wk";
                case Elements.vsElement.SUPERWEAK:
                    return "Wk";
            }
        }
        return "??";
    }

    //makes the scanner gradually visible
    private IEnumerator fadeIn() {
        while (alpha < 1)
        {
            alpha += aDelta;
            left.color = new Color(255f, 255f, 255f, alpha);
            right.color = new Color(255f, 255f, 255f, alpha);
            brackets.color = new Color(255f, 255f, 255f, alpha);
			inputDisableImage.color = new Color(255f, 255f, 255f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    //makes the scanner gradually invisible
    private IEnumerator fadeOut() {
        while (alpha > 0)
        {
            alpha -= aDelta;
            left.color = new Color(255f, 255f, 255f, alpha);
            right.color = new Color(255f, 255f, 255f, alpha);
            brackets.color = new Color(255f, 255f, 255f, alpha);
			inputDisableImage.color = new Color(255f, 255f, 255f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
}
