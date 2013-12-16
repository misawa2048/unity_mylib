#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateLineCircle : MonoBehaviour {
	const string DEF_NAME = "LineCircle";
	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Create ()
	{
		const int DEG_NUM=32;
		GameObject newGameobject = new GameObject (DEF_NAME+DEG_NUM.ToString());
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = _CreateLineCircle(DEG_NUM);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME+DEG_NUM.ToString()+"XY";
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	static private Mesh _CreateLineCircle(int _vertNum){
		Vector3[] vertices = new Vector3[(_vertNum)];
		int[] triangles = new int[(((_vertNum))/3+1)*3];
		Vector2[] uv = new Vector2[(_vertNum)];
		Color[] colors = new Color[(_vertNum)];
		Vector3[] normals = new Vector3[(_vertNum)];
		
		int cnt = 0;
		for(int ii = 0; ii < _vertNum; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			vertices[cnt] = new Vector3(fx,fy,0.0f);
			triangles[cnt] = cnt;
			uv[cnt] = new Vector2(vertices[cnt].x+0.5f,vertices[cnt].y+0.5f);
			colors[cnt] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.LineStrip,0);
		return mesh;
	}
}
#endif
