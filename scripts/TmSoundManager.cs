using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ First=0, Last=1 }
		private const string TAG_EMPTY = "_empty_";
		
		[System.Serializable]
		public class AudioCtrl{
			[HideInInspector]
			public string name;
			[HideInInspector]
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
		
		void Start () {
			initAudioSource();
		}
		
		void Update () {
			updateAudioSource ();
			if(Input.GetMouseButtonDown(0)){
				debugPlay();
			}
		}
		
		//----------------------------
		public bool Play(Kind _kind, string _tag, AudioClip _clip, int _maxTracks, Order _order, int _priority){
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
		public bool Play(PlayInfo _info){
			bool ret = false;
			AudioCtrl ac = audioCtrl[(int)_info.kind];
			int playingTagNum=0;
			for(int i = 0; i< ac.track.Length; ++i){
				if(ac.track[i].name == _info.tag){
					playingTagNum++;
				}
			}
			if((playingTagNum>=_info.maxTracks)&&(_info.order == Order.First)){
				return false;
			}
			Track track = null;
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
				track.source.clip = _info.clip;
				track.source.Play();
				ret = true;
				//				Debug.Log("Play("+_tag+")");
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
					ret = true;
				}
			}
			return ret;
		}
		
		private int debugPlay(){
			int num = 0;
			for(int j=0; j<audioCtrl.Length;++j){
				for(int i = 0; i< audioCtrl[j].numSource; ++i){
					if(audioCtrl[j].tmpAcArr.Length>0){
						AudioClip ac = audioCtrl[j].tmpAcArr[Random.Range(0,audioCtrl[j].tmpAcArr.Length)];
						if(ac!=null){
							if(Play(audioCtrl[j].kind, ac.name, ac, 2, Order.First, 0)){
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
