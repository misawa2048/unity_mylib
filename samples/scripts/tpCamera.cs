using UnityEngine;
using System.Collections;

public class tpCamera : MonoBehaviour {
	public GameObject plObj;
	private Vector3 mDefPos;

	// Use this for initialization
	void Start () {
		mDefPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = plObj.transform.position;
		pos.z = mDefPos.z;
		transform.position = pos;
	}
}
