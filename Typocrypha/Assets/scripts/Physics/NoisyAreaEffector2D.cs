using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// applies random directional force to objects in zone
public class NoisyAreaEffector2D : MonoBehaviour {
	public float magnitude; // amount of force

	void OnTriggerStay2D(Collider2D other) {
		other.attachedRigidbody.AddForce (Random.insideUnitCircle * magnitude);
	}
}
