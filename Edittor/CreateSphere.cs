#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateSphere : EditorWindow 
{
	const string DEF_NAME = "Sphere";
    static int mDivH = 16;
    static int mDivV=8;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreateSphere mWindow;
	
	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateSphere)EditorWindow.GetWindow(typeof(CreateSphere));
		mWindow.Show();
	}
	static void Create(int _divH, int _divV, Color _vertCol){
		string name = DEF_NAME+_divH.ToString()+"x"+_divV.ToString();
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreateSphere(_divH, _divV, _vertCol);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        mDivH = EditorGUILayout.IntField("DivH", mDivH);
        mDivV = EditorGUILayout.IntField ("DivV", mDivV);
		mColor = EditorGUILayout.ColorField("vertexColor",mColor);
		if(GUILayout.Button("Create")) {
			Create(mDivH,mDivV,mColor);
			mWindow.Close();
		}
	}
	
}
#endif
