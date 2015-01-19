using UnityEngine;
using System.Collections;

// Edit > ProjectSettings > ScriptExecitopmOrder > +TmSystem > -n
// (GameObject)sysObj > Tag > AddTag > "tagSystem"
public class mouseSample : MonoBehaviour {
	private TmSystem mSys;
	
	void Awake(){
		mSys = TmSystem.instance;
	}
	
	// Use this for initialization
	void Start () {
		// gameObject(contains colllodar) become draggable.
		mSys.mw.setDragMode(TmMouseWrapper.DRAG_MODE.CONST_CAMERA_NORMAL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
