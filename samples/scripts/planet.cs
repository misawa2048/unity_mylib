using UnityEngine;
using System.Collections;

public class planet : MonoBehaviour {
	public float speed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up*speed*Time.deltaTime);
	}
}
