using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib{
	public class TmRains : MonoBehaviour {
		[SerializeField]
		int RainNum = 100;
		[SerializeField]
		float Radius = 5f;
		[SerializeField]
		float RandomRadius = 0.5f;
		[SerializeField]
		Vector3 FromDir = Vector3.up * 10f;

		Mesh rainMesh;
		// Use this for initialization
		void Start () {
			Vector3[] vertces = new Vector3[RainNum*2];
			for (int i = 0; i < RainNum; ++i) {
				Vector2 sPos = Random.insideUnitCircle * Radius;
				Vector2 rPos = Random.insideUnitCircle * RandomRadius;
				Vector3 wPos = new Vector3 (sPos.x, 0f, sPos.y);
				vertces [i*2+0] = wPos;
				vertces [i*2+1] = wPos + FromDir + new Vector3(rPos.x,0f,rPos.y);
			}
			rainMesh = TmLib.TmMesh.CreateLine (vertces, TmLib.TmMesh.LineMeshType.Lines, Color.white);
			GetComponent<MeshFilter> ().mesh = rainMesh;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
