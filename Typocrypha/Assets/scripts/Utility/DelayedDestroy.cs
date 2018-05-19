using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// allows object to be destroyed after a delay
public class DelayedDestroy : MonoBehaviour {
	// destroys attached objects after 'sec' seconds
	public void delayedDestroy(float sec) {
		StartCoroutine (delayedDestroyCR (sec));
	}

	IEnumerator delayedDestroyCR(float sec) {
		yield return new WaitForSeconds (sec);
		Destroy (gameObject);
	}

}
