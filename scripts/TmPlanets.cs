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

		public static float[,] coodParams = new float[,]
		{ //sclX,sclY,ofsX,ofsY,sclY1,sclY1,ofs1X,ofsY1,ringMin,ringMax,Re
			{8f,4f,0f,4f,0f,0f,0f,0f,0.0f,0.0f},  // Earth=0,	// 地球
			{2f,1f,4f,0f,0f,0f,0f,0f,0.0f,0.0f},  // Sun,		// 太陽
			{4f,2f,0f,2f,0f,0f,0f,0f,0.0f,0.0f},  // Moon,		// 月
			{2f,1f,4f,2f,0f,0f,0f,0f,0.0f,0.0f},  // Mars,		// 火星
			{2f,1f,4f,3f,0f,0f,0f,0f,0.0f,0.0f},  // Mercury,	// 水星
			{4f,2f,0f,0f,0f,0f,0f,0f,0.0f,0.0f},  // Jupiter,	// 木星
			{2f,1f,6f,3f,0f,0f,0f,0f,0.0f,0.0f},  // Venus,		// 金星
			{1f,1f,6f,2f,1f,1f,7f,2f,1.3f,2.3f},  // Saturn,	// 土星
			{1f,1f,4f,1f,0f,0f,0f,0f,0.0f,0.0f},  // Uranus,	// 天王星
			{1f,1f,6f,0f,0f,0f,0f,0f,0.0f,0.0f},  // Pluto,		// 冥王星
			{1f,1f,6f,1f,0f,0f,0f,0f,0.0f,0.0f},  // Neptune,	// 海王星
		};

		//https://ja.wikipedia.org/wiki/大きさ順の太陽系天体の一覧
		//https://ja.wikipedia.org/wiki/赤道傾斜角
		//https://matome.naver.jp/odai/2134408823942992001
		public static float[,] planetParams = new float[,]
		{// 距離AU      大きさ 赤道傾斜角 自転周期 公転周期 
			{1.0f,     1.0f,   23.4f,  1.0f,   1.0f,   },  // Earth=0,	// 地球
			{0f,       109.25f,0f,     27.25f, 0f,     },  // Sun,		// 太陽
			{0.000257f,0.273f, 5.1454f,29.27f, 29.27f, },  // Moon,		// 月
			{1.52368f, 0.532f, 25.0f,  1.026f, 1.88f,  },  // Mars,		// 火星
			{0.3871f,  0.383f, 0.027f, 58.155f,0.24f,  },  // Mercury,	// 水星
			{5.2026f,  10.97f, 3.08f,  0.408f, 11.86f, },  // Jupiter,	// 木星
			{0.72333f, 0.950f, 177.36f,243.0f, 0.615f, },  // Venus,	// 金星
			{9.55491f, 9.14f,  26.7f,  0.426f, 29.46f, },  // Saturn,	// 土星
			{19.21845f,3.98f,  97.9f,  0.746f, 84.25f, },  // Uranus,	// 天王星
			{39.44507f,0.187f, 1.769f, 6.39f,  247.74f,},  // Pluto,	// 冥王星
			{30.06896f,3.87f,  29.6f,  0.671f, 164.79f,},  // Neptune,	// 海王星
		};

		//ベースとなるGameObject(sphereモデル)にテクスチャ(planet.png)をセット
		public static void makeupToPlanet(Planet _planet, GameObject _planetGo, Material _ringMat=null){
			int id = (int)_planet;
			Transform parentOld = _planetGo.transform.parent;
			GameObject angGo = new GameObject (_planetGo.name + "ang");
			angGo.transform.position = _planetGo.transform.position;
			angGo.transform.rotation = _planetGo.transform.rotation;
			_planetGo.transform.SetParent (angGo.transform);
			GameObject pivotGo = new GameObject (_planetGo.name + "pivot");
			pivotGo.transform.position = angGo.transform.position;
			pivotGo.transform.rotation = angGo.transform.rotation;
			angGo.transform.SetParent (pivotGo.transform);
			angGo.transform.localRotation = Quaternion.Euler(planetParams[id,2],0f,0f);
			pivotGo.transform.SetParent (parentOld);
			MeshRenderer plaMr = _planetGo.GetComponent<MeshRenderer>();
			float sx0 = 0.125f * coodParams [id, 0] - 0.125f*0.01f;
			float sy0 = 0.125f * coodParams [id, 1] - 0.125f*0.01f;
			float ox0 = 0.125f * coodParams [id, 2] + 0.125f*0.005f;
			float oy0 = 0.125f * coodParams [id, 3] + 0.125f*0.005f;

			plaMr.material.SetTextureScale ("_MainTex",new Vector2(sx0, sy0));
			plaMr.material.SetTextureOffset ("_MainTex", new Vector2(ox0, oy0));
			if (coodParams [id, 4] > 0f) {
				float sx1 = 0.125f * coodParams [id, 4] - 0.125f*0.01f;
				float sy1 = 0.125f * coodParams [id, 5] - 0.125f*0.01f;
				float ox1 = 0.125f * coodParams [id, 6] + 0.125f*0.005f;
				float oy1 = 0.125f * coodParams [id, 7] + 0.125f*0.005f;

				GameObject ringGo = new GameObject ("Ring");
				MeshFilter mf = ringGo.AddComponent<MeshFilter> ();
				MeshRenderer mr = ringGo.AddComponent<MeshRenderer> ();
				mf.sharedMesh = TmMesh.CreatePolyRing (64, TmMesh.AxisType.XZ, coodParams[id,8], coodParams[id,9]);
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
