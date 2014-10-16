#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateTube : MonoBehaviour {
	const string DEF_NAME = "Tube";
	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Create ()
	{
		const int DEGXY_NUM=32;
		const int DEGZ_NUM=1;
		GameObject newGameobject = new GameObject (DEF_NAME+DEGXY_NUM.ToString()+"x"+DEGZ_NUM.ToString());
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreateTubeMesh(DEGXY_NUM,DEGZ_NUM);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME+DEGXY_NUM.ToString()+"x"+DEGZ_NUM.ToString();
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
