#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateLineMesh : MonoBehaviour
{

	[MenuItem ("GameObject/Create Other/Other/line mesh")]
	static void Create ()
	{
		const int MESH_W = 7;
		const int MESH_H = 7;
		GameObject newGameobject = new GameObject ("line mesh"+MESH_W.ToString()+"x"+MESH_H.ToString());

		Vector3[] vertices = new Vector3[(MESH_W+1)*(MESH_H+1)*2];
		int[] triangles = new int[(((MESH_W+1)*(MESH_H+1)*2)/3+1)*3];
		Vector2[] uv = new Vector2[(MESH_W+1)*(MESH_H+1)*2];
		Color[] colors = new Color[(MESH_W+1)*(MESH_H+1)*2];
		Vector3[] normals = new Vector3[(MESH_W+1)*(MESH_H+1)*2];

		int cnt = 0;
		for(int ix = 0; ix <= MESH_W; ++ix){
			vertices[cnt*2+0] = new Vector3(((float)ix/(float)MESH_W - 0.5f),-0.5f,0.0f);
			vertices[cnt*2+1] = new Vector3(((float)ix/(float)MESH_W - 0.5f), 0.5f,0.0f);
			triangles[cnt*2+0] = cnt*2+0;
			triangles[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector3(((float)ix/(float)MESH_W),0.0f,0.0f);
			uv[cnt*2+1] = new Vector3(((float)ix/(float)MESH_W),1.0f,0.0f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		for(int iy = 0; iy <= MESH_H; ++iy){
			vertices[cnt*2+0] = new Vector3(-0.5f,((float)iy/(float)MESH_H - 0.5f),0.0f);
			vertices[cnt*2+1] = new Vector3( 0.5f,((float)iy/(float)MESH_H - 0.5f),0.0f);
			triangles[cnt*2+0] = cnt*2+0;
			triangles[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector3(0.0f,((float)iy/(float)MESH_H),0.0f);
			uv[cnt*2+1] = new Vector3(1.0f,((float)iy/(float)MESH_H),0.0f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		//meshRenderer.sharedMaterial.mainTexture = 
		//	(Texture)AssetDatabase.LoadAssetAtPath("Assets/test.png", typeof(Texture2D));
			
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = new Mesh ();
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = "LineMeshXY"+MESH_W.ToString()+"x"+MESH_H.ToString();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Lines,0);
		mesh.RecalculateNormals ();	// 法線の再計算
		mesh.RecalculateBounds ();	// バウンディングボリュームの再計算
		mesh.Optimize ();
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
}
#endif
