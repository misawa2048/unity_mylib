using UnityEngine;
using System.Collections;

public class TmLevelLoader : MonoBehaviour {
	public const float VERSION = 0.1f;
	private const string PREFAB_NAME ="_LoaderPrefab"; // Instantiate 
	public const string NAME = "_Loader";

	public enum MODE{
		IDLE  = 0,
		IN = 1,
		BUSY  = 2,
		OUT = 3,
	};
	public string nextLevelName="";
	public MODE mode = MODE.IDLE;
	public Texture2D fadeTexture=null;
	public Color fadeColor = Color.black;
	public float fadeTime=1f;
	private float mTimer;
	private string mLoadLevelName="";
	private bool mIsLoadBusy=false;
	private static TmLevelLoader m_Instance = null;
	public static bool hasInstance{ get { return m_Instance!=null; } }
	public static TmLevelLoader instance{
		get{
			if(m_Instance==null){
				GameObject loederObj;
				UnityEngine.Object resObj = Resources.Load(PREFAB_NAME);
				if(resObj!=null){
					loederObj = GameObject.Instantiate(resObj) as GameObject;
					loederObj.name = NAME;
					m_Instance = loederObj.GetComponent<TmLevelLoader>();
				}else{ // Instantiate 
					loederObj = new GameObject(NAME);
					m_Instance = loederObj.AddComponent<TmLevelLoader>();
				}
				loederObj.transform.parent = Camera.main.transform;
				loederObj.transform.localPosition = Vector3.zero;
			}
			return m_Instance;
		}
	}

	void Awake () {
		if(m_Instance==null){
			m_Instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy(this.gameObject);
		}
		if(fadeTexture==null){
			Color[] cols = new Color[32*32];
			for(int ii = 0; ii < 32*32; ++ii){
				cols[ii]=fadeColor;
			}
			fadeTexture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
			fadeTexture.SetPixels(cols);
			fadeTexture.Apply();
		}
	}
	// Use this for initialization
	void Start () {
		switch(mode){
		case MODE.IDLE: mTimer = 0;        break;
		case MODE.IN:   mTimer = 0;        break;
		case MODE.BUSY: mTimer = fadeTime; break;
		case MODE.OUT:  mTimer = fadeTime; break;
		}
		if(nextLevelName!=""){
			loadLevel(nextLevelName);
		}
	}
	
	// Update is called once per frame
	void Update () {
		fadeUpdate();
//		Application.loadedLevelName
	}

	void OnGUI ()
	{
		Color col = fadeColor;
		col.a *= (mTimer/fadeTime);
		GUI.color = col;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeTexture);
	}

	public static void LoadLevel(string _name=""){
		instance.loadLevel(_name);
	}
	private void loadLevel(string _name=""){
		if(!mIsLoadBusy){
			if(_name==""){
				_name = nextLevelName;
			}
			StartCoroutine("loadLevelCo",_name);
		}
	}

	//---------------------------------------------------------
	private void fadeUpdate(){
		float dt = Time.deltaTime;
		switch(mode){
		case MODE.IDLE: dt = -fadeTime;       break;
		case MODE.IN:   dt = Time.deltaTime;  break;
		case MODE.BUSY: dt = fadeTime;        break;
		case MODE.OUT:  dt = -Time.deltaTime; break;
		}
		mTimer = Mathf.Clamp(mTimer+dt,0f,fadeTime);
		if(mTimer==0f){
			mode = MODE.IDLE;
		}else if(mTimer==fadeTime){
			mode = MODE.BUSY;
		}
	}
	//---------------------------------------------------------
	IEnumerator loadLevelCo(string _name){
		mLoadLevelName = _name;
		mTimer = 0f;
		mode = MODE.IN;
		mIsLoadBusy = true;
		while(mode==MODE.IN){
			yield return null;
		}
		Application.LoadLevel(_name);
		while(Application.loadedLevelName != mLoadLevelName){
			yield return null;
		}
		mode = MODE.OUT;
		mIsLoadBusy = false;
		yield break;
	}
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
}
