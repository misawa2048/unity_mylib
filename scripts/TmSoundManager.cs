using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace TmLib{
	public class TmSoundManager : MonoBehaviour {
		public enum Kind{ SE=0, BGM=1, Voice=2 }
		public enum Order{ First=0, Last=1 }
		
		[System.Serializable]
		public class Track{
			public string name;
			public AudioSource source;
			public AudioClip clip;
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
			[HideInInspector]
			public Track[] track;
			public AudioClip[] tmpAcArr;
		}
		
		public AudioCtrl[] audioCtrl;
		
		// Use this for initialization
		void Start () {
			init();
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		
		public bool Play(Kind _kind, AudioClip _clip, string _tag, int _maxNum, Order _order, int _priority){
			AudioCtrl ac = audioCtrl[(int)_kind];
			Track track = ac.track[0];
			for(int i = 0; i< ac.track.Length; ++i){
				track = ac.track[i];
				if(track.lifeTime<=0f){
					break;
				}
			}
			track.clip = _clip;
			track.name = _tag;
			track.lifeTime = _clip.length;
			track.order = _order;
			track.priority = _priority;
			track.source.PlayOneShot(_clip);
			return true;
		}
		
		private void init(){
			for(int i=0; i<audioCtrl.Length;++i){
				addAudioSource(audioCtrl[i]);
			}
		}
		
		private void addAudioSource(AudioCtrl _actrl){
			_actrl.name = _actrl.kind.ToString();
			_actrl.track = new Track[_actrl.numSource];
			for(int i = 0; i< _actrl.numSource; ++i){
				_actrl.track[i] = new Track();
				_actrl.track[i].source = gameObject.AddComponent<AudioSource>();
				_actrl.track[i].source.outputAudioMixerGroup = _actrl.amGroup;
				//			_actrl.track[i].source.name = _actrl.name;
				//test
				if(_actrl.tmpAcArr.Length>0){
					AudioClip ac = _actrl.tmpAcArr[Random.Range(0,_actrl.tmpAcArr.Length)];
					Play(_actrl.kind, ac, ac.name, 2, Order.Last, 0);
				}
			}
		}
	}
}
