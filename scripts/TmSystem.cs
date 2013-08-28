using UnityEngine;
using System.Collections;

public class TmSystem : MonoBehaviour {
	public const float VERSION = 0.5f;
	private const string PREFAB_NAME = "sysPrefab"; // Instantiate 
	public const string NAME = "_sys";
	public const string TAG_NAME = "tagSystem";
	public TmMouseWrapper mw = new TmMouseWrapper();
//	public TmTouchWrapper tw = new TmTouchWrapper();
	
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
		public float volumeMaster = 1.0f;
		public float volumeSe = 1.0f;
		public float volumeBgm = 1.0f;
		public float volumeVoice = 1.0f;
	};
	[System.Serializable]
	public class ClipList{
		public AudioClip[] clipList;
	}

	public string AD_KEY = "";
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
//				DontDestroyOnLoad(sysObj);
			}
			return m_Instance;
		}
	}
	private SysData mSysData = new SysData();
	private AudioSource[] sysAudioSource = new AudioSource[3];

	void Awake () {
		if(m_Instance==null){
			m_Instance = this;
			DontDestroyOnLoad(gameObject);
			for(int ii = 0; ii < SOUND_CH_NUM; ++ii){
				sysAudioSource[ii] = gameObject.AddComponent<AudioSource>();
				sysAudioSource[ii].volume = 1.0f;
				sysAudioSource[ii].loop = (ii==(int)SOUND_CH.BGM);
				sysAudioSource[ii].playOnAwake = false;
			}
			bool ret = loadSysData();
			if(!ret) Debug.Log("NoSaveData");
		}else{
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
//		tw.update();
	}

	//---------------------------------------------------------
	public bool saveSysData(){
		bool ret = false;
		PlayerPrefs.SetInt("hasSysSaveData",1);
		if(PlayerPrefs.HasKey("hasSysSaveData")){
			ret = true;
			mSysData.hasSysSaveData = true;
			PlayerPrefs.SetInt("achievementFlag",mSysData.achievementFlag);
			PlayerPrefs.SetFloat("volumeMaster",mSysData.volumeMaster);
			PlayerPrefs.SetFloat("volumeSe",mSysData.volumeSe);
			PlayerPrefs.SetFloat("volumeBgm",mSysData.volumeBgm);
			PlayerPrefs.SetFloat("volumeVoice",mSysData.volumeVoice);
		}
		return ret;
	}
	private bool loadSysData(){
		bool ret = false;
		if(PlayerPrefs.HasKey("hasSysSaveData")){
			ret = true;
			mSysData.hasSysSaveData = true;
			mSysData.achievementFlag = PlayerPrefs.GetInt("achievementFlag");
			mSysData.volumeMaster = PlayerPrefs.GetFloat("volumeMaster");
			mSysData.volumeSe = PlayerPrefs.GetFloat("volumeSe");
			mSysData.volumeBgm = PlayerPrefs.GetFloat("volumeBgm");
			mSysData.volumeVoice = PlayerPrefs.GetFloat("volumeVoice");
			AudioListener.volume = mSysData.volumeMaster;
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
		float vol=getChannelVolume(_ch);
		if(_isOneShot){
			if(_clip==null)	return false;
			sysAudioSource[(int)_ch].PlayOneShot(_clip,vol * _volRate);
		}else{
			if(_clip!=null){
				sysAudioSource[(int)_ch].Stop();
			}
			sysAudioSource[(int)_ch].volume = vol * _volRate;
			if(_clip!=null){
				sysAudioSource[(int)_ch].clip = _clip;
				sysAudioSource[(int)_ch].Play();
			}
		}
		return true;
	}
	//---------------------------------------------------------
	public void soundStop(SOUND_CH _ch){
		sysAudioSource[(int)_ch].Stop();
	}
	//---------------------------------------------------------
	public float getMasterVolume(){
		return mSysData.volumeMaster;
	}
	//---------------------------------------------------------
	public float setMasterVolume(float _rate, bool _isAutoSave=true){
		float vol=getMasterVolume();
		_rate = Mathf.Clamp01(_rate);
		AudioListener.volume = _rate;
		mSysData.volumeMaster = _rate;
		if(_isAutoSave){
			saveSysData();
		}
		return vol;
	}
	//---------------------------------------------------------
	public float getChannelVolume(SOUND_CH _ch){
		float vol=1.0f;
		switch(_ch){
			case SOUND_CH.SE:    vol = mSysData.volumeSe;    break;
			case SOUND_CH.BGM:   vol = mSysData.volumeBgm;   break;
			case SOUND_CH.VOICE: vol = mSysData.volumeVoice; break;
		}
		return vol;
	}
	//---------------------------------------------------------
	public float setChannelVolume(SOUND_CH _ch,float _rate, bool _isAutoSave=true){
		float vol=getChannelVolume(_ch);
		_rate = Mathf.Clamp01(_rate);
		switch(_ch){
			case SOUND_CH.SE:    mSysData.volumeSe = _rate;    break;
			case SOUND_CH.BGM:   mSysData.volumeBgm = _rate;;   break;
			case SOUND_CH.VOICE: mSysData.volumeVoice = _rate; break;
		}
		if(_isAutoSave){
			saveSysData();
		}
		return vol;
	}
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
	//---------------------------------------------------------
}
