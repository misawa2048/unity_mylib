using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

//  - How to use -
//  private TmKeyRec _key = new TmKeyRec();
//  FixedUpdate(){ _key.fixedUpdate(); }
//  _key.setRecState(REC/STOP/PLAY);

public class TmKeyRec{
	public const float VERSION = 0.20f;
	private const int DEF_REC_BUFF_SIZE = 65535;
	public DEBUG_MODE debugMode = DEBUG_MODE.NONE;
	
	//++++++++++++++++++++++++++++++++++++++++++
	public enum DEBUG_MODE{
		NONE             = 0,
		DISP_PAD         = 1,
		DISP_ANALOG      = 2,
		DISP_PAD_ANALOG  = 3,
		ALL              = -1
	};
	
	public enum REC_STATE{
		STOP,
		REC,
		PLAY,
		PAUSE
	};
	
	public class PAD_KEY{
		public const int MOUSE_L = (1<<0);
		public const int MOUSE_R = (1<<1);
		public const int MOUSE_M = TRIG_D; // TRIG_Dと共用 
		public const int WHEEL_U = (1<<2);
		public const int WHEEL_D = (1<<3);
		public const int UP      = (1<<4);
		public const int DOWN    = (1<<5);
		public const int LEFT    = (1<<6);
		public const int RIGHT   = (1<<7);
		public const int TRIG_A  = (1<<8);
		public const int TRIG_B  = (1<<9);
		public const int TRIG_C  = (1<<10);
		public const int TRIG_D  = (1<<11); // MOUSE_Mと共用 
		public const int START   = (1<<12);
		public const int SELECT  = (1<<13);
		public const int PAUSE   = (1<<14);
		public const int DEBUG   = (1<<15);
		
		private GameObject mDebugDispObj;
		private int mData;
		public int data { get { return mData; } }
		public bool isSameValue(PAD_KEY _tgt){ return (_tgt.data == mData); }
		
		public PAD_KEY(){ }
		public PAD_KEY(PAD_KEY _origin){
			mData = _origin.data;
		}
		public PAD_KEY clone(){ return new PAD_KEY(this); }
		
		public void updateInfo(int _setData=int.MaxValue){
			if(_setData!=int.MaxValue){
				mData = _setData;
			}else{
				mData = 0;
				if(Input.GetMouseButton(0)) mData |= MOUSE_L;
				if(Input.GetMouseButton(1)) mData |= MOUSE_R;
				if(Input.GetMouseButton(2)) mData |= MOUSE_M; // TRIG_Dと共用 
	
				if(Input.GetAxis("Mouse ScrollWheel") < 0.0f) mData |= WHEEL_U;
				if(Input.GetAxis("Mouse ScrollWheel") > 0.0f) mData |= WHEEL_D;
				
				if(Input.GetKey(KeyCode.UpArrow))    mData |= UP;
				if(Input.GetKey(KeyCode.DownArrow))  mData |= DOWN;
				if(Input.GetKey(KeyCode.LeftArrow))  mData |= LEFT;
				if(Input.GetKey(KeyCode.RightArrow)) mData |= RIGHT;
				
				if(Input.GetKey(KeyCode.Space)) mData |= TRIG_A;
				if(Input.GetKey(KeyCode.X))     mData |= TRIG_B;
				if(Input.GetKey(KeyCode.Z))     mData |= TRIG_C;
				if(Input.GetKey(KeyCode.C))     mData |= TRIG_D; // MOUSE_Mと共用 
				
				if(Input.GetKey(KeyCode.Home))  mData |= START;
				if(Input.GetKey(KeyCode.End))   mData |= SELECT;
				if(Input.GetKey(KeyCode.Pause)) mData |= PAUSE;
				if(Input.GetKey(KeyCode.Break)) mData |= DEBUG;
			}
		}
	}
	//++++++++++++++++++++++++++++++++++++++++++
#if false
	public class PAD : PAD_KEY{
		private int mOld;
		private int mTrg;
		public int dataOld { get { return mOld; } }
		public int dataTrg { get { return mTrg; } }

		public PAD(){ }
		public PAD(PAD _origin){
			base.clone();
			mOld = _origin.mOld;
			mTrg = _origin.mTrg;
		}
		public PAD clone(){ return new PAD(this); }

		public void updateInfo(int _setData=0){
			mOld = base.data;
			base.updateInfo(_setData);
			mTrg = base.data & (base.data^mOld);
		}
	}
#endif	
	//++++++++++++++++++++++++++++++++++++++++++
	public class ANALOG{
		public class RATE{
			public const int DIV = 4096; // アナログ値分割(0-(DIV-1)) 
			private int mRate; // 0-(DIV-1) 
			public int rate{
				get { return mRate;}
				set { mRate = (int)Mathf.Clamp(value,0,(DIV-1)); }
			}
			
			public RATE(){}
			public RATE(RATE _origin){
				mRate = _origin.mRate;
			}
			public RATE clone(){ return new RATE(this); }
			
			public float rateF{ // -1.0f - 1.0f
				get{ return (mRate / (float)(DIV-1))*2.0f-1.0f; }
				set{
					float df = value;
					if(df>=0.0f){
						df = (df %2.0f);
						if(df > 1.0f){
							df = -2.0f+df;
						}
					}else{
						df = (-df %2.0f);
						if(df > 1.0f){
							df = -2.0f+df;
						}
						df *= -1;
					}
					mRate = (int)((df+1.0f)*0.5f * (float)(DIV-1));
				}
			}
		}
		
		public RATE hRate; // 0-(DIV-1) 
		public RATE vRate; // 0-(DIV-1) 
		public bool isSameValue(ANALOG _tgt){ return ((_tgt.hRate.rate == hRate.rate)&&(_tgt.vRate.rate == vRate.rate)); }
		public float angle{
			get{ return Mathf.Atan2(hRate.rateF,vRate.rateF)/Mathf.PI; }
			set{ vRate.rateF = Mathf.Cos(value*Mathf.PI); hRate.rateF = Mathf.Sin(value*Mathf.PI); }
		}
		
		public ANALOG(){
			hRate = new RATE();
			vRate = new RATE();
		}
		public ANALOG(ANALOG _origin){
			hRate = new RATE(_origin.hRate);
			vRate = new RATE(_origin.vRate);
		}
		public ANALOG clone(){ return new ANALOG(this); }
		
		public void updateInfo(float _setAng=0.0f){
			angle = _setAng;
		}
		public void updateInfo(Vector3 _pos, Rect _rect){
			Vector3 center = new Vector3(_rect.center.x*Screen.width,_rect.center.y*Screen.height,_pos.z);
			angle = Mathf.Atan2((_pos.x-center.x)/Screen.width,(_pos.y-center.y)/Screen.height)/Mathf.PI;
		}
	}
	//++++++++++++++++++++++++++++++++++++++++++
	public class KeyInfo{
		private float mDeltaTime;
		private int mCount;
		private PAD_KEY mPad;
		private ANALOG mAnL;
		private ANALOG mAnR;
		private void updateDeltaTime(){ mDeltaTime = Time.deltaTime; }
		public float deltaTime { get{ return mDeltaTime; } }
		public float count { get{ return mCount; } }
		public PAD_KEY pad { get{ return mPad; } }
		public ANALOG anL { get{ return mAnL; } }
		public ANALOG anR { get{ return mAnR; } }
		
		public KeyInfo(KeyInfo _origin):this(){
			mDeltaTime = _origin.mDeltaTime;
			mCount = _origin.mCount;
			mPad = _origin.mPad.clone();
			mAnL = _origin.mAnL.clone();
			mAnR = _origin.mAnR.clone();
		}
		public KeyInfo(){
			mCount = 1;
			mPad = new PAD_KEY();
			mAnL = new ANALOG();
			mAnR = new ANALOG();
		}
		
		public KeyInfo clone(){ return new KeyInfo(this);	}
		
		public void updateInfo(int _padData=0, float _angL=0.0f, float _angR=0.0f){
			updateDeltaTime();
			mPad.updateInfo(_padData);
			mAnL.updateInfo(_angL);
			mAnR.updateInfo(_angR);
		}
		public void updateInfo(int _padData, Rect _rectL, Rect _rectR){
			updateDeltaTime();
			mPad.updateInfo(_padData);
			mAnL.updateInfo(Input.mousePosition,_rectL);
			mAnR.updateInfo(Input.mousePosition,_rectR);
		}

		public bool isSameValue(KeyInfo _tgt){ return ((_tgt.pad.isSameValue(mPad))&&(_tgt.anL.isSameValue(mAnL))&&(_tgt.anR.isSameValue(mAnR))); }
//		public KeyInfo[] compressedInfo(KeyInfo[] _srcArr){
//			int cnt = 0;
//			for(int ii = 0; ii < _srcArr.Length; ++ii){
//				
//			}
//		}
		public bool decompressInfo(byte[] _compressed){ return false; }
	}
	
//=========================================
	private KeyInfo mInfo;
	private KeyInfo[] mRecInfo;
	private int mBufPtr;
	private int mRecSize;
	
	private GameObject mDebugPadDispObj;
	private GameObject mDebugAnLDispObj;
	private GameObject mDebugAnRDispObj;
	
	private int mKey;
	private int mKeyOld;
	private int mKeyTrg;
	public int keyOld { get { return mKeyOld; } }
	public int keyTrg { get { return mKeyTrg; } }
	
	public int recSize { get{ return mRecSize; } }
	private REC_STATE mRecState;
	
	public KeyInfo keyInfo { get{ return mInfo; } }
	public KeyInfo[] recInfo {
		get{
			KeyInfo[] ret = new KeyInfo[mBufPtr];
			mRecInfo.CopyTo(ret,0);
			return ret;
		}
	}
	
	public TmKeyRec(){
		mInfo = new KeyInfo();
		mRecInfo = new KeyInfo[DEF_REC_BUFF_SIZE];
		mBufPtr = mRecSize = 0;
		mKey = mKeyOld = mKeyTrg = 0;
	}
	public TmKeyRec(TmKeyRec _origin){
		mInfo = new KeyInfo(_origin.keyInfo);
		mRecInfo = new KeyInfo[DEF_REC_BUFF_SIZE];
		_origin.mRecInfo.CopyTo(mRecInfo,0);
		mBufPtr = _origin.mBufPtr;
		mRecSize = _origin.mRecSize;
		mRecState = _origin.mRecState;
		mKey = _origin.mKey;
		mKeyOld = _origin.mKeyOld;
		mKeyTrg = _origin.mKeyTrg;
	}
	public TmKeyRec clone(){ return new TmKeyRec(this);	}
	
	public int fixedUpdate(int _padData=int.MaxValue, float _angL=float.MaxValue, float _angR=float.MaxValue){
		int ret = -1;
		mKeyOld = mKey;
		if(mRecState==REC_STATE.PLAY){
			KeyInfo outInfo;
			ret = playOne(out outInfo);
			if(mKey >=0){
				mInfo = outInfo;
			}
		}else{
			mInfo.updateInfo(_padData, _angL, _angR);
			if(mRecState==REC_STATE.REC){
				ret = recOne(mInfo);
			}
		}
		mKey = mInfo.pad.data;
		mKeyTrg = mKey & (mKey^mKeyOld);
		
		if(((int)debugMode & (int)DEBUG_MODE.DISP_PAD)!=0){
			mDebugPadDispObj = debugPadDisp(mDebugPadDispObj);
		}
		if(((int)debugMode & (int)DEBUG_MODE.DISP_ANALOG)!=0){
			mDebugAnLDispObj = debugAnalogDisp(mDebugAnLDispObj, mInfo.anL.angle, new Rect(0.0f,0.0f,0.5f,0.5f));
			mDebugAnRDispObj = debugAnalogDisp(mDebugAnRDispObj, mInfo.anR.angle, new Rect(0.5f,0.0f,0.5f,0.5f));
		}

		return ret;
	}
	
	public REC_STATE setRecState(REC_STATE _newState){
		REC_STATE old = mRecState;
		mRecState = _newState;
		if(old != _newState){
			mBufPtr = 0;
			if(_newState == REC_STATE.REC){
				mRecSize = 0;
			}
		}
		return old;
	}

//	public KeyInfo[] compressedInfo(){}
//	public bool decompressInfo(byte[] _compressed){ return false; }
	
	private int recOne(KeyInfo _data){
		int ret = -1;
		if(mBufPtr < DEF_REC_BUFF_SIZE){
			mRecInfo[mBufPtr] = _data.clone();
			ret = mBufPtr;
			mBufPtr++;
			mRecSize = mBufPtr;
		}else{
			Debug.Log("TmKeyRec:DEF_REC_BUFF_SIZE over!");	
		}
		return ret;
	}

	private int playOne(out KeyInfo _outInfo, int _ptr=-1){
		_outInfo = new KeyInfo();
		int ret = -1;
		if(_ptr>=0){
			mBufPtr = _ptr;
		}
		if(mBufPtr < mRecSize){
			_outInfo = mRecInfo[mBufPtr];
			ret = mBufPtr;
			mBufPtr++;
		}
// Debug.Log(mBufPtr+"/"+mRecSize+"ang="+_outInfo.anL.angle);
		return ret;
	}
	
	//----------------------------------------------------------------------------
	public GameObject debugPadDisp(GameObject _obj){
		if(_obj==null){
			_obj = new GameObject("_debugPAD_KEY");
			_obj.AddComponent<GUIText>();
			_obj.transform.position = Vector3.up;
			_obj.guiText.fontSize = 12;
		}
		_obj.guiText.text = System.Convert.ToString(keyTrg, 02).PadLeft(32, '0');
		return _obj;
	}

	public GameObject debugAnalogDisp(GameObject _obj,float _angle, Rect _rect){
		Vector3 point = Input.mousePosition;
		return drawGismoScreenPointToWorldPosition(_obj,_rect,point,_angle,0.025f,1.0f);
	}
	public GameObject drawGismoScreenPointToWorldPosition(GameObject _obj, Rect _rect, Vector3 _point,float _angle,float _scale,float _ofsZ){
		_point.x = Mathf.Clamp(_point.x,_rect.xMin*Screen.width,_rect.xMax*Screen.width);
		_point.y = Mathf.Clamp(_point.y,_rect.yMin*Screen.height,_rect.yMax*Screen.height);
		if(_obj==null){
			_obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
			_obj.name = "_debugANALOG";
		}
		Ray ray = Camera.main.ScreenPointToRay(_point);
		Vector3 p0 = ray.GetPoint(_ofsZ);
		Plane plane = new Plane(ray.direction,p0);
		ray = Camera.main.ScreenPointToRay(_point+Vector3.left * _scale * Screen.width);
		float enter;
		Vector3 p1 = ray.GetPoint(_ofsZ);
		if( plane.Raycast(ray, out enter) ){
			p1 = ray.GetPoint(enter);
		}
		Vector3 p2 = p1 + (p0-p1);
		ray = Camera.main.ScreenPointToRay(_point+Vector3.up * _scale * Screen.height);
		Vector3 p3 = ray.GetPoint(_ofsZ);
		if( plane.Raycast(ray, out enter) ){
			p3 = ray.GetPoint(enter);
		}
		Vector3 p4 = p3 + (p0-p3);
		_obj.transform.localScale = new Vector3((p2-p1).magnitude,(p4-p3).magnitude,1.0f);
		_obj.transform.position = p0;
		Quaternion qua = Quaternion.AngleAxis(-_angle*180.0f,Camera.main.transform.forward);
		_obj.transform.rotation = qua * Camera.main.transform.rotation;
		return _obj;
	}
}
