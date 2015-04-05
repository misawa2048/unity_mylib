using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

//-------------------------------------------------------------------
// 0. attach this script onto an empty gameObject.
// 1. set UnityAds key(key_android/key_ios);
//2. UnityAdsController.Show( result => { if(result == ShowResult.Finished){ } } );
//-------------------------------------------------------------------

public class UnityAdsController : MonoBehaviour {
	public const float VERSION = 1.0f;
	private const string PREFAB_NAME = "UnityAdsControllerPrefab";
	private const string NAME = "_UnityAdsController";
	public enum State{
		WaitToStart,
		NetCheck,
		Init,
		WaitToStandBy,
		StandBy,
		Error,
	}

	public const string key_android = "28877";
	public const string key_ios = "28876";
	private const string zoneID = "rewardedVideoZone"; //  dev setings / extrasettings

	private static UnityAdsController mInstance = null;
	public static UnityAdsController instance{
		get{
			if(mInstance==null){
				GameObject adObj;
				UnityEngine.Object resObj = Resources.Load(PREFAB_NAME);
				if(resObj!=null){
					adObj = GameObject.Instantiate(resObj) as GameObject;
					adObj.name = NAME;
					mInstance = adObj.GetComponent<UnityAdsController>();
				}
			}
			return mInstance;
		}
	}

	private string mKey;
	private State mState;
	public static State state{ get{ return (mInstance==null) ? State.Error : mInstance.mState; } }

	void Awake(){
		if(mInstance!=null){
			Destroy(gameObject);
			return;
		}
		mInstance = this;
		mState = (Advertisement.isSupported) ? State.WaitToStart : State.Error;
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
#if UNITY_ANDROID
		initialize(key_android);
#elif UNITY_IPHONE
		initialize(key_ios);
#endif
	}
	
	// Update is called once per frame
	void Update () {
		switch(mState){
		case State.WaitToStart: break;
		case State.NetCheck:
			if((Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)||(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)){
				mState = State.Init;
			}
			break;
		case State.Init:
			Advertisement.Initialize (mKey,false);  //Application.platform==RuntimePlatform.IPhonePlayer
			mState = State.WaitToStandBy;
			break;
		case State.WaitToStandBy:
			if (Advertisement.isInitialized && Advertisement.isReady()) {
				mState = State.StandBy;
			}
			break;
		case State.StandBy: break;
		case State.Error:   break;
		}
	}

	private void initialize(string _key){
		if(mState==State.WaitToStart){
			mInstance.mKey = _key;
			mInstance.mState = State.NetCheck;
		}else{
			Debug.Log("UnityAds can't re-initializing.");
		}
	}
	
	static public bool Show(string _zoneID, ShowOptions _options){
		bool ret = false;
		if((mInstance!=null)&&(mInstance.mState==State.StandBy)){
			ret = true;
			Advertisement.Show(_zoneID,_options);
		}
		return ret;
	}

	static public bool Show(System.Action<ShowResult> _callback){
		bool ret = false;
		if((mInstance!=null)&&(mInstance.mState==State.StandBy)){
			ShowOptions options = new ShowOptions();
			options.pause = true; // game sound
			options.resultCallback = _callback;
			Advertisement.Show(zoneID,options);
			ret = true;
		}
		return ret;
	}
}
