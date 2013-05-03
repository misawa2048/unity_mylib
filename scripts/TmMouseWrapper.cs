using UnityEngine;
using System.Collections;

public class TmMouseWrapper{
	public const float VERSION = 2.1f;
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
	}
	public enum GESTURE_DIR{
		NONE=0,
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	
	public TmMouseWrapper(){ start();	}
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
	private Vector3    mDragSpd;
	private Vector3    mDragObjOfs;
	private Vector3    mDragSpeed; // average per sec
	private Vector3    mDragScrSpeed; // average per sec
	private float      mDragTime; // total sec from On to Up
	private float      mDragScrTime; // total sec from On to Up
	private float      mDragSweepRate=0.00001f; // per sec
	private float      mDragSweepMin=0.01f;
	private GameObject mTarget;
	private GameObject mTargetOld;
	private GameObject mSweepTarget;
	private int        mMouseHitLayerMask=-1;
	private int        mDraggableLayerMask=-1;
	private float      mRayDist = 50.0f;
	private float      mGestureMinRate = 0.1f;
	private GESTURE_DIR mMouseGestureDir = GESTURE_DIR.NONE;
	private DRAG_MODE   mDragMode = DRAG_MODE.NONE;
	public STATE mouseState{ get{ return mMouseState; } }
	public STATE buttonState{ get{ return mButtonState; } }
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
	public Vector3 dragScrSpeed{ get { return mDragScrSpeed; } }
	public GameObject dragTarget { get { return mTarget; } }
	public GameObject dragTargetOld { get { return mTargetOld; } }
	public GameObject sweepTarget { get { return mSweepTarget; } }
	
	public DRAG_MODE setDragMode(DRAG_MODE mode){DRAG_MODE old = mDragMode; mDragMode = mode; return old; }
	public float setDragSweepRate(float rate){float old = mDragSweepRate; mDragSweepRate = Mathf.Clamp01(rate); return old; }
	public float setRayDist(float dist){float old = mRayDist; mRayDist = dist; return old; }
	public float setGestureMinRate(float rate){float old = mGestureMinRate; mGestureMinRate = rate; return old; }
	public int setHitLayerMask(int mask){int old = mMouseHitLayerMask; mMouseHitLayerMask = mask; return old; }
	public int setDraggableLayerMask(int mask){int old = mDraggableLayerMask; mDraggableLayerMask = mask; return old; }
	public bool isOnMouseTarget(GameObject obj){ return ((mIsMouseHit)&&(mMouseHit.collider.gameObject==obj)); }
	public bool isOnDragTarget(){ return ((mIsMouseHit)&&(mMouseHit.collider.gameObject==mTarget)); }
	public bool isOnDragTarget(GameObject obj){ return (isOnDragTarget())&&(mTarget==obj); }

	public void start (){
		mIsDrag = false;
		mButtonState = STATE.NONE;
		mTarget = null;
		mTargetOld = null;
		mSweepTarget = null;
		mDragSpd = Vector3.zero;
	}
	public void update (){
		mDragPosOld = mDragPos;
		mMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		mIsMouseHit = Physics.Raycast(mMouseRay,out mMouseHit,mRayDist,mMouseHitLayerMask);
//			if( mIsMouseHit && (mMouseHit.collider.gameObject!=mHitBodyPrefab)){ mIsMouseHit = false; }

		if(mIsMouseHit && Input.GetMouseButtonDown(0)) { mIsDrag =true;  mMouseState = STATE.DOWN; mTarget = mMouseHit.collider.gameObject; }
		else if(mIsDrag && Input.GetMouseButtonUp(0))  { mIsDrag =false; mMouseState = STATE.UP;   mTarget = null; }
		else if(mIsDrag && Input.GetMouseButton(0))    { mMouseState = STATE.DRAG; }
		else if(Input.GetMouseButton(0))               { mMouseState = STATE.ON; }
		else{ mMouseState = STATE.NONE; }

		if(Input.GetMouseButtonDown(0)) { mButtonState = STATE.DOWN; }
		else if(Input.GetMouseButtonUp(0)) { mButtonState = STATE.UP; }
		else if(Input.GetMouseButton(0)) { mButtonState = STATE.ON; }
		else{ mButtonState = STATE.NONE; }
		
//		if(mIsMouseHit){ mDragPos = mMouseHit.point; }
//		else{ mDragPos = mMouseRay.origin + mMouseRay.direction * mRayDist; }
		mDragPos = calcDragPos();

		if((mIsMouseHit)&&((mMouseState == STATE.NONE)||(mMouseState == STATE.DOWN))) {
			mDragSttPos = mDragPos;
			mDragPosOld = mDragPos;
			mTargetOld = mTarget;
			mDragTime = 0.0f;
			mDragSpeed = Vector3.zero;
		}else if((mMouseState == STATE.DRAG)||(mMouseState == STATE.UP)) {
			mDragTime += Time.deltaTime;
			mDragSpeed = (mDragPos - mDragSttPos)/mDragTime;
		}
		
		if((mButtonState == STATE.NONE)||(mButtonState == STATE.DOWN)) {
			mDragSttScrPos = Input.mousePosition;
			mDragObjOfs = getDragObjOfs();
			mDragScrTime = 0.0f;
			mDragScrSpeed = Vector3.zero;
		}else if((mButtonState == STATE.ON)||(mButtonState == STATE.UP)){
			mDragScrTime += Time.deltaTime;
			mDragScrSpeed = (Input.mousePosition - mDragSttScrPos)/mDragScrTime;
		}
		
		mMouseGestureDir = getGestureDir(mGestureMinRate);
		dragTargetByMode();
		sweepTargetByMode();
		
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
	private Vector3 calcDragPos(){
		Vector3 dragPos;
		if(mIsMouseHit){
			dragPos = mMouseHit.point;
		}else{
			dragPos = mDragPos;
			if(mDragMode == DRAG_MODE.CONST_Z){
				dragPos = mouseRay.GetPoint((dragPos - Camera.main.transform.position).magnitude);
				dragPos.z = mDragPos.z;
			}
		}
		return dragPos;
	}
	private void dragTargetByMode(){
		if(mDragMode == DRAG_MODE.NONE)     return;
		if((mTarget==null)||(((1<<mTarget.layer)&mDraggableLayerMask)==0))    return;

		if(mMouseState == STATE.DRAG){
			Vector3 dragPos = mTarget.transform.position - mDragObjOfs;
			dragPos = mouseRay.GetPoint((dragPos - Camera.main.transform.position).magnitude);
			dragPos += mDragObjOfs;
			if(mDragMode == DRAG_MODE.CONST_Z){
				dragPos.z = mTarget.transform.position.z;
			}
			mTarget.transform.position = dragPos;
		}
	}
	private bool sweepTargetByMode(){
		if(mDragMode == DRAG_MODE.NONE)     return false;
		if((mSweepTarget==null)||(((1<<mSweepTarget.layer)&mDraggableLayerMask)==0))    return false;
		
		if((mMouseState != STATE.NONE)||(mMouseState == STATE.UP)){
			mDragSpd = (mDragSpd * 3.0f + mDragPos - mDragPosOld)*0.25f;
			if(mTarget!=null){ mSweepTarget = mTarget; }
		}
		if((mMouseState == STATE.NONE)||(mMouseState == STATE.UP)){
			mDragSpd *= Mathf.Pow(mDragSweepRate , Time.deltaTime);
			if(mDragMode == DRAG_MODE.CONST_Z){
				mDragSpd.z = 0.0f;
			}
			if(mDragSpd.magnitude < mDragSweepMin){ mSweepTarget = null; mDragSpd = Vector3.zero; }
			else{ mSweepTarget.transform.position += mDragSpd; }
		}
		return true;
	}
	private Vector3 getDragObjOfs(){
		Vector3 ret = Vector3.zero;
		if( isMouseHit && (mTarget!=null) ){
			ret = mTarget.transform.position - mMouseHit.point;
		}
		return ret;
	}
	
	private void debugDraw(){
		Color col;
		if(mMouseState != STATE.DRAG){ col = mIsMouseHit ? Color.cyan : Color.gray; }
		else{ col = mIsMouseHit ? ((mMouseHit.collider.gameObject==mTarget) ? Color.yellow : Color.white) : Color.red; }
		Debug.DrawRay(mMouseRay.origin,mMouseRay.direction*mRayDist, col);
	}
}
