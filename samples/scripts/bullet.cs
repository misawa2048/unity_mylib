using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {
	private Vector3 mSpd;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += mSpd*Time.deltaTime;
		if(!TmUtils.IsInnerScreen(transform.position)){
			Destroy(gameObject);
		}
	}

	private void SM_setSpeed(Vector3 _spd){
		mSpd = _spd;
	}
}
