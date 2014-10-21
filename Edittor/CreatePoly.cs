#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreatePoly : EditorWindow 
{
	const string DEF_NAME = "Poly";
	static int mVertNum=4;
	static float mOfsDeg=0.0f;
	static float mStarRate=0.0f;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreatePoly mWindow;

	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreatePoly)EditorWindow.GetWindow(typeof(CreatePoly));
		mWindow.Show();
	}
	static void Create(int _vertNum, Color _vertCol, float _startDeg, float _starRate){
		string name = (mStarRate==0.0f)?DEF_NAME+mVertNum.ToString():"Star"+mVertNum.ToString()+"_"+mStarRate.ToString();
		name += (mOfsDeg==0.0f)?"" : "Ofs"+mOfsDeg.ToString();
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreatePoly(_vertNum, _vertCol, _startDeg, _starRate);
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
		if(GUILayout.Button("Create")) {
			Create(mVertNum,mColor,mOfsDeg,mStarRate);
			mWindow.Close();
		}
	}
	
}
#endif
