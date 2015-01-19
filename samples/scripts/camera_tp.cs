using UnityEngine;
using System.Collections;

public class camera_tp : MonoBehaviour {
	public GameObject plObj;
	private float mDefZ;

	// Use this for initialization
	void Start () {
		mDefZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = plObj.transform.position;
		pos.z = mDefZ;
		transform.position = pos;
	}
}
