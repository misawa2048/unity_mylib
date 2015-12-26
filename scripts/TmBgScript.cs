using UnityEngine;
using System.Collections;

public class TmBgScript : MonoBehaviour {
	public Vector2 spd = new Vector2(0.01f,0.01f);
	public string texPropertyName = "_MainTex";
	private static TmBgScript instance = null;
	private Vector2 ofs;
	
	void Awake(){
		if(instance!=null){
			DestroyImmediate(gameObject);
		}else{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
		ofs = GetComponent<Renderer>().material.GetTextureOffset(texPropertyName);
	}
	
	// Update is called once per frame
	void Update () {
		ofs += spd * Time.deltaTime;
		GetComponent<Renderer>().material.SetTextureOffset (texPropertyName, ofs);
	}
}
