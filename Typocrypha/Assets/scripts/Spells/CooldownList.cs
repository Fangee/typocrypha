using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Manages Spell cooldowns and cooldown UI. contains private class CooldownBar, which handles the progress bar aspect
public class CooldownList : MonoBehaviour {

    public GameObject bar_prefab;

    List<CooldownBar> cooldownBars = new List<CooldownBar>();
    HashSet<string> onCooldown = new HashSet<string>();
    const int capacity = 5;
	const float bar_distance = 0.525F; //0.39F;

    //Returns if the list is full
    public bool isFull()
    {
        return cooldownBars.Count >= capacity;
    }
    public bool isOnCooldown(SpellData s)
    {
        return onCooldown.Contains(s.root);
    }
    //Add spell to the cooldown list
    //Precondition: isFull() = false;
    public bool add(SpellData s, float cooldownTime)
    {
        string wordOnCooldown = s.root;
        onCooldown.Add(wordOnCooldown);
        CooldownBar c = new CooldownBar(bar_prefab, transform, transform.position, wordOnCooldown, cooldownTime);
        c.bar.transform.Translate(Vector3.down * cooldownBars.Count * bar_distance, 0);
		c.bar.transform.Translate(Vector3.left * 0.25F, 0);
        cooldownBars.Add(c);
        StartCoroutine(timer(cooldownTime, wordOnCooldown, c));
        return true;
    }
    // keep track of time, update bars, and delete bars if done
    IEnumerator timer(float finish_time, string wordOnCooldown, CooldownBar c)
    {
        float curr_time = 0f;
        while (curr_time < finish_time)
        {
			yield return new WaitForEndOfFrame ();
			yield return new WaitWhile (() => BattleManagerS.main.battlePause);
			curr_time += Time.deltaTime;
            c.Update(curr_time);
        }
        onCooldown.Remove(wordOnCooldown);
        cooldownBars.Remove(c);
        GameObject.Destroy(c.bar.gameObject);
        rePosition();
    }
    //reposition bars (and later spell names after list collapses)
    private void rePosition()
    {
        int i = 0;
        foreach(CooldownBar c in cooldownBars)
        {
			c.bar.transform.position = new Vector3 (0, i * bar_distance, 0) + transform.position;
			c.bar.transform.Translate(Vector3.left * 0.25F, 0);
            i--;
        }
    }
	//remove all bars
	public void removeAll() {
		StopAllCoroutines ();
		foreach (CooldownBar c in cooldownBars) {
			GameObject.Destroy (c.bar.gameObject);
		}
		cooldownBars.Clear ();
        onCooldown.Clear();
	}

    private class CooldownBar
    {
        public BarMeter bar;
        float finish_time;

        public CooldownBar(GameObject bar_prefab, Transform t, Vector3 world_pos, string name, float finishTime)
        {
            finish_time = finishTime;
            GameObject new_bar = GameObject.Instantiate(bar_prefab, t);
            new_bar.transform.localScale = new Vector3(1, 1, 1);
            new_bar.transform.position = world_pos;
            bar = new_bar.GetComponent<BarMeter>();
			bar.setText(name.ToUpper());
        }

        // Update is called once per frame
        public void Update(float curr_time)
        {
            bar.setValue(curr_time / finish_time);
        }
    }

}
