using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib
{
	public class TmTubeTrail : MonoBehaviour {
		struct TrailInfo{
			public Vector3 ofsPos;
			public Vector3 targetUp;
			public Quaternion ofsRot;
		}

		[SerializeField]
		int divX=8;
		[SerializeField]
		int divZ=32;

		[SerializeField]
		AnimationCurve trailShape = new AnimationCurve(new Keyframe[2]{new Keyframe(0f,0f),new Keyframe(1f,1f)});

		MeshFilter mf;
		Vector3[] vertBuff;	// 全頂点
		Vector3[] circleBuff; // divXの1つ分
		TrailInfo[] trailInfo;
		int trailInfoPtr;
		Vector3 posOld;
		Quaternion rotOld;
		Transform targetTr;
		int startCnt;


		// Use this for initialization
		void Start () {
			GameObject go = new GameObject ("TrailTarget");
			go.transform.position = transform.position;
			go.transform.rotation = transform.rotation;
			go.transform.SetParent (transform.parent);
			targetTr = go.transform;
			transform.SetParent (null);

			int vertNum = (divX + 1) * (divZ + 1);
			posOld = transform.position;
			rotOld = Quaternion.identity;
			vertBuff = new Vector3[vertNum];
			trailInfo = new TrailInfo[divZ+1];
			for(int i = 0; i < trailInfo.Length; ++i) {
				trailInfo [i] = new TrailInfo ();
			}
			startCnt = trailInfo.Length;
			trailInfoPtr = 0;
//			Debug.Log (vertNum);

			mf = GetComponent<MeshFilter> ();
			Mesh mesh = TmMesh.CreateTubeMesh (divX, divZ, TmMesh.AxisType.XY,true);
			vertBuff = mesh.vertices.Clone () as Vector3[];
//			Debug.Log (vertBuff.Length);

			circleBuff = new Vector3[divX]; // 計算用なので共有頂点で考えて良い
			for (int i = 0; i < divX; ++i) {
				float nowRad = ((float)i/(float)divX)*Mathf.PI*2f;
				circleBuff[i] = new Vector3(Mathf.Sin(nowRad) * 0.5f,-Mathf.Cos(nowRad) * 0.5f,0f);
			}
			for (int zz = 0; zz < (divZ + 1); ++zz) {
				for (int xx = 0; xx < (divX + 0); ++xx) { // 0:現時点（TubeNomalBugFix）時点ではdivX + 1の頂点は未使用なので
					Vector3 tmpPos = circleBuff[xx % divX];
					tmpPos.x *= (float)zz / (float)(divZ);
					tmpPos.y *= (float)zz / (float)(divZ);
					tmpPos.z = -0.5f + ((float)zz / (float)divZ);
					vertBuff [zz * (divX + 1) + xx] = tmpPos;
				}
			}
			mesh.vertices = vertBuff;
			mf.mesh = mesh;
		}

		// Update is called once per frame
		void LateUpdate () {
			transform.position = targetTr.position;
			transform.rotation = Quaternion.identity;

			trailInfo [trailInfoPtr].targetUp = targetTr.up;
			Vector3 dirVec = transform.position - posOld;
			trailInfo [trailInfoPtr].ofsPos = dirVec;
			Quaternion moveRot = (trailInfo [trailInfoPtr].ofsPos == Vector3.zero) ? rotOld : Quaternion.LookRotation (dirVec);
			trailInfo [trailInfoPtr].ofsRot = moveRot; //Quaternion.RotateTowards(rotOld,moveRot,30f);

			int tmpPtr = trailInfoPtr; //(trailInfoPtr + trailInfo.Length - 1) % trailInfo.Length; // 開始場所
			trailInfoPtr = (trailInfoPtr + 1) % trailInfo.Length;

			rotOld = moveRot;
			posOld = transform.position;

			startCnt = Mathf.Max (startCnt - 1, 0); // 開始時トレイルを細くする
			float startRate = (float)startCnt/(float)trailInfo.Length;

			for(int ii = 0; ii < trailInfo.Length; ++ii ){
			}

//			transform.rotation = Quaternion.identity;

			Vector3 tmpPos = Vector3.zero;
			Quaternion tmpRot = targetTr.rotation;
			for (int zz = 0; zz < (divZ + 1); ++zz) {
				Vector3 tmpOfsPos = Quaternion.Inverse(transform.rotation) * trailInfo [tmpPtr].ofsPos;
				Vector3 tmpDir = tmpOfsPos;
				tmpRot = (tmpDir == Vector3.zero) ? rotOld : Quaternion.LookRotation (tmpDir,trailInfo [tmpPtr].targetUp);

				for (int xx = 0; xx < (divX + 1); ++xx) {
					Vector3 tmpRingPos = circleBuff[xx % divX];
					float ringSize = trailShape.Evaluate(1f-(float)zz / (float)(divZ)); // (1f-(float)zz / (float)(divZ));
					// local
					tmpRingPos.x *= Mathf.Max(ringSize - startRate,0f);
					tmpRingPos.y *= Mathf.Max(ringSize - startRate,0f);
					tmpRingPos.z = 0f;
					tmpRingPos = tmpRot * tmpRingPos;
					tmpRingPos += tmpPos / transform.localScale.z;
					vertBuff [zz * (divX + 1) + xx] = tmpRingPos;
				}
				tmpPtr = (tmpPtr + trailInfo.Length - 1) % trailInfo.Length;
				tmpPos -= tmpOfsPos;
			}
			mf.mesh.vertices = vertBuff;
			mf.mesh.RecalculateBounds ();
			mf.mesh.RecalculateNormals ();
		}
	}
} // namespace TmLib
