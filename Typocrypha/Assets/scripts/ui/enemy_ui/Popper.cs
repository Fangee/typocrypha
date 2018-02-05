using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for object that handles pop-up graphics
public class Popper : MonoBehaviour {

	public GameObject poptext_prefab;
	GameObject txtObj;
	PopText pt;

	public GameObject popimage_prefab;
	GameObject imgObj;
	PopImage pi;

    AssetBundle ass;

    void Awake () {
        ass = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "sprites"));
    }

	//prints (message) for (delay) seconds at (pos) 
	public void spawnText (string message, float delay, Vector3 pos) {
		txtObj = GameObject.Instantiate (poptext_prefab, transform);
		txtObj.transform.position = pos;
		pt = txtObj.GetComponent<PopText>();
		pt.display (message, delay);
	}

	//prints image (picName) for (delay) seconds at (pos) 
	public void spawnSprite (string picPath, float delay, Vector3 pos) {
		imgObj = GameObject.Instantiate (popimage_prefab, transform);
		imgObj.transform.position = pos;
		pi = imgObj.GetComponent<PopImage>();
        pi.display (ass.LoadAsset<Sprite> (picPath), delay);
	}

}
