using UnityEngine;
using System.Collections;
using System;

public class TmGesture{
	public const float VERSION = 0.1f; // TmGesture version 
	public class TmTouchEvent{
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
		public Gesture gesture;
		public int[] fingerId;
	}
	
	private int mMaxTouches;
	private float mPinchTh;
	private float mDblTapTime;
	private Vector2[] mSttPosition;
	private Vector2[] mPosition;
	private bool[] mActive;
	private TmTouchEvent.Gesture[] mGesture;
	
	public delegate void OnGestureHandler(TmTouchEvent eve);
	public event OnGestureHandler onGestureHandler;
	
	public TmGesture(int _maxTouches, float _pinchTh, float _dblTapTime){
		mMaxTouches = _maxTouches;
		mPinchTh = Mathf.Abs(_pinchTh);
		mDblTapTime = _dblTapTime;
		mSttPosition = new Vector2[_maxTouches];
		mPosition = new Vector2[_maxTouches];
		mActive = new bool[_maxTouches];
		mGesture = new TmTouchEvent.Gesture[_maxTouches];
		for(int i = 0; i < _maxTouches; ++i){
			mActive[i] = false;
			mGesture[i] = TmTouchEvent.Gesture.NONE;
		}
	}
	
	public void update(Touch[] _tc=null){
		if(_tc==null){ _tc = Input.touches; }
		for(int i = 0; i < mMaxTouches; ++i){
			mActive[i] = false;
		}
		for(int i = 0; i < _tc.Length; ++i){
			int fgId = _tc[i].fingerId;
			if(fgId < mMaxTouches){
				mActive[fgId] = true;
				mPosition[fgId] = _tc[i].position;
				if(_tc[i].phase == TouchPhase.Began){
					mSttPosition[fgId] = _tc[i].position;
				}
			}
		}
		for(int i = 0; i < mMaxTouches; ++i){
			if(!mActive[i]){
				mGesture[i] = TmTouchEvent.Gesture.NONE;
			}
		}
		
		for(int i = 0; i < _tc.Length; ++i){
			int fId0 =  _tc[i].fingerId;
			if((fId0<mActive.Length)&& mActive[fId0]){
				Vector2 s0 = mSttPosition[fId0];
				Vector2 p0 = mPosition[fId0];
				for(int j = i+1; j < _tc.Length; ++j){
					int fId1 =  _tc[j].fingerId;
					if((fId1<mActive.Length)&& mActive[fId1]){
						Vector2 s1 = mSttPosition[fId1];
						Vector2 p1 = mPosition[fId1];
						float sttDist = (s1-s0).magnitude;
						float nowDist = (p1-p0).magnitude;
						float chgDist = nowDist-sttDist;
						Vector2 sc = s0+s1*0.5f;
						Vector2 pc = p0+p1*0.5f;
						bool isCenterFixed = ((pc-sc).magnitude<Mathf.Abs(chgDist)*0.5f);
						if((chgDist > mPinchTh)&& isCenterFixed ){
							int[] ids = new int[2]{Mathf.Min(fId0,fId1),Mathf.Max(fId0,fId1)};
							setEvent(ids,TmTouchEvent.Gesture.PINCHI_OUT);
						}
						if((chgDist < -mPinchTh)&& isCenterFixed ){
							int[] ids = new int[2]{Mathf.Min(fId0,fId1),Mathf.Max(fId0,fId1)};
							setEvent(ids,TmTouchEvent.Gesture.PINCHI_IN);
						}
					}
				}
			}
		}
#if UNITY_EDITOR
		dbgDisp();
#endif
	}
	
	private bool setEvent(int[] _ids, TmTouchEvent.Gesture _gesture){
		bool result = false;
		if((onGestureHandler!=null)&&(_ids.Length>0)){
			if(mGesture[_ids[0]]!=_gesture){
				foreach(int id in _ids){
					mGesture[id]=_gesture;
				}
				TmTouchEvent eve = new TmTouchEvent();
				eve.gesture = _gesture;
				onGestureHandler(eve);
				result = true;
			}
		}
		return result;
	}

#if UNITY_EDITOR
	private void dbgDisp(){
		for(int i=0; i<mMaxTouches; ++i){
			if(mActive[i]){
				Vector3 wSttPos = dbgWPos(mSttPosition[i]);
				Vector3 wPos = dbgWPos(mPosition[i]);
				Debug.DrawLine(wSttPos,wPos,Color.white);
			}
		}
	}
	private Vector3 dbgWPos(Vector2 _scrPos){
		Vector3 vPos = new Vector3(_scrPos.x,_scrPos.y,10f);
		vPos.x /= (float)Screen.width;
		vPos.y /= (float)Screen.height;
		return Camera.main.ViewportToWorldPoint(vPos);
	}
#endif
}
