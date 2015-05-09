#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreatePanel : MonoBehaviour {
	const string DEF_NAME = "CustomPanel";
	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Create ()
	{
		const int MESH_W = 1;
		const int MESH_H = 1;
		GameObject newGameobject = new GameObject (DEF_NAME);
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		meshFilter.mesh = TmMesh.CreateTileMesh(MESH_W,MESH_H);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
