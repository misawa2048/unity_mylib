using UnityEngine;
using System.Collections;

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
	
	public SpriteAnimation(string animationName, int first, int last, bool loopPlayback){
		name = animationName;
		frames = new int[(last-first<1?1:last-first)];
		for(int ii=0; ii<frames.Length;++ii){ frames[ii]=ii+first; }
		loop = loopPlayback;
	}
}

// 簡易アニメーション
// アニメーション結果を自前Materialに保存またはメッシュを書き換え、
// 使いまわすことでDrawCallBatcingを適用させる 
public class TmSpriteAnim : MonoBehaviour {
	private const float ANIM_TIME_MIN = 0.0001f;
	public Material outMatreial;
	public Vector2 size;
	public Vector2 offset;
	public Vector2[] frames;
	public AnimAttribute[] frameAttrs;
	public SpriteAnimation[] animations;
	public string playOnAwake = "";
	public bool scaleAtUv = false;
	public bool setOnGrid = true;
	public bool reverse = false;
	public float fps = 20.0f;
	private Vector2 _defSize;
	private SpriteAnimation _currentAnm;
	private float _animPtr;
	private AnimAttribute _frameAttr;
	private AnimAttribute _animAttr;
	private AnimAttribute _frameAttrOld;
	private AnimAttribute _animAttrOld;
	private Vector2 _uvOfs;
	private Vector2 _uvPos;
	private Vector2 _texSizeInv;
	private Vector2[] _defUvs = null;
	private Vector3[] _defVtxs = null;
	private Material _tgetMat;
	private bool _isEndOfFrame;
	public bool setOutMaterial(Material _mat){ outMatreial = _mat; return true; }
	public bool isEndFrame{ get{ return (_isEndOfFrame); } }
	public void setUvOfs(Vector2 _uv){ _uvOfs = _uv; }
	public int frameFlag { get{ return (_frameAttr!=null ? _frameAttr.flag : 0); } }
	public int animFlag { get{ return (_animAttr!=null ? _animAttr.flag : 0); } }
	public int frameFragTrigger { get{ return ((_frameAttrOld != _frameAttr) ? frameFlag : 0); } }
	public int animFragTrigger { get{ return ((_animAttrOld != _animAttr) ? animFlag : 0); } }

	public Mesh getMesh(){
		Mesh ret = null;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if((meshFilter!=null)&&(meshFilter.mesh!=null)){
			ret = meshFilter.mesh;
		}
		return ret;
	}
	
	void Awake(){
		_tgetMat = outMatreial!=null ? outMatreial : renderer.sharedMaterial;
		_texSizeInv = new  Vector2(1.0f/(float)(_tgetMat.mainTexture.width),1.0f/(float)(_tgetMat.mainTexture.height));
		_defSize = size;
		if(!scaleAtUv){
			_defSize.Scale(_texSizeInv);
		}
		Mesh nowMesh = getMesh();
		if(frames.Length>0){
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
				_defUvs = new Vector2[sharedMesh.vertexCount];
				_defVtxs = new Vector3[sharedMesh.vertexCount];
				for(int ii = 0; ii < sharedMesh.vertexCount; ++ii){
					_defUvs[ii] = new Vector2(sharedMesh.uv[ii].x,sharedMesh.uv[ii].y);
					_defVtxs[ii] = new Vector3(sharedMesh.vertices[ii].x,sharedMesh.vertices[ii].y,sharedMesh.vertices[ii].z);
				}
			}
		}
		_frameAttr = _frameAttrOld = null;
		_animAttr = _animAttrOld = null;
		_currentAnm = null;
		_animPtr = 0.0f;
		_uvOfs = offset;
		_uvOfs.y *= -1.0f;
		if(setOnGrid){
			_uvOfs = Vector3.Scale(_uvOfs,size ); 
		}
		if(!scaleAtUv){
			_uvOfs.Scale(_texSizeInv);
		}
		if(outMatreial!=null){
			Vector2 sz = size;
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
		_frameAttrOld = _frameAttr;
		_animAttrOld = _animAttr;
		if((_currentAnm==null)||(_currentAnm.frames.Length<=1)) return;
		
		float animSpeed = Time.deltaTime*(fps<0.1f?0.1f:fps) * (!reverse?1.0f:-1.0f);
		float oldPtr = _animPtr;
		_animPtr += animSpeed;
		if(!reverse){
			_isEndOfFrame = ((_animPtr+animSpeed) > (float)_currentAnm.frames.Length);
		}else{
			_isEndOfFrame = ((_animPtr+animSpeed) < 0.0f);
		}
		if(_currentAnm.loop){
			_animPtr = (_animPtr)%(float)_currentAnm.frames.Length;
			if(_animPtr<0.0f){
				_animPtr += (float)_currentAnm.frames.Length;
			}
		}else{
			if(!reverse){
				_animPtr = Mathf.Min(_animPtr,(float)_currentAnm.frames.Length-ANIM_TIME_MIN);
			}else{
				_animPtr = Mathf.Max(0.0f,_animPtr);
			}
		}
		if(Mathf.FloorToInt(oldPtr) != Mathf.FloorToInt(_animPtr)){
			updateAnim();
		}
	}

	public bool PlayAnimation(int _id){
		bool ret = false;
		if(_id < animations.Length){
			_currentAnm = animations[_id];
			_animPtr = 0.0f;
			_isEndOfFrame = false;
			updateAnim();
			ret = true;
		}
		return ret;
	}
	public bool PlayAnimation(string _animName){
		bool ret = false;
		for(int ii = 0; ii < animations.Length; ++ii){
			if(animations[ii].name==_animName){
				ret = PlayAnimation(ii);
				break;
			}
		}
		return ret;
	}
	
	public Mesh setMeshColor(Color _col){
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
	public Mesh setMeshScale(Vector3 _scale){
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
	
	private void updateAnim(){
		int animFrame = Mathf.FloorToInt(_animPtr);
		if((_currentAnm!=null)&&(animFrame < _currentAnm.frames.Length)){
			int viewFrame = _currentAnm.frames[animFrame];
			_uvPos = _uvOfs+getDefFrame(viewFrame);
			if(outMatreial!=null){
				outMatreial.SetTextureOffset("_MainTex",_uvPos);
			}else{
				setMeshUv();
//				setMeshColor(Color.red);
			}
			// attribute取得
			_frameAttr = null;
			for( int ii = 0; ii < frameAttrs.Length; ++ii){
				if(frameAttrs[ii].frame==viewFrame){
					_frameAttr = frameAttrs[ii];
					break;
				}
			}
			_animAttr = null;
			for( int ii = 0; ii < _currentAnm.attrs.Length; ++ii){
				if(_currentAnm.attrs[ii].frame==animFrame){
					_animAttr = _currentAnm.attrs[ii];
					break;
				}
			}
		}
	}
	private Mesh setMeshUv(){
		Mesh nowMesh = getMesh();
		if(nowMesh!=null){
//			Vector2[] defUvs = GetComponent<MeshFilter>().sharedMesh.uv;
			Vector2[] tmpUv = new Vector2[_defUvs.Length];
			Vector2 sz = size;
			if(!scaleAtUv){
				sz.Scale(_texSizeInv);
			}
			for(int ii = 0; ii< _defUvs.Length; ++ii){
				tmpUv[ii] = Vector2.Scale(_defUvs[ii],sz) + _uvPos;
			}
			nowMesh.uv = tmpUv;
		}
		return nowMesh;
	}

	private Vector2 getDefFrame(int _frameId){
		Vector2 defFrame = frames[_frameId];
		if(setOnGrid){
			defFrame = Vector3.Scale(defFrame,size ); 
		}
		if(!scaleAtUv){
			Material tgetMat = outMatreial!=null ? outMatreial : renderer.sharedMaterial;
			Vector2 texSizeInv = new  Vector2(1.0f/(float)(tgetMat.mainTexture.width),1.0f/(float)(tgetMat.mainTexture.height));
			defFrame.Scale(texSizeInv);
		}
		defFrame.y = 1.0f-defFrame.y-_defSize.y;
		return defFrame;
	}
	private Vector3 getDefVertex(int _vtxId){
//		return(GetComponent<MeshFilter>().sharedMesh.vertices[_vtxId]);
		return _defVtxs[_vtxId];
	}
	
	private Mesh initMesh4(Mesh _mesh){
		_mesh.vertices = new Vector3[]{
			new Vector3 (-0.5f, 0.5f, 0.0f),
			new Vector3 (0.5f, 0.5f, 0.0f),
			new Vector3 (0.5f, -0.5f, 0.0f),
			new Vector3 (-0.5f, -0.5f, 0.0f)
		};
		_mesh.triangles = new int[]{ 0, 1, 2, 2, 3, 0 };
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
		_mesh.RecalculateNormals ();
		_mesh.RecalculateBounds ();
		_mesh.Optimize();
		return _mesh;
	}

}
