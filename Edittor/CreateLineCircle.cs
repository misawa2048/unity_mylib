#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateLineCircle : EditorWindow {
	const string DEF_NAME_C = "LineCircle";
	const string DEF_NAME_S = "LineStar";
	static int mDivNum=32;
	static float mOfsDeg=0.0f;
	static float mStarRate=0.0f;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreateLineCircle mWindow;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;

	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME_C)]
	static void Init ()
	{
		mWindow = (CreateLineCircle)EditorWindow.GetWindow(typeof(CreateLineCircle));
		mWindow.Show();
	}
	static void Create (int _divNum, float _ofsDeg, TmMesh.AxisType _type, Color _vertCol, float _starRate)
	{
		string name = ((_starRate==0.0f)?DEF_NAME_C:DEF_NAME_S)+_divNum.ToString();
		name += ((_type==TmMesh.AxisType.XY)?"XY":"XZ");
		GameObject newGameobject = new GameObject (name);
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreateLineCircle(_divNum,_ofsDeg,_type,_vertCol,_starRate);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mDivNum = EditorGUILayout.IntField ("divNum", mDivNum);
		mOfsDeg = EditorGUILayout.FloatField ("offsetDegree", mOfsDeg);
		mStarRate = EditorGUILayout.FloatField ("starRate", mStarRate);
		mColor = EditorGUILayout.ColorField("vertexColor",mColor);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mDivNum,mOfsDeg,mAxisType,mColor,mStarRate);
			mWindow.Close();
		}
	}
}
#endif
