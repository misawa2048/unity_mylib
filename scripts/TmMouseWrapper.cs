using UnityEngine;
using System.Collections;

public class TmMouseWrapper{
	public const float VERSION = 3.0f;
	public enum STATE{
		NONE=0,
		DOWN,
		DRAG,
		ON,
		UP
	}
	public enum DRAG_MODE{
		NONE=0,
		CONST_X,
		CONST_Y,
		CONST_Z,
		CONST_CAMERA_NORMAL,
		CONST_CAMERA_DIST,
	}
	public enum GESTURE_DIR{
		NONE=0,
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	
	public TmMouseWrapper(){ awake();	}
	private bool       mStarted=false;
	private bool       mIsDrag;
	private STATE      mMouseState;
	private STATE      mButtonState;
	private bool       mIsMouseHit;
	private Ray        mMouseRay;
	private RaycastHit mMouseHit;
	private Vector3    mDragSttPos;
	private Vector3    mDragSttScrPos;
	private Vector3    mDragPos;
	private Vector3    mDragPosOld;
	private Vector3    mDragObjOfs;
	private Vector3    mDragSpeed; // average per sec
	private GameObject mTarget;
	private GameObject mTargetOld;
	private int        mMouseHitLayerMask=-1;
	private int        mDraggableLayerMask=-1;
	private float      mRayDist = 50.0f;
	private float      mGestureMinRate = 0.1f;
	private GESTURE_DIR mMouseGestureDir = GESTURE_DIR.NONE;
	private DRAG_MODE   mDragMode = DRAG_MODE.NONE;
	private float      mDefPlaneDist = 10.0f;
	private Plane      mMousePlane;
	private Plane      mDragPlane;
	public STATE mouseState{ get{ return mMouseState; } }
	public STATE buttonState{ get{ return mButtonState; } }
	public bool isMouseState(STATE _state){ return (_state==mMouseState); }
	public bool isButtonState(STATE _state){ return (_state==mButtonState); }
	public GESTURE_DIR mouseGestureDir { get { return mMouseGestureDir; } }
	public bool isMouseHit{ get{ return mIsMouseHit; } }
	public RaycastHit mouseHit{ get{ return mMouseHit; } }
	public Vector3 dragSttScrPos{ get{ return mDragSttScrPos; } }
	public Vector3 dragScrVec{ get{ return (Input.mousePosition - mDragSttScrPos); } }
	public Ray mouseRay{ get { return mMouseRay; } }
	public Vector3 dragVec{ get { return (mDragPos - mDragSttPos); } }
	public Vector3 dragPos{ get { return mDragPos; } }
	public Vector3 dragPosOld{ get { return mDragPosOld; } }
	public Vector3 dragSpeed{ get { return mDragSpeed; } }
	public Vector3 dragScrSpeed{ get { return dragScrVec; } }
	public GameObject hitTarget { get { return ((mIsMouseHit)&&(mMouseHit.collider!=null)) ? mMouseHit.collider.gameObject : null; } }
	public GameObject dragTarget { get { return mTarget; } }
	public GameObject dragTargetOld { get { return mTargetOld; } }
	
	public DRAG_MODE setDragMode(DRAG_MODE mode){DRAG_MODE old = mDragMode; mDragMode = mode; return old; }
	public float setRayDist(float dist){float old = mRayDist; mRayDist = dist; return old; }
	public float setGestureMinRate(float rate){float old = mGestureMinRate; mGestureMinRate = rate; return old; }
	public int setHitLayerMask(int mask){int old = mMouseHitLayerMask; mMouseHitLayerMask = mask; return old; }
	public int setDraggableLayerMask(int mask){int old = mDraggableLayerMask; mDraggableLayerMask = mask; return old; }
	public bool isHover(GameObject obj){ return ((mIsMouseHit)&&(mMouseHit.collider!=null)&&(mMouseHit.collider.gameObject==obj)); }
	public bool isOnDragTarget(){ return isHover(mTarget); }
	public bool isOnDragTarget(GameObject obj){ return (isOnDragTarget())&&(mTarget==obj); }

	private void awake (){
		mIsDrag = false;
		mButtonState = STATE.NONE;
		mTarget = null;
		mTargetOld = null;
		mDragSpeed = Vector3.zero;
	}
	private void start(){
		mStarted = true;
		mMousePlane = calcPlane();
	}
	public void update (){
		if(!mStarted){ start(); }
		mDragPosOld = mDragPos;
		mMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		mIsMouseHit = Physics.Raycast(mMouseRay,out mMouseHit,mRayDist,mMouseHitLayerMask);
//		if( mIsMouseHit && (mMouseHit.collider.gameObject!=mHitBodyPrefab)){ mIsMouseHit = false; }

		if(mIsMouseHit && Input.GetMouseButtonDown(0)) { mIsDrag =true;  mMouseState = STATE.DOWN; mTarget = mMouseHit.collider.gameObject; }
		else if(mIsDrag && Input.GetMouseButtonUp(0))  { mIsDrag =false; mMouseState = STATE.UP;   mTarget = null; }
		else if(mIsDrag && Input.GetMouseButton(0))    { mMouseState = STATE.DRAG; }
		else if(Input.GetMouseButton(0))               { mMouseState = STATE.ON; }
		else{ mMouseState = STATE.NONE; }

		if(Input.GetMouseButtonDown(0)) { mButtonState = STATE.DOWN; }
		else if(Input.GetMouseButtonUp(0)) { mButtonState = STATE.UP; }
		else if(Input.GetMouseButton(0)) { mButtonState = STATE.ON; }
		else{ mButtonState = STATE.NONE; }
		
		mMousePlane = calcPlane();
		
		if((mButtonState == STATE.NONE)||(mButtonState == STATE.DOWN)) {
			mDragPlane = mMousePlane;
			mDragSttScrPos = Input.mousePosition;
			if(mIsMouseHit){
				mDragSttPos = mMouseHit.point;
			}else{
				mDragSttPos = mMouseRay.origin+mMouseRay.direction*mDefPlaneDist;
			}
		}

		float tmpDist;
		mDragSpeed = mDragPos;
		if(mDragPlane.Raycast(mouseRay, out tmpDist)){
			mDragPos = mouseRay.GetPoint(tmpDist);
		}
		mDragSpeed = mDragPos - mDragSpeed;

		if((mIsMouseHit)&&((mMouseState == STATE.NONE)||(mMouseState == STATE.DOWN))) {
			mTargetOld = mTarget;
			mDragSpeed = Vector3.zero;
		}
		
		
		mMouseGestureDir = getGestureDir(mGestureMinRate);
		dragTargetByMode();
		
		debugDraw();
	}
	
	private GESTURE_DIR getGestureDir(float minRate){
		GESTURE_DIR retDir = GESTURE_DIR.NONE;
		Vector3 dir = (Input.mousePosition - mDragSttScrPos)/Screen.width; // 幅を基準 
		if(Mathf.Abs(dir.magnitude)>minRate){
			if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y)){
				retDir = (dir.x>0.0f) ? GESTURE_DIR.RIGHT : GESTURE_DIR.LEFT;
			}else{
				retDir = (dir.y>0.0f) ? GESTURE_DIR.UP : GESTURE_DIR.DOWN;
			}
		}
		return retDir;
	}
	private Plane calcPlane(){
		if(mDragMode == DRAG_MODE.CONST_CAMERA_DIST){
			float dist;
			if(mIsMouseHit){
				dist = (mMouseHit.point-Camera.main.transform.position).magnitude;
			}else{
				dist = mDefPlaneDist;
			}
			return (new Plane(mMouseRay.direction,dist));
		}else{
			Vector3 inPos;
			if(mIsMouseHit){
				inPos = mMouseHit.point;
			}else{
				inPos = mMouseRay.origin+mMouseRay.direction*mDefPlaneDist;
			}
			return (new Plane(-Camera.main.transform.forward,inPos));
		}
	}
	
	private void dragTargetByMode(){
		if(mDragMode == DRAG_MODE.NONE)     return;
		if((mTarget==null)||(((1<<mTarget.layer)&mDraggableLayerMask)==0))    return;
		
		Vector3 dragPos = Vector3.zero;
		if((mMouseState == STATE.DOWN)||(mMouseState == STATE.DRAG)){
			if(mDragMode == DRAG_MODE.CONST_CAMERA_DIST){
				dragPos = mouseRay.GetPoint(mDragPlane.distance);
			}else{
				float tmpDist;
				if(mDragPlane.Raycast(mouseRay, out tmpDist)){
					dragPos = mouseRay.GetPoint(tmpDist);
				}
			}
			if(mMouseState == STATE.DOWN){
				if( isMouseHit && (mTarget!=null) ){
					mDragObjOfs = mTarget.transform.position - dragPos;
				}
			}
			dragPos += mDragObjOfs;
			if(mDragMode == DRAG_MODE.CONST_X){	dragPos.x = mTarget.transform.position.x; }
			else if(mDragMode == DRAG_MODE.CONST_Y){	dragPos.y = mTarget.transform.position.y; }
			else if(mDragMode == DRAG_MODE.CONST_Z){	dragPos.z = mTarget.transform.position.z; }
		
			mTarget.transform.position = dragPos;
		}
	}
	
	private void debugDraw(){
		Color col;
		if(mMouseState != STATE.DRAG){ col = mIsMouseHit ? Color.cyan : Color.gray; }
		else{ col = mIsMouseHit ? ((mMouseHit.collider.gameObject==mTarget) ? Color.yellow : Color.white) : Color.red; }
		Debug.DrawRay(mMouseRay.origin,mMouseRay.direction*mRayDist, col);
		debugDrawGizmo(mDragPos,col);
	}
	private void debugDrawGizmo(Vector3 _pos, Color _col, float _scl=1.0f){
		Debug.DrawLine(_pos-Vector3.forward*_scl,_pos+Vector3.forward*_scl,_col);
		Debug.DrawLine(_pos-Vector3.right*_scl,_pos+Vector3.right*_scl,_col);
		Debug.DrawLine(_pos-Vector3.up*_scl,_pos+Vector3.up,_col*_scl);
	}
}
