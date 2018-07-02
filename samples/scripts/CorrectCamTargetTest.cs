using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TmLib;

public class CorrectCamTargetTest : MonoBehaviour {
	[SerializeField] Vector3 m_targetOfs = Vector3.up;
	[SerializeField] float m_cameraDist = 2f;
	[SerializeField] float m_cameraYaw = 45f;
	[SerializeField] float m_plHalfH = 0.5f;
	[SerializeField] float m_plRad = 0.5f;
	[SerializeField] float m_camRad = 0.2f;
	Vector3 tgtPos;
	Vector3 camPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		CapsuleCollider coll = GetComponent<CapsuleCollider> ();
		coll.height = (m_plHalfH + m_plRad) * 2.0f;
		coll.radius = m_plRad;
		tgtPos = transform.position + transform.rotation * m_targetOfs;
		camPos = tgtPos + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
		Vector3 plHOfs = transform.rotation * (Vector3.up * m_plHalfH);

		Vector3 corTgt;
		corTgt = TmMath.CorrectCamTarget(tgtPos, camPos, transform.position, plHOfs, m_plRad, m_camRad);

		Vector3 corCam = corTgt + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
		Debug.DrawLine (corTgt, corCam, Color.red);
	}

	/// <summary>
	/// Corrects the cam target.
	/// playerCollision(Cupsule), カメラ視線_tgtPos-_camPosから、
	/// Cupsuleの内側から始まる補正カメラtgtPosを返す
	/// </summary>
	/// <returns>The cam target.</returns>
	/// <param name="_tgtPos">world Tgt position.</param>
	/// <param name="_camPos">world Cam position.</param>
	/// <param name="_plPos">world Pl position.</param>
	/// <param name="_plHOfs">world Pl H ofs.</param>
	/// <param name="_plRad">Pl RAD.</param>
	/// <param name="_camRad">Cam RAD.</param>
	/*
	CapsuleCollider coll = GetComponent<CapsuleCollider> ();
	coll.height = (m_plHalfH + m_plRad) * 2.0f;
	coll.radius = m_plRad;
	tgtPos = transform.position + transform.rotation * m_targetOfs;
	camPos = tgtPos + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
	Vector3 plHOfs = transform.rotation * (Vector3.up * m_plHalfH);

	Vector3 corTgt;
	corTgt = CorrectCamTarget(tgtPos, camPos, transform.position, plHOfs, m_plRad, m_camRad);

	Vector3 corCam = corTgt + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
	Debug.DrawLine (corTgt, corCam, Color.red);
	*/
	Vector3 CorrectCamTarget(Vector3 _tgtPos, Vector3 _camPos, Vector3 _plPos, Vector3 _plHOfs, float _plRad, float _camRad){
		Vector3 retTgtPos = _tgtPos;
		Vector3 _plPos0 = _plPos - _plHOfs;
		Vector3 _plPos1 = _plPos + _plHOfs;
		Vector3 p0, q0;
		TmMath.LineToLineDistance (out p0, out q0, _plPos0, _plPos1, _tgtPos, _camPos, true);
		q0 = TmMath.NearestPointOnLine (tgtPos, camPos, p0, true);
		float dist = (q0 - p0).magnitude;
		if (dist > (m_plRad + m_camRad)) {
			retTgtPos = p0 + (q0 - p0).normalized * (m_plRad + m_camRad);
		} else {
			Vector3 dir = (tgtPos-camPos).normalized;
			retTgtPos = q0 + dir * (dist - (m_plRad + m_camRad));
		}
		return retTgtPos;
	}

	void OnDrawGizmos(){
		Gizmos.DrawLine (tgtPos,camPos);
	}
}
