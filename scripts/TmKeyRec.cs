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
	public const float VERSION = 0.25f;
	private const int DEF_REC_BUFF_SIZE = 65535;
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
	
	public enum BUFF_TYPE{
		NORMAL,
		RING
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
		public const int KEY_NUM_MAX = 16;
		
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
#if true
	public class PAD{
		public const float DEF_REP_STT_TIME = 1.0f;
		public const float DEF_REP_CONT_TIME = 0.1f;
		private int mKey;
		private int mOld;
		private int mTrg;
		private int mRel;
		private int mRpt;
		private float[]	mRepTimer;
		private bool[] mIsRep;
		public int key { get { return mKey; } }
		public int old { get { return mOld; } }
		public int trg { get { return mTrg; } }
		public int rel { get { return mRel; } }
		public int rpt { get { return mRpt; } }

		public PAD():this(null){}
		public PAD(PAD _origin){
			mKey = (_origin!=null) ? _origin.mKey : 0;
			mOld = (_origin!=null) ? _origin.mOld : 0;
			mTrg = (_origin!=null) ? _origin.mTrg : 0;
			mRepTimer =  (_origin!=null) ? (float[])_origin.mRepTimer.Clone() : new float[PAD_KEY.KEY_NUM_MAX];
			mIsRep =  (_origin!=null) ? (bool[])_origin.mIsRep.Clone() : new bool[PAD_KEY.KEY_NUM_MAX];
		}
		public PAD clone(){ return new PAD(this); }

		public void updateInfo(int _setData=0){
			mOld = mKey;
			mKey = _setData;
			mTrg = mKey & (mKey^mOld);
			mRel = ~mKey & mOld;

			// リピート情報の更新 
			mRpt = mTrg;
			for(int ii =0; ii<PAD_KEY.KEY_NUM_MAX; ++ii){
				if( (mOld & (1<<ii)) != 0 ){
					float repTime = (mIsRep[ii]) ? DEF_REP_CONT_TIME : DEF_REP_STT_TIME;
					mRepTimer[ii] += Time.deltaTime;
					if(mRepTimer[ii] >= repTime){
						mIsRep[ii] = true;
						mRepTimer[ii]-=repTime;
						mRpt |= (1<<ii);
					}
				}else{
					mIsRep[ii] = false;
					mRepTimer[ii]=0.0f;
				}
			}
			
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
		
		public KeyInfo(){
			mDeltaTime = 0.0f;
			mCount = 1;
			mPad = new PAD_KEY();
			mAnL = new ANALOG();
			mAnR = new ANALOG();
		}
		public KeyInfo(KeyInfo _origin){
			mDeltaTime = _origin.mDeltaTime;
			mCount = _origin.mCount;
			mPad = _origin.mPad.clone();
			mAnL = _origin.mAnL.clone();
			mAnR = _origin.mAnR.clone();
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
	public class KeyInfoDebug{
		public DEBUG_MODE debugMode = DEBUG_MODE.NONE;
		private GameObject mPadDispObj;
		private GameObject mAnLDispObj;
		private GameObject mAnRDispObj;
		
		public void disp(REC_STATE _recState, int _padKey, float _angL, Rect _rectL, float _angR, Rect _rectR){
			Color col;
			switch(_recState){
				case REC_STATE.PAUSE : col = Color.white;   break;
				case REC_STATE.PLAY:   col = Color.green;   break;
				case REC_STATE.REC:    col = Color.red;     break;
				default:               col = Color.gray;    break;
			}
			if(((int)debugMode & (int)DEBUG_MODE.DISP_PAD)!=0){
				mPadDispObj = padDisp(mPadDispObj, _padKey, col);
			}
			if(((int)debugMode & (int)DEBUG_MODE.DISP_ANALOG)!=0){
				mAnLDispObj = analogDisp(mAnLDispObj, _angL, col, _rectL);
				mAnRDispObj = analogDisp(mAnRDispObj, _angR, col, _rectR);
			}
		}
		
		private GameObject padDisp(GameObject _obj, int _pad, Color _col){
			if(_obj==null){
				_obj = new GameObject("_debugPAD_KEY");
				_obj.AddComponent<GUIText>();
				_obj.transform.position = Vector3.up;
				_obj.guiText.fontSize = 10;
			}
			_obj.guiText.color = _col;
			_obj.guiText.text = System.Convert.ToString(_pad, 02).PadLeft(32, '0');
			return _obj;
		}
	
		private GameObject analogDisp(GameObject _obj,float _angle, Color _col, Rect _rect){
			Vector3 point = Input.mousePosition;
			return drawGismoScreenPointToWorldPosition(_obj,_angle,_col,_rect,point,0.025f,1.0f);
		}
		private GameObject drawGismoScreenPointToWorldPosition(GameObject _obj,float _angle, Color _col, Rect _rect, Vector3 _point,float _scale,float _ofsZ){
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
			_obj.renderer.material.color = _col;
			return _obj;
		}
	}
	
//=========================================
	private KeyInfo mInfo;
	private KeyInfo[] mRecInfo;
	private int mBuffSize;
	private int mBuffSttPtr;
	private int mBuffPtr;
	private int mRecSize;
	
	private KeyInfoDebug mDebug;
	public DEBUG_MODE debugMode { set { mDebug.debugMode = value; } get { return mDebug.debugMode; } }
	
	private PAD mPad;
	public int padKey { get { return mPad.key; } }
	public int padOld { get { return mPad.old; } }
	public int padTrg { get { return mPad.trg; } }
	public int padRel { get { return mPad.rel; } }
	public int padRpt { get { return mPad.rpt; } }
	
	public int recSize { get{ return mRecSize; } }
	private REC_STATE mRecState;
	private BUFF_TYPE mBuffType;
	public REC_STATE recState { get { return mRecState; } }
	private BUFF_TYPE buffType { get { return mBuffType; } }
	
	public KeyInfo keyInfo { get{ return mInfo; } }
	public KeyInfo[] recInfo { get{	return (KeyInfo[])mRecInfo.Clone(); } }
	
	public TmKeyRec():this(DEF_REC_BUFF_SIZE,BUFF_TYPE.NORMAL){}
	public TmKeyRec(int _buffSize, BUFF_TYPE _buffType){
		mBuffSttPtr = 0;
		mBuffSize = _buffSize;
		mInfo = new KeyInfo();
		mRecInfo = null;
		mBuffPtr = mRecSize = 0;
		mRecState = REC_STATE.STOP;
		mBuffType = _buffType;
		mPad = new PAD();
		mDebug = new KeyInfoDebug();
	}
	public TmKeyRec(TmKeyRec _origin){
		mBuffSttPtr = _origin.mBuffSttPtr;
		mBuffSize = _origin.mBuffSize;
		mInfo = new KeyInfo(_origin.keyInfo);
		mRecInfo = (KeyInfo[])_origin.mRecInfo.Clone();
		mBuffPtr = _origin.mBuffPtr;
		mRecSize = _origin.mRecSize;
		mRecState = _origin.mRecState;
		mBuffType = _origin.mBuffType;
		mPad = new PAD(_origin.mPad);
		mDebug = new KeyInfoDebug();
	}
	public TmKeyRec clone(){ return new TmKeyRec(this);	}
	
	public int fixedUpdate(int _padData=int.MaxValue, float _angL=float.MaxValue, float _angR=float.MaxValue){
		int ret = -1;
		if(mRecState==REC_STATE.PLAY){
			KeyInfo outInfo;
			ret = playOne(out outInfo);
			if(ret >=0){
				mInfo = outInfo;
			}
		}else{
			mInfo.updateInfo(_padData, _angL, _angR);
			if(mRecState==REC_STATE.REC){
				ret = recOne(mInfo);
			}
		}
		mPad.updateInfo(mInfo.pad.data);

		//debugDisp(mRecState);
		mDebug.disp(mRecState,padKey,mInfo.anL.angle,new Rect(0.0f,0.0f,0.5f,0.5f),mInfo.anR.angle,new Rect(0.5f,0.0f,0.5f,0.5f));
		return ret;
	}
	
	public REC_STATE setRecState(REC_STATE _newState){
		REC_STATE old = mRecState;
		mRecState = _newState;
		if(old != _newState){
			if(mRecInfo==null){
				mRecInfo = new KeyInfo[mBuffSize];
			}
			if(_newState != REC_STATE.PAUSE){
				mBuffPtr = 0;
			}
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
		if(mBuffPtr < mBuffSize){
			mRecInfo[mBuffPtr] = _data.clone();
			ret = mBuffPtr;
			mBuffPtr++;
			mRecSize = mBuffPtr;
		}else{
			Debug.Log("TmKeyRec:DEF_REC_BUFF_SIZE over!");	
		}
		return ret;
	}

	private int playOne(out KeyInfo _outInfo, int _ptr=-1){
		_outInfo = new KeyInfo();
		int ret = -1;
		if(_ptr>=0){
			mBuffPtr = (_ptr < mRecSize) ? _ptr : (mRecSize-1);
		}
		if(mBuffPtr < mRecSize){
			_outInfo = mRecInfo[mBuffPtr];
			ret = mBuffPtr;
			mBuffPtr++;
		}
// Debug.Log(mBuffPtr+"/"+mRecSize+"ang="+_outInfo.anL.angle);
		return ret;
	}
}
