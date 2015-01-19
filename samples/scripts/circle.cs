using UnityEngine;
using System.Collections;

public class circle : MonoBehaviour {
	public float mSpd = 1.0f;
	public float mLifeSpd = 0.1f;
	private Mesh mMesh;
	private Color mCol;
	private float mScale;
	private float mAlpha;

	// Use this for initialization
	void Start () {
		mMesh = TmMesh.CreateLineCircle(128,Color.yellow);
		gameObject.GetComponent<MeshFilter>().mesh = mMesh;
		mCol = Color.white;
		mScale = transform.localScale.x;
		mAlpha = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		mAlpha -= mLifeSpd*Time.deltaTime;
		if(mAlpha <=0.0f){
			Destroy(gameObject);
		}else{
			mScale += mSpd * Time.deltaTime;
			transform.localScale = Vector3.one * mScale;
			mCol.a = mAlpha;
			renderer.material.SetColor("_Color",mCol);
		}
	}
}
