using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Manages Spell cooldowns and cooldown UI. contains private class CooldownBar, which handles the progress bar aspect
public class CooldownList : MonoBehaviour {

    public GameObject bar_prefab;

    List<CooldownBar> spells = new List<CooldownBar>();
    List<Ref<bool>> refs = new List<Ref<bool>>();
    List<Ref<float>> flots = new List<Ref<float>>();
    const int capacity = 5;
	const float bar_distance = 0.525F; //0.39F;

    //Returns if the list is full
    public bool isFull()
    {
        return spells.Count >= capacity;
    }
    //Add spell to the cooldown list
    //Precondition: isFull() = false;
    public bool add(string root, float cooldown, Ref<float> curr_time, Ref<bool> is_finished)
    {
        CooldownBar c = new CooldownBar(bar_prefab, transform, root, transform.position, curr_time, cooldown);
        c.bar.transform.Translate(Vector3.down * spells.Count * bar_distance, 0);
		c.bar.transform.Translate(Vector3.left * 0.25F, 0);
        spells.Add(c);
        refs.Add(is_finished);
        flots.Add(curr_time);
        StartCoroutine(timer(cooldown, curr_time, is_finished, c));
        return true;
    }
	//Update all progress bars
	void Update ()
    {
		foreach(CooldownBar c in spells)
        {
            c.Update();
        }
	}
    // keep track of time, update bars, and delete bars if done
    IEnumerator timer(float finish_time, Ref<float> curr_time, Ref<bool> is_finished, CooldownBar c)
    {
        is_finished.Obj = false;
        while (curr_time.Obj < finish_time)
        {
			yield return new WaitForEndOfFrame ();
			yield return new WaitWhile (() => BattleManager.main.pause);
			curr_time.Obj += Time.deltaTime;
        }
        spells.Remove(c);
        GameObject.Destroy(c.bar.gameObject);
        is_finished.Obj = true;
        curr_time.Obj = 0;
        rePosition();
    }
    //reposition bars (and later spell names after list collapses)
    private void rePosition()
    {
        int i = 0;
        foreach(CooldownBar c in spells)
        {
			c.bar.transform.position = new Vector3 (0, i * bar_distance, 0) + transform.position;
			c.bar.transform.Translate(Vector3.left * 0.25F, 0);
            i--;
        }
    }
	//remove all bars
	public void removeAll() {
		StopAllCoroutines ();
		foreach (CooldownBar c in spells) {
			GameObject.Destroy (c.bar.gameObject);
		}
        foreach(Ref<bool> b in refs) {
            b.Obj = true;
        }
        foreach(Ref<float> f in flots)
        {
            f.Obj = 0f;
        }
        refs.Clear();
        flots.Clear();
		spells.Clear ();
	}

    private class CooldownBar
    {
        public BarMeter bar;
        Ref<float> curr_time;
        float finish_time;

        public CooldownBar(GameObject bar_prefab, Transform t, string name, Vector3 world_pos, Ref<float> curr_time, float finish)
        {
            this.curr_time = curr_time;
            finish_time = finish;
            GameObject new_bar = GameObject.Instantiate(bar_prefab, t);
            new_bar.transform.localScale = new Vector3(1, 1, 1);
            new_bar.transform.position = world_pos;
            bar = new_bar.GetComponent<BarMeter>();
			bar.setText(name.ToUpper());
        }

        // Update is called once per frame
        public void Update()
        {
            bar.setValue(curr_time.Obj / finish_time);
        }
    }

}
