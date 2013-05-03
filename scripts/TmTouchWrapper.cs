using UnityEngine;

public class TmTouchWrapper{
	public const float VERSION = 1.21f; // TouchWrapper version 
	private const float PLAY_MAX = 0.3f; // 遊び幅 
	public enum Gesture{
		NONE,
		PINCHI_IN,
		PINCHI_OUT,
		SWYPE_UP,
		SWYPE_DOWN,
		SWYPE_LEFT,
		SWYPE_RIGHT,
		DOUBLE_TAP,
	}
	public struct MyTouch{
		public MyTouch(int _id, Vector2 _pos, Vector2 _deltaPos=new Vector2(), float _dtTime=0, int _tapCnt=0, TouchPhase _phase=TouchPhase.Began ){
			fingerId_=_id; position_=_pos; deltaPosition_=_deltaPos; deltaTime_=_dtTime; tapCount_=_tapCnt; phase_=_phase;
		}
		private int fingerId_;			public int fingerId{ get{ return fingerId_; } }
		private Vector2 position_;		public Vector2 position{ get { return position_; } }
		private Vector2 deltaPosition_;	public Vector2 deltaPosition{ get{ return deltaPosition_; } }
		private float deltaTime_;		public float deltaTime{ get{ return deltaTime_; } }
		private int tapCount_;			public int tapCount{ get { return tapCount_; } }
		private TouchPhase phase_;		public TouchPhase phase{ get { return phase_; } }
	};
	public TmTouchWrapper(){ start();	}
	public bool isTouches { get { return(myTouches_.Length>=2); }}
	public float angle { get { return((touchSttAng_==float.MaxValue) ? 0.0f : (touchAng_ - touchSttAng_)); }}
	public float size { get { return((touchSttSize_==float.MaxValue) ? 0.0f : (touchSize_ - touchSttSize_)); }}
	public Vector2 vec { get { return((swypeSttPos_.x==float.MaxValue) ? Vector2.zero : (swypePos_ - swypeSttPos_)); }}
	public float deltaAngle { get { return(valPlay(touchAng_, touchOldAng_, Mathf.PI*0.5f*PLAY_MAX)); }}
	public float deltaSize { get { return(valPlay(touchSize_, touchOldSize_, PLAY_MAX)); }}
	public Vector2 deltaVec { get { return(valPlay(swypePos_, swypeOldPos_, Screen.width*PLAY_MAX)); }}
	public Gesture gesture { get { return gesture_; }}
	public MyTouch[] touches { get { return myTouches_; } }
	public bool isDoubleTap { get { return isDoubleTap_; } }

	private float moveMax;
	private MyTouch[] myTouches_;
	private float touchCkSize_;
	private float touchCkAng_;
	private float touchSize_;
	private float touchAng_;
	private float touchSttSize_;
	private float touchSttAng_;
	private float touchOldSize_;
	private float touchOldAng_;
	private Vector2 swypePos_;
	private Vector2 swypeSttPos_;
	private Vector2 swypeOldPos_;
	private Gesture gesture_;
	private Vector2 tmpSwypeCenter_;
	private float wheelMove_;

	private const float TAP_TIME = 0.5f;
	private int tapRno_;
	private float tapTimer_;
	private Vector2 tapCenter_;
	private bool isDoubleTap_;

	public void start(){
		moveMax = (Screen.width>Screen.height) ? Screen.height : Screen.width; // せまい方基準 
		touchCkSize_ = 0.5f;
		touchCkAng_ = 0.0f;
		touchSttSize_ = float.MaxValue;
		touchSttAng_ = float.MaxValue;
		touchOldSize_ = float.MaxValue;
		touchOldAng_ = float.MaxValue;
		swypePos_ = Vector2.zero;
		swypeSttPos_ = new Vector2(float.MaxValue,float.MaxValue);
		swypeOldPos_ = new Vector2(float.MaxValue,float.MaxValue);
		gesture_ = Gesture.NONE;
		tmpSwypeCenter_ = new Vector2(Screen.width*0.5f,Screen.height*0.5f);
		wheelMove_ = 0.0f;
		tapRno_ = 0;
		tapTimer_ = 0.0f;
		tapCenter_ = new Vector2(Screen.width*0.5f,Screen.height*0.5f);
		isDoubleTap_ = false;
	}		

	public void update(){
		myTouches_ = new MyTouch[Input.touchCount];
		if( Input.touchCount != 0){
			tmpSwypeCenter_ = Vector2.zero;
			for( int ii = 0; ii < Input.touchCount; ++ii ){
				Touch tw = Input.touches[ii];
				myTouches_[ii] = new MyTouch(tw.fingerId,tw.position,tw.deltaPosition,tw.deltaTime,tw.tapCount,tw.phase);
				tmpSwypeCenter_ += tw.position;
			}
			tmpSwypeCenter_ /= Input.touchCount;
		}else{
			tmpSwypeCenter_ = Input.mousePosition;
			wheelMove_ = (Input.GetAxis("Mouse ScrollWheel")!=0.0f) ? Input.GetAxis("Mouse ScrollWheel") : wheelMove_*0.5f;
			if((Mathf.Abs(wheelMove_)>0.001f)||(Input.GetMouseButton(2))){
				if(Input.GetMouseButton(2)){ // swype,zoom,pinch
					touchCkSize_ = Mathf.Clamp(touchCkSize_ + wheelMove_*0.5f,0.1f,0.9f);
				}else{
					touchCkAng_ += wheelMove_*0.5f;
				}
				myTouches_ = new MyTouch[2];
				Vector2 tmpPos = new Vector2(-touchCkSize_*0.5f,0.0f);
				tmpPos.y = Mathf.Sin(touchCkAng_)*tmpPos.x;
				tmpPos.x = Mathf.Cos(touchCkAng_)*tmpPos.x;
				myTouches_[0] = new MyTouch(0,new Vector2(moveMax*(0.5f + tmpPos.x),moveMax*(0.5f + tmpPos.y)));
				myTouches_[1] = new MyTouch(1,new Vector2(moveMax*(0.5f - tmpPos.x),moveMax*(0.5f - tmpPos.y)));
			}else{
				touchCkSize_ = 0.5f;
				touchCkAng_ = 0.0f;
			}
			
		}
		if( myTouches_.Length >= 2){
			Vector2 vec = myTouches_[1].position - myTouches_[0].position;
			touchOldAng_ = touchAng_;
			touchAng_ = Mathf.Atan2(vec.y,vec.x);
			if(touchSttAng_ == float.MaxValue){
				touchSttAng_ = touchAng_;
				touchOldAng_ = touchAng_;
			}
			touchOldSize_ = touchSize_;
			touchSize_ = vec.magnitude / moveMax;
			if(touchSttSize_ == float.MaxValue){
				touchSttSize_ = touchSize_;
				touchOldSize_ = touchSize_;
			}
			swypeOldPos_ = swypePos_;
			swypePos_ = tmpSwypeCenter_;
			if(swypeSttPos_.x == float.MaxValue){
				swypeSttPos_ = swypePos_;
				swypeOldPos_ = swypePos_;
			}
			// for debug
			if(myTouches_.Length >= 2){
				Vector3 v0 = Camera.main.ScreenToWorldPoint(new Vector3(myTouches_[0].position.x,myTouches_[0].position.y,-Camera.main.transform.position.z));
				Vector3 v1 = Camera.main.ScreenToWorldPoint(new Vector3(myTouches_[1].position.x,myTouches_[1].position.y,-Camera.main.transform.position.z));
				Debug.DrawLine(v0,v1,((deltaSize<0.0f) ? Color.red : ((deltaSize>0.0f) ? Color.blue : Color.white)));
			}
		}else{
			touchSttSize_ = float.MaxValue;
			touchOldSize_ = float.MaxValue;
			touchSttAng_ = float.MaxValue;
			touchOldAng_ = float.MaxValue;
			swypeSttPos_ = new Vector2(float.MaxValue,float.MaxValue);
			swypeOldPos_ = new Vector2(float.MaxValue,float.MaxValue);
//			tmpSwypeCenter_ = new Vector2(Screen.width*0.5f,Screen.height*0.5f);
		}
		
		isDoubleTap_ = doubleTapCheck();
	}
	
	private bool doubleTapCheck(){
		bool ret = false;
		tapTimer_ += Time.deltaTime;
		if(tapTimer_ > TAP_TIME){ tapRno_=0; }
		switch(tapRno_){
			case 0:	if(!isTouches){ ++tapRno_; tapTimer_=0.0f; }  break;
			case 1:	if(isTouches){  ++tapRno_; tapTimer_=0.0f; tapCenter_ = tmpSwypeCenter_; }  break;
			case 2:	if(!isTouches){ ++tapRno_; tapTimer_=0.0f; }  break;
			case 3:	if(isTouches){
					if((tapCenter_-tmpSwypeCenter_).magnitude < moveMax*0.1f){
						++tapRno_; tapTimer_=0.0f;
					}
				}
				break;
			case 4:	if(!isTouches){ tapRno_=0; ret = true; } break;
		}
		return ret;
	}
	private float valPlay(float _in, float _old, float _max){
		return((_old==float.MaxValue) ? 0.0f : mulPlay(_in-_old, _max));
	}
	private Vector2 valPlay(Vector2 _in, Vector2 _old, float _max){
		return((_old.x==float.MaxValue) ? Vector2.zero : mulPlay(_in-_old, _max));
	}
	private float mulPlay(float size, float max){
		float sign = (size<0 ? -1.0f : 1.0f);
		size = Mathf.Abs(size);
		max = Mathf.Abs(max);
		if((size*max==0.0f)||(max<size)) return(sign * size);
		float val = size * Mathf.Sin((size/max)*Mathf.PI*0.5f);
		return( sign * val );
	}
	private Vector2 mulPlay(Vector2 size, float max){
		return new Vector2( mulPlay(size.x, max) , mulPlay(size.y, max) );
	}
	float normalAngle(float angle){
		double outAngle = (double)angle;
		if(outAngle > Mathf.PI) {	outAngle -= (Mathf.PI*2.0); }
		if(outAngle < -Mathf.PI){	outAngle += (Mathf.PI*2.0); }
		return (float)outAngle;
	}
}
