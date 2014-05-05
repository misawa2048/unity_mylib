#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateStar : MonoBehaviour
{

	[MenuItem ("GameObject/Create Other/Other/Star")]
	static void Create ()
	{
		const int DEG_NUM=5;
		GameObject newGameobject = new GameObject ("Star"+DEG_NUM.ToString());
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
			
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmUtils.CreatePoly(DEG_NUM,0.5f);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = "Star"+DEG_NUM.ToString()+"XY";

		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
}
#endif
