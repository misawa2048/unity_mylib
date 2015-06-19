using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

namespace TmLib{
	public class TmFileUtil {
		//toJson
		public static string AnimCurveToJson(AnimationCurve _curve){
			string ret="";
			Dictionary<string,object> dic = new Dictionary<string, object> ();
			string[] keyStr = new string[_curve.length];
			for (int i =0; i< _curve.length; ++i) {
				keyStr[i] = KeyframeToJson(_curve.keys[i]);
			}
			dic.Add ("keys", keyStr);
			dic.Add ("postWrapMode", (int)_curve.postWrapMode);
			dic.Add ("preWrapMode", (int)_curve.preWrapMode);
			ret = Json.Serialize (dic);
			return ret;
		}
		public static string KeyframeToJson(Keyframe _key){
			string ret = "";
			Dictionary<string,object> dic = new Dictionary<string, object> ();
			dic.Add ("inTangent", _key.inTangent);
			dic.Add ("outTangent", _key.outTangent);
			dic.Add ("tangentMode", _key.tangentMode);
			dic.Add ("time", _key.time);
			dic.Add ("value", _key.value);
			ret = Json.Serialize (dic);
			return ret;
		}
		
		// fromJson
		public static Keyframe JsonToKeyframe(string _jsonStr){
			Keyframe key = new Keyframe ();
			Dictionary<string,object> dic;
			dic = Json.Deserialize (_jsonStr) as Dictionary<string,object>;
			key.inTangent = objToFloat(dic["inTangent"]);
			key.outTangent = objToFloat(dic["outTangent"]);
			key.tangentMode = objToInt(dic["tangentMode"]);
			key.time = objToFloat(dic["time"]);
			key.value = objToFloat(dic["value"]);
			return key;
		}
		public static AnimationCurve JsonToAnimCurve(string _jsonStr){
			Dictionary<string,object> dic;
			dic = Json.Deserialize (_jsonStr) as Dictionary<string,object>;
			List<object> keyList = (List<object>)dic["keys"];
			Keyframe[] keys = new Keyframe[keyList.Count];
			for (int i = 0; i < keyList.Count; ++i) {
				keys[i] = JsonToKeyframe((string)keyList[i]);
			}
			AnimationCurve curve = new AnimationCurve (keys);
			curve.postWrapMode = (WrapMode)objToInt(dic ["postWrapMode"]);
			curve.preWrapMode = (WrapMode)objToInt(dic ["preWrapMode"]);
			return curve;
		}
		
		//Parse(for MiniJSON bug)
		public static float objToFloat(object _obj) {
			float ret=0f;
			if (_obj != null) {
				float.TryParse (_obj.ToString (), out ret);
			}
			return ret;
		}
		public static int objToInt(object _obj) {
			int ret=0;
			if (_obj != null) {
				int.TryParse (_obj.ToString (), out ret);
			}
			return ret;
		}
	}
}

