#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateCustomTube : EditorWindow {
	const string DEF_NAME = "CustomTube";
	static int mDivNum=32;
	static int mCvDivNum=4;
	static string mName = DEF_NAME;
	static bool mIsInv=false;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreateCustomTube mWindow;
	static AnimationCurve mCurve = AnimationCurve.Linear(0f,0f,1f,0.5f);
	
	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateCustomTube)EditorWindow.GetWindow(typeof(CreateCustomTube));
		mWindow.Show();
	}
	static void Create (int _divNum, int _cvdivNum, AnimationCurve _cv, TmMesh.AxisType _type, bool _isInv)
	{
		mName = string.IsNullOrEmpty(mName) ? DEF_NAME : mName;
		string name = mName+(_isInv?"Inv":"")+_divNum.ToString()+"x"+_cvdivNum.ToString();
		name += ((_type==TmMesh.AxisType.XY)?"XY":"XZ");
		GameObject newGameobject = new GameObject (name);
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreateTubeMesh(_divNum,_cvdivNum,_cv,_type,new Color(0.5f,0.5f,0.5f,1.0f), _isInv);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + mesh.name + ".asset");
		AssetDatabase.CreateAsset (mesh, path);
		AssetDatabase.SaveAssets ();
	}
	
	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mName = EditorGUILayout.TextField ("name", mName);
		mDivNum = EditorGUILayout.IntField ("divNum", mDivNum);
		mIsInv = EditorGUILayout.Toggle ("isInverse", mIsInv);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		mCvDivNum = EditorGUILayout.IntField ("curveDivNum", mCvDivNum);
		mCurve = EditorGUILayout.CurveField("curve", mCurve);
		if(GUILayout.Button("Create")) {
			Create(mDivNum,mCvDivNum,mCurve,mAxisType,mIsInv);
			mWindow.Close();
		}
	}
}
#endif
