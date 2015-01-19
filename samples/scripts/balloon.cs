using UnityEngine;
using System.Collections;

public class balloon : MonoBehaviour {
	private const float LIFE_TIME = 1.0f;
	private TmSpriteAnim mAnm;
	private TmSpriteAnim mIconAnm;
	private float mTimer;
	// Use this for initialization
	void Start () {
		mAnm = GetComponent<TmSpriteAnim>();
		mIconAnm = transform.GetChild(0).GetComponent<TmSpriteAnim>();
		mAnm.StopAnimation();
		mIconAnm.StopAnimation();
		
		mAnm.SetMeshUVByFrame(Random.Range(0,mAnm.frames.Length));
		mIconAnm.SetMeshUVByFrame(Random.Range(0,mIconAnm.frames.Length));
		
		mTimer = LIFE_TIME;
	}
	
	// Update is called once per frame
	void Update () {
		mTimer -= Time.deltaTime;
		if(mTimer>0.0f){
			transform.Translate(Vector3.up * Time.deltaTime);
			mAnm.SetMeshColor(new Color(0.5f,0.5f,0.5f,mTimer/LIFE_TIME));
			mIconAnm.SetMeshColor(new Color(0.5f,0.5f,0.5f,mTimer/LIFE_TIME));
		}else{
			Destroy(gameObject);
		}
	}
}
