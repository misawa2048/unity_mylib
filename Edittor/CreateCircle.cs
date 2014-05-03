#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateCircle : MonoBehaviour {
	const string DEF_NAME = "Circle";
	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Create ()
	{
		const int DEG_NUM=32;
		GameObject newGameobject = new GameObject (DEF_NAME+DEG_NUM.ToString());
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmUtils.CreatePoly(DEG_NUM);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME+DEG_NUM.ToString()+"XY";
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
