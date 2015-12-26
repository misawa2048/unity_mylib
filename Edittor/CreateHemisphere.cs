#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using TmLib;

public class CreateHemisphere : EditorWindow 
{
	const string DEF_NAME = "Hemisphere";
    static int mDivH = 16;
    static int mDivV=4; // キー間 
	static float mSttLatDeg=0f;
	static float mEndLatDeg=180f;
	static float mSttLongDeg=0f;
	static float mEndLongDeg=360f;
    static bool mIsInv = false;
    static Color mColor = new Color(0.5f,0.5f,0.5f,1f);
	static CreateHemisphere mWindow;
	
	[MenuItem ("GameObject/Create Other/ELIX/"+DEF_NAME)]
	static void Init ()
	{
		mWindow = (CreateHemisphere)EditorWindow.GetWindow(typeof(CreateHemisphere));
		mWindow.Show();
	}
	static void Create(int _divH, int _divV, float _sttH, float _sizeH, float _sttV, float _sizeV, Color _vertCol, bool _isInv)
    {
		string name = DEF_NAME+_divH.ToString()+"x"+_divV.ToString();
        name += "_" + Mathf.Floor(_sttH) + "_" + Mathf.Floor(_sizeH);
        name += "_" + Mathf.Floor(_sttV) + "_" + Mathf.Floor(_sizeV);

        AnimationCurve cv = createLatSphereCv(4*_divV, _sttV, _sizeV);

        GameObject newGameobject = new GameObject (name);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();		
		meshFilter.mesh = TmMesh.CreateTubeMesh(_divH, _divV, _sttH, _sizeH, cv, TmMesh.AxisType.XZ, _vertCol, _isInv);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = name;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	void OnGUI() {
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        mDivH = EditorGUILayout.IntField("DivH", mDivH);
        mDivV = EditorGUILayout.IntField ("DivV", mDivV);
        mSttLongDeg = EditorGUILayout.FloatField ("Longitude Start Deg", Mathf.Clamp(mSttLongDeg, 0f, 360f));
        mEndLongDeg = EditorGUILayout.FloatField ("Longitude End Deg", Mathf.Clamp(mEndLongDeg, mSttLongDeg, 360f));
        mSttLatDeg = EditorGUILayout.FloatField ("Latitude Start Deg", Mathf.Clamp(mSttLatDeg,0f,180f));
        mEndLatDeg = EditorGUILayout.FloatField ("Latitude End Deg", Mathf.Clamp(mEndLatDeg, mSttLatDeg, 180f));
		mColor = EditorGUILayout.ColorField("vertexColor",mColor);
        mIsInv = EditorGUILayout.Toggle("invert Normal", mIsInv);
        if (GUILayout.Button("Create")) {
			Create(mDivH, mDivV, mSttLongDeg, (mEndLongDeg- mSttLongDeg), mSttLatDeg, (mEndLatDeg- mSttLatDeg), mColor, mIsInv);
			mWindow.Close();
		}
	}
	
	static private AnimationCurve createLatSphereCv(int _div, float _sttLatDeg = 0f, float _sizeLatDeg = 180f, float _scale=1f)
    {
        float sttRate = Mathf.Clamp01(_sttLatDeg / 180f);
        float sizeRate = Mathf.Clamp01(_sizeLatDeg / 180f);

        Keyframe[] keys = new Keyframe[_div];
		for(int ii = 0; ii < _div; ++ii){
            float rate = (float)ii / (float)(_div - 1);
            float nowRate = (sttRate + rate * sizeRate);
            float t = Mathf.Lerp(0.5f, -0.5f, nowRate);
            float tan = Mathf.Lerp(-1f, 1f, nowRate);
            keys[ii] = new Keyframe((t+0.5f) * _scale, Mathf.Sqrt(_scale * _scale * 0.25f -t * t),tan,tan);
            keys[ii].tangentMode = 21; // Liner
		}
        AnimationCurve cv = new AnimationCurve(keys);
        return cv;
	}
}
#endif
