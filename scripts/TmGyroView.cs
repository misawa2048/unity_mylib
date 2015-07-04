using UnityEngine;
using System.Collections;

namespace TmLib{
	public class TmGyroView : MonoBehaviour {
		private const float BASE_MOVE_X = 0.2f;
		private const float BASE_MOVE_Y = 0.2f;
		public float defDirY;
		public GameObject targetRotOb;
		private float mReloadTime;
		private Quaternion mGyroRot;
		private Vector3 mBaseDir;
		private Vector3 mDragSttPos;
		private Vector3 mDragSttDir;
		
		#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
		#else
		public GameObject debugGyroObj;
		private Vector3 mEuler;
		#endif
		
		// Use this for initialization
		void Start () {
			if(targetRotOb==null){
				targetRotOb = gameObject;
			}
			if(SystemInfo.supportsGyroscope){
				mReloadTime = (Input.gyro.enabled) ? 0.2f : 1f;
				Input.gyro.enabled=true;
			}
			StartCoroutine(initCo());
		}
		
		// Update is called once per frame
		void Update () {
			mBaseDir = updateBaseRot(); // drag to rot Y
			mGyroRot = updateRot ();
			Quaternion rot = Quaternion.AngleAxis(defDirY,Vector3.up);
			rot *= Quaternion.Euler(mBaseDir) * mGyroRot;
			targetRotOb.transform.rotation = rot; 
		}
		
		private IEnumerator initCo(){
			mGyroRot = updateRot ();
			
			#if (UNITY_IPHONE||UNITY_ANDROID) && (!UNITY_EDITOR)
			float waitTime = mReloadTime;
			while(waitTime>=0f){
				waitTime -= Time.deltaTime;
				defDirY = -mGyroRot.eulerAngles.y;
				yield return null;
			}
			#else
			defDirY = transform.rotation.eulerAngles.y;
			mEuler = Vector3.zero;
			#endif
			yield break;
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
		#else
		private Quaternion updateBasedKey(){
			float spd = Time.deltaTime * 50f;
			if(debugGyroObj!=null){
				mEuler = debugGyroObj.transform.localRotation.eulerAngles;
			}
			if(Input.GetKey(KeyCode.UpArrow))   { mEuler.x -= spd; }
			if(Input.GetKey(KeyCode.DownArrow)) { mEuler.x += spd; }
			if(Input.GetKey(KeyCode.LeftArrow)) { mEuler.y -= spd; }
			if(Input.GetKey(KeyCode.RightArrow)){ mEuler.y += spd; }
			if(debugGyroObj!=null){
				debugGyroObj.transform.localRotation = Quaternion.Euler(mEuler);
			}
			return Quaternion.Euler(mEuler);
		}
		#endif
		
	}} //namespace TmLib
