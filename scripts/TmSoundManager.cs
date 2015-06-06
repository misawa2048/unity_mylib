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
		
		[System.Serializable]
		public class AudioCtrl{
			//			[HideInInspector]
			public string name;
			//			[HideInInspector]
			public Kind kind;
			public AudioMixerGroup amGroup;
			public int numSource;
			//			[HideInInspector]
			public Track[] track;
			public AudioClip[] tmpAcArr;
		}
		
		[System.Serializable]
		public class Track{
			public string name;
			public AudioSource source;
			public float lifeTime;
			public Order order;
			public int priority;
			public Vector3 position;
			public GameObject target;
			public void updatePos(){
				if (source != null) {
					Vector3 pos = position;
					if (target != null) {
						pos = target.transform.TransformDirection(pos);
						pos += target.transform.position;
					}
					source.transform.position = pos;
				}
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
			public Vector3 position;
			public GameObject target;
		}
		
		public AudioCtrl[] audioCtrl;
		
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
		public Track Play(Kind _kind, string _tag, AudioClip _clip, int _maxTracks, Order _order, int _priority){
			PlayInfo info = new PlayInfo();
			info.kind = _kind;
			info.tag = _tag;
			info.clip = _clip;
			info.lifeTime = (_clip!=null) ? _clip.length : 0;
			info.order = _order;
			info.maxTracks = _maxTracks;
			info.priority = _priority;
			info.position = gameObject.transform.position;
			info.target = null;
			return Play(info);
		}
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
				track.position = _info.position;
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
			_actrl.track = new Track[_actrl.numSource];
			for(int i = 0; i< _actrl.numSource; ++i){
				GameObject trackObj = new GameObject("Track_"+_actrl.name+"_"+i.ToString());
				trackObj.transform.position = gameObject.transform.position;
				trackObj.transform.parent = gameObject.transform;
				_actrl.track[i] = new Track();
				Track track = _actrl.track[i];
				track.name = TAG_EMPTY;
				track.source = trackObj.AddComponent<AudioSource>();
				track.source.outputAudioMixerGroup = _actrl.amGroup;
				track.source.loop = false;
				track.source.playOnAwake = false;
				track.source.spatialBlend = 1.0f;
			}
			//test
			for(int i = 0; i< _actrl.numSource; ++i){
				if(_actrl.tmpAcArr.Length>0){
					AudioClip ac = _actrl.tmpAcArr[Random.Range(0,_actrl.tmpAcArr.Length)];
					if(ac!=null){
						Play(_actrl.kind, ac.name, ac, 2, Order.Last, 0);
					}
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
			for(int i = 0; i< _actrl.numSource; ++i){
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
					ret = true;
				}
			}
			return ret;
		}
		private void updateTrackPos(Track _track){
			if (_track != null){
				Vector3 pos = _track.position;
				if(_track.target != null) {
					pos = _track.target.transform.TransformDirection(pos);
					pos += _track.target.transform.position;
				}
				_track.source.transform.position = pos;
			}
		}
		
		private int debugPlay(){
			int num = 0;
			for(int j=0; j<audioCtrl.Length;++j){
				for(int i = 0; i< audioCtrl[j].numSource; ++i){
					if(audioCtrl[j].tmpAcArr.Length>0){
						AudioClip ac = audioCtrl[j].tmpAcArr[Random.Range(0,audioCtrl[j].tmpAcArr.Length)];
						if(ac!=null){
							if(Play(audioCtrl[j].kind, ac.name, ac, 2, Order.First, 0)!=null){
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
