using UnityEngine;
using System.Collections;

public class TmMouseWrapper{
	public const float VERSION = 3.1f;
	public enum STATE{
		NONE=0,
		DOWN,
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
	private Camera     mCamera;
	private bool       mStarted=false;
	private bool       mIsDrag;
	private STATE      mButtonState;
	private bool       mIsMouseHit;
	private bool       mIsMouseHit2D;
	private Ray        mMouseRay;
	private RaycastHit mMouseHit;
	private RaycastHit2D mMouseHit2D;
	private Vector3    mMousePosition;
	private Vector3    mDragSttPos;
	private Vector3    mDragSttScrPos;
	private Vector3    mDragPos;
	private Vector3    mDragPosOld;
	private Vector3    mDragObjOfs;
	private Vector3    mDragSpeed; // average per sec
	private GameObject mTarget;
	private GameObject mTargetOld;
	private GameObject mDragTarget;
	private GameObject mDragTargetOld;
	private GameObject mTarget2D;
	private GameObject mTarget2DOld;
	private GameObject mDragTarget2D;
	private GameObject mDragTarget2DOld;
	private int        mMouseHitLayerMask=-1;
	private int        mDraggableLayerMask=-1;
	private float      mRayDist = 50.0f;
	private float      mGestureMinRate = 0.1f;
	private GESTURE_DIR mMouseGestureDir = GESTURE_DIR.NONE;
	private DRAG_MODE   mDragMode = DRAG_MODE.NONE;
	private float      mDefPlaneDist = 10.0f;
	private Plane      mMousePlane;
	private Plane      mDragPlane;
	public STATE buttonState{ get{ return mButtonState; } }
	public Camera setCamera(Camera _cam){ Camera old = mCamera; mCamera = _cam; return old; }
	public bool isDrag(){ return mIsDrag; }
	public bool isDragContinue(){ return (mIsDrag&&(mButtonState==STATE.ON)); }
	public bool isButtonState(STATE _state){ return (_state==mButtonState); }
	public GESTURE_DIR mouseGestureDir { get { return mMouseGestureDir; } }
	public bool isMouseHit{ get{ return mIsMouseHit; } }
	public bool isMouseHit2D{ get{ return mIsMouseHit2D; } }
	public RaycastHit mouseHit{ get{ return mMouseHit; } }
	public RaycastHit2D mouseHit2D{ get{ return mMouseHit2D; } }
	public Vector3 dragSttScrPos{ get{ return mDragSttScrPos; } }
	public Vector3 dragScrVec{ get{ return (mMousePosition - mDragSttScrPos); } }
	public Ray mouseRay{ get { return mMouseRay; } }
	public Vector3 dragVec{ get { return (mDragPos - mDragSttPos); } }
	public Vector3 dragPos{ get { return mDragPos; } }
	public Vector3 dragPosOld{ get { return mDragPosOld; } }
	public Vector3 dragSpeed{ get { return mDragSpeed; } }
	public Vector3 dragScrSpeed{ get { return dragScrVec; } }
	public Quaternion dragScrRot(Vector3 _center){	return Quaternion.FromToRotation(mMousePosition - _center,mDragSttScrPos - _center); }
	public GameObject hitTarget { get { return mTarget; } }
	public GameObject hitTargetOld { get { return mTargetOld; } }
	public GameObject dragTarget { get { return mDragTarget; } }
	public GameObject dragTargetOld { get { return mDragTargetOld; } }
	public GameObject hitTarget2D { get { return mTarget2D; } }
	public GameObject hitTarget2DOld { get { return mTarget2DOld; } }
	public GameObject dragTarget2D { get { return mDragTarget2D; } }
	public GameObject dragTarget2DOld { get { return mDragTarget2DOld; } }
	public Vector3 dragTargetOfs { get { return mDragObjOfs; } }
	
	public DRAG_MODE setDragMode(DRAG_MODE mode){DRAG_MODE old = mDragMode; mDragMode = mode; return old; }
	public float setRayDist(float dist){float old = mRayDist; mRayDist = dist; return old; }
	public float setGestureMinRate(float rate){float old = mGestureMinRate; mGestureMinRate = rate; return old; }
	public int setHitLayerMask(int mask){int old = mMouseHitLayerMask; mMouseHitLayerMask = mask; return old; }
	public int setDraggableLayerMask(int mask){int old = mDraggableLayerMask; mDraggableLayerMask = mask; return old; }
	public bool isHover(GameObject obj){ return ((obj!=null)&&(mTarget==obj)); }
	public bool isEnter(GameObject obj){ return ((obj!=null)&&(mTarget==obj)&&(mTarget!=mTargetOld)); }
	public bool isOnDragTarget(){ return isHover(mDragTarget); }
	public bool isOnDragTarget(GameObject obj){ return (isOnDragTarget())&&(mDragTarget==obj); }
	public bool isHover2D(GameObject obj){ return ((obj!=null)&&(mTarget2D==obj)); }
	public bool isEnter2D(GameObject obj){ return ((obj!=null)&&(mTarget2D==obj)&&(mTarget2D!=mTarget2DOld)); }
	public bool isOnDragTarget2D(){ return isHover(mDragTarget2D); }
	public bool isOnDragTarget2D(GameObject obj){ return (isOnDragTarget2D())&&(mDragTarget2D==obj); }
	
	private void awake (){
		mIsDrag = false;
		mButtonState = STATE.NONE;
		mTarget = mTargetOld = null;
		mTarget2D = mTarget2DOld = null;
		mDragTarget = mDragTargetOld = null;
		mDragTarget2D = mDragTarget2DOld = null;
		mDragSpeed = Vector3.zero;
	}
	private void start(){
		mStarted = true;
		mCamera = Camera.main;
		mMousePlane = calcPlane();
	}
	public void update (bool _isAutoUpdateInput = true){
		if(!mStarted){ start(); }
		if(_isAutoUpdateInput){ updateInput(); }
		
		mDragPosOld = mDragPos;
		mMouseRay = mCamera.ScreenPointToRay(mMousePosition);
		mIsMouseHit = Physics.Raycast(mMouseRay,out mMouseHit,mRayDist,mMouseHitLayerMask);
		Vector3 wPos = mCamera.ScreenToWorldPoint(mMousePosition);
		mMouseHit2D = Physics2D.Raycast(wPos,wPos);
		mIsMouseHit2D = (mMouseHit2D.collider!=null);
		mTargetOld = mTarget;
		mTarget = ((mIsMouseHit)&&(mMouseHit.collider!=null)) ? mMouseHit.collider.gameObject : null;
		mTarget2DOld = mTarget2D;
		mTarget2D = ((mIsMouseHit2D)&&(mMouseHit2D.collider!=null)) ? mMouseHit2D.collider.gameObject : null;
		//		if( mIsMouseHit && (mMouseHit.collider.gameObject!=mHitBodyPrefab)){ mIsMouseHit = false; }
		
		if(mIsMouseHit && isButtonState(STATE.DOWN)) {
			mIsDrag =true;  
			mDragTarget = mIsMouseHit ? mMouseHit.collider.gameObject : null;
			mDragTarget2D = mIsMouseHit2D ? mMouseHit2D.collider.gameObject : null;
		}
		else if(mIsDrag && isButtonState(STATE.UP))  {
			mIsDrag =false;
			mDragTarget = null;
			mDragTarget2D = null;
		}
		
		mMousePlane = calcPlane();
		
		if((mButtonState == STATE.NONE)||(mButtonState == STATE.DOWN)) {
			mDragPlane = mMousePlane;
			mDragSttScrPos = mMousePosition;
			if(mIsMouseHit){
				mDragSttPos = mMouseHit.point;
			}else{
				mDragSttPos = mMouseRay.origin+mMouseRay.direction*mDefPlaneDist;
			}
		}
		
		float tmpDist;
		mDragSpeed = mDragPos;
		if(mDragPlane.Raycast(mMouseRay, out tmpDist)){
			mDragPos = mMouseRay.GetPoint(tmpDist);
		}
		mDragSpeed = mDragPos - mDragSpeed;
		
		if((mIsMouseHit)&&((mButtonState == STATE.NONE)||(mButtonState == STATE.DOWN))) {
			mDragTargetOld = mDragTarget;
			mDragTarget2DOld = mDragTarget2D;
			mDragSpeed = Vector3.zero;
		}
		
		
		mMouseGestureDir = getGestureDir(mGestureMinRate);
		dragTargetByMode();
		dragTarget2DByMode();
		
		debugDraw();
	}
	
	public void updateInput(STATE _buttonState, Vector3 _mousePosition){
		mMousePosition = _mousePosition;
		mButtonState = _buttonState;
	}
	private void updateInput(){
		mMousePosition = Input.mousePosition;
		if(Input.GetMouseButtonDown(0))    { mButtonState = STATE.DOWN; }
		else if(Input.GetMouseButtonUp(0)) { mButtonState = STATE.UP; }
		else if(Input.GetMouseButton(0))   { mButtonState = STATE.ON; }
		else{ mButtonState = STATE.NONE; }
	}
	
	private GESTURE_DIR getGestureDir(float minRate){
		GESTURE_DIR retDir = GESTURE_DIR.NONE;
		Vector3 dir = (mMousePosition - mDragSttScrPos)/Screen.width; // 幅を基準 
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
				dist = (mMouseHit.point-mCamera.transform.position).magnitude;
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
			return (new Plane(-mCamera.transform.forward,inPos));
		}
	}
	
	private void dragTargetByMode(){
		if(mDragTarget==null)    return;
		
		Vector3 dragPos = Vector3.zero;
		if((mIsMouseHit&&isButtonState(STATE.DOWN))||(isButtonState(STATE.ON))){
			if(mDragMode == DRAG_MODE.CONST_CAMERA_DIST){
				dragPos = mMouseRay.GetPoint(mDragPlane.distance);
			}else{
				float tmpDist;
				if(mDragPlane.Raycast(mMouseRay, out tmpDist)){
					dragPos = mMouseRay.GetPoint(tmpDist);
				}
			}
			if(mIsMouseHit&&isButtonState(STATE.DOWN)){
				if( isMouseHit && (mDragTarget!=null) ){
					mDragObjOfs = mDragTarget.transform.position - dragPos;
				}
			}
			
			if(mDragMode == DRAG_MODE.NONE)     return;
			if(((1<<mDragTarget.layer)&mDraggableLayerMask)==0)  return;
			
			dragPos += mDragObjOfs;
			if(mDragMode == DRAG_MODE.CONST_X){	dragPos.x = mDragTarget.transform.position.x; }
			else if(mDragMode == DRAG_MODE.CONST_Y){	dragPos.y = mDragTarget.transform.position.y; }
			else if(mDragMode == DRAG_MODE.CONST_Z){	dragPos.z = mDragTarget.transform.position.z; }
			
			mDragTarget.transform.position = dragPos;
		}
	}
	private void dragTarget2DByMode(){
	}
	
	private void debugDraw(){
		Color col;
		if(mIsDrag && isButtonState(STATE.ON)){ col = mIsMouseHit ? Color.cyan : Color.gray; }
		else{ col = mIsMouseHit ? ((mMouseHit.collider.gameObject==mDragTarget) ? Color.yellow : Color.white) : Color.red; }
		Debug.DrawRay(mMouseRay.origin,mMouseRay.direction*mRayDist, col);
		debugDrawGizmo(mDragPos,col,0.2f);
	}
	private void debugDrawGizmo(Vector3 _pos, Color _col, float _scl=1.0f){
		Debug.DrawLine(_pos-Vector3.forward*_scl,_pos+Vector3.forward*_scl,_col);
		Debug.DrawLine(_pos-Vector3.right*_scl,_pos+Vector3.right*_scl,_col);
		Debug.DrawLine(_pos-Vector3.up*_scl,_pos+Vector3.up,_col*_scl);
	}
}
