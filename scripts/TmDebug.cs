// If you want to use other than 'UNITY_EDITOR',define that word to
// "Project Settings > Player > Other Settings > Scripting Debug Symbol" 
using UnityEngine;

namespace TmLib{
	public class TmDebug {

        //---------------------------------------------------------------------------
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end){
			DrawLine(start,end, Color.white,0.0f,true);
		}
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration=0f, bool depthTest=true){
			Debug.DrawLine(start,end,color,duration,depthTest);
		}

        //---------------------------------------------------------------------------
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir){
			DrawRay(start,dir,Color.white,0.0f,true);
		}
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration=0f, bool depthTest=true){
			Debug.DrawRay(start,dir,color,duration,depthTest);
		}

        //---------------------------------------------------------------------------
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message){
			Debug.Log(message);
		}
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message,Object context){
			Debug.Log(message,context);
		}

        //---------------------------------------------------------------------------
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawMouseRay(float dist=1000f,Camera cam=null){
			if(cam == null) cam = Camera.main;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			DrawRay(ray.origin,ray.direction*dist,Input.GetMouseButton(0) ? Color.yellow : Color.gray);
		}
        //---------------------------------------------------------------------------
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle( Vector3 _pos, Quaternion _rot, float _rad, Color _col, int _div=16){
			Vector3 stt=_pos, end;
			float dd = 360f/(float)_div;
			for(int i=0; i<=_div; ++i){
				Quaternion qt = Quaternion.AngleAxis(dd*(float)i, Vector3.forward);
				end = _pos + _rot*qt*(Vector3.up * _rad);
				if(i>0){ Debug.DrawLine(stt,end,_col); }
				stt=end;
			}
		}
        //---------------------------------------------------------------------------
        private static Vector3 GetCenterOfsRotPos(Vector3 _center, Vector3 _ofs, Quaternion _rot)
        {
            return _center + _rot * _ofs;
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRect(Vector3 _center, Vector2 _rect, Quaternion _rot, Color _col)
        {
            Vector3 p1 = GetCenterOfsRotPos(_center, new Vector3(-_rect.x, 0f, -_rect.y), _rot);
            Vector3 p2 = GetCenterOfsRotPos(_center, new Vector3(-_rect.x, 0f, _rect.y), _rot);
            Vector3 p3 = GetCenterOfsRotPos(_center, new Vector3(_rect.x, 0f, _rect.y), _rot);
            Vector3 p4 = GetCenterOfsRotPos(_center, new Vector3(_rect.x, 0f, -_rect.y), _rot);
            Debug.DrawLine(p1, p2, _col);
            Debug.DrawLine(p2, p3, _col);
            Debug.DrawLine(p3, p4, _col);
            Debug.DrawLine(p4, p1, _col);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircleScale(Vector3 _center, int _divNum, float _sttOfs, float _endOfs, Quaternion _rot, Color _col)
        {
            for(int i=0; i < _divNum; ++i)
            {
                float ang = (float)i / (float)_divNum * 360f;
                Quaternion ofsRot = Quaternion.AngleAxis(ang, Vector3.up);
                Vector3 p1 = GetCenterOfsRotPos(_center, new Vector3(0f, 0f, _sttOfs), _rot* ofsRot);
                Vector3 p2 = GetCenterOfsRotPos(_center, new Vector3(0f, 0f, _endOfs), _rot* ofsRot);
                Debug.DrawLine(p1, p2, _col);
            }
        }
    }
} //namespace TmLib
