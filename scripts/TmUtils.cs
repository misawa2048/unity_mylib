using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Xml;
public class TmUtils {
	// ----------------
	// Time関係 
	// ----------------
	public static float TotalHours{ get{ return (float)(System.DateTime.Now.TimeOfDay.TotalHours); } }
	public static float TotalHoursToHourAng(float _hour){ return(_hour * 30.0f); }
	public static float TotalHoursToMinuteAng(float _hour){ return(_hour * 360.0f); }
	public static float TotalHoursToSecondAng(float _hour){ return(_hour * 360.0f * 60.0f); }
	
	public static int AngToHour(float _ang){ return(((int)(_ang/30.0f))%12); }
	public static int AngToMinute(float _ang){ return(((int)(_ang*2.0f))%60); }
	public static int AngToSecond(float _ang){ return(((int)(_ang*120.0f))%60); }
	public static string AngToTimeStr(float _ang){
		return string.Format("{0:00}:{1:00}:{2:00}",AngToHour(_ang),AngToMinute(_ang),AngToSecond(_ang));
	}
	
	public static Quaternion AngToRotation(float _ang, Vector3 _axis, Quaternion _zeroRot){
		Quaternion tmpRot = Quaternion.identity;
		tmpRot.eulerAngles = _axis * _ang;
		return (_zeroRot * tmpRot);
	}
	
	// ----------------
	// Leap関係 
	// ----------------
	// ex. deltaPos *= LeapBreak(dragRate,0.5f,Time.deltaTime);
	public static float LeapBreak(float _oldRate, float _breakRate, float _deltaTime){
		return(Mathf.Clamp01(_oldRate) * Mathf.Pow(_breakRate , _deltaTime));
	}
	
	// ----------------
	// GUI関係 
	// ----------------
	// カメラから、_onPlanePosが含まれる平面までの距離を取得 
	public static float GetDistanceFromCameratoPlane(Camera _cam, Vector3 _onPlanePos){
		Plane plane = new Plane(_cam.transform.forward,_onPlanePos);
		return -plane.GetDistanceToPoint(_cam.transform.position);
	}
	public static float GetDistanceFromCameratoPlane(Vector3 _onPlanePos){
		return GetDistanceFromCameratoPlane(Camera.main, _onPlanePos);
	}
	// カメラからの距離_dist、_viewPosRateの位置になるようなworld座標を取得 
	public static Vector3 GetPosOnGUI(float _dist, Vector2 _viewPosRate){
		return Camera.main.ViewportToWorldPoint(new Vector3(_viewPosRate.x,_viewPosRate.y,_dist));
	}
	public static Vector3 GetPosOnGUI(Vector3 _basePos, Vector2 _viewPosRate){
		return GetPosOnGUI(GetDistanceFromCameratoPlane(_basePos),_viewPosRate);
	}
	// カメラからの距離_dist、_scaleRate	の大きさの矩形が画面左下から_viewPosRateの位置になるようなworld座標を取得 
	public static Vector3 GetPosOnGUI(float _dist, Vector2 _viewPosRate, Vector2 _scaleRate, TextAnchor _ancor = TextAnchor.MiddleCenter){
		Vector2 d=Vector2.zero;
		switch(_ancor){ // 中心原点からの相対 
		case TextAnchor.UpperLeft:    d.x =  0.5f;  d.y = -0.5f;  break;
		case TextAnchor.UpperCenter:  d.x =  0.0f;  d.y = -0.5f;  break;
		case TextAnchor.UpperRight:   d.x = -0.5f;  d.y = -0.5f;  break;
		case TextAnchor.MiddleLeft:   d.x =  0.5f;  d.y =  0.0f;  break;
		case TextAnchor.MiddleCenter: d.x =  0.0f;  d.y =  0.0f;  break;
		case TextAnchor.MiddleRight:  d.x = -0.5f;  d.y =  0.0f;  break;
		case TextAnchor.LowerLeft:    d.x =  0.5f;  d.y =  0.5f;  break;
		case TextAnchor.LowerCenter:  d.x =  0.0f;  d.y =  0.5f;  break;
		case TextAnchor.LowerRight:   d.x = -0.5f;  d.y =  0.5f;  break;
		}
		return GetPosOnGUI(_dist, _viewPosRate+Vector2.Scale(d,_scaleRate));
	}
	public static Vector3 GetPosOnGUI(Vector3 _basePos, Vector2 _viewPosRate, Vector2 _scaleRate, TextAnchor _ancor = TextAnchor.MiddleCenter){
		return GetPosOnGUI(GetDistanceFromCameratoPlane(_basePos),_viewPosRate,_scaleRate,_ancor);
	}
	
	// 現在の_distを基準として, quadObjectが_scaleRateになるスケール  
	public static Vector2 GetScaleOnGUI(float _dist, Vector2 _scaleRate){
		Vector3 p0 = TmUtils.GetPosOnGUI(_dist,Vector2.zero);
		Vector3 p1 = TmUtils.GetPosOnGUI(_dist,Vector2.right*_scaleRate.x);
		Vector3 p2 = TmUtils.GetPosOnGUI(_dist,Vector2.up*_scaleRate.y);
		return new Vector2((p1-p0).magnitude,(p2-p0).magnitude);
	}
	public static Vector2 GetScaleOnGUI(Vector3 _basePos, Vector2 _scaleRate){
		return GetScaleOnGUI(GetDistanceFromCameratoPlane(_basePos), _scaleRate);
	}
	
	// (0,0)-(1,1)のRectをワールド系のRectにして返す 
	public static Rect ViewportToWorldRect(Camera _cam, Rect _vierportRect, float _dist){
		Vector2 wScale = GetScaleOnGUI(_dist,new Vector2(_vierportRect.width,_vierportRect.height));
		Vector3 bottomLeftVec = new Vector3(_vierportRect.xMin,_vierportRect.yMin,_dist);
		bottomLeftVec = _cam.ViewportToWorldPoint(bottomLeftVec);
		return new Rect(bottomLeftVec.x,bottomLeftVec.y,wScale.x,wScale.y);
	}
	public static Rect ViewportToWorldRect(Rect _vierportRect, float _dist){
		return ViewportToWorldRect(Camera.main,_vierportRect,_dist);
	}
	// (0,0)-(1,1)のRectをワールド系のBoundsにして返す 
	public static Bounds ViewportToWorldBounds(Camera _cam, Rect _vierportRect, float _dist){
		Vector3 bottomLeftVec = new Vector3(_vierportRect.xMin,_vierportRect.yMin,_dist);
		bottomLeftVec = _cam.ViewportToWorldPoint(bottomLeftVec);
		Vector3 upperRightVec = new Vector3(_vierportRect.xMax,_vierportRect.yMax,_dist);
		upperRightVec = _cam.ViewportToWorldPoint(upperRightVec);
		Vector3 worldSizeVec = (upperRightVec - bottomLeftVec);
		float zoomRate = Mathf.Sqrt(worldSizeVec.sqrMagnitude/(_vierportRect.width*_vierportRect.width + _vierportRect.height*_vierportRect.height));
		upperRightVec = bottomLeftVec + (_cam.transform.right*_vierportRect.width + _cam.transform.up*_vierportRect.width)*zoomRate;
		Bounds retBounds = new Bounds(bottomLeftVec,upperRightVec);
		return retBounds;
	}
	public static Bounds ViewportToWorldBounds(Rect _vierportRect, float _dist){
		return ViewportToWorldBounds(Camera.main,_vierportRect,_dist);
	}

	public static Rect ScreenToWorldRect(Camera _cam, Rect _screenRect, float _dist){
		Rect retRect = new Rect(_screenRect);
		retRect.x /= Screen.width;
		retRect.width /= Screen.width;
		retRect.y /= Screen.height;
		retRect.height /= Screen.height;
		return ViewportToWorldRect(_cam, retRect,_dist);
	}
	public static Rect ScreenToWorldRect(Rect _screenRect, float _dist){
		return ScreenToWorldRect(Camera.main,_screenRect,_dist);
	}

	// ワールド系の(_worldRect,_dist)を左下(0,0)-(1,1)のRectにして返す 
	public static Rect WorldToViewportRect(Camera _cam, Rect _worldSizeRect, float _dist){
		Rect retRect;
		float worldZ = _dist + _cam.transform.position.z;
		Vector3 worldCenter = new Vector3(_worldSizeRect.center.x,_worldSizeRect.center.y,worldZ);
		Vector3 worldBottomLeft = new Vector3(_worldSizeRect.xMin,_worldSizeRect.yMin,worldZ);
		Vector3 viewportCenter = _cam.WorldToViewportPoint(worldCenter);
		Vector3 viewportBottomLeft = _cam.WorldToViewportPoint(worldBottomLeft);
		Vector3 lengthVec = (viewportCenter - viewportBottomLeft) * 2.0f;
		retRect = new Rect(viewportBottomLeft.x,viewportBottomLeft.y,lengthVec.x,lengthVec.y);
		return retRect;
	}
	public static Rect WorldToViewportRect(Rect _worldSizeRect, float _dist){
		return WorldToViewportRect(Camera.main,_worldSizeRect,_dist);
	}

	public static Rect WorldToScreenRect(Rect _worldSizeRect, float _dist){
		Rect retRect = WorldToViewportRect(_worldSizeRect,_dist);
		retRect.x *= Screen.width;
		retRect.width *= Screen.width;
		retRect.y *= Screen.height;
		retRect.height *= Screen.height;
		return retRect;
	}
	
	//  画面内の最も近い点を取得  
	public static Vector3 NearestPosInScreen(Vector3 _wpos, float _dist=float.MinValue){
		if(_dist==float.MinValue){
			Plane plane = new Plane(Camera.main.transform.forward,_wpos);
			_dist = -plane.GetDistanceToPoint(Camera.main.transform.position);
		}
		Rect wr = ViewportToWorldRect(new Rect(0,0,1,1), _dist);
		Vector2 ret = _wpos;
		ret.x = (_wpos.x < wr.xMin) ? wr.xMin : (_wpos.x > wr.xMax) ? wr.xMax : _wpos.x;
		ret.y = (_wpos.y < wr.yMin) ? wr.yMin : (_wpos.y > wr.yMax) ? wr.yMax : _wpos.y;
		return ret;
	}
	
	// 画面変位 
	public static Vector2 ScreenDisplacement(Vector3 _worldPos){
		Vector2 scr = new Vector2((float)Screen.width,(float)Screen.height) * 0.5f;
		Vector3 sPos = Camera.main.WorldToScreenPoint(_worldPos);
		return new Vector2(Mathf.Abs((scr.x-sPos.x)/scr.x),Mathf.Abs((scr.y-sPos.y)/scr.y));
	}
	// 画面内判定 
	public static bool IsInnerScreen(Vector3 _worldPos, float _rate=1.0f){
		Vector2 displacement = ScreenDisplacement(_worldPos);
		return(Mathf.Max(displacement.x, displacement.y) <= _rate);
	}
	
	// 現在のRectを含む最小のべき乗Rectを取得(左下原点) 
	public static Rect PowerOfTwoRect(Rect _srcRect, TextAnchor _ancor = TextAnchor.LowerLeft){
		Rect retRect = new Rect(_srcRect);
		retRect.width = (int)Mathf.Pow(2.0f,(Mathf.Floor(Mathf.Log((float)(_srcRect.width-1), 2.0f)) + 1.0f));
		retRect.height = (int)Mathf.Pow(2.0f,(Mathf.Floor(Mathf.Log((float)(_srcRect.height-1), 2.0f)) + 1.0f));
		Vector2 d = new Vector2(retRect.width - _srcRect.width,retRect.height - _srcRect.height);
		switch(_ancor){ // 左下原点からの相対 
		case TextAnchor.UpperLeft:    d.x *= 0.0f;  d.y *= 1.0f;  break;
		case TextAnchor.UpperCenter:  d.x *= 0.5f;  d.y *= 1.0f;  break;
		case TextAnchor.UpperRight:   d.x *= 1.0f;  d.y *= 1.0f;  break;
		case TextAnchor.MiddleLeft:   d.x *= 0.0f;  d.y *= 0.5f;  break;
		case TextAnchor.MiddleCenter: d.x *= 0.5f;  d.y *= 0.5f;  break;
		case TextAnchor.MiddleRight:  d.x *= 1.0f;  d.y *= 0.5f;  break;
		case TextAnchor.LowerLeft:    d.x *= 0.0f;  d.y *= 0.0f;  break;
		case TextAnchor.LowerCenter:  d.x *= 0.5f;  d.y *= 0.0f;  break;
		case TextAnchor.LowerRight:   d.x *= 1.0f;  d.y *= 0.0f;  break;
		}
		retRect.x = (int)(retRect.x+d.x);
		retRect.y = (int)(retRect.y+d.y);
		return retRect;
	}
	
	// カメラアスペクトをセット 
	public static float SetAspect(Vector2 _size, Camera _cam=null) {
		if(_cam==null) _cam = Camera.main;
		float tmpAspect = _size.y / _size.x;
		float myAspect = (float)Screen.height / (float)Screen.width;
		float outAspect;
		
		if( tmpAspect > myAspect ){
			outAspect = myAspect / tmpAspect;
			_cam.rect = new Rect( ( 1.0f - outAspect )*0.5f, 0.0f, outAspect, 1.0f );
		}else{
			outAspect = tmpAspect / myAspect;
			_cam.rect = new Rect( 0.0f, ( 1.0f - outAspect )*0.5f, 1.0f, outAspect );
		}
		return outAspect;
	}
	
	// get fit to screen size offset
	// ScreenToWorldRect(,Rect(-retVec.x*0.5f,-retVec.y*0.5f,scrSize.x+retVec.x,scrSize.y+retVec.y),)
	public static Vector2 ToFixRectOfs(float _srcXpY, float _tgtXpY, bool _isInner){
		Vector2 retVec= Vector2.zero;
		if(_isInner){
			if(_srcXpY>_tgtXpY){ retVec.y = -(1f-_tgtXpY/_srcXpY); }
			else               { retVec.x = -(1f-_srcXpY/_tgtXpY); }
		}else{
			if(_srcXpY>_tgtXpY){ retVec.x = (_srcXpY-_tgtXpY)/_tgtXpY; }
			else{                retVec.y = (_tgtXpY-_srcXpY)/_srcXpY; }
		}
		return retVec;
	}
	
	// ----------------
	// uGUI関係 
	// ----------------
	public static bool CheckUGUIHit(Vector3 _scrPos){ // Input.mousePosition
		bool ret = false;
		PointerEventData pointer = new PointerEventData (EventSystem.current);
		pointer.position = _scrPos;
		List<RaycastResult> result = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (pointer, result);
		if (result.Count > 0) {
			ret = true;
		}
		return ret;
	}
	
	// ----------------
	// MathUtil関係 
	// ----------------
	// float[,]のメッシュを再分割/統合 
	public static float[,] DivHeightMapArr(float[,] _heightArr, float _hRate, bool _isInv=false, int _rndSeed=0){
		if(_rndSeed!=0){ Random.seed = _rndSeed; }
		
		float[,] retArr = _heightArr;
		int srcDivX=_heightArr.GetLength(0);
		int srcDivY=_heightArr.GetLength(1);
		if(_isInv){ // 結合 
			if((srcDivX>2)&&(srcDivY>2)){
				retArr = new float[(srcDivX+1)/2,(srcDivY+1)/2];
				for(int iy = 0; iy<((srcDivY+1)/2); ++iy){
					for(int ix = 0; ix<((srcDivX+1)/2); ++ix){
						retArr[ix,iy] = _heightArr[ix*2,iy*2];
					}
				}
			}
		}else{ // 分割 
			retArr = new float[srcDivX*2-1,srcDivY*2-1];
			for(int iy = 0; iy<srcDivY; ++iy){
				for(int ix = 0; ix<srcDivX; ++ix){
					retArr[ix*2,iy*2] = _heightArr[ix,iy];
				}
			}
			for(int iy = 1; iy<retArr.GetLength(1)-1; iy+=2){
				for(int ix = 1; ix<retArr.GetLength(0)-1; ix+=2){
					float f00 = retArr[ix-1,iy-1];
					float f10 = retArr[ix+1,iy-1];
					float f01 = retArr[ix-1,iy+1];
					float f11 = retArr[ix+1,iy+1];
					retArr[ix-1,iy] = ((Random.value-0.5f)*(_hRate/(float)srcDivX))+(f00+f01)*0.5f;
					retArr[ix+1,iy] = ((Random.value-0.5f)*(_hRate/(float)srcDivX))+(f10+f11)*0.5f;
					retArr[ix,iy-1] = ((Random.value-0.5f)*(_hRate/(float)srcDivX))+(f00+f10)*0.5f;
					retArr[ix,iy+1] = ((Random.value-0.5f)*(_hRate/(float)srcDivX))+(f01+f11)*0.5f;
					retArr[ix,iy]   = ((Random.value-0.5f)*(_hRate/(float)srcDivX))+(f00+f10+f01+f11)*0.25f;
				}
			}
		}
		return retArr;
	}

	// ----------------
	// TextAsset関係 
	// ----------------
	// csvファイルを2次元配列に格納(_commentで始まる行はコメント) 
	public static string[,] CsvToMap(TextAsset _csvData, string _comment="//"){
		List<List<string>> dataListArr = new List<List<string>>();
		string[] lines = _csvData.text.Split("\n"[0]);
		int maxLength = 0;
		if(lines.Length>0){
			foreach( string line in lines){
				if((_comment!="")&&(!line.StartsWith(_comment))){
					List<string> dataList = new List<string>();
					string[] datas = line.Split(","[0]);
					maxLength = Mathf.Max(maxLength,datas.Length);
					foreach( string data in datas){
						dataList.Add(data);
					}
					dataListArr.Add(dataList);
				}
			}
		}
		string[,] retMap = new string[maxLength,dataListArr.Count];
		if((dataListArr.Count>0)&&(maxLength>0)){
			for(int y = 0; y < dataListArr.Count; ++y){
				for(int x = 0; x < dataListArr[y].Count; ++x){
					retMap[x,y] = dataListArr[y][x].Replace("\r","");
				}
				for(int x = dataListArr[y].Count; x < maxLength; ++x){
					retMap[x,y] = "";
				}
			}
		}
		return retMap;
	}
	
	// xmlファイルをXmlDocumentに格納 
	public static XmlDocument XmlToDoc(TextAsset _xmlData){
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(_xmlData.text);
		return doc;
	}

}
