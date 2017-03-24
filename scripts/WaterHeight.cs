using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterUtils{
	public class WaterHeight : MonoBehaviour {
		[SerializeField]
		MeshRenderer targetMr; // set Water MeshRendrer here (Tile)
		Material mat;
		// Time (t = time since current level load) values from Unity
		// float4 _Time; // (t/20, t, t*2, t*3)

		// Use this for initialization
		void Start () {
			mat = targetMr.sharedMaterial;
		}

		// Update is called once per frame
		void Update () {
			Transform targetTr = transform.GetChild (0);
			if (targetTr == null)
				return;

            // move position and rotation same as water.
//            Vector3 ampScale = Vector3.one * targetTr.transform.lossyScale.y; // new Vector3(1f, targetTr.transform.lossyScale.y, 1f);
            Vector2 xzVtx = new Vector2 (transform.position.x, transform.position.z);
			Vector4 steepness = mat.GetVector ("_GSteepness");
			Vector4 amp = mat.GetVector ("_GAmplitude");
			Vector4 freq = mat.GetVector ("_GFrequency");
			Vector4 speed = mat.GetVector ("_GSpeed");
			Vector4 dirAB = mat.GetVector ("_GDirectionAB");
			Vector4 dirCD = mat.GetVector ("_GDirectionCD");
			Vector3 ofs = GerstnerOffset4(xzVtx, steepness, amp, freq, speed, dirAB, dirCD);
            ofs.y *= targetMr.transform.lossyScale.y;
            targetTr.localPosition = ofs;
			Vector3 nml = GerstnerNormal4(xzVtx, 1f, amp, freq, speed, dirAB, dirCD);
			targetTr.localRotation = Quaternion.FromToRotation(Vector3.up,nml*180f/Mathf.PI);
		}

		Vector3 GerstnerOffset4 (Vector2 xzVtx, Vector4 steepness, Vector4 amp, Vector4 freq, Vector4 speed, Vector4 dirAB, Vector4 dirCD) 
		{
			float t = Time.timeSinceLevelLoad;
			Vector4 _Time = new Vector4(t/20, t, t*2, t*3);
			Vector3 offsets;

			Vector4 AB = Vector4.Scale(Vector4.Scale(xxyy(steepness) , xxyy(amp)) , dirAB); //steepness.xxyy * amp.xxyy * dirAB.xyzw;
			Vector4 CD = Vector4.Scale(Vector4.Scale(zzww(steepness) , zzww(amp)) , dirCD); //steepness.zzww * amp.zzww * dirCD.xyzw;

			Vector4 dotABCD = Vector4.Scale(freq , new Vector4 (
				Vector2.Dot(new Vector2(dirAB.x,dirAB.y),xzVtx),
				Vector2.Dot(new Vector2(dirAB.z,dirAB.w),xzVtx),
				Vector2.Dot(new Vector2(dirCD.x,dirCD.y),xzVtx),
				Vector2.Dot(new Vector2(dirCD.z,dirCD.w),xzVtx)
			)); //freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
			Vector4 TIME = Vector4.Scale(Vector4.one * _Time.y , speed); //_Time.yyyy * speed;

			Vector4 COS = new Vector4(
				Mathf.Cos (dotABCD.x + TIME.x),
				Mathf.Cos (dotABCD.y + TIME.y),
				Mathf.Cos (dotABCD.z + TIME.z),
				Mathf.Cos (dotABCD.w + TIME.w)
			); //cos (dotABCD + TIME);
			Vector4 SIN = new Vector4(
				Mathf.Sin (dotABCD.x + TIME.x),
				Mathf.Sin (dotABCD.y + TIME.y),
				Mathf.Sin (dotABCD.z + TIME.z),
				Mathf.Sin (dotABCD.w + TIME.w)
			); //sin (dotABCD + TIME);

			offsets.x = Vector4.Dot(COS, new Vector4(AB.x,AB.z, CD.x,CD.z)); // dot(COS, Vector4(AB.xz, CD.xz));
			offsets.z = Vector4.Dot(COS, new Vector4(AB.y,AB.w, CD.y,CD.w)); // dot(COS, Vector4(AB.yw, CD.yw));
			offsets.y = Vector4.Dot(SIN, amp); // dot(SIN, amp);

			return offsets;			
		}

		Vector3 GerstnerNormal4(Vector2 xzVtx, float _GerstnerIntensity, Vector4 amp, Vector4 freq, Vector4 speed, Vector4 dirAB, Vector4 dirCD)
		{
			float t = Time.timeSinceLevelLoad;
			Vector4 _Time = new Vector4(t / 20, t, t * 2, t * 3);
			Vector3 nrml = new Vector3(0f, 2.0f, 0f);

			Vector4 AB = Vector4.Scale(Vector4.Scale(xxyy(freq), xxyy(amp)), dirAB); //freq.xxyy * amp.xxyy * dirAB.xyzw;
			Vector4 CD = Vector4.Scale(Vector4.Scale(zzww(freq), zzww(amp)), dirCD); //freq.zzww * amp.zzww * dirCD.xyzw;

			Vector4 dotABCD = Vector4.Scale(freq, new Vector4(
				Vector2.Dot(new Vector2(dirAB.x, dirAB.y), xzVtx),
				Vector2.Dot(new Vector2(dirAB.z, dirAB.w), xzVtx),
				Vector2.Dot(new Vector2(dirCD.x, dirCD.y), xzVtx),
				Vector2.Dot(new Vector2(dirCD.z, dirCD.w), xzVtx)
			)); //freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
			Vector4 TIME = Vector4.Scale(Vector4.one * _Time.y, speed); //_Time.yyyy * speed;

			Vector4 COS = new Vector4(
				Mathf.Cos(dotABCD.x + TIME.x),
				Mathf.Cos(dotABCD.y + TIME.y),
				Mathf.Cos(dotABCD.z + TIME.z),
				Mathf.Cos(dotABCD.w + TIME.w)
			); //cos (dotABCD + TIME);

			nrml.x -= Vector4.Dot(COS, new Vector4(AB.x, AB.z, CD.x, CD.z)); // dot(COS, half4(AB.xz, CD.xz));
			nrml.z -= Vector4.Dot(COS, new Vector4(AB.y, AB.w, CD.y, CD.w)); // dot(COS, half4(AB.yw, CD.yw));

			nrml.x *= _GerstnerIntensity;
			nrml.z *= _GerstnerIntensity; //nrml.xz *= _GerstnerIntensity;
			nrml.Normalize(); // nrml = normalize(nrml);

			return nrml;
		}

		Vector4 xxyy(Vector4 _in){ return new Vector4 (_in.x, _in.x, _in.y, _in.y); }
		Vector4 zzww(Vector4 _in){ return new Vector4 (_in.z, _in.z, _in.w, _in.w); }
	}
} // namespace WaterUtils
