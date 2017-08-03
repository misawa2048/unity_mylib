using System.Collections;
using System;
using UnityEngine;

namespace TmLib{
	public class TmExpDayTime : MonoBehaviour {

		// Use this for initialization
		void Start () {
			DateTime expDt = new DateTime (2017, 10, 01, 0, 0, 0);
			TimeSpan ts = expDt - DateTime.Now;
			if (ts.Days < 0) {
				Debug.Log ("the expiration date was reached");
				Application.Quit ();
			}
		}
		
		// Update is called once per frame
		void Update () {
		}
	}
}
