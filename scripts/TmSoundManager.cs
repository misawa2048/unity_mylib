using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ Last=0, First=1 }
		private const string TAG_EMPTY = "_[empty]_";
		private static TmSoundManager _instance = null;
		public static TmSoundManager instance{ get{ return _instance; } }

		//---option----
		public enum OptionType{ Ramdom, Loop, SpatialBlend, Priority }

		public TmSoundManager(){
			audioCtrl = new AudioCtrl[3];
			for (int i = 0; i < audioCtrl.Length; ++i) {
				audioCtrl [i] = new AudioCtrl((Kind)i,((Kind)i == Kind.SE) ? 16:2);
				audioCtrl [i].name = audioCtrl [i].kind.ToString ();
			}
			audioCtrl [(int)Kind.BGM].config.loop=true;
		}
		
		[System.Serializable]
		public class Config{
			public AudioMixerGroup amGroup;
			public bool loop = false;
			public float maxDistance=500f;
			public float spatialBlend=1f; //0:2D - 3D:1
		}
		
		[System.Serializable]
		public class AudioCtrl{
			[HideInInspector]
			public string name;
			[HideInInspector]
			public Kind kind;
			public Config config;
			public GameObject trackPrefab;
			//			[HideInInspector]
			public Track[] track;
			public AudioCtrl(Kind _kind, int _num=1){
				kind = _kind;
				config = new Config();
				track = new Track[_num];
				for(int i = 0; i < track.Length; ++i){
					track[i] = new Track();
				}
			}
		}
		
		[System.Serializable]
		public class Track{
			public enum FadeState{ Min,In, Max,Out }
			public string name=TAG_EMPTY;
			public AudioSource source;
			//			public float unbarrageTime;
			public float minLife;
			public float fadeTime;
			public Order order = Order.Last;
			public int priority;
			public float masterVolume;
			public float directivity; //0f-1f
			public Vector3 offset;
			public GameObject target;
			public Dictionary<OptionType, dynamic> option;
			private float mPlayingTime;
			private FadeState mFadeState;
			public FadeState fadeState { get { return mFadeState; } }
			private float mFadeRate;
			public float fadeRate { get{ return mFadeRate; } }
			private float mFadeOutSttTime;
			public Vector3 getPosition() {
				Vector3 pos = offset;
				if (target != null) {
					source.transform.rotation = target.transform.rotation;
					pos = target.transform.TransformDirection(pos);
					pos += target.transform.position;
				}
				return pos;
			}
			public void init(){
				mPlayingTime = 0f;
				mFadeRate = 0f;
				mFadeOutSttTime = source.loop ? float.MaxValue : (source.clip.length - fadeTime);
			}
			public void play(AudioListener _listener=null){
				init ();
				updateVolume (_listener);
				source.Play();
			}
			public void stop (float _fadeTime=-1f){
				if (mPlayingTime < mFadeOutSttTime) {
					mFadeOutSttTime = mPlayingTime;
				}
				if (_fadeTime >= 0f) {
					fadeTime = _fadeTime;
				}
			}
			public void update(AudioListener _listener=null){
				mPlayingTime += Time.deltaTime;
				updatePos (_listener);
				updateVolume (_listener);
			}
			private void updateVolume(AudioListener _listener=null){
				float tmpVol = updateFadeVolume();
				if((directivity>0f)&&(_listener!=null)){
					tmpVol *= updateDirectivityVolume(_listener.transform);
				}
				source.volume = masterVolume * tmpVol;
			}
			private void updatePos(AudioListener _listener=null){
				if (source != null) {
					source.transform.position = getPosition();
				}
			}
			private float updateFadeVolume(){
				float fadeDiff = (fadeTime>0) ? Mathf.Clamp01(Time.deltaTime/fadeTime) : 1f;
				mFadeState = FadeState.Min;
				if ((source != null) && (source.isPlaying)){
					if(mPlayingTime > mFadeOutSttTime){
						mFadeState = FadeState.Out;
						mFadeRate = Mathf.Clamp01(mFadeRate - fadeDiff);
						if(mFadeRate<=0f){
							source.Stop();
						}
					}else if(mPlayingTime < fadeTime){
						mFadeState = FadeState.In;
						mFadeRate = Mathf.Clamp01(mFadeRate + fadeDiff);
					}else{
						mFadeState = FadeState.Max;
						mFadeRate = 1.0f;
					}
				}
				return mFadeRate;
			}
			private float updateDirectivityVolume(Transform _hearTr){
				Vector3 dir = source.transform.position- -_hearTr.position;
				float d = Vector3.Dot(dir.normalized,_hearTr.forward);
				return Mathf.Lerp(1f,(d+1f)*0.5f,Mathf.Clamp01(directivity));
			}
		}
		
		public class PlayInfo{
			public Kind kind=Kind.SE;
			public string tag;
			public AudioClip clip;
			//			public float unbarrageTime=0.1f;
			public float minLife=1f;
			public float fadeTime=0f;
			public Order order=Order.Last;
			public int maxTracks=16;
			public int priority=0;
			public float masterVolume=1f;
			public float directivity=0f;
			public Vector3 offset=Vector3.zero;
			public GameObject target=null;
			public Dictionary<OptionType, dynamic> option=null;
		}
		
		//----------------------------
		public AudioListener listener;
		public AudioCtrl[] audioCtrl;
		
		//----------------------------
		void Awake(){
			_instance = this;
			initAudioSource();
		}
		void Start () {
			if (listener == null) {
				listener = Camera.main.GetComponent<AudioListener>();
			}
		}
		
		void Update () {
			updateAudioSource ();
#if UNITY_EDITOR
			debugInfo ();
#endif
		}
		
		//----------------------------
		public Track Play(AudioClip _clip, Vector3 _pos, Kind _kind=Kind.SE){
			PlayInfo info = new PlayInfo ();
			info.kind =_kind;
			info.tag = _clip.name;
			info.clip = _clip;
			info.offset = _pos;
			return Play (info);
		}

		public Track Play(PlayInfo _info){
			Track track = null;
			AudioCtrl ac = audioCtrl[(int)_info.kind];
			int playingTagNum=0;
			int overwritableTagNum = 0;
			for(int i = 0; i< ac.track.Length; ++i){
				if(ac.track[i].name == _info.tag){
					playingTagNum++;
					float old = ac.track[i].source.time / ac.track[i].source.clip.length;
					if(old >= ac.track[i].minLife){
						overwritableTagNum++;
					}
				}
			}
			if((_info.order == Order.First)&&((playingTagNum-overwritableTagNum)>=_info.maxTracks)){
				return track;
			}
			for(int i = 0; i< ac.track.Length; ++i){
				if((playingTagNum<_info.maxTracks)&&(!ac.track[i].source.isPlaying)){
					track = ac.track[i];
					break;
				}else {
					if((ac.track[i].source!=null)&&(ac.track[i].source.clip!=null)){
						float old = ac.track[i].source.time / ac.track[i].source.clip.length;
						if(old >= ac.track[i].minLife){
//							Debug.Log(">>"+overwritableTagNum);
							track = ac.track[i];
							break;
						}
					}
				}
			}
			if((_info.clip!=null)&&(track!=null)){
				//init
				if(ac.trackPrefab==null){
					track.source.loop = ac.config.loop;
					track.source.spatialBlend = ac.config.spatialBlend;
					track.source.maxDistance = ac.config.maxDistance;
				}

				if(_info.option!=null){
					track.option = _info.option;
					analyzeOption (track);
				}

				track.name = _info.tag;
				//				track.unbarrageTime = _info.unbarrageTime;
				track.minLife = _info.minLife;
				track.fadeTime = _info.fadeTime;
				track.order = _info.order;
				track.priority = _info.priority;
				track.masterVolume = _info.masterVolume;
				track.directivity = _info.directivity;
				track.offset = _info.offset;
				track.target = _info.target;
				track.source.clip = _info.clip;
				track.play(listener);
				//				Debug.Log("Play("+_tag+")");
			}
			return track;
		}

		//  fade track from nowSouece to newSource
		public bool Fade(PlayInfo _info, Track _track, float _time){
			bool ret = false;
			if (_track != null) {
				if(_track.source!=null){
					if(_track.source.isPlaying){
					}
				}
			}
			return ret;
		}

		//----------------------------
		
		private void initAudioSource(){
			for(int i=0; i<audioCtrl.Length;++i){
				addAudioSource(audioCtrl[i]);
			}
		}
		
		private void addAudioSource(AudioCtrl _actrl){
			_actrl.name = _actrl.kind.ToString();
			//			_actrl.track = new Track[_actrl.numTrack];
			for(int i = 0; i< _actrl.track.Length; ++i){
				GameObject trackObj;
				AudioSource comAudioSource=null;
				if(_actrl.trackPrefab!=null){
					trackObj = GameObject.Instantiate(_actrl.trackPrefab);
				}else{
					trackObj = new GameObject();
					trackObj.AddComponent<AudioSource>();
				}
				comAudioSource = trackObj.GetComponent<AudioSource>();
				if(comAudioSource==null){
					Debug.Log("Warning. Track must have Audiosource.");
				}else{
					if(_actrl.trackPrefab!=null){
						_actrl.config.amGroup = comAudioSource.outputAudioMixerGroup;
						_actrl.config.loop = comAudioSource.loop;
						_actrl.config.spatialBlend = comAudioSource.spatialBlend;
						_actrl.config.maxDistance = comAudioSource.maxDistance;
					}
				}
				trackObj.name = "Track_"+_actrl.name+"_"+i.ToString();
				trackObj.transform.position = gameObject.transform.position;
				trackObj.transform.parent = gameObject.transform;
				//				_actrl.track[i] = new Track();
				Track track = _actrl.track[i];
				track.source = comAudioSource;
				track.source.outputAudioMixerGroup = _actrl.config.amGroup;
				track.source.playOnAwake = false;
				track.source.dopplerLevel = 1f;
			}
		}
		
		private void updateAudioSource(){
			for(int i=0; i<audioCtrl.Length;++i){
				updateAudioSource(audioCtrl[i]);
			}
		}
		private int updateAudioSource(AudioCtrl _actrl){
			int playingTrackNum=0;
			_actrl.name = _actrl.kind.ToString();
			for(int i = 0; i< _actrl.track.Length; ++i){
				if(updateTrack(_actrl.track[i])){
					playingTrackNum++;
				}
			}
			return playingTrackNum;
		}
		private bool updateTrack(Track _track){
			bool ret = false;
			if(_track.source.clip!=null){
				if(!_track.source.isPlaying){
					_track.source.clip = null;
					_track.name = TAG_EMPTY;
				}else{
					_track.update(listener);
					ret = true;
				}
			}
			if(_track.option!=null){
				updateOption (_track);
			}

			return ret;
		}

		private int analyzeOption(Track _track){
			// change _track.masterVolume value. 
			// change _track.source.loop value. 
			int retCnt = 0;
			Dictionary<OptionType, dynamic> opt = _track.option;
			foreach(var v in opt){
				switch(v.Key){
				case OptionType.Ramdom : 
					if( v.Value.GetType().Equals(typeof(Vector2))){
						retCnt++;
						Vector2 val = (Vector2)v.Value;
						_track.source.pitch = Random.Range(val.x,val.y);
						_track.masterVolume *= Random.Range(0.8f,1.0f);
					}
					break;
				case OptionType.Loop :
					if( v.Value.GetType().Equals(typeof(bool))){
						retCnt++;
						_track.source.loop = (bool)v.Value;
					}
					break;
				case OptionType.SpatialBlend :
					if( v.Value.GetType().Equals(typeof(float))){
						retCnt++;
						_track.source.spatialBlend = (float)v.Value;
					}
					break;
				case OptionType.Priority :
					if( v.Value.GetType().Equals(typeof(int))){
						retCnt++;
						_track.source.priority = (int)v.Value;
					}
					break;
				}
			}
			return retCnt;
		}
		private int updateOption(Track _track){
//			Dictionary<OptionType, dynamic> opt = _track.option;
			return 0;
		}

		//---------------------------
		private int debugInfo(){
			int ret = 0;
			foreach(AudioCtrl ac in audioCtrl){
				int cnt = 0;
				foreach(Track tr in ac.track){
					string baseStr = "Track_"+ac.name+"_"+cnt.ToString("D2")+"_";
					cnt++;
					if(tr.source!=null){
						if(tr.source.isPlaying){
							ret++;
							tr.source.gameObject.name=baseStr + tr.name;
							Debug.DrawLine(tr.getPosition()-Vector3.up,tr.getPosition()+Vector3.up,Color.yellow);
						}else{
							tr.source.gameObject.name=baseStr + tr.name;
						}
					}
				}
			}
			return ret;
		}
		
	}
}
