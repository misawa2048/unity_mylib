using UnityEngine;
using System.Collections;

public class bgScript : MonoBehaviour {
	public Vector2 spd = new Vector2(0.01f,-0.01f);
	public string texPropertyName = "_MainTex";
	private Vector2 ofs;
	
	// Use this for initialization
	void Start () {
		ofs = renderer.material.GetTextureOffset(texPropertyName);
	}
	
	// Update is called once per frame
	void Update () {
		ofs += spd * Time.deltaTime;
		renderer.material.SetTextureOffset (texPropertyName, ofs);
	}
}
