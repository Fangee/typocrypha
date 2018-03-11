using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages target reticule effects and position
public class TargetReticule : MonoBehaviour {
	public Transform base_tr;  // transform of base object
	public Transform inner_tr; // transform of inner target reticule
	public Transform outer_tr; // transform of outer target reticule
	public SpriteRenderer left_arrow;  // left arrow key sprite
	public SpriteRenderer right_arrow; // right arrow key sprite
	public SpriteRenderer no_target;   // displayed when no active target

	float base_r_speed; // base rotation speed
	float r_speed; // actual speed of rotation
	Vector2 vel; // velocity of target

	public float def_r_speed = 0.15f; // default speed of rotation
	public float slow_r_speed = 0.01f; // default slow rotation speed 
	public float max_h_speed = 100f; // maximum horizontal speed
	public float smooth_time = 0.15f; // time it takes for target to reach position
	public float h_r_scale = 8f; // scaling factor from horizontal speed to rotation speed

	Scouter scouter; //script for targeting info

	void Start () {
		base_r_speed = def_r_speed;
		r_speed = base_r_speed;

        scouter = transform.GetChild(0).gameObject.GetComponent<Scouter>();
	}
	
	// Update is called once per frame
	void Update () {
		// slide target
		base_tr.localPosition = Vector2.SmoothDamp (base_tr.localPosition, BattleManager.main.target_pos, ref vel, smooth_time, max_h_speed, Time.deltaTime);
		// rotate target rings
		inner_tr.RotateAround (inner_tr.position, Vector3.back, r_speed);
		outer_tr.RotateAround (outer_tr.position, -1 * Vector3.back, r_speed);
		// scale rotation with velocity
		r_speed = base_r_speed + vel.magnitude / h_r_scale;
	}

    //reveals info on enemy
    public void showScouter() {
        scouter.Show();
    }

    //hide enemy info
    public void hideScouter() {
        scouter.Hide();
    }

	// updates target based on targetted enemy
	public void updateTarget() {
		// don't update if battle not set up yet
		if (BattleManager.main.target_ind >= BattleManager.main.enemy_arr.Length) return;
		// show/hide left/right arrow key indicators based on position
		switch (BattleManager.main.target_ind) {
			case 0:
				left_arrow.enabled = false;
				right_arrow.enabled = true;
				break;
			case 1:
				left_arrow.enabled = true;
				right_arrow.enabled = true;
				break;
			case 2:
				left_arrow.enabled = true;
				right_arrow.enabled = false;
				break;
		}
		// check if no enemy targeted
		if (BattleManager.main.enemy_arr[BattleManager.main.target_ind] == null || BattleManager.main.enemy_arr [BattleManager.main.target_ind].Is_dead) {
			no_target.enabled = true;
			base_r_speed = 0f;
		} else {
			no_target.enabled = false;
			base_r_speed = def_r_speed;
		}
        scouter.updateInfo();
	}
}
