using UnityEngine;
using System.Collections;

public class enenyShooter : MonoBehaviour {
	public GameObject bulletPrefab;
	public GameObject counterShooterObj;
	public float reloadTime;
	public float bulletSpeed;
	private GameObject mPlObj;
	private float mTimer;
	// Use this for initialization
	void Start () {
		mTimer = 0.0f;
		mPlObj = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(mPlObj.transform.position);
		mTimer += Time.deltaTime;
		if(mTimer > reloadTime){
			mTimer -= reloadTime;
			Vector3 spd = transform.forward*bulletSpeed;
			GameObject shlObj = GameObject.Instantiate(bulletPrefab,transform.position,transform.rotation) as GameObject;
			shlObj.SendMessage("SM_setSpeed",spd);
			shooter.bulletInfo info = new shooter.bulletInfo();
			info.pos = transform.position;
			info.spd = spd;
			counterShooterObj.SendMessage("SM_setTargetInfo",info);
		}
	}
}
