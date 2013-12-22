using UnityEngine;
using System.Collections;

// 簡易アニメーションSpriteRenderer版 
public class TmSpriteAnim2D : MonoBehaviour {
	[System.Serializable]
	public class AnimAttribute{
		public int frame;
		public int flag;
	}
	[System.Serializable]
	public class SpriteAnimation{
		public string name;
		public int[] frames;
		public AnimAttribute[] attrs;
		public bool loop;
		
		public SpriteAnimation(string _name, int[] _frames, bool _loop){
			name = _name;
			frames = _frames;
			loop = _loop;
		}
	}
	
	public enum FLIP{
		NONE,
		LR,
		UD,
		LRUD
	};
	public const float VERSION = 0.1f;
	private const float ANIM_TIME_MIN = 0.0001f;
	public Sprite[] frames2D;
	public AnimAttribute[] frameAttrs;
	public SpriteAnimation[] animations;
	public string playOnAwake = "";
	public bool reverse = false;
	public FLIP flip = FLIP.NONE;
	public float fps = 20.0f;
	private bool mEnabled;
	private SpriteAnimation mCurrentAnm;
	private float mAnimPtr;
	private AnimAttribute mFrameAttr;
	private AnimAttribute mAnimAttr;
	private AnimAttribute mFrameAttrOld;
	private AnimAttribute mAnimAttrOld;
	private bool mIsEndOfFrame;
	private SpriteRenderer mSprRend;
	public bool isPlay { get{ return mEnabled; } }
	public bool isEndFrame{ get{ return (mIsEndOfFrame); } }
	public int frameFlag { get{ return (mFrameAttr!=null ? mFrameAttr.flag : 0); } }
	public int animFlag { get{ return (mAnimAttr!=null ? mAnimAttr.flag : 0); } }
	public int frameFragTrigger { get{ return ((mFrameAttrOld != mFrameAttr) ? frameFlag : 0); } }
	public int animFragTrigger { get{ return ((mAnimAttrOld != mAnimAttr) ? animFlag : 0); } }

	void Awake(){
		if(!this.enabled) return;
		
		mEnabled = true;
		mFrameAttr = mFrameAttrOld = null;
		mAnimAttr = mAnimAttrOld = null;
		mCurrentAnm = null;
		mAnimPtr = 0.0f;
		mSprRend = GetComponent<SpriteRenderer>();
		mSprRend.color = Color.red;
		if(playOnAwake!=""){
			PlayAnimation(playOnAwake);
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		mFrameAttrOld = mFrameAttr;
		mAnimAttrOld = mAnimAttr;
		if((mCurrentAnm==null)||(mCurrentAnm.frames.Length<=1)) return;
		if(!mEnabled) return;
		
		float oldPtr = mAnimPtr;
		float animSpeed = Time.deltaTime*(fps<0.1f?0.1f:fps) * (!reverse?1.0f:-1.0f);
		mAnimPtr += animSpeed;
		if(!reverse){
			mIsEndOfFrame = ((mAnimPtr+animSpeed) > (float)mCurrentAnm.frames.Length);
		}else{
			mIsEndOfFrame = ((mAnimPtr+animSpeed) < 0.0f);
		}
		if(mCurrentAnm.loop){
			mAnimPtr = (mAnimPtr)%(float)mCurrentAnm.frames.Length;
			if(mAnimPtr<0.0f){
				mAnimPtr += (float)mCurrentAnm.frames.Length;
			}
		}else{
			if(!reverse){
				mAnimPtr = Mathf.Min(mAnimPtr,(float)mCurrentAnm.frames.Length-ANIM_TIME_MIN);
			}else{
				mAnimPtr = Mathf.Max(0.0f,mAnimPtr);
			}
		}

		if(Mathf.FloorToInt(oldPtr) != Mathf.FloorToInt(mAnimPtr)){
			updateAnim();
		}

		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x);
		scl.y = Mathf.Abs(scl.y);
		if((flip == FLIP.LR)||(flip == FLIP.LRUD)){
			scl.x *= -1.0f;
		}
		if((flip == FLIP.UD)||(flip == FLIP.LRUD)){
			scl.y *= -1.0f;
		}
		transform.localScale = scl;
	}

	public bool PlayAnimation(int _id, bool _sameAnimReset=false){
		bool ret = false;
		if(_id < animations.Length){
			mEnabled = true;
			if(_sameAnimReset || (mCurrentAnm != animations[_id])){
				mCurrentAnm = animations[_id];
				mAnimPtr = 0.0f;
				mIsEndOfFrame = false;
				updateAnim();
			}
			ret = true;
		}
		return ret;
	}
	public bool PlayAnimation(string _animName, bool _sameAnimReset=false){
		bool ret = false;
		for(int ii = 0; ii < animations.Length; ++ii){
			if(animations[ii].name==_animName){
				ret = PlayAnimation(ii,_sameAnimReset);
				break;
			}
		}
		return ret;
	}
	public void StopAnimation(){
		mEnabled = false;
	}
	
	public Color SetColor(Color _col){
		Color ret = _col;
		if(mSprRend!=null){
			ret = mSprRend.color;
			mSprRend.color = _col;
		}
		return ret;
	}

	public Sprite[] AddFrame(Sprite _spr){
		if(frames2D==null){
			frames2D = new Sprite[0];
		}
		Sprite[] ret = new Sprite[frames2D.Length+1];
		frames2D.CopyTo(ret,0);
		ret[frames2D.Length]=_spr;
		frames2D = ret;
		return frames2D;
	}
	
	public SpriteAnimation[] AddAnimation(string _name, int[] _frames, bool _loop){
		if(animations==null){
			animations = new SpriteAnimation[0];
		}
		SpriteAnimation[] ret = new SpriteAnimation[animations.Length+1];
		animations.CopyTo(ret,0);
		ret[animations.Length] = new SpriteAnimation(_name, _frames, _loop);
		animations = ret;
		return ret;
	}
	
	private void updateAnim(){
		int animFrame = Mathf.FloorToInt(mAnimPtr);
		if((mCurrentAnm!=null)&&(animFrame < mCurrentAnm.frames.Length)){
			int viewFrame = mCurrentAnm.frames[animFrame];
			if(mSprRend!=null){
				mSprRend.sprite = frames2D[viewFrame];
			}

			// attribute取得
			mFrameAttr = null;
			if(frameAttrs!=null){
				for( int ii = 0; ii < frameAttrs.Length; ++ii){
					if(frameAttrs[ii].frame==viewFrame){
						mFrameAttr = frameAttrs[ii];
						break;
					}
				}
			}
			mAnimAttr = null;
			if(mCurrentAnm.attrs != null){
				for( int ii = 0; ii < mCurrentAnm.attrs.Length; ++ii){
					if(mCurrentAnm.attrs[ii].frame==animFrame){
						mAnimAttr = mCurrentAnm.attrs[ii];
						break;
					}
				}
			}
		}
	}
}
