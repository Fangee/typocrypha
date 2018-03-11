using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scouter : MonoBehaviour {

    TextMesh left;
    TextMesh right;

	// Use this for initialization
	void Awake () {
        left = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        right = transform.GetChild(1).gameObject.GetComponent<TextMesh>();
	}

    //Show scouter
    public void Show() {
        gameObject.SetActive(true);
        updateInfo();
    }

    //Hide scouter
    public void Hide() {
        gameObject.SetActive(false);
    }

    //Update the info on the enemy
    public void updateInfo() {
        //Targeted enemy
        Enemy foe = BattleManager.main.enemy_arr[BattleManager.main.target_ind];
        //Check if enemy exists
        if (foe == null || foe.Is_dead)
        {
            //You're not looking at anything you moron
            left.text = "??";
            right.text = "??";
        }
        else if (left != null)
        {
            //Get those numbers
            left.text = 
                foe.Stats.attack.ToString() + "\n" +
                foe.Stats.defense.ToString() + "\n" +
                foe.Stats.speed.ToString() + "\n" +
                foe.Stats.accuracy.ToString() + "\n" +
                foe.Stats.evasion.ToString();
            right.text = 
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.@null])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.fire])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.ice])) + "\n" +
                abbreviate(Elements.getLevel(foe.Stats.vsElement[Elements.volt])) + "\n" + "\n";
           
        }
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
        return "??";
    }
}
