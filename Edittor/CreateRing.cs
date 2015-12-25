#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateRing : EditorWindow
{
	static int mDivNum=256;
	static float mMaxRad = 1.0f;
	static float mMinRad = 0.5f;
	static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreateRing mWindow;

	[MenuItem ("GameObject/Create Other/ELIX/Ring")]
	static void Init ()
	{
		mWindow = (CreateRing)EditorWindow.GetWindow(typeof(CreateRing));
		mWindow.Show();
	}

	static void Create (int _divNum, float _maxRad, float _minRad, TmMesh.AxisType _type, Color _color)
	{
		string name = "Ring" + _divNum.ToString () + ((_type == TmMesh.AxisType.XY) ? "XY" : "XZ");
		GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		//meshRenderer.sharedMaterial.mainTexture = 
		//	(Texture)AssetDatabase.LoadAssetAtPath("Assets/test.png", typeof(Texture2D));
			
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		
		meshFilter.mesh = TmMesh.CreatePolyRing(_divNum, _type, _minRad, _maxRad, _color);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;

		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}

	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mDivNum = EditorGUILayout.IntField ("div num", mDivNum);
		mMaxRad = EditorGUILayout.FloatField ("max rad", mMaxRad);
		mMinRad = EditorGUILayout.FloatField ("min rad", mMinRad);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type",(System.Enum)mAxisType);
		if(GUILayout.Button("Create")) {
			Create(mDivNum,mMaxRad,mMinRad,mAxisType,mColor);
			mWindow.Close();
		}
	}
}
#endif
