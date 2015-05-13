#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreatePoly : EditorWindow 
{
	const string DEF_NAME_P = "Poly";
	const string DEF_NAME_S = "Star";
	static int mVertNum=4;
	static float mOfsDeg=0.0f;
	static float mStarRate=0.0f;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreatePoly mWindow;

	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME_P)]
	static void Init ()
	{
		mWindow = (CreatePoly)EditorWindow.GetWindow(typeof(CreatePoly));
		mWindow.Show();
	}
	static void Create(int _vertNum, TmMesh.AxisType _type, Color _vertCol, float _startDeg, float _starRate){
		string name = ((_starRate==0.0f)?DEF_NAME_P:DEF_NAME_S)+_vertNum.ToString();
		name += ((_type==TmMesh.AxisType.XY)?"XY":"XZ");
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreatePoly(_vertNum, _type, _vertCol, _startDeg, _starRate);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mVertNum = EditorGUILayout.IntField ("vertNum", mVertNum);
		mOfsDeg = EditorGUILayout.FloatField ("offsetDegree", mOfsDeg);
		mStarRate = EditorGUILayout.FloatField ("starRate", mStarRate);
		mColor = EditorGUILayout.ColorField("vertexColor",mColor);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mVertNum,mAxisType,mColor,mOfsDeg,mStarRate);
			mWindow.Close();
		}
	}
	
}
#endif
