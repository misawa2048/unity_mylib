using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

//-------------------------------------------------------------------
// 1. attach this script onto an empty gameObject.
// 2. set UnityAds key(key_android/key_ios).
// 3. wait while(!UnityAdsController.isReady).
// 4. UnityAdsController.Show( result => { if(result == ShowResult.Finished){ } } );
//-------------------------------------------------------------------

public class UnityAdsController : MonoBehaviour {
	public const float VERSION = 1.0f;
	private const bool IsTest = false;
	private const string PREFAB_NAME = "UnityAdsControllerPrefab";
	private const string NAME = "_UnityAdsController";
	public enum State{
		WaitToStart,
		NetCheck,
		Init,
		WaitToReady,
		Ready,
		Error,
	}

	public const string key_android = "33100";
	public const string key_ios = "33099";
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
	public static bool isReady{ get{ return (mInstance==null) ? false : (mInstance.mState==State.Ready); } }
	public static bool IsState(State _state){
		return (mInstance==null) ? false : (mInstance.mState == _state);
	}

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
			Advertisement.Initialize (mKey,IsTest);  //Application.platform==RuntimePlatform.IPhonePlayer
			mState = State.WaitToReady;
			break;
		case State.WaitToReady:
			if (Advertisement.isInitialized && Advertisement.isReady()) {
				mState = State.Ready;
			}
			break;
		case State.Ready: break;
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
		if((mInstance!=null)&&(mInstance.mState==State.Ready)){
			ret = true;
			Advertisement.Show(_zoneID,_options);
		}
		return ret;
	}

	static public bool Show(System.Action<ShowResult> _callback){
		bool ret = false;
		if((mInstance!=null)&&(mInstance.mState==State.Ready)){
			ShowOptions options = new ShowOptions();
			options.pause = true; // game sound
			options.resultCallback = _callback;
			Advertisement.Show(zoneID,options);
			ret = true;
		}
		return ret;
	}
}
