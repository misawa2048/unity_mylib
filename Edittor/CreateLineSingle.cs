#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateLineSingle : MonoBehaviour {
	const string DEF_NAME = "LineSingle";
	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Create ()
	{
		const int DEG_NUM=2;
		GameObject newGameobject = new GameObject (DEF_NAME+DEG_NUM.ToString());
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		Vector3[] verts = new Vector3[]{Vector3.back*0.5f, Vector3.forward*0.5f};
		meshFilter.mesh = TmMesh.CreateLine(verts,true);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME+DEG_NUM.ToString()+"XZ";
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
