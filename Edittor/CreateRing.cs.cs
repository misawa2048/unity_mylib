#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateRing : MonoBehaviour
{

	[MenuItem ("GameObject/Create Other/Other/Ring")]
	static void Create ()
	{
		const int DEG_NUM=256;
		const float MAX_RAD = 1.0f;
		const float MIN_RAD = 0.5f;
//		const int UV_DEG_NUM = 1; //DEG_NUM;
		GameObject newGameobject = new GameObject ("Ring"+DEG_NUM.ToString()+"XY");
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		//meshRenderer.sharedMaterial.mainTexture = 
		//	(Texture)AssetDatabase.LoadAssetAtPath("Assets/test.png", typeof(Texture2D));
			
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = new Mesh ();
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = "Ring"+DEG_NUM.ToString()+"XY";
		
		Vector3[] vertices = new Vector3[(DEG_NUM+1)*2];
		int[] triangles = new int[(DEG_NUM+1)*6];
		Vector2[] uv = new Vector2[(DEG_NUM+1)*2];
		Color[] colors = new Color[(DEG_NUM+1)*2];
		Vector3[] normals = new Vector3[(DEG_NUM+1)*2];

		int cnt = 0;
		for(int ii = 0; ii <= DEG_NUM; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)DEG_NUM));
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)DEG_NUM));
			vertices[cnt*2+0] = new Vector3(fx*MAX_RAD*0.5f,fy*MAX_RAD*0.5f,0.0f);
			vertices[cnt*2+1] = new Vector3(fx*MIN_RAD*0.5f,fy*MIN_RAD*0.5f,0.0f);
			triangles[cnt*6+0] = cnt*2+0;
			triangles[cnt*6+1] = cnt*2+1;
			triangles[cnt*6+2] = (ii<(DEG_NUM)) ? (cnt*2+2) : 0;
			triangles[cnt*6+3] = cnt*2+0;
			triangles[cnt*6+4] = (ii>0) ? (cnt*2-1) : 0;
			triangles[cnt*6+5] = cnt*2+1;
//			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
//			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			uv[cnt*2+0] = new Vector2((float)ii/(float)DEG_NUM,0.0f);
			uv[cnt*2+1] = new Vector2((float)ii/(float)DEG_NUM,1.0f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Triangles,0);

		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
