using UnityEngine;
using System.Collections;

public class TmGyroView : MonoBehaviour {
	private const float BASE_MOVE_X = 0.2f;
	private const float BASE_MOVE_Y = 0.2f;
	public GameObject targetRotOb;
	private Quaternion mGyroRot;
	private Vector3 mBaseDir;
	private Vector3 mDragSttPos;
	private Vector3 mDragSttDir;
	
	#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
	private float mAng;
	#else
	private Vector3 mEuler;
	#endif
	
	// Use this for initialization
	void Start () {
		if(targetRotOb==null){
			targetRotOb = gameObject;
		}
		mGyroRot = updateRot ();
		mBaseDir.y = -mGyroRot.eulerAngles.y;
		if(SystemInfo.supportsGyroscope){
			Input.gyro.enabled=true;
		}
		#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
		mAng = getRotAng(Input.deviceOrientation);
		#else
		mEuler = Vector3.zero;
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		mBaseDir = updateBaseRot(); // drag to rot Y
		mGyroRot = updateRot ();
		targetRotOb.transform.rotation = Quaternion.Euler(mBaseDir) * mGyroRot; 
	}
	
	private Vector3 updateBaseRot(){
		Vector3 ret = mBaseDir;
		if(Input.GetMouseButtonDown(0)){
			mDragSttPos=Input.mousePosition;
			mDragSttDir = mBaseDir;
		}else if(Input.GetMouseButton(0)){
			Vector3 vec = Input.mousePosition - mDragSttPos;
			if(SystemInfo.supportsGyroscope){
				vec.y = 0f;
			}
			ret = mDragSttDir+new Vector3(-vec.y*BASE_MOVE_X,vec.x*BASE_MOVE_Y,0f);
		}
		return ret;
	}
	private Quaternion updateRot(){
		Quaternion ret = Quaternion.identity;
		#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
		if(SystemInfo.supportsGyroscope){
			ret = updateBasedOnGyro();
		}else{
			//			ret = updateBasedOnAccel();
		}
		#else
		ret = updateBasedKey();
		#endif
		return ret;
	}
	
	#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
	private Quaternion updateBasedOnGyro(){
		if(!SystemInfo.supportsGyroscope){
			return Quaternion.identity;
		}
		return Quaternion.AngleAxis(90.0f,Vector3.right)*Input.gyro.attitude*Quaternion.AngleAxis(180.0f,Vector3.forward);
	}
	
	private Quaternion updateBasedOnAccel(){
		float ang = getRotAng(Input.deviceOrientation);
		mAng = Mathf.Lerp(mAng,ang,0.1f);
		return Quaternion.AngleAxis(mAng,Vector3.forward);
	}
	private float getRotAng(DeviceOrientation _ori){
		float rotAng = 0.0f;
		if(Screen.orientation != ScreenOrientation.AutoRotation){
			if(_ori==DeviceOrientation.Portrait){ rotAng = -90.0f; }
			if(_ori==DeviceOrientation.PortraitUpsideDown){ rotAng = 90.0f; }
			if(_ori==DeviceOrientation.LandscapeLeft){ rotAng = 0.0f; }
			if(_ori==DeviceOrientation.LandscapeRight){ rotAng = 180.0f; }
		}
		return rotAng;
	}
	#else
	private Quaternion updateBasedKey(){
		float spd = Time.deltaTime * 50f;
		if(Input.GetKey(KeyCode.UpArrow))   { mEuler.x -= spd; }
		if(Input.GetKey(KeyCode.DownArrow)) { mEuler.x += spd; }
		if(Input.GetKey(KeyCode.LeftArrow)) { mEuler.y -= spd; }
		if(Input.GetKey(KeyCode.RightArrow)){ mEuler.y += spd; }
		return Quaternion.Euler(mEuler);
	}
	#endif
	
}