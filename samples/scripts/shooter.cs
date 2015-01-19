using UnityEngine;
using System.Collections;

public class shooter : MonoBehaviour {
	public GameObject bulletPrefab;
	public float bulletSpeed;
	public class bulletInfo{
		public Vector3 pos;
		public Vector3 spd;
	};

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void SM_setTargetInfo(bulletInfo _info){
		float t = TmMath.CollideTime(_info.pos,_info.spd,transform.position,bulletSpeed);
		if(t>0.0f){
			Vector3 pos = _info.pos + _info.spd * t;
			transform.LookAt(pos);
			GameObject shlObj = GameObject.Instantiate(bulletPrefab,transform.position,transform.rotation) as GameObject;
			shlObj.SendMessage("SM_setSpeed",shlObj.transform.forward*bulletSpeed);
		}
	}
}
