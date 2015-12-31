using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TmLib{
	public class TmPlanets {
		public enum Planet{
			Earth=0,	// 地球
			Sun=1,		// 太陽
			Moon=2,		// 月
			Mars=3,		// 火星
			Mercury=4,	// 水星
			Jupiter=5,	// 木星
			Venus=6,	// 金星
			Saturn=7,	// 土星
			Uranus=8,	// 天王星
			Pluto=9,	// 冥王星
			Neptune=10,	// 海王星
		}

		//https://ja.wikipedia.org/wiki/大きさ順の太陽系天体の一覧
		public static float[,] plParams = new float[,]
		{ //sclY,sclY,ofsX,ofsY,sclY1,sclY1,ofs1X,ofsY1,ringMin,ringMax,Re
			{8f,4f,0f,4f,0f,0f,0f,0f,0.0f,0.0f,1.0f},  // Earth=0,	// 地球
			{2f,1f,4f,0f,0f,0f,0f,0f,0.0f,0.0f,109.25f},  // Sun,		// 太陽
			{4f,2f,0f,2f,0f,0f,0f,0f,0.0f,0.0f,0.273f},  // Moon,		// 月
			{2f,1f,4f,2f,0f,0f,0f,0f,0.0f,0.0f,0.532f},  // Mars,		// 火星
			{2f,1f,4f,3f,0f,0f,0f,0f,0.0f,0.0f,0.383f},  // Mercury,	// 水星
			{4f,2f,0f,0f,0f,0f,0f,0f,0.0f,0.0f,10.97f},  // Jupiter,	// 木星
			{2f,1f,6f,3f,0f,0f,0f,0f,0.0f,0.0f,0.950f},  // Venus,		// 金星
			{1f,1f,6f,2f,1f,1f,7f,2f,1.3f,2.3f,9.14f},  // Saturn,	// 土星
			{1f,1f,4f,1f,0f,0f,0f,0f,0.0f,0.0f,3.98f},  // Uranus,	// 天王星
			{1f,1f,6f,0f,0f,0f,0f,0f,0.0f,0.0f,0.187f},  // Pluto,		// 冥王星
			{1f,1f,6f,1f,0f,0f,0f,0f,0.0f,0.0f,3.87f},  // Neptune,	// 海王星
		};

		//ベースとなるGameObject(sphereモデル)にテクスチャ(planet.png)をセット
		public static void makeupToPlanet(Planet _planet, GameObject _planetGo, Material _planetMat,Material _ringMat=null){
			MeshRenderer plaMr = _planetGo.GetComponent<MeshRenderer>();
			int id = (int)_planet;
			float sx0 = 0.125f * plParams [id, 0] - 0.125f*0.01f;
			float sy0 = 0.125f * plParams [id, 1] - 0.125f*0.01f;
			float ox0 = 0.125f * plParams [id, 2] + 0.125f*0.005f;
			float oy0 = 0.125f * plParams [id, 3] + 0.125f*0.005f;

			plaMr.material.SetTextureScale ("_MainTex",new Vector2(sx0, sy0));
			plaMr.material.SetTextureOffset ("_MainTex", new Vector2(ox0, oy0));
			if (plParams [id, 4] > 0f) {
				float sx1 = 0.125f * plParams [id, 4] - 0.125f*0.01f;
				float sy1 = 0.125f * plParams [id, 5] - 0.125f*0.01f;
				float ox1 = 0.125f * plParams [id, 6] + 0.125f*0.005f;
				float oy1 = 0.125f * plParams [id, 7] + 0.125f*0.005f;

				GameObject ringGo = new GameObject ("Ring");
				MeshFilter mf = ringGo.AddComponent<MeshFilter> ();
				MeshRenderer mr = ringGo.AddComponent<MeshRenderer> ();
				mf.sharedMesh = TmMesh.CreatePolyRing (64, TmMesh.AxisType.XZ, plParams[id,8], plParams[id,9]);
				mr.material = _ringMat;
				ringGo.transform.parent = _planetGo.transform;
				ringGo.transform.localPosition = Vector3.zero;
				ringGo.transform.localScale = Vector3.one;
				ringGo.transform.localRotation = Quaternion.identity;
				mr.material.SetTextureScale ("_MainTex",new Vector2(sx1, sy1));
				mr.material.SetTextureOffset ("_MainTex", new Vector2(ox1, oy1));
			}
		}
	}

}
