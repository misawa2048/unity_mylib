using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ First=0, Last=1 }
		private const string TAG_EMPTY = "_[empty]_";
		private static TmSoundManager _instance = null;
		public static TmSoundManager instance{ get{ return _instance; } }
		
		//---option----
		public enum OptionType{ Ramdom, Loop  }
		
		public TmSoundManager(){
			audioCtrl = new AudioCtrl[3];
			for (int i = 0; i < audioCtrl.Length; ++i) {
				audioCtrl [i] = new AudioCtrl((Kind)i,((Kind)i == Kind.SE) ? 16:2);
				audioCtrl [i].name = audioCtrl [i].kind.ToString ();
			}
			audioCtrl [1].config.loop=true;
		}
		
		[System.Serializable]
		public class Config{
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
			public AudioMixerGroup amGroup;
			public Config config;
			public GameObject trackPrefab;
			//			[HideInInspector]
			public Track[] track;
			public AudioCtrl(Kind _kind, int _num=1){
				kind = _kind;
				config = new Config();
				track = new Track[_num];
			}
		}
		
		[System.Serializable]
		public class Track{
			public string name=TAG_EMPTY;
			public AudioSource source;
			//			public float unbarrageTime;
			public float minLife;
			public Order order;
			public int priority;
			public float masterVolume;
			public float directivity; //0f-1f
			public Vector3 offset;
			public GameObject target;
			public Dictionary<OptionType, dynamic> option;
			public void updatePos(){
				if (source != null) {
					Vector3 pos = offset;
					if (target != null) {
						source.transform.rotation = target.transform.rotation;
						pos = target.transform.TransformDirection(pos);
						pos += target.transform.position;
					}
					source.transform.position = pos;
				}
			}
			public void updateDirectivityVolume(Transform _camTr){
				Vector3 dir = source.transform.position- -_camTr.position;
				float d = Vector3.Dot(dir.normalized,_camTr.forward);
				source.volume = masterVolume * Mathf.Lerp(1f,(d+1f)*0.5f,Mathf.Clamp01(directivity));
			}
		}
		
		public class PlayInfo{
			public Kind kind=Kind.SE;
			public string tag;
			public AudioClip clip;
			//			public float unbarrageTime=0.1f;
			public float minLife=1f;
			public Order order=Order.Last;
			public int maxTracks=1;
			public int priority=0;
			public float masterVolume=1f;
			public float directivity=0f;
			public Vector3 offset;
			public GameObject target;
			public Dictionary<OptionType, dynamic> option=null;
		}
		
		//----------------------------
		public Camera targetCam;
		public AudioCtrl[] audioCtrl;
		
		//----------------------------
		void Awake(){
			_instance = this;
			initAudioSource();
		}
		void Start () {
		}
		
		void Update () {
			updateAudioSource ();
			#if UNITY_EDITOR
			debugInfo ();
			#endif
		}
		
		//----------------------------
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
					if(ac.track[i].source!=null){
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
				if(_info.option!=null){
					track.option = _info.option;
					analyzeOption (track);
				}
				
				track.name = _info.tag;
				//				track.unbarrageTime = _info.unbarrageTime;
				track.minLife = _info.minLife;
				track.order = _info.order;
				track.priority = _info.priority;
				track.masterVolume = _info.masterVolume;
				track.directivity = _info.directivity;
				track.offset = _info.offset;
				track.target = _info.target;
				track.source.clip = _info.clip;
				track.updatePos();
				track.source.volume = _info.masterVolume;
				track.source.Play();
				//				Debug.Log("Play("+_tag+")");
			}
			return track;
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
					comAudioSource = trackObj.GetComponent<AudioSource>();
				}else{
					trackObj = new GameObject();
				}
				if(comAudioSource==null){
					comAudioSource = trackObj.AddComponent<AudioSource>();
				}
				trackObj.name = "Track_"+_actrl.name+"_"+i.ToString();
				trackObj.transform.position = gameObject.transform.position;
				trackObj.transform.parent = gameObject.transform;
				//				_actrl.track[i] = new Track();
				Track track = _actrl.track[i];
				track.source = comAudioSource;
				track.source.outputAudioMixerGroup = _actrl.amGroup;
				track.source.playOnAwake = false;
				track.source.dopplerLevel = 1f;
				if(_actrl.trackPrefab==null){
					track.source.loop = _actrl.config.loop;
					track.source.spatialBlend = _actrl.config.spatialBlend;
					track.source.maxDistance = _actrl.config.spatialBlend;
				}
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
					_track.updatePos();
					if((_track.directivity>0f)&&(targetCam!=null)){
						_track.updateDirectivityVolume(targetCam.transform);
					}
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
