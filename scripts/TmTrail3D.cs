using UnityEngine;
using System.Collections;

public class TmTrail3D : MonoBehaviour {
	public enum Status{
		Idle,
		ToStart,
		Busy,
		ToEnd,
	};
	private const float FADE_TIME = 0.2f;
	public float time = 0.5f;
	public int divNum = 1;
	public Status status;
	public float autoChangeDist=0f;
	public TmTrail3D(Vector3 _ofs0, Vector3 _ofs1, float _time=0.5f, int _divNum = 1){
		mOfs0=_ofs0;  mOfs1=_ofs1;  time=_time;  divNum=_divNum;
	}
	private float mFadeTimer;
	private int mDataNum;
	private int mDivNum;
	private Vector3 mOfs0;
	private Vector3 mOfs1;
	private MeshFilter mMf;
	private LinePos[] mWorldVecs;
	private Vector3[] mDefOfsVecs; // use if vertices<4 
	private int mSttPtr;
	private int mUseDataNum;
	private Color mDefCol;
	private Vector3 mOldPos;
	private Quaternion mOldRot;
	private Status mOldStatus;
	
	public class LinePos{
		public Vector3 pos;
		public Quaternion rot;
		public LinePos(Vector3 _pos, Quaternion _rot){
			pos = _pos;
			rot = _rot;
		}
	}
	
	// Use this for initialization
	void Start () {
		// auto pos set
		Vector3 tmpScl = transform.localScale;
		float tmpMax = Mathf.Max (tmpScl.x, Mathf.Max (tmpScl.y, tmpScl.z));
		if(tmpScl.x==tmpMax){ tmpScl.y = 0f; tmpScl.z = 0f; }
		else if(tmpScl.y==tmpMax){ tmpScl.x = 0f; tmpScl.z = 0f; }
		else { tmpScl.x = 0f; tmpScl.y = 0f; }
		tmpScl = transform.localRotation*tmpScl;
		mOfs0 = tmpScl * -0.5f;
		mOfs1 = tmpScl * 0.5f;
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.identity;

		switch(status){
		case Status.Idle:       mFadeTimer = 0f;         break;
		case Status.ToStart:    mFadeTimer = 0f;         break;
		case Status.Busy:       mFadeTimer = FADE_TIME;  break;
		case Status.ToEnd:      mFadeTimer = FADE_TIME;  break;
		}
		
		mDefCol = renderer.material.color;
		mMf = GetComponent<MeshFilter> ();
		mMf.sharedMesh = new Mesh ();
		mOldStatus = Status.Idle;
		mDataNum = (int)(time / Time.fixedDeltaTime);
		mDivNum = divNum;
		if(	mDataNum < 2){ mDataNum = 2; }
		if(mDivNum<1){ mDivNum = 1; }
		
		mWorldVecs = new LinePos[mDataNum*mDivNum];
		mDefOfsVecs = new Vector3[4];
		mDefOfsVecs[0]=mDefOfsVecs[1]=mDefOfsVecs[2]=mDefOfsVecs[3]=Vector3.zero;
		mOldPos = transform.position;
		mOldRot = transform.rotation;
		dataInit ();
	}
	
	// Update is called once per frame
	void Update () {
		moveUpdate ();
		if(status!=Status.Idle){
			dataUpdate ();
		}
		
		mOldStatus = status;
		mOldPos = transform.position;
		mOldRot = transform.rotation;
	}

	public LinePos getLinePosByRate(float _rate){
		int pt = (int)(_rate * (float)mWorldVecs.Length);
		pt = Mathf.Clamp(pt,0,mWorldVecs.Length);
		pt = (pt+mSttPtr)%mWorldVecs.Length;
		return mWorldVecs[pt];
	}

	//<!data init
	private void dataInit(){
		for(int ii = 0; ii < mWorldVecs.Length; ++ii){
			mWorldVecs[ii] = new LinePos(transform.position,transform.rotation);
		}
		mOldPos = transform.position;
		mOldRot = transform.rotation;
		mSttPtr = 0;
		mUseDataNum = 0;

		Mesh mesh = CreateTriStrip (mDefOfsVecs);
		Mesh.Destroy (mMf.sharedMesh);
		mMf.sharedMesh = mesh;
	}
	
	//<!data update
	private void dataUpdate () {
		float distSq = (mOldPos-transform.position).sqrMagnitude;
		float rotDiff = Quaternion.Angle(mOldRot,transform.rotation);
		if((distSq<0.0001)&&(rotDiff<0.1f)){ // no move 
			mUseDataNum -= mDivNum;
			if(mUseDataNum<0){
				mUseDataNum = 0;
			}
		}else{
			for(int ii = 0; ii < mDivNum; ++ii){
				float rate = (float)(ii+1)/(float)mDivNum;
				Vector3 hPos = Vector3.Slerp(mOldPos,transform.position,rate);
				Quaternion hRot = Quaternion.Slerp(mOldRot,transform.rotation,rate);
				mWorldVecs [mSttPtr].pos = hPos;
				mWorldVecs [mSttPtr].rot = hRot;
				mSttPtr = (mSttPtr+1) % mWorldVecs.Length;
			}
			mUseDataNum += mDivNum;
			if(mUseDataNum > mDataNum*mDivNum){
				mUseDataNum = mDataNum*mDivNum;
			}
		}
		
		Vector3[] ofsVecs;
		if(mUseDataNum<2){
			ofsVecs = mDefOfsVecs;
		}else{
			ofsVecs = new Vector3[mUseDataNum*2];
			Quaternion inv = Quaternion.Inverse (transform.rotation);
			int ptr = (mSttPtr+(mDataNum*mDivNum-mUseDataNum)) % mWorldVecs.Length;
			if(ptr<0){ ptr+= mWorldVecs.Length; } // 最後尾のデータ 
			for(int ii = 0; ii < mUseDataNum; ++ii ){
				Vector3 pos0 = mWorldVecs[ptr].pos + mWorldVecs[ptr].rot * mOfs0;
				Vector3 pos1 = mWorldVecs[ptr].pos + mWorldVecs[ptr].rot * mOfs1;
				Vector3 lPos0 = inv * (pos0 - transform.position);
				Vector3 lPos1 = inv * (pos1 - transform.position);
				ofsVecs [ii*2+0] = lPos0;
				ofsVecs [ii*2+1] = lPos1;
				ptr++; // 後ろからさかのぼる 
				ptr %= mWorldVecs.Length;
			}
		}
		Mesh mesh = CreateTriStrip (ofsVecs);
		Mesh.Destroy (mMf.sharedMesh);
		mMf.sharedMesh = mesh;
	}
	
	private void moveUpdate () {
		if(autoChangeDist>0f){
			float diff = (transform.position - mOldPos).magnitude;
			switch(status){
			case Status.Idle:
				if(diff > autoChangeDist){
					status = Status.ToStart;
				}
				break;
			case Status.ToStart:
				if(diff < autoChangeDist*0.1f){
					status = Status.ToEnd;
				}
				break;
			case Status.Busy:
				if(diff < autoChangeDist*0.1f){
					status = Status.ToEnd;
				}
				break;
			case Status.ToEnd:
				if(diff > autoChangeDist){
					status = Status.ToStart;
				}
				break;
			}
		}
		
		if(status!=mOldStatus){
			if(mOldStatus==Status.Idle){
				dataInit();
			}
		}
		switch(status){
		case Status.Idle:
			mFadeTimer = 0f;
			break;
		case Status.ToStart:
			mFadeTimer = Mathf.Clamp(mFadeTimer+Time.deltaTime,0f,FADE_TIME);
			if(mFadeTimer==FADE_TIME){ status = Status.Busy; }
			break;
		case Status.Busy:
			mFadeTimer = FADE_TIME;
			break;
		case Status.ToEnd:
			mFadeTimer = Mathf.Clamp(mFadeTimer-Time.deltaTime,0f,FADE_TIME);
			if(mFadeTimer==0f){ status = Status.Idle; dataInit(); }
			break;
		}
		Color col = mDefCol;
		col.a = mDefCol.a * (mFadeTimer / FADE_TIME);
		renderer.material.color = col;
	}
	
	//------------------------------
	//! SendMessage関連 
	private void SM_start(bool _immediate=false){
		status = (_immediate) ? Status.Busy : Status.ToStart;
		autoChangeDist = 0f;
	}
	private void SM_end(bool _immediate=false){
		status = (_immediate) ? Status.Idle : Status.ToEnd;
		autoChangeDist = 0f;
	}
	//------------------------------
	
	//============================
	public static Mesh CreateTriStrip(Vector3[] _verts){
		return CreateTriStrip(_verts, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateTriStrip(Vector3[] _verts, Color _color){
		if (_verts.Length < 3) {
			return null;
		}
		int _vertNum = _verts.Length;
		Vector3[] verts = _verts;
		Vector2[] uvs = new Vector2[_vertNum];
		Vector3[] norms = new Vector3[_vertNum];
		Vector4[] tgts = new Vector4[_vertNum];
		Color[] cols = new Color[_vertNum];
		
		for(int ii=0; ii< _vertNum; ++ii){
			float tu = (float)(ii/2)/(float)((_vertNum/2)-1);
			float tv = (float)(ii&1);
			uvs[ii]= new Vector2(tu,tv);
			cols[ii] = _color;
//			cols[ii].r = (float)ii/(float)_vertNum; //!for test
//			cols[ii].g = (float)ii/(float)_vertNum; //!for test
			norms[ii]= new Vector3(0.0f,0.0f,-1.0f);
			tgts[ii]= new Vector4(1f,0f,0f,0f);
		}
		
		int[] tris = new int[(_vertNum-2)*3];
		for(int ii=0; ii< _vertNum-2; ++ii){
			if((ii&1)==0){
				tris[ii*3+0] = ii+0;
				tris[ii*3+1] = ii+1;
				tris[ii*3+2] = ii+2;
			}else{
				tris[ii*3+0] = ii+2;
				tris[ii*3+1] = ii+1;
				tris[ii*3+2] = ii+0;
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.colors = cols;
		mesh.normals = norms;
		mesh.tangents = tgts;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
}
