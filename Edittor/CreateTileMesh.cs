#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateTileMesh : EditorWindow 
{
	const string DEF_NAME = "TileMesh";
	static int mDivX=5;
	static int mDivY=5;
	static bool mIsUnitPerGrid=false;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreateTileMesh mWindow;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;

	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateTileMesh)EditorWindow.GetWindow(typeof(CreateTileMesh));
		mWindow.Show();
	}
	static void Create(int _divX, int _divY, TmMesh.AxisType _type, Color _vertCol, bool _isUnitPerGrid){
		string name = DEF_NAME+_type.ToString()+mDivX.ToString()+"x"+mDivY.ToString()+(_isUnitPerGrid?"U":"");
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreateTileMesh(_divX, _divY, _type, _vertCol, _isUnitPerGrid);
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
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mDivX,mDivY,mAxisType,mColor,mIsUnitPerGrid);
			mWindow.Close();
		}
	}
	
}
#endif
