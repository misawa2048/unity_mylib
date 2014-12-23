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
}
