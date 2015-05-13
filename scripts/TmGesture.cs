using UnityEngine;
using System.Collections;
using System;

namespace TmLib{
	public class TmGesture{
		public const float VERSION = 0.3f; // TmGesture version 
		public class TmTouchEvent{
			public enum Gesture{
				NONE,
				PINCHI_IN,
				PINCHI_OUT,
				LONG_TAP,
				DOUBLE_TAP,
				SWYPE_UP,
				SWYPE_DOWN,
				SWYPE_LEFT,
				SWYPE_RIGHT,
			}
			public Gesture gesture;
			public Touch[] touches;
			public TmTouchEvent(){
				gesture = Gesture.NONE;
				touches = null;
			}
		}
		public class MyInatia{
			private float mInertiaTime = 1f;
			private bool mInetiaBreak = false;
			private float mInetiaTimer;
			private Vector3 mSttScrPos;
			private Vector3 mScrPos;
			private Vector2 mScrDragRate;
			private Vector2 mInetiaRate;
			private float mPinchRate;
			public  float inetiaTime{ get { return mInertiaTime; } set{ mInertiaTime = Mathf.Max(0.01f,value);} }
			public  Vector2 inetiaRate{ get { return mInetiaRate; } }
			public  float pinchRate{ get { return mPinchRate; } }
			
			public void update(){
				if (mInetiaBreak) {
					if (Input.touches.Length == 0) {
						mInetiaBreak = false;
					}
				}else if (Input.touches.Length > 1) {
					mInetiaBreak = true;
				}
				#if UNITY_EDITOR
				if(true){
					#else
					if((!mInetiaBreak)&&(Input.touches.Length == 1)){
						#endif
						mScrPos = Input.mousePosition;
						if (Input.GetMouseButtonDown(0)) {
							mSttScrPos = mScrPos;
							mScrDragRate = Vector2.zero;
						}
						if (Input.GetMouseButton(0)) {
							float size = (float)Mathf.Max (Screen.width,Screen.height);
							mScrDragRate = (mScrPos - mSttScrPos)/size;
							mInetiaTimer = mInertiaTime;
						}
					}
					mInetiaRate = Vector2.Lerp (Vector2.zero, mScrDragRate, mInetiaTimer / mInertiaTime);
					mInetiaTimer = Mathf.Max (mInetiaTimer - Time.deltaTime, 0f);
					
					mPinchRate += (float)(Input.GetAxis ("Mouse ScrollWheel")*1f);
					mPinchRate = Mathf.Clamp01 (mPinchRate);
				}
			}
			
			private int mMaxTouches;
			private float mPinchTh;
			private float mDblTapTime;
			private Vector2[] mSttPosition;
			private Vector2[] mPosition;
			private float[] mHoldTime;
			private bool[] mActive;
			private TmTouchEvent[] mGestureEve;
			private MyInatia mIna;
			public  float inetiaTime{ get { return mIna.inetiaTime; } set{ mIna.inetiaTime = value;} }
			public  Vector2 inetiaRate{ get { return mIna.inetiaRate; } }
			public  float pinchRate{ get { return mIna.pinchRate; } }
			
			public delegate void OnGestureHandler(TmTouchEvent eve);
			public event OnGestureHandler onGestureHandler;
			
			// _pinchTh = px
			public TmGesture(int _maxTouches=4, float _pinchThPx=50f, float _dblTapTime=0.8f){
				mMaxTouches = _maxTouches;
				mPinchTh = Mathf.Abs(_pinchThPx);
				mDblTapTime = _dblTapTime;
				mSttPosition = new Vector2[_maxTouches];
				mPosition = new Vector2[_maxTouches];
				mHoldTime = new float[_maxTouches];
				mActive = new bool[_maxTouches];
				mGestureEve = new TmTouchEvent[_maxTouches];
				for(int i = 0; i < _maxTouches; ++i){
					mActive[i] = false;
					mGestureEve[i] = new TmTouchEvent();
					mGestureEve[i].gesture = TmTouchEvent.Gesture.NONE;
					mGestureEve[i].touches = null;
					mHoldTime[i]=0f;
				}
				mIna = new MyInatia(); 
			}
			
			public void update(Touch[] _tc=null){
				mIna.update(); 
				if(_tc==null){ _tc = Input.touches; }
				for(int i = 0; i < mMaxTouches; ++i){
					mActive[i] = false;
				}
				for(int i = 0; i < _tc.Length; ++i){
					int fgId = _tc[i].fingerId;
					if(fgId < mMaxTouches){
						mActive[fgId] = true;
						mPosition[fgId] = _tc[i].position;
						mHoldTime[fgId] += Time.deltaTime;
						if(_tc[i].phase == TouchPhase.Began){
							mSttPosition[fgId] = _tc[i].position;
							mHoldTime[fgId] = 0f;
						}else if(_tc[i].phase == TouchPhase.Moved){
							mHoldTime[fgId] = 0f;
						}
						if(_tc.Length!=1){
							mHoldTime[fgId] = 0f;
						}
					}
				}
				for(int i = 0; i < mMaxTouches; ++i){
					if(!mActive[i]){
						mGestureEve[i].gesture = TmTouchEvent.Gesture.NONE;
						mGestureEve[i].touches = null;
					}
				}
				
				for(int i = 0; i < _tc.Length; ++i){
					int fId0 =  _tc[i].fingerId;
					if((fId0<mActive.Length)&& mActive[fId0]){
						Vector2 s0 = mSttPosition[fId0];
						Vector2 p0 = mPosition[fId0];
						if(mHoldTime[fId0] > mDblTapTime){
							if((s0-p0).magnitude < mPinchTh*0.25f){
								Touch[] tc = new Touch[1]{_tc[i]};
								setEvent(tc,TmTouchEvent.Gesture.LONG_TAP);
								continue;
							}
						}
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
									Touch[] tc = new Touch[2]{_tc[(fId0<fId1)?i:j],_tc[(fId0<fId1)?j:i]};
									setEvent(tc,TmTouchEvent.Gesture.PINCHI_OUT);
								}else
								if((chgDist < -mPinchTh)&& isCenterFixed ){
									Touch[] tc = new Touch[2]{_tc[(fId0<fId1)?i:j],_tc[(fId0<fId1)?j:i]};
									setEvent(tc,TmTouchEvent.Gesture.PINCHI_IN);
								}
							}
						}
					}
				}
				#if UNITY_EDITOR
				dbgDisp();
				#endif
			}
			
			private bool setEvent(Touch[] _tc, TmTouchEvent.Gesture _gesture){
				bool result = false;
				if((onGestureHandler!=null)&&(_tc.Length>0)){
					//			if(mGestureEve[_tc[0].fingerId].gesture!=_gesture){
					foreach(Touch t in _tc){
						mGestureEve[t.fingerId].gesture=_gesture;
						mGestureEve[t.fingerId].touches = _tc;
					}
					TmTouchEvent eve = new TmTouchEvent();
					eve.gesture = _gesture;
					eve.touches = _tc;
					onGestureHandler(eve);
					result = true;
					//			}
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
	}// namespace TmLib
