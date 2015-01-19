using UnityEngine;
using System.Collections;

public class detectClossCircles : MonoBehaviour {
	public string tagStr;
	public float generateInterval;
	public GameObject generateFieldObj;
	public GameObject generatePrefab;
	private GameObject[] mObjs;
	private float mGenerateTimer;

	// Use this for initialization
	void Start () {
		mObjs = GameObject.FindGameObjectsWithTag(tagStr);
		mGenerateTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		mGenerateTimer += Time.deltaTime;
		if(mGenerateTimer >= generateInterval){
			mGenerateTimer -= generateInterval;
			Vector3 pos = Random.insideUnitSphere*0.5f;
			pos = Vector3.Scale(pos,generateFieldObj.transform.localScale);
			pos.z = 0.0f;
			pos += generateFieldObj.transform.position;
			Instantiate(generatePrefab,pos,Quaternion.identity);
		}

		mObjs = GameObject.FindGameObjectsWithTag(tagStr);
		int num = mObjs.Length;
		if(num<2) return;

		int cnt = 0;
		for(int ii = 0; ii < num; ++ii){
			for(int jj = ii+1; jj < num; ++jj){
				int result = detectColl(mObjs[ii],mObjs[jj]);
				if(result>=0){
					cnt += result;
				}
			}
		}
//		Debug.Log ("i/c="+(float)cnt/(float)(num*num));
	}

	private int detectColl(GameObject _objA, GameObject _objB){
		Vector2 posA = _objA.transform.position;
		Vector2 posB = _objB.transform.position;
		float rA = _objA.transform.localScale.x * 0.5f;
		float rB = _objB.transform.localScale.x * 0.5f;
		Vector2[] retPos;
		int result = TmMath.CircleIntersection(out retPos,posA,rA,posB,rB);
		if(result==2){
			Debug.DrawLine(retPos[0],retPos[1],Color.green);
		}
		return result;
	}
}
