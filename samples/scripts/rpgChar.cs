using UnityEngine;
using System.Collections;

public class rpgChar : MonoBehaviour {
	public GameObject balloonPrefab;
	private TmSystem mSys;
	private TmKeyRec mKey = new TmKeyRec(1024,TmKeyRec.BUFF_TYPE.RING);
//	private Vector3 mSttScrPos;
	
	void Awake(){
		mSys = TmSystem.instance;
		
	}
	
	// Use this for initialization
	void Start () {
		mKey.debugMode = TmKeyRec.DEBUG_MODE.DISP_PAD;
//		mSttScrPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		// animation check
		if((gameObject.GetComponent<TmSpriteAnim>().animFragTrigger&0x01) != 0){
			GameObject.Instantiate(balloonPrefab,transform.position+balloonPrefab.transform.position,Quaternion.identity);
		}
		
		// key
		if((mSys.mw.dragTarget == gameObject)&&(mSys.mw.isButtonState(TmMouseWrapper.STATE.DOWN))){
			mKey.setState(TmKeyRec.STATE.REC);
		}else if((mSys.mw.dragTargetOld == gameObject)&&(mSys.mw.isButtonState(TmMouseWrapper.STATE.UP))){
			mKey.setState(TmKeyRec.STATE.STOP);
			short flag = TmKeyRec.USE_FLAG.DTIME|TmKeyRec.USE_FLAG.PAD|TmKeyRec.USE_FLAG.ANL;
			byte[] compArr = mKey.compressKeyInfoStream(flag);
//			byte[] compArr = mKey.compressedKeyInfo;
			Debug.Log("binSize="+compArr.Length+":"+mKey.recSize);
			compArr = CLZF2.Compress(compArr);
			Debug.Log("cmpSize="+compArr.Length+":"+mKey.recSize);
			compArr = CLZF2.Decompress(compArr);
			mKey.decompressKeyInfoStream(compArr);
//			mKey.decompressKeyInfo(compArr);

			mKey.setState(TmKeyRec.STATE.PLAY);
		}
		Vector3 scrPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		int ret = mKey.update(Time.deltaTime,int.MaxValue,scrPos.x/Screen.width,scrPos.y/Screen.height,0.0f,0.0f);
		if(ret>0){
			if(mKey.state != TmKeyRec.STATE.STOP){
				scrPos.x = mKey.keyInfo.anL.vRate.rateF * (float)Screen.width;
				scrPos.y = mKey.keyInfo.anL.hRate.rateF * (float)Screen.height;
				transform.position = Camera.main.ScreenToWorldPoint(scrPos);
			}
		}else{
//			mKey.resetState();
		}
	}

	void OnGUI(){
		Rect rect = new Rect(10,10,150,50);
		if(GUI.Button(rect,"www")){
			Debug.Log("BTN");
			StartCoroutine("setData");
		}
	}

	private IEnumerator setData(){
		string gApiUrl = "http://tmgamedb2.appspot.com/";
//		string gApiUrl = "http://elix-jp.sakura.ne.jp/wordpress/";

		string paramStr="?a=0";
//		paramStr+="nFunc="+"setTable"+"&nGame=jumpGame"+"&nStage=0000"+"&nName=m9"+"&nScore=9999"+"&nContent=HELLO_WORLD";
		paramStr+="&_rnd="+Random.value.ToString(); //キャッシュ防止
		string apiUrl = gApiUrl+"scoreSet.jsp"+paramStr;

		Debug.Log(apiUrl);
		WWWForm form = new WWWForm();
		form.AddField("nType","add");
		form.AddField("nSelGame","jumpGame");
		form.AddField("nSelStage","0000");
		form.AddField("nScore","0123");
		form.AddField("nName","m9");
		form.AddField("nContent","Hello,World");

		WWW www = new WWW(apiUrl,form);
		yield return www;
		Debug.Log(www.size.ToString()+":"+www.text);
	}

}
