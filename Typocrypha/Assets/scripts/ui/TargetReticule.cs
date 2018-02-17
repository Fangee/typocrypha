using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages target reticule effects and position
public class TargetReticule : MonoBehaviour {
	public Transform target_tr; // transform of target reticule

	float r_speed; // speed of rotation
	Vector2 vel; // velocity of target

	const float base_r_speed = 0.1f; // default speed of rotation
	const float max_h_speed = 100f; // maximum horizontal speed
	const float smooth_time = 0.15f; // time it takes for target to reach position
	const float h_r_scale = 8f; // scaling factor from horizontal speed to rotation speed

	void Start () {
		r_speed = base_r_speed;
	}
	
	// Update is called once per frame
	void Update () {
		target_tr.RotateAround (target_tr.position, Vector3.back, r_speed);
		target_tr.localPosition = Vector2.SmoothDamp (target_tr.localPosition, BattleManager.main.target_pos, ref vel, smooth_time, max_h_speed, Time.deltaTime);
		r_speed = base_r_speed + vel.magnitude / h_r_scale;
	}
}
