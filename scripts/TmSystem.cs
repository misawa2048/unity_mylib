using UnityEngine;
using System.Collections;

public class TmSystem : MonoBehaviour {
	public const float VERSION = 0.3f;
	private const string PREFAB_NAME = "sysPrefab"; // Instantiate 
	public const string NAME = "_sys";
	public const string TAG_NAME = "tagSystem";
	public TmMouseWrapper mw = new TmMouseWrapper();
	public TmTouchWrapper tw = new TmTouchWrapper();
	
	public enum MODE{
		INIT  = 0,
		TITLE = 1,
		GAME = 2,
		SETTINGS = 3,
	};
	public const int SOUND_CH_NUM = 3;
	public enum SOUND_CH{
		SE  = 0,
		BGM = 1,
		VOICE = 2
	};
	[System.Serializable]
	public class SysData{
		public int achievementFlag=0;
		public bool hasSysSaveData=false;
		public float volumeSe = 1.0f;
		public float volumeBgm = 1.0f;
		public float volumeVoice = 1.0f;
	};
	[System.Serializable]
	public class ClipList{
		public AudioClip[] clipList;
	}

	public MODE mode = MODE.INIT;
	public ClipList sysSeList;
	private static TmSystem m_Instance = null;
	public static bool hasInstance{ get { return m_Instance!=null; } }
	public static TmSystem instance{
		get{
			if(m_Instance==null){
				GameObject sysObj;
				UnityEngine.Object resObj = Resources.Load(PREFAB_NAME);
				if(resObj!=null){
					sysObj = GameObject.Instantiate(resObj) as GameObject;
					sysObj.name = NAME;
					m_Instance = sysObj.GetComponent<TmSystem>();
				}else{ // Instantiate 
					sysObj = new GameObject(NAME);
					sysObj.tag = TAG_NAME;
					m_Instance = sysObj.AddComponent<TmSystem>();
				}
				DontDestroyOnLoad(sysObj);
			}
			return m_Instance;
		}
	}
	private SysData mSysData = new SysData();
	private AudioSource[] sysAudioSource = new AudioSource[3];

	void Awake () {
		if(m_Instance==null){
			m_Instance = this;
			for(int ii = 0; ii < SOUND_CH_NUM; ++ii){
				sysAudioSource[ii] = gameObject.AddComponent<AudioSource>();
				sysAudioSource[ii].volume = 1.0f;
				sysAudioSource[ii].loop = (ii==(int)SOUND_CH.BGM);
				sysAudioSource[ii].playOnAwake = false;
			}
			bool ret = loadSysData();
			if(!ret) Debug.Log("NoSaveData");
		}else{
			Debug.Log("Too many TmSystem.");
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){ Application.Quit(); }
		mw.update();
		tw.update();
	}

	//---------------------------------------------------------
	public bool saveSysData(){
		bool ret = false;
		PlayerPrefs.SetInt("hasSysSaveData",1);
		if(PlayerPrefs.GetInt("hasSysSaveData")==1){
			ret = true;
			mSysData.hasSysSaveData = true;
			PlayerPrefs.SetInt("achievementFlag",mSysData.achievementFlag);
			PlayerPrefs.SetFloat("volumeSe",mSysData.volumeSe);
			PlayerPrefs.SetFloat("volumeBgm",mSysData.volumeBgm);
			PlayerPrefs.SetFloat("volumeVoice",mSysData.volumeVoice);
		}
		return ret;
	}
	private bool loadSysData(){
		bool ret = false;
		if(PlayerPrefs.GetInt("hasSysSaveData")==1){
			ret = true;
			mSysData.hasSysSaveData = true;
//			AudioListener.volume = volumeSe;
			mSysData.achievementFlag = PlayerPrefs.GetInt("achievementFlag");
			mSysData.volumeSe = PlayerPrefs.GetFloat("volumeSe");
			mSysData.volumeBgm = PlayerPrefs.GetFloat("volumeBgm");
			mSysData.volumeVoice = PlayerPrefs.GetFloat("volumeVoice");
		}

		return ret;
	}
	//---------------------------------------------------------
	public bool soundCall(SOUND_CH _ch, int _sysClipId, float _volRate=1.0f, bool _isOneShot=false){
		bool ret = false;
		if( (sysSeList!=null) && (sysSeList.clipList.Length > _sysClipId) ){
			ret = soundCall(_ch, sysSeList.clipList[_sysClipId], _volRate, _isOneShot);
		}
		return ret;
	}
	public bool soundCall(SOUND_CH _ch, AudioClip _clip, float _volRate=1.0f, bool _isOneShot=false){
		if(_clip==null)	return false;
		
		float vol=1.0f;
		switch(_ch){
			case SOUND_CH.SE:    vol = mSysData.volumeSe;    break;
			case SOUND_CH.BGM:   vol = mSysData.volumeBgm;   break;
			case SOUND_CH.VOICE: vol = mSysData.volumeVoice; break;
		}
			
		if(_isOneShot){
			sysAudioSource[(int)_ch].PlayOneShot(_clip,vol * _volRate);
		}else{
			sysAudioSource[(int)_ch].Stop();
			sysAudioSource[(int)_ch].volume = vol * _volRate;
			sysAudioSource[(int)_ch].clip = _clip;
			sysAudioSource[(int)_ch].Play();
		}
		return true;
	}
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
}
