#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateTube : EditorWindow {
	const string DEF_NAME = "Tube";
	static int mDivNum=32;
	static bool mIsInv=false;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreateTube mWindow;

	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateTube)EditorWindow.GetWindow(typeof(CreateTube));
		mWindow.Show();
	}
	static void Create (int _divNum, TmMesh.AxisType _type, bool _isInv)
	{
		const int DEGZ_NUM=1;
		string name = DEF_NAME+(_isInv?"Inv":"")+_divNum.ToString()+((_type==TmMesh.AxisType.XY)?"XY":"XZ");
		GameObject newGameobject = new GameObject (name);
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreateTubeMesh(_divNum,DEGZ_NUM,_type, _isInv);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}

	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mDivNum = EditorGUILayout.IntField ("divNum", mDivNum);
		mIsInv = EditorGUILayout.Toggle ("isInverse", mIsInv);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mDivNum,mAxisType,mIsInv);
			mWindow.Close();
		}
	}
}
#endif
