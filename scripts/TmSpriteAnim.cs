using UnityEngine;
using System.Collections;

// 簡易アニメーション
// アニメーション結果を自前Materialに保存またはメッシュを書き換え、
// 使いまわすことでDrawCallBatcingを適用させる 
namespace TmLib{
	public class TmSpriteAnim : MonoBehaviour {
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
		public const float VERSION = 2.30f;
		private const float ANIM_TIME_MIN = 0.0001f;
		private const float EDGE_CLIP_RATE = 0.001f;
		private Material outMatreial=null;
		public Vector2 size;
		public Vector2 offset;
		public Sprite[] frames2D; // use only "Slice>Grid" sprite.
		public Vector2[] frames;
		public AnimAttribute[] frameAttrs;
		public SpriteAnimation[] animations;
		public string playOnAwake = "";
		public bool scaleAtUv = true;
		public bool setOnGrid = true;
		public bool reverse = false;
		public FLIP flip = FLIP.NONE;
		public float fps = 20.0f;
		private bool mEnabled;
		private Vector2 mDefSize { get{ return( (scaleAtUv) ? size : Vector2.Scale(size,mTexSizeInv) ); } }
		private SpriteAnimation mCurrentAnm;
		private float mAnimPtr;
		private AnimAttribute mFrameAttr;
		private AnimAttribute mAnimAttr;
		private AnimAttribute mFrameAttrOld;
		private AnimAttribute mAnimAttrOld;
		private Vector2 mUvOfs;
		private Vector2 mUvPos;
		private Vector2 mTexSizeInv = Vector2.one;
		private Vector2[] mDefUvs = null;
		private Vector3[] mDefVtcs = null;
		private Material mTgetMat;
		private bool mIsEndOfFrame;
		public bool isPlay { get{ return mEnabled; } }
		public bool setOutMaterial(Material _mat){ outMatreial = _mat; return true; }
		public bool isEndFrame{ get{ return (mIsEndOfFrame); } }
		public void setUvOfs(Vector2 _uv){ mUvOfs = _uv; }
		public int frameFlag { get{ return (mFrameAttr!=null ? mFrameAttr.flag : 0); } }
		public int animFlag { get{ return (mAnimAttr!=null ? mAnimAttr.flag : 0); } }
		public int frameFragTrigger { get{ return ((mFrameAttrOld != mFrameAttr) ? frameFlag : 0); } }
		public int animFragTrigger { get{ return ((mAnimAttrOld != mAnimAttr) ? animFlag : 0); } }
		
		public Mesh getMesh(){
			Mesh ret = null;
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if((meshFilter!=null)&&(meshFilter.mesh!=null)){
				ret = meshFilter.mesh;
			}
			return ret;
		}
		
		void Awake(){
			if(!this.enabled) return;
			
			if(frames2D.Length>0){
				scaleAtUv = setOnGrid = true;
				frames = new Vector2[frames2D.Length];
				for(int ii = 0; ii < frames2D.Length; ++ii){
					float x = frames2D[ii].rect.x / frames2D[ii].rect.width;
					float y = (frames2D[ii].texture.height-frames2D[ii].rect.y) / frames2D[ii].rect.height -1.0f;
					frames[ii] = new Vector2(x,y);
				}
			}
			
			mEnabled = true;
			Renderer renderer = GetComponent<Renderer> ();
			mTgetMat = outMatreial!=null ? outMatreial : renderer!=null ? renderer.sharedMaterial : null;
			if((mTgetMat != null)&&(mTgetMat.mainTexture!=null)){
				mTexSizeInv = new  Vector2(1.0f/(float)(mTgetMat.mainTexture.width),1.0f/(float)(mTgetMat.mainTexture.height));
			}
			Mesh nowMesh = getMesh();
			if(nowMesh==null){
				MeshFilter meshFilter = GetComponent<MeshFilter>();
				if(meshFilter==null){
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				nowMesh = initMesh4(new Mesh());
				meshFilter.mesh = meshFilter.sharedMesh = nowMesh;
			}
			{
				Mesh sharedMesh = GetComponent<MeshFilter>().sharedMesh;
				mDefUvs = new Vector2[sharedMesh.vertexCount];
				mDefVtcs = new Vector3[sharedMesh.vertexCount];
				for(int ii = 0; ii < sharedMesh.vertexCount; ++ii){
					mDefUvs[ii] = new Vector2(sharedMesh.uv[ii].x,sharedMesh.uv[ii].y);
					mDefVtcs[ii] = new Vector3(sharedMesh.vertices[ii].x,sharedMesh.vertices[ii].y,sharedMesh.vertices[ii].z);
				}
			}
			mFrameAttr = mFrameAttrOld = null;
			mAnimAttr = mAnimAttrOld = null;
			mCurrentAnm = null;
			mAnimPtr = 0.0f;
			mUvOfs = offset;
			mUvOfs.y *= -1.0f;
			if(setOnGrid){
				mUvOfs = Vector3.Scale(mUvOfs,mDefSize ); 
			}
			if(!scaleAtUv){
				mUvOfs.Scale(mTexSizeInv);
			}
			if(outMatreial!=null){
				Vector2 sz = mDefSize;
				if(!scaleAtUv){
					sz.x /= (float)(outMatreial.GetTexture("_MainTex").width);
					sz.y /= (float)(outMatreial.GetTexture("_MainTex").height);
				}
				outMatreial.SetTextureScale("_MainTex",sz); 
			}
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
			mAnimPtr = updateAnimPtr(mAnimPtr,animSpeed);
			if(Mathf.FloorToInt(oldPtr) != Mathf.FloorToInt(mAnimPtr)){
				updateAnim();
			}
			updateMesh();
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
					updateMesh();
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
		
		public Mesh SetMeshColor(Color _col){
			Mesh nowMesh = getMesh();
			if(nowMesh!=null){
				Color[] cols = new Color[nowMesh.vertexCount];
				for(int ii = 0; ii < nowMesh.vertexCount; ++ii){
					cols[ii] = _col;
				}
				nowMesh.colors = cols;
			}
			return nowMesh;
		}
		
		public Mesh SetMeshScale(Vector3 _scale){
			Mesh nowMesh = getMesh();
			if(nowMesh!=null){
				Vector3[] scaleVecs = new Vector3[nowMesh.vertexCount];
				for(int ii = 0; ii < nowMesh.vertexCount; ++ii){
					scaleVecs[ii] = Vector3.Scale(getDefVertex(ii), _scale);
				}
				nowMesh.vertices = scaleVecs;
				nowMesh.RecalculateBounds ();
				nowMesh.Optimize();
			}
			return nowMesh;
		}
		
		public Mesh SetMeshUV(Vector2 _uvPos, Vector2 _size, FLIP _flip, bool _scaleAtUv=true){
			return setMeshUV(_uvPos, _size, _flip, _scaleAtUv);
		}
		public Mesh SetMeshUVByFrame(int _frame){
			if(frames.Length-1 < _frame) return getMesh();
			mUvPos = mUvOfs+getDefFrame(_frame);
			return setMeshUV(mUvPos, mDefSize, flip, scaleAtUv);
		}
		
		public Vector2[] AddFrame(Vector2 _vec){
			if(frames==null){
				frames = new Vector2[0];
			}
			Vector2[] ret = new Vector2[frames.Length+1];
			frames.CopyTo(ret,0);
			ret[frames.Length]=_vec;
			frames = ret;
			return frames;
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
		
		//! from texture tiling to TmSprAnmScale Settings.
		public bool replaceTextureTiling(Material _sharedMat){ 
			bool ret = false;
			Renderer renderer = GetComponent<Renderer> ();
			Material mat = renderer.material;
			Vector2 txSze = mat.GetTextureScale("_MainTex");
			Vector2 txOfs = mat.GetTextureOffset("_MainTex");
			if((txSze!=Vector2.one)&&(txOfs!=Vector2.one)){
				ret = true;
				setOnGrid = true;
				scaleAtUv = true;
				size = txSze;
				txOfs.y = 1.0f-(txSze.y+txOfs.y);
				AddFrame(new Vector2(txOfs.x/txSze.x,txOfs.y/txSze.y));
				SetMeshUVByFrame(0);
				renderer.material = _sharedMat;
			}
			return ret;
		}
		
		private float updateAnimPtr(float _ptr, float _animSpeed){
			_ptr += _animSpeed;
			if(!reverse){
				mIsEndOfFrame = ((_ptr+_animSpeed) > (float)mCurrentAnm.frames.Length);
			}else{
				mIsEndOfFrame = ((_ptr+_animSpeed) < 0.0f);
			}
			if(mCurrentAnm.loop){
				_ptr = (_ptr)%(float)mCurrentAnm.frames.Length;
				if(_ptr<0.0f){
					_ptr += (float)mCurrentAnm.frames.Length;
				}
			}else{
				if(!reverse){
					_ptr = Mathf.Min(_ptr,(float)mCurrentAnm.frames.Length-ANIM_TIME_MIN);
				}else{
					_ptr = Mathf.Max(0.0f,_ptr);
				}
			}
			return _ptr;
		}
		
		private void updateAnim(){
			int animFrame = Mathf.FloorToInt(mAnimPtr);
			if((mCurrentAnm!=null)&&(animFrame < mCurrentAnm.frames.Length)){
				int viewFrame = mCurrentAnm.frames[animFrame];
				mUvPos = mUvOfs+getDefFrame(viewFrame);
				
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
		
		private void updateMesh(){
			if(outMatreial!=null){
				outMatreial.SetTextureOffset("_MainTex",mUvPos);
			}else{
				setMeshUv();
			}
		}
		
		private Mesh setMeshUv(){
			return setMeshUV(mUvPos,mDefSize,flip,scaleAtUv);
		}
		private Mesh setMeshUV(Vector2 _uvPos, Vector2 _size, FLIP _flip, bool _scaleAtUv){
			Mesh nowMesh = getMesh();
			if(nowMesh!=null){
				Vector2[] tmpUv = new Vector2[mDefUvs.Length];
				if(!_scaleAtUv){
					_size.Scale(mTexSizeInv);
				}
				if((_flip == FLIP.LR)||(_flip == FLIP.LRUD)){
					_uvPos.x += _size.x;
					_size.x *= -1.0f;
				}
				if((_flip == FLIP.UD)||(_flip == FLIP.LRUD)){
					_uvPos.y += _size.y;
					_size.y *= -1.0f;
				}
				_size *= (1.0f-EDGE_CLIP_RATE);
				_uvPos += _size*(EDGE_CLIP_RATE*0.5f);
				for(int ii = 0; ii< mDefUvs.Length; ++ii){
					tmpUv[ii] = Vector2.Scale(mDefUvs[ii],_size) + _uvPos;
				}
				nowMesh.uv = tmpUv;
				nowMesh.Optimize();
			}
			return nowMesh;
		}
		
		
		private Vector2 getDefFrame(int _frameId){
			Vector2 defFrame = frames[_frameId];
			if(setOnGrid){
				defFrame = Vector3.Scale(defFrame,mDefSize ); 
			}
			if(!scaleAtUv){
				Renderer renderer = GetComponent<Renderer> ();
				Material tgetMat = outMatreial!=null ? outMatreial : renderer.sharedMaterial;
				Vector2 texSizeInv = new  Vector2(1.0f/(float)(tgetMat.mainTexture.width),1.0f/(float)(tgetMat.mainTexture.height));
				defFrame.Scale(texSizeInv);
			}
			defFrame.y = 1.0f-defFrame.y-mDefSize.y;
			return defFrame;
		}
		private Vector3 getDefVertex(int _vtxId){
			//		return(GetComponent<MeshFilter>().sharedMesh.vertices[_vtxId]);
			return mDefVtcs[_vtxId];
		}
		
		private Mesh initMesh4(Mesh _mesh){
			_mesh.vertices = new Vector3[]{
				new Vector3 (-0.5f, 0.5f, 0.0f),
				new Vector3 (0.5f, 0.5f, 0.0f),
				new Vector3 (0.5f, -0.5f, 0.0f),
				new Vector3 (-0.5f, -0.5f, 0.0f)
			};
			_mesh.uv = new Vector2[]{
				new Vector2 (0.0f, 1.0f),
				new Vector2 (1.0f, 1.0f),
				new Vector2 (1.0f, 0.0f),
				new Vector2 (0.0f, 0.0f)
			};
			_mesh.colors = new Color[]{
				new Color(0.5f,0.5f,0.5f,1.0f),
				new Color(0.5f,0.5f,0.5f,1.0f),
				new Color(0.5f,0.5f,0.5f,1.0f),
				new Color(0.5f,0.5f,0.5f,1.0f)
			};
			_mesh.normals = new Vector3[]{
				new Vector3 (0.0f, 0.0f, 1.0f),
				new Vector3 (0.0f, 0.0f, 1.0f),
				new Vector3 (0.0f, 0.0f, 1.0f),
				new Vector3 (0.0f, 0.0f, 1.0f)
			};
			_mesh.SetTriangles(new int[]{ 0, 1, 2, 2, 3, 0 },0);
			_mesh.RecalculateNormals ();
			_mesh.RecalculateBounds ();
			_mesh.Optimize();
			return _mesh;
		}
	}
} //namespace TmLib
