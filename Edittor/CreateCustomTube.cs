#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using TmLib;

public class CreateCustomTube : EditorWindow
{
	//-------------------------------------------------------------------
	// PlayerSettings>OtherSettings>ScriptingDefineSymbols : TM_USE_FILE
	//-------------------------------------------------------------------
	const string DEF_NAME = "CustomTube";
	const string DEF_FILE_DIR = "../";
	const string FILE_EXT = "json";
	public enum UvDiv { Single = 1, Div2x2 = 2, Div4x4 = 4, Div8x8 = 8 }
	static int mDivNum = 32;
	static int mCvDivNum = 4;
	static string mName = DEF_NAME;
	static bool mIsInv = false;
	static TmMesh.AxisType mAxisType = TmMesh.AxisType.XY;
	static CreateCustomTube mWindow;
	static AnimationCurve mCurve = AnimationCurve.Linear(0f, 0f, 1f, 0.5f);
	static UvDiv mUvDiv = UvDiv.Single;
	static int mUvDivId = 0;
	static bool mUseBone = false;
	static int mBoneNum = 0;

	[MenuItem("GameObject/Create Other/ELIX/" + DEF_NAME)]
	static void Init()
	{
		mWindow = (CreateCustomTube)EditorWindow.GetWindow(typeof(CreateCustomTube));
		mWindow.Show();
	}
	static void Create(int _divNum, int _cvdivNum, UvDiv _UvDiv, int _UvDivId, AnimationCurve _cv, TmMesh.AxisType _type, bool _isInv)
	{
		mName = string.IsNullOrEmpty(mName) ? DEF_NAME : mName;
		string name = mName + (_isInv ? "Inv" : "") + _divNum.ToString() + "x" + _cvdivNum.ToString();
		if (_UvDiv != UvDiv.Single)
		{
			name += "UV" + ((int)_UvDiv).ToString() + "_" + _UvDivId.ToString("D2");
		}
		name += ((_type == TmMesh.AxisType.XY) ? "XY" : "XZ");
		GameObject newGameobject = new GameObject(name);
		float dd = (1f / (float)_UvDiv);
		float dx = (float)(_UvDivId % (int)_UvDiv) * dd;
		float dy = (float)((_UvDiv - 1) - (_UvDivId / (int)_UvDiv)) * dd;
		Rect uvRect = new Rect(dx, dy, dd, dd);
		Mesh mesh;
		if (!mUseBone)
		{
			MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer>();
			meshRenderer.material = new Material(Shader.Find("Diffuse"));
			MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter>();
			mesh = TmMesh.CreateTubeMesh(_divNum, _cvdivNum, uvRect, _cv, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), _isInv);
			meshFilter.sharedMesh = mesh;
			mesh.name = name;
		}
		else
		{
			SkinnedMeshRenderer meshRenderer = newGameobject.AddComponent<SkinnedMeshRenderer>();
			meshRenderer.material = new Material(Shader.Find("Diffuse"));
			Transform[] boneTr = new Transform[mBoneNum];
			for (int i = 0; i < mBoneNum; ++i)
			{
				GameObject boneObj = new GameObject("bone" + i);
				boneObj.transform.parent = newGameobject.transform;
				boneObj.transform.localPosition = Vector3.zero;
				boneTr[i] = boneObj.transform;
			}
			meshRenderer.bones = boneTr;
			meshRenderer.rootBone = boneTr[0];
			mesh = TmMesh.CreateTubeMesh(_divNum, _cvdivNum, uvRect, _cv, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), _isInv, boneTr);
			meshRenderer.sharedMesh = mesh;
			meshRenderer.name = name;
		}

		string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset");
		AssetDatabase.CreateAsset(mesh, path);
		AssetDatabase.SaveAssets();
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mName = EditorGUILayout.TextField("name", mName);
		mDivNum = EditorGUILayout.IntField("divNum", mDivNum);
		mCvDivNum = EditorGUILayout.IntField("curveDivNum", mCvDivNum);
		mIsInv = EditorGUILayout.Toggle("isInverse", mIsInv);
		mAxisType = (TmMesh.AxisType)EditorGUILayout.EnumPopup("type", (System.Enum)mAxisType);
		mUvDiv = (UvDiv)EditorGUILayout.EnumPopup("uvDivNum", (System.Enum)mUvDiv);
		if (mUvDiv != UvDiv.Single)
		{
			mUvDivId = EditorGUILayout.IntSlider("uvDivID", mUvDivId, 0, (int)mUvDiv * (int)mUvDiv - 1);
		}
		mUseBone = EditorGUILayout.Toggle("useBone", mUseBone);
		if (mUseBone)
		{
			mBoneNum = Mathf.Max(EditorGUILayout.IntField("boneNum", mBoneNum), 2);
		}
		mCurve = EditorGUILayout.CurveField("curve", mCurve);
		GUILayout.BeginHorizontal();
#if TM_USE_FILE
#if (!UNITY_WEBPLAYER)
if (GUILayout.Button("LoadCurve")) {
string path = EditorUtility.OpenFilePanel("Load curve",DEF_FILE_DIR,FILE_EXT);
if(path!=""){
string str = LoadFromFile(path);
if(str!=""){
mCurve = TmFileUtil.JsonToAnimCurve (str);
}
}
}
if(GUILayout.Button("SaveCurve")) {
string path = EditorUtility.SaveFilePanel("Save curve",DEF_FILE_DIR,mName + "."+FILE_EXT,FILE_EXT);
string str = TmFileUtil.AnimCurveToJson(mCurve);
SaveToFile(path,str);
}
#else
GUIStyle style = new GUIStyle();
style.normal.textColor = new Color(0.7f,0f,0f);
GUILayout.TextField(" * Can't Load/Save on this plstform. Change plstform to 'PC'.", style);
#endif
#endif
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Create"))
		{
			Create(mDivNum, mCvDivNum, mUvDiv, mUvDivId, mCurve, mAxisType, mIsInv);
			mWindow.Close();
		}
	}

#if TM_USE_FILE && (!UNITY_WEBPLAYER)
//Save/Load
public static bool SaveToFile(string _path, string _str){
bool ret = false;
if (_path != "") {
ret = true;
File.WriteAllText (_path, _str, System.Text.Encoding.UTF8);
}
return ret;
}

public static string LoadFromFile(string _path){
string retStr = "";
if(File.Exists(_path)){
retStr = File.ReadAllText (_path);
}
return retStr;
}
#endif

}
#endif

