#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateLineMesh : MonoBehaviour {
	const string DEF_NAME = "LineMesh";
	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Create ()
	{
		const int MESH_W = 8;
		const int MESH_H = 8;
		GameObject newGameobject = new GameObject (DEF_NAME+MESH_W.ToString()+"x"+MESH_H.ToString());
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		meshFilter.mesh = TmUtils.CreateGridXY(MESH_W,MESH_H);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME+MESH_W.ToString()+"x"+MESH_H.ToString()+"XY";

		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
}
#endif
