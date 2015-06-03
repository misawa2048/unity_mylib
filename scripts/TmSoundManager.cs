using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ First=0, Last=1 }
		private const string TAG_EMPTY = "_empty_";
		
		[System.Serializable]
		public class Track{
			public string name;
			public AudioSource source;
			public float lifeTime;
			public Order order;
			public int priority;
		}
		
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
		
		public bool Play(Kind _kind, AudioClip _clip, string _tag, int _maxNum, Order _order, int _priority){
			bool ret = false;
			AudioCtrl ac = audioCtrl[(int)_kind];
			int playingTagNum=0;
			for(int i = 0; i< ac.track.Length; ++i){
				if(ac.track[i].name == _tag){
					playingTagNum++;
				}
			}
			if((playingTagNum>=_maxNum)&&(_order == Order.First)){
				return false;
			}
			Track track = null;
			for(int i = 0; i< ac.track.Length; ++i){
				if(!ac.track[i].source.isPlaying){
					track = ac.track[i];
					break;
				}else if((playingTagNum>=_maxNum)&&(ac.track[i].name == _tag)){
					track = ac.track[i];
					break;
				}
			}
			if((_clip!=null)&&(track!=null)){
				track.name = _tag;
				track.lifeTime = _clip.length;
				track.order = _order;
				track.priority = _priority;
				track.source.clip = _clip;
				track.source.Play();
				ret = true;
				//				Debug.Log("Play("+_tag+")");
			}
			return ret;
		}
		
		private void initAudioSource(){
			for(int i=0; i<audioCtrl.Length;++i){
				addAudioSource(audioCtrl[i]);
			}
		}
		
		private void addAudioSource(AudioCtrl _actrl){
			_actrl.name = _actrl.kind.ToString();
			_actrl.track = new Track[_actrl.numSource];
			for(int i = 0; i< _actrl.numSource; ++i){
				_actrl.track[i] = new Track();
				Track track = _actrl.track[i];
				track.name = TAG_EMPTY;
				track.source = gameObject.AddComponent<AudioSource>();
				track.source.outputAudioMixerGroup = _actrl.amGroup;
				track.source.loop = false;
				track.source.playOnAwake = false;
			}
			//test
			for(int i = 0; i< _actrl.numSource; ++i){
				if(_actrl.tmpAcArr.Length>0){
					AudioClip ac = _actrl.tmpAcArr[Random.Range(0,_actrl.tmpAcArr.Length)];
					if(ac!=null){
						Play(_actrl.kind, ac, ac.name, 2, Order.Last, 0);
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
				Track track = _actrl.track[i];
				if(track.source.clip!=null){
					if(!track.source.isPlaying){
						track.source.clip = null;
						track.name = TAG_EMPTY;
					}else{
						playingTrackNum++;
					}
				}
			}
			return playingTrackNum;
		}
		
		private int debugPlay(){
			int num = 0;
			for(int j=0; j<audioCtrl.Length;++j){
				for(int i = 0; i< audioCtrl[j].numSource; ++i){
					if(audioCtrl[j].tmpAcArr.Length>0){
						AudioClip ac = audioCtrl[j].tmpAcArr[Random.Range(0,audioCtrl[j].tmpAcArr.Length)];
						if(ac!=null){
							if(Play(audioCtrl[j].kind, ac, ac.name, 2, Order.First, 0)){
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
