using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ First=0, Last=1 }
		private const string TAG_EMPTY = "_empty_";
		private static TmSoundManager _instance = null;
		public static TmSoundManager instance{ get{ return _instance; } }
		
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
			public AudioClip[] dbgAcArr;
			public AudioCtrl(Kind _kind, int _num=1){
				kind = _kind;
				config = new Config();
				track = new Track[_num];
			}
		}
		
		[System.Serializable]
		public class Track{
			public string name = TAG_EMPTY;
			public AudioSource source;
			public float lifeTime;
			public Order order;
			public int priority;
			public float directivity; //0f-1f
			public Vector3 offset;
			public GameObject target;
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
				source.volume = Mathf.Lerp(1f,(d+1f)*0.5f,Mathf.Clamp01(directivity));
			}
		}
		
		public class PlayInfo{
			public Kind kind;
			public string tag;
			public AudioClip clip;
			public float lifeTime;
			public Order order;
			public int maxTracks;
			public int priority;
			public float directivity;
			public Vector3 offset;
			public GameObject target;
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
			//			if(Input.GetMouseButtonDown(0)){ debugPlay(); }
		}
		
		//----------------------------
		public Track Play(PlayInfo _info){
			Track track = null;
			AudioCtrl ac = audioCtrl[(int)_info.kind];
			int playingTagNum=0;
			for(int i = 0; i< ac.track.Length; ++i){
				if(ac.track[i].name == _info.tag){
					playingTagNum++;
				}
			}
			if((playingTagNum>=_info.maxTracks)&&(_info.order == Order.First)){
				return track;
			}
			for(int i = 0; i< ac.track.Length; ++i){
				if(!ac.track[i].source.isPlaying){
					track = ac.track[i];
					break;
				}else if((playingTagNum>=_info.maxTracks)&&(ac.track[i].name == _info.tag)){
					track = ac.track[i];
					break;
				}
			}
			if((_info.clip!=null)&&(track!=null)){
				track.name = _info.tag;
				track.lifeTime = _info.lifeTime;
				track.order = _info.order;
				track.priority = _info.priority;
				track.directivity = _info.directivity;
				track.offset = _info.offset;
				track.target = _info.target;
				track.source.clip = _info.clip;
				track.updatePos();
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
			return ret;
		}
		
		private int debugPlay(){
			int num = 0;
			for(int j=0; j<audioCtrl.Length;++j){
				for(int i = 0; i< audioCtrl[j].track.Length; ++i){
					if(audioCtrl[j].dbgAcArr.Length>0){
						AudioClip ac = audioCtrl[j].dbgAcArr[Random.Range(0,audioCtrl[j].dbgAcArr.Length)];
						if(ac!=null){
							TmSoundManager.PlayInfo info = new TmSoundManager.PlayInfo();
							info.clip = ac;
							info.kind = audioCtrl[j].kind;
							info.maxTracks = 2;
							info.order = TmSoundManager.Order.First;
							info.tag = ac.name;
							info.target = this.gameObject;
							info.offset = Vector3.zero;
							if(Play(info)!=null){
								num++;
							}
						}
					}
				}
			}
			Debug.Log("num="+num);
			return num;
		}
	}
}
