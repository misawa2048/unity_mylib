#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateTileMesh : EditorWindow 
{
	const string DEF_NAME = "TileMesh";
	static int mDivX=5;
	static int mDivY=5;
	static bool mIsUnitPerGrid=false;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreateTileMesh mWindow;

	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateTileMesh)EditorWindow.GetWindow(typeof(CreateTileMesh));
		mWindow.Show();
	}
	static void Create(int _divX, int _divY, Color _vertCol, bool _isUnitPerGrid){
		string name = DEF_NAME+mDivX.ToString()+"x"+mDivY.ToString()+(_isUnitPerGrid?"U":"");
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreateTileMesh(_divX, _divY, _vertCol, _isUnitPerGrid);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}

	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mDivX = EditorGUILayout.IntField ("divX", mDivX);
		mDivY = EditorGUILayout.IntField ("divY", mDivY);
		mIsUnitPerGrid = EditorGUILayout.Toggle("isUnitPerGrid",mIsUnitPerGrid);
		mColor = EditorGUILayout.ColorField("vertexColor",mColor);
		if(GUILayout.Button("Create")) {
			Create(mDivX,mDivY,mColor,mIsUnitPerGrid);
			mWindow.Close();
		}
	}

}
#endif
