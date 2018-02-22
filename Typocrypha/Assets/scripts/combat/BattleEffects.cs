using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// edits by James Iwamasa

// causes various effects for battle scenes
public class BattleEffects : MonoBehaviour {
	public static BattleEffects main = null; // static global ref
	public Transform cam_pos; // main camera
	public SpriteRenderer dimmer; // dimmer image
	public Canvas canvas; // canvas component
	public Animator camera_animator; // animator for camera object

	private int old_dim_layer;

	const int dimmer_show = -2; // layer of dimmer when showing enemies
	const int undim_layer = -1; // layer of enemy when enemy sprite is shown
	const int dim_layer = -5;   // layer of enemy when enemy sprite is dimmed

	const float fps30 = 0.0333f; // length of frame at 30fps
	const float fps60 = 0.0166f; // length of frame at 60fps

	void Awake() {
		if (main == null) main = this;
	}

    // turn dim on/off with no associated sprite
    public void setDim(bool dim)
    {
        if (dim)
            dimmer.color = new Color(0, 0, 0, 0.5f);
        else
            dimmer.color = new Color(0, 0, 0, 0);
    }
	// turn dim on/off
	public void setDim(bool dim, SpriteRenderer target) {
		setDim(dim, new SpriteRenderer[1]{ target });
	}
    // turn dim on/off (for multiple sprites)
    public void setDim(bool dim, SpriteRenderer[] targets)
    {
        if (dim)
        {
            if (targets != null)
            {
                foreach (SpriteRenderer s in targets)
                {
					s.sortingOrder = undim_layer;
                }
            }
            dimmer.color = new Color(0, 0, 0, 0.5f);
			old_dim_layer = dimmer.sortingOrder;
			dimmer.sortingOrder = dimmer_show;
        }
        else
        {
            if (targets != null)
            {
                foreach (SpriteRenderer s in targets)
                {
					s.sortingOrder = dim_layer;
                }
            }
            dimmer.color = new Color(0, 0, 0, 0);
			dimmer.sortingOrder = old_dim_layer;
        }
    }
		
	// shake the screen for sec seconds and amt intensity
	public void screenShake(float sec, float amt) {
		StartCoroutine (screenShakeCR(sec, amt));
	}

	// coroutine that shakes screen over time
	IEnumerator screenShakeCR(float sec, float amt) {
		Vector3 old_pos = cam_pos.position;
		float curr_time = 0;
		while (curr_time < sec) {
			cam_pos.position = Random.insideUnitCircle * amt;
			yield return new WaitForEndOfFrame();
			curr_time += Time.deltaTime;
		}
		cam_pos.position = old_pos;
	}

	// shake a sprite for sec seconds and amt intensity
	public void spriteShake(Transform pos, float sec, float amt) {
		StartCoroutine(spriteShakeCR (pos, sec, amt));
	}

	// coroutine that shakes sprite over time
	IEnumerator spriteShakeCR(Transform pos, float sec, float amt) {
		Vector3 old_pos = new Vector3(pos.position.x, pos.position.y, pos.position.z);
		float curr_time = 0;
		while (curr_time < sec) {
			pos.position = (Vector3)old_pos + (Vector3)(Random.insideUnitCircle * amt);
			//yield return new WaitForEndOfFrame ();
			//curr_time += Time.deltaTime;
			yield return new WaitForSeconds(fps60);
			curr_time += fps60;
		}
		pos.position = old_pos;
	}

	// shift a sprite to the right for sec seconds and amt intensity
	public void spriteShift(Transform pos, float sec, float amt) {
		StartCoroutine(spriteShiftCR (pos, sec, amt));
	}

	// coroutine that shifts sprite over time
	IEnumerator spriteShiftCR(Transform pos, float sec, float amt) {
		Vector3 old_pos = pos.position;
		Vector3 shift_pos = new Vector3 (0.15f, 0.0f, 0.0f);
		Vector3 check_pos;
		float curr_time = 0;
		while (curr_time < (sec*2)) {
			if (curr_time >= sec) {
				shift_pos.x -= 0.04f;
			} else {
				if ((shift_pos.x - 0.02f) > 0.0f) {
					shift_pos.x -= 0.02f;
				} else {
					shift_pos.x = 0.0f;
				}
			}
			check_pos = (pos.position += (Vector3)shift_pos);
			if (check_pos.x < old_pos.x) {
				pos.position = old_pos;
			} else {
				pos.position += (Vector3)shift_pos;
			}
			//yield return new WaitForEndOfFrame ();
			//curr_time += Time.deltaTime;
			yield return new WaitForSeconds(fps60);
			curr_time += fps60;
		}
		pos.position = old_pos;
	}

	// spins camera with speed SPEED
	public void spinCamera(float speed) {
		camera_animator.speed = speed;
		camera_animator.Play ("spin");
	}
}
