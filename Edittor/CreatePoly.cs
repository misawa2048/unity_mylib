#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreatePoly : MonoBehaviour
{

	[MenuItem ("GameObject/Create Other/Other/Simple Poly")]
	static void Create ()
	{
		const int DEG_NUM=32;
		const int UV_DEG_NUM=DEG_NUM;
		GameObject newGameobject = new GameObject ("CustomPoly"+DEG_NUM.ToString());
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		//meshRenderer.sharedMaterial.mainTexture = 
		//	(Texture)AssetDatabase.LoadAssetAtPath("Assets/test.png", typeof(Texture2D));
			
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = new Mesh ();
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = "CustomPanel"+DEG_NUM.ToString()+"XY";
		
		Vector3[] verts = new Vector3[DEG_NUM+1];
		Vector2[] uvs = new Vector2[DEG_NUM+1];
		Vector3[] norms = new Vector3[DEG_NUM+1];
		Vector4[] tgts = new Vector4[DEG_NUM+1];
		Color[] cols = new Color[DEG_NUM+1];
		verts[0]= new Vector3(0.0f,0.0f,0.0f);
		uvs[0]= new Vector2(0.5f,0.5f);
		norms[0]= new Vector3(0.0f,0.0f,1.0f);
		tgts[0]= new Vector4(1.0f,0.0f,0.0f,0.0f);
		cols[0] = new Color(1.0f,1.0f,1.0f,1.0f);
		for(int ii=0; ii< DEG_NUM; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)DEG_NUM))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)DEG_NUM))*0.5f;
			verts[ii+1]= new Vector3(fx,fy,0.0f);
			norms[ii+1]= new Vector3(0.0f,0.0f,1.0f);
			tgts[ii+1]= new Vector4(fx,fy,0.0f,0.0f);
//			tgts[ii+1]= new Vector4(1.0f,0.0f,0.0f,0.0f);
			cols[ii+1] = new Color(1.0f,1.0f,1.0f,1.0f);
		}
		
		for(int ii=0; ii< DEG_NUM; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)UV_DEG_NUM))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)UV_DEG_NUM))*0.5f;
			uvs[ii+1]= new Vector2(fx+0.5f,fy+0.5f);
		}

		int[] tris = new int[DEG_NUM*3];
		for(int ii=0; ii< DEG_NUM; ++ii){
			tris[ii*3+0] = 0;
			tris[ii*3+1] = (ii<(DEG_NUM-1)) ? (ii+2) : 1;
			tris[ii*3+2] = ii+1;
		}
		
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.normals = norms;
		mesh.tangents = tgts;
		mesh.colors = cols;
		
		mesh.RecalculateNormals ();	// 法線の再計算
		mesh.RecalculateBounds ();	// バウンディングボリュームの再計算
		mesh.Optimize ();
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
