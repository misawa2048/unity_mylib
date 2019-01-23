using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using MidiJack;
#endif

public class miditest : MonoBehaviour {
	[SerializeField] int keyStt = 21;// 0;
	[SerializeField] int keyNum = 88;// 121;
	GameObject[] keyArr;
	float minX = -2.4f; // 高い音側
	float maxX = 2.4f; // 低い音側

	// Use this for initialization
	void Start () {
//        Application.targetFrameRate = 60;
		#if !UNITY_WEBGL
		MidiJack.MidiMaster.noteOnDelegate = noteOnDel;
		MidiJack.MidiMaster.noteOffDelegate = noteOffDel;
		#endif
		keyArr = createKeyboadObj (keyStt,keyNum);
	}
	
	// Update is called once per frame
	void Update () {
		float spd = Time.deltaTime * 0.5f;
		Vector3 nowPos = transform.position;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			nowPos.x += spd;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			nowPos.x -= spd;
		}
		nowPos.x = Mathf.Clamp (nowPos.x, minX, maxX);
		transform.position = nowPos;
	}

	#if !UNITY_WEBGL
	void noteOnDel(MidiChannel ch, int note, float vel){
//		Debug.Log (ch.ToString()+":"+note.ToString()+":"+vel.ToString());
		if ((note >= keyStt) && (note < (keyStt+keyNum))) {
			keyArr [note].transform.localRotation = Quaternion.Euler (-4f, 0f, 0f);
		}
	}
	void noteOffDel(MidiChannel ch, int note){
		if ((note >= keyStt) && (note < (keyStt+keyNum))) {
			keyArr [note].transform.localRotation = Quaternion.identity;
		}
	}
	#endif

	GameObject[] createKeyboadObj(int _keyStt, int _keyNum){
		float[] ofsX = {  0f,0.5f,1f,1.5f,2f,3f,3.5f,4f,4.5f,5f,5.5f,6f,};
		GameObject[] objs = new GameObject[_keyStt+_keyNum];
		for (int i = _keyStt; i < (_keyStt+_keyNum); ++i) {
			float ofs = (float)(i / 12)*7f;
			float ox = (float)(ofs + ofsX [i % 12]);
			float oz = ((ofsX [i % 12]%1)==0f) ? 0f:0.15f;
			Color col = (oz == 0f) ? Color.white : new Color(0.3f,0.3f,0.3f);
			Vector3 scl = (oz == 0f) ? new Vector3 (0.1f, 0.1f, 0.5f) : new Vector3 (0.06f, 0.1f, 0.3f);
			GameObject pivot = new GameObject ("key" + i.ToString());
			pivot.transform.SetParent (gameObject.transform);
			pivot.transform.localPosition = new Vector3 ( ox * 0.11f, oz * 0.6f, oz*0.8f);
			pivot.transform.localRotation = Quaternion.identity;
			GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.SetParent (pivot.transform);
			obj.transform.localScale = scl;
			obj.transform.localPosition = new Vector3 (0f, 0f, -1f);
			obj.transform.localRotation = Quaternion.identity;
			obj.GetComponent<MeshRenderer> ().material.SetColor ("_Color", col);
			objs [i] = pivot;
		}
		return objs;
	}
}
