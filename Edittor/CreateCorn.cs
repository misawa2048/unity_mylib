#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateCorn : EditorWindow {
	const string DEF_NAME = "Corn";
	static int mDivNum=32;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreateCorn mWindow;

	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateCorn)EditorWindow.GetWindow(typeof(CreateCorn));
		mWindow.Show();
	}
	static void Create (int _divNum, TmMesh.AxisType _type)
	{
		const int DEGZ_NUM=1;
		string name = DEF_NAME+_divNum.ToString()+((_type==TmMesh.AxisType.XY)?"XY":"XZ");
		GameObject newGameobject = new GameObject (name);
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreateTubeMesh(_divNum, DEGZ_NUM, _type, 0.5f, 0f, new Color(0.5f,0.5f,0.5f,1.0f),false);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}

	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mDivNum = EditorGUILayout.IntField ("divNum", mDivNum);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mDivNum,mAxisType);
			mWindow.Close();
		}
	}
}
#endif
