using UnityEngine;
using System.Collections;

public class pointInterpolation : MonoBehaviour {
	public float moveSpd =1.0f;
	public GameObject movePosObj;
	public GameObject[] pointObj;
	private Vector3[] mPoint;
	private float mPosPt;
	private float mPosPtMax;
	// Use this for initialization
	void Start () {
		if(pointObj.Length<2) return;

		mPoint = new Vector3[pointObj.Length+1];
		setPos();
		movePosObj.transform.position = pointObj[0].transform.position;
		mPosPt = 0.0f;
		mPosPtMax = (float)(pointObj.Length);
	}
	
	// Update is called once per frame
	void Update () {
		if(pointObj.Length<2) return;
		setPos();

		float rate = mPosPt-Mathf.Floor(mPosPt);
		int nowPt = Mathf.FloorToInt(mPosPt);
		int nextPt = Mathf.FloorToInt(mPosPt)+1;
		int next2Pt = Mathf.FloorToInt(mPosPt)+2;
		if(nextPt > pointObj.Length-1){
			nextPt -= pointObj.Length;
		}
		if(next2Pt > pointObj.Length-1){
			next2Pt -= pointObj.Length;
		}
		Vector3 pos0 = mPoint[Mathf.FloorToInt(mPosPt)];
		Vector3 pos1 = mPoint[nextPt];
		float dd = (pos1-pos0).magnitude;
		pos0 += (mPoint[nextPt]-mPoint[nowPt]) * rate;
		pos1 -= (mPoint[next2Pt]-mPoint[nextPt]) * (1.0f-rate);
		movePosObj.transform.position = pos0*(1.0f-rate)+pos1*rate;

		mPosPt += Time.deltaTime * moveSpd /dd;
		if(mPosPt > mPosPtMax){
			mPosPt -= mPosPtMax;
		}
	}

	private int setPos(){
		int num = pointObj.Length;
		for(int ii = 0; ii < num; ++ii){
			mPoint[ii] = pointObj[ii].transform.position;
		}
		mPoint[num] = mPoint[num-1]+(mPoint[num-1]-mPoint[num-2]);
		return num;
	}
}
