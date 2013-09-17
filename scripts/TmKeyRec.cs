using UnityEngine;
using System.Collections;

//  private TmKeyRec _key = new TmKeyRec();
//  FixedUpdate(){ _key.fixedUpdate(); }
//  _key.setRecState(REC/STOP/PLAY);

public class TmKeyRec{
	public const float VERSION = 0.03f;
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
		
		private GameObject _debugDispObj;
		private int _data;
		private int _old;
		private int _trg;
		public int data { get { return _data; } }
		public int data_old { get { return _old; } }
		public int data_trg { get { return _trg; } }
		
		public PAD_KEY(){ }
		public PAD_KEY(PAD_KEY _origin){
			_data = _origin.data;
			_old = _origin._old;
			_trg = _origin._trg;
		}
		public PAD_KEY clone(){ return new PAD_KEY(this); }
		
		public void updateInfo(int _setData=int.MaxValue){
			_old = _data;
			if(_setData!=int.MaxValue){
				_data = _setData;
			}else{
				_data = 0;
				if(Input.GetMouseButton(0)) _data |= MOUSE_L;
				if(Input.GetMouseButton(1)) _data |= MOUSE_R;
				if(Input.GetMouseButton(2)) _data |= MOUSE_M; // TRIG_Dと共用 
	
				if(Input.GetAxis("Mouse ScrollWheel") < 0.0f) _data |= WHEEL_U;
				if(Input.GetAxis("Mouse ScrollWheel") > 0.0f) _data |= WHEEL_D;
				
				if(Input.GetKey(KeyCode.UpArrow))    _data |= UP;
				if(Input.GetKey(KeyCode.DownArrow))  _data |= DOWN;
				if(Input.GetKey(KeyCode.LeftArrow))  _data |= LEFT;
				if(Input.GetKey(KeyCode.RightArrow)) _data |= RIGHT;
				
				if(Input.GetKey(KeyCode.Space)) _data |= TRIG_A;
				if(Input.GetKey(KeyCode.X))     _data |= TRIG_B;
				if(Input.GetKey(KeyCode.Z))     _data |= TRIG_C;
				if(Input.GetKey(KeyCode.C))     _data |= TRIG_D; // MOUSE_Mと共用 
				
				if(Input.GetKey(KeyCode.Home))  _data |= START;
				if(Input.GetKey(KeyCode.End))   _data |= SELECT;
				if(Input.GetKey(KeyCode.Pause)) _data |= PAUSE;
				if(Input.GetKey(KeyCode.Break)) _data |= DEBUG;
			}
			_trg = _data & (_data^_old);
		}
		
		public void debugDisp(){
			if(_debugDispObj==null){
				_debugDispObj = new GameObject("_debugPAD_KEY");
				_debugDispObj.AddComponent<GUIText>();
				_debugDispObj.transform.position = Vector3.up;
				_debugDispObj.guiText.fontSize = 12;
			}
			_debugDispObj.guiText.text = System.Convert.ToString(_data, 02).PadLeft(32, '0');
		}
	}
	
	//++++++++++++++++++++++++++++++++++++++++++
	public class ANALOG{
		public class RATE{
			public const int DIV = 4096; // アナログ値分割(0-(DIV-1)) 
			private int _rate; // 0-(DIV-1) 
			public int rate{
				get { return _rate;}
				set { _rate = (int)Mathf.Clamp(value,0,(DIV-1)); }
			}
			
			public RATE(){}
			public RATE(RATE _origin){
				_rate = _origin._rate;
			}
			public RATE clone(){ return new RATE(this); }
			
			public float rateF{ // -1.0f - 1.0f
				get{ return (_rate / (float)(DIV-1))*2.0f-1.0f; }
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
					_rate = (int)((df+1.0f)*0.5f * (float)(DIV-1));
				}
			}
		}
		
		private Rect _touchRect;
		public Rect touchRect { get{ return _touchRect; } }
		private GameObject _debugDispObj;
		public RATE hRate; // 0-(DIV-1) 
		public RATE vRate; // 0-(DIV-1) 
		public float angle{
			get{ return Mathf.Atan2(hRate.rateF,vRate.rateF)/Mathf.PI; }
			set{ vRate.rateF = Mathf.Cos(value*Mathf.PI); hRate.rateF = Mathf.Sin(value*Mathf.PI); }
		}
		
		public ANALOG():this(new Rect(0.0f,0.0f,1.0f,1.0f)){ }
		public ANALOG(Rect _rect){
			_touchRect = new Rect(_rect);
			hRate = new RATE();
			vRate = new RATE();
		}
		public ANALOG(ANALOG _origin){
			_touchRect = new Rect(_origin._touchRect);
			hRate = new RATE(_origin.hRate);
			vRate = new RATE(_origin.vRate);
		}
		public ANALOG clone(){ return new ANALOG(this); }
		
		public void updateInfo(float _setAng=float.MaxValue){
			if(_setAng!=float.MaxValue){
				angle = _setAng;
			}else{
				if(Input.GetMouseButton(0)){
					Vector3 pos = Input.mousePosition;
					Vector3 center = new Vector3(_touchRect.center.x*Screen.width,_touchRect.center.y*Screen.height,pos.z);
					angle = Mathf.Atan2((pos.x-center.x)/Screen.width,(pos.y-center.y)/Screen.height)/Mathf.PI;
				}
			}
		}
		
		public void debugDisp(){
			Vector3 point = Input.mousePosition;
			drawGismoScreenPointToWorldPosition(point,angle,0.025f,1.0f);
		}
		
		public void drawGismoScreenPointToWorldPosition(Vector3 _point,float _angle,float _scale,float _ofsZ){
			_point.x = Mathf.Clamp(_point.x,_touchRect.xMin*Screen.width,_touchRect.xMax*Screen.width);
			_point.y = Mathf.Clamp(_point.y,_touchRect.yMin*Screen.height,_touchRect.yMax*Screen.height);
			if(_debugDispObj==null){
				_debugDispObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
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
			_debugDispObj.transform.localScale = new Vector3((p2-p1).magnitude,(p4-p3).magnitude,1.0f);
			_debugDispObj.transform.position = p0;
			Quaternion qua = Quaternion.AngleAxis(-_angle*180.0f,Camera.main.transform.forward);
			_debugDispObj.transform.rotation = qua * Camera.main.transform.rotation;
		}
	}
	//++++++++++++++++++++++++++++++++++++++++++
	public class KeyInfo{
		private float _deltaTime;
		private PAD_KEY _pad;
		private ANALOG _anL;
		private ANALOG _anR;
		private void updateDeltaTime(){ _deltaTime = Time.deltaTime; }
		public float deltaTime { get{ return _deltaTime; } }
		public PAD_KEY pad { get{ return _pad; } }
		public ANALOG anL { get{ return _anL; } }
		public ANALOG anR { get{ return _anR; } }
		
		public KeyInfo(KeyInfo _origin):this(_origin._anL.touchRect,_origin._anR.touchRect){
			_deltaTime = _origin._deltaTime;
			_pad = _origin._pad.clone();
			_anL = _origin._anL.clone();
			_anR = _origin._anR.clone();
		}
		public KeyInfo():this(new Rect(0.0f,0.0f,0.5f,0.5f),new Rect(0.5f,0.0f,0.5f,0.5f)){ }
		public KeyInfo(Rect _lRect, Rect _rRect){
			_pad = new PAD_KEY();
			_anL = new ANALOG(_lRect);
			_anR = new ANALOG(_rRect);
		}
		
		public KeyInfo clone(){ return new KeyInfo(this);	}
		
		public void updateInfo(int _padData=int.MaxValue, float _angL=float.MaxValue, float _angR=float.MaxValue){
			updateDeltaTime();
			_pad.updateInfo(_padData);
			_anL.updateInfo(_angL);
			_anR.updateInfo(_angR);
		}
	}
	
//=========================================
	private KeyInfo _info;
	private KeyInfo[] _recInfo;
	private int _bufPtr;
	private int _recSize;
	public int recSize { get{ return _recSize; } }
	private REC_STATE _recState;
	
	public KeyInfo keyInfo { get{ return _info; } }
	public KeyInfo[] recInfo {
		get{
			KeyInfo[] ret = new KeyInfo[_bufPtr];
			_recInfo.CopyTo(ret,0);
			return ret;
		}
	}
	
	public TmKeyRec():this(new Rect(0.0f,0.0f,0.5f,0.5f),new Rect(0.5f,0.0f,0.5f,0.5f)){ }
	public TmKeyRec(TmKeyRec _origin){
		_info = new KeyInfo(_origin.keyInfo);
		_recInfo = new KeyInfo[DEF_REC_BUFF_SIZE];
		_origin._recInfo.CopyTo(_recInfo,0);
		_bufPtr = _origin._bufPtr;
		_recSize = _origin._recSize;
		_recState = _origin._recState;
	}
	public TmKeyRec(Rect _lRect, Rect _rRect){
		_info = new KeyInfo(_lRect, _rRect);
		_recInfo = new KeyInfo[DEF_REC_BUFF_SIZE];
		_bufPtr = _recSize = 0;
	}
	public TmKeyRec clone(){ return new TmKeyRec(this);	}
	
	public int fixedUpdate(int _padData=int.MaxValue, float _angL=float.MaxValue, float _angR=float.MaxValue){
		int ret = -1;
		if(_recState==REC_STATE.PLAY){
			KeyInfo outInfo;
			ret = playOne(out outInfo);
			if(ret >=0){
				_info = outInfo;
			}
		}else{
			_info.updateInfo(_padData, _angL, _angR);
			if(_recState==REC_STATE.REC){
				ret = recOne(_info);
			}
		}
		
		if(((int)debugMode & (int)DEBUG_MODE.DISP_PAD)!=0){
			_info.pad.debugDisp();
		}
		if(((int)debugMode & (int)DEBUG_MODE.DISP_ANALOG)!=0){
			_info.anL.debugDisp();
			_info.anR.debugDisp();
		}
		return ret;
	}
	
	public REC_STATE setRecState(REC_STATE _newState){
		REC_STATE old = _recState;
		_recState = _newState;
		if(old != _newState){
			_bufPtr = 0;
			if(_newState == REC_STATE.REC){
				_recSize = 0;
			}
		}
		return old;
	}
	
	private int recOne(KeyInfo _data){
		int ret = -1;
		if(_bufPtr < DEF_REC_BUFF_SIZE){
			_recInfo[_bufPtr] = _data.clone();
			ret = _bufPtr;
			_bufPtr++;
			_recSize = _bufPtr;
		}else{
			Debug.Log("TmKeyRec:DEF_REC_BUFF_SIZE over!");	
		}
		return ret;
	}

	private int playOne(out KeyInfo _outInfo, int _ptr=-1){
		_outInfo = new KeyInfo();
		int ret = -1;
		if(_ptr>=0){
			_bufPtr = _ptr;
		}
		if(_bufPtr < _recSize){
			_outInfo = _recInfo[_bufPtr];
			ret = _bufPtr;
			_bufPtr++;
		}
// Debug.Log(_bufPtr+"/"+_recSize+"ang="+_outInfo.anL.angle);
		return ret;
	}
}
