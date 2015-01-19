using UnityEngine;
using System.Collections;

public class lineToLineDistCalc : MonoBehaviour {
	public GameObject p1Obj;
	public GameObject p2Obj;
	public GameObject q1Obj;
	public GameObject q2Obj;
	private string mName;
	// Use this for initialization
	void Start () {
		mName = gameObject.name;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p1 = p1Obj.transform.position;
		Vector3 p2 = p2Obj.transform.position;
		Vector3 q1 = q1Obj.transform.position;
		Vector3 q2 = q2Obj.transform.position;
		Vector3 p0,q0;
		float d = TmMath.LineToLineDistance(out p0,out q0,p1,p2,q1,q2,false);
		Debug.DrawLine(p1,p2);
		Debug.DrawLine(q1,q2);
		Debug.DrawLine(p0,q0);
		gameObject.name = mName + ":"+d.ToString();
		TmDebug.DrawCircle(p1,p1Obj.transform.rotation,1f,Color.white);
	}
}
