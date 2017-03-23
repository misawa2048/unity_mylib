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
		float Length = 5f;

		Mesh rainMesh;
		// Use this for initialization
		void Start () {
			Vector3[] vertces = new Vector3[RainNum*2];
			for (int i = 0; i < RainNum; ++i) {
				Vector2 sPos = Random.insideUnitCircle * Radius;
				Vector3 wPos = new Vector3 (sPos.x, (Random.value-0.5f) * Length, sPos.y);
				vertces [i*2+0] = wPos;
				vertces [i*2+1] = wPos + Vector3.down*Length + Random.insideUnitSphere * Radius * 0.1f;
			}
			rainMesh = TmLib.TmMesh.CreateLine (vertces, TmLib.TmMesh.LineMeshType.Lines, Color.white);
//			rainMesh = TmLib.TmMesh.CreateLineCircle(8,0f,TmMesh.AxisType.XY,Color.white,0.5f);
			GetComponent<MeshFilter> ().mesh = rainMesh;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
