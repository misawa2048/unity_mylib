#define TM_IS_DEBUG
using UnityEngine;

public class TmDebug {

	//---------------------------------------------------------------------------
	public static void DrawLine(Vector3 start, Vector3 end){
		#if TM_IS_DEBUG
		DrawLine(start,end, Color.white,0.0f,true);
		#endif
	}
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration=0f, bool depthTest=true){
		#if TM_IS_DEBUG
		Debug.DrawLine(start,end,color,duration,depthTest);
		#endif
	}

	//---------------------------------------------------------------------------
	public static void DrawRay(Vector3 start, Vector3 dir){
		#if TM_IS_DEBUG
		DrawRay(start,dir,Color.white,0.0f,true);
		#endif
	}
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration=0f, bool depthTest=true){
		#if TM_IS_DEBUG
		Debug.DrawRay(start,dir,color,duration,depthTest);
		#endif
	}

	//---------------------------------------------------------------------------
	public static void Log(object message){
		#if TM_IS_DEBUG
		Debug.Log(message);
		#endif
	}
	public static void Log(object message,Object context){
		#if TM_IS_DEBUG
		Debug.Log(message,context);
		#endif
	}

	//---------------------------------------------------------------------------
	public static void DrawMouseRay(float dist=1000f,Camera cam=null){
		#if TM_IS_DEBUG
		if(cam == null) cam = Camera.main;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		DrawRay(ray.origin,ray.direction*dist,Input.GetMouseButton(0) ? Color.yellow : Color.gray);
		#endif
	}
	//---------------------------------------------------------------------------
	public static void DrawCircle( Vector3 _pos, Quaternion _rot, float _rad, Color _col, int _div=16){
		#if TM_IS_DEBUG
		Vector3 stt=_pos, end;
		float dd = 360f/(float)_div;
		for(int i=0; i<=_div; ++i){
			Quaternion qt = Quaternion.AngleAxis(dd*(float)i, Vector3.forward);
			end = _pos + _rot*qt*(Vector3.up * _rad);
			if(i>0){ Debug.DrawLine(stt,end,_col); }
			stt=end;
		}
		#endif
	}
}
