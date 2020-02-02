using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Xml;

namespace TmLib{
	public class TmUtils {
		// ----------------
		// Time関係 
		// ----------------
		public static float TotalHours{ get{ return (float)(System.DateTime.Now.TimeOfDay.TotalHours); } }
		public static float TotalHoursToHourAng(float _hour){ return(_hour * -30.0f); }
		public static float TotalHoursToMinuteAng(float _hour){ return(_hour * -360.0f); }
		public static float TotalHoursToSecondAng(float _hour){ return(_hour * -360.0f * 60.0f); }

		public static float DateTimeToHourAng(System.DateTime _dt){ return TotalHoursToHourAng ((float)_dt.TimeOfDay.TotalHours); }
		public static float DateTimeToMinuteAng(System.DateTime _dt){ return TotalHoursToMinuteAng((float)_dt.TimeOfDay.TotalHours); }
		public static float DateTimeToSecondAng(System.DateTime _dt){ return TotalHoursToSecondAng((float)_dt.TimeOfDay.TotalHours); }

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
        // カメラ関係 
        // ----------------
        // カメラプロジェクションの比率を変更(ex:SideySide=(0.5f,1f))
        public static void SetCameraProjection(Camera _cam, Vector2 _scale)
        {
            _cam.ResetProjectionMatrix();
            Matrix4x4 mat = _cam.projectionMatrix;
            mat.m00 *= _scale.x;
            mat.m11 *= _scale.y;
            _cam.projectionMatrix = mat;
        }

        // カメラ消失点オフセット
        static public bool SetVanishingPoint(Vector2 perspectiveOffset, Camera cam)
        {
            if (cam != null)
            {
                cam.ResetProjectionMatrix();
                Matrix4x4 m = cam.projectionMatrix;
                float w = 2f * cam.nearClipPlane / m.m00;
                float h = 2f * cam.nearClipPlane / m.m11;
                float left = -w / 2f - perspectiveOffset.x;
                float right = left + w;
                float bottom = -h / 2f - perspectiveOffset.y;
                float top = bottom + h;
                cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
            }
            return (cam != null);
        }

		// カメラから距離distの平面における画面内サイズ
        public static Rect GetViewRectInPlane(Camera _cam, float _dist){
            float w=0f,h=0f;
            if(!_cam.orthographic){
                Vector3[] frustumCorners = new Vector3[4];
                _cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), -_dist, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
                h = (frustumCorners[1]-frustumCorners[0]).magnitude*0.5f;
                w = (frustumCorners[2]-frustumCorners[1]).magnitude*0.5f;
            }else{
                h = _cam.orthographicSize;
                float wph = ((float)(Screen.width)/(float)(Screen.height));
                w = h*wph;
            }
            return new Rect(0f,0f,w,h);
        }
        public static Rect GetViewRectInPlane(Camera _cam, Vector3 _poiOnPlane){
            Plane zPl = new Plane(_cam.transform.forward,_poiOnPlane);
            float distToZpl = -zPl.GetDistanceToPoint(_cam.transform.position);
            return GetViewRectInPlane(_cam, distToZpl);
        }

        // perspectiveCenterOffset
        static public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float x = (2f * near) / (right - left);
            float y = (2f * near) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2f * far * near) / (far - near);
            float e = -1f;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x; m[0, 1] = 0f; m[0, 2] = a; m[0, 3] = 0f;
            m[1, 0] = 0f; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0f;
            m[2, 0] = 0f; m[2, 1] = 0f; m[2, 2] = c; m[2, 3] = d;
            m[3, 0] = 0f; m[3, 1] = 0f; m[3, 2] = e; m[3, 3] = 0f;
            return m;
        }

        // _rotType = 0:0[deg] 1:90[deg] 2:180[deg] 3:270[deg]
        static public Texture2D RenderTexToTex2D(RenderTexture _renderTex, int _rotType = 0)
        {
            Texture2D retTex = null;
            Texture2D defTex = new Texture2D(_renderTex.width, _renderTex.height, TextureFormat.ARGB32, false);
            RenderTexture.active = _renderTex;
            defTex.ReadPixels(new Rect(0, 0, _renderTex.width, _renderTex.height), 0, 0);
            defTex.Apply();

            switch (_rotType){
            default:
                retTex = defTex;
                break;
            case 1:
            case 2:
            case 3:
                if (_rotType == 2)
                    retTex = new Texture2D(defTex.width, defTex.height, TextureFormat.ARGB32, false);
                else
                    retTex = new Texture2D(defTex.height, defTex.width, TextureFormat.ARGB32, false);

                for (int iy = 0; iy < defTex.height; ++iy){
                    for (int ix = 0; ix < defTex.width; ++ix){
                        int ry = (_rotType == 2) ? defTex.height - iy : ((_rotType == 1) ? defTex.width - ix : ix);
                        int rx = (_rotType == 2) ? defTex.width - ix : ((_rotType == 1) ? iy : defTex.height - iy);
                        retTex.SetPixel(rx, ry, defTex.GetPixel(ix, iy));
                    }
                }
                retTex.Apply();
                break;
            }
            return retTex;
        }

        // _rotType = 0:0[deg] 1:90[deg] 2:180[deg] 3:270[deg]
        static public Texture2D WebCamTexToTex2D(WebCamTexture _webCamTex, int _rotType = 0)
        {
            Texture2D retTex = null;
            Texture2D defTex = new Texture2D(_webCamTex.width, _webCamTex.height, TextureFormat.ARGB32, false);
            Color[] cols = _webCamTex.GetPixels();
            defTex.SetPixels(cols);
            defTex.Apply();

            switch (_rotType)
            {
                default:
                    retTex = defTex;
                    break;
                case 1:
                case 2:
                case 3:
                    if (_rotType == 2)
                        retTex = new Texture2D(defTex.width, defTex.height, TextureFormat.ARGB32, false);
                    else
                        retTex = new Texture2D(defTex.height, defTex.width, TextureFormat.ARGB32, false);

                    for (int iy = 0; iy < defTex.height; ++iy)
                    {
                        for (int ix = 0; ix < defTex.width; ++ix)
                        {
                            int ry = (_rotType == 2) ? defTex.height - iy : ((_rotType == 1) ? defTex.width - ix : ix);
                            int rx = (_rotType == 2) ? defTex.width - ix : ((_rotType == 1) ? iy : defTex.height - iy);
                            retTex.SetPixel(rx, ry, defTex.GetPixel(ix, iy));
                        }
                    }
                    retTex.Apply();
                    break;
            }
            return retTex;
        }

        // UVにスケール＆オフセットをかけたあとのUV 
		public static Vector2 GetTexturUvByParam(Vector2 _uv, Vector2 _scl, Vector2 _ofs){
			Vector2 destUv = Vector2.Scale(_uv,_scl);
			destUv.x = Mathf.Repeat(_ofs.x + destUv.x,1f);
			destUv.y = Mathf.Repeat(_ofs.y + destUv.y,1f);
			return destUv;
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
		public static bool IsUGUIHit(Vector3 _scrPos){ // Input.mousePosition
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
		public static float[,] DivHeightMapArr(float[,] _heightArr, float _hRate, bool _isUnite=false, int _rndSeed=0){
			if(_rndSeed!=0){ Random.InitState(_rndSeed); }
			
			float[,] retArr = _heightArr;
			int srcDivX=_heightArr.GetLength(0);
			int srcDivY=_heightArr.GetLength(1);
			if(_isUnite){ // 結合 
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
						retArr[ix-1,iy] = (retArr[ix-1,iy-1]+retArr[ix-1,iy+1])*0.5f;
						retArr[ix+1,iy] = (retArr[ix+1,iy-1]+retArr[ix+1,iy+1])*0.5f;
						retArr[ix,iy-1] = (retArr[ix-1,iy-1]+retArr[ix+1,iy-1])*0.5f;
						retArr[ix,iy+1] = (retArr[ix-1,iy+1]+retArr[ix+1,iy+1])*0.5f;
						retArr[ix,iy]   = (retArr[ix-1,iy-1]+retArr[ix+1,iy-1]+retArr[ix-1,iy+1]+retArr[ix+1,iy+1])*0.25f;
						if((ix>1)&&(iy>1)&&(ix<retArr.GetLength(0)-2)&&(iy<retArr.GetLength(1)-2)){
							retArr[ix-1,iy] += ((Random.value-0.5f)*(_hRate/(float)srcDivX));
							retArr[ix+1,iy] += ((Random.value-0.5f)*(_hRate/(float)srcDivX));
							retArr[ix,iy-1] += ((Random.value-0.5f)*(_hRate/(float)srcDivX));
							retArr[ix,iy+1] += ((Random.value-0.5f)*(_hRate/(float)srcDivX));
							retArr[ix,iy]   += ((Random.value-0.5f)*(_hRate/(float)srcDivX));
						}
					}
				}
			}
			return retArr;
		}
		
		// float[,]のメッシュ height 
		public static float GetMapHeight(float[,] _heightArr, Vector2 _pos){
			int w = _heightArr.GetLength (0);
			int h = _heightArr.GetLength (1);
			Vector2 rPos = new Vector2(_pos.x*(float)(w-1),_pos.y*(float)(h-1));
			rPos *= 0.99999f;
			Vector2 dPos = new Vector2 (1f-(rPos.x - Mathf.Floor (rPos.x)), 1f-(rPos.y - Mathf.Floor (rPos.y)));
			int bx = (int)Mathf.Clamp(Mathf.Floor (rPos.x),0f,(float)(w-1));
			int by = (int)Mathf.Clamp(Mathf.Floor (rPos.y),0f,(float)(h-1));
			float retHeight = _heightArr [bx, by] * (dPos.x * dPos.y);
			retHeight += _heightArr [bx+1, by] * ((1f-dPos.x) * dPos.y);
			retHeight += _heightArr [bx, by+1] * (dPos.x * (1f-dPos.y));
			retHeight += _heightArr [bx+1, by+1] * ((1f-dPos.x) * (1f-dPos.y));
			return retHeight;
		}
		
		// ----------------
		// TextAsset関係 
		// ----------------
		// csvファイルを2次元配列に格納(_commentで始まる行はコメント) 
		public static string[,] CsvToMap(TextAsset _csvData, string _comment="//", int _maxLines=int.MaxValue){
			List<List<string>> dataListArr = new List<List<string>>();
			string[] lines = _csvData.text.Split("\n"[0]);
			int maxLength = 0;
			if(lines.Length>0){
				int lineCnt=0;
				foreach( string line in lines){
					if((_comment!="")&&(!line.StartsWith(_comment))){
						List<string> dataList = new List<string>();
						string[] datas = line.Split(","[0]);
						maxLength = Mathf.Max(maxLength,datas.Length);
						foreach( string data in datas){
							dataList.Add(data);
						}
						dataListArr.Add(dataList);
						lineCnt++;
						if(lineCnt>=_maxLines){
							break;
						}
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

		// Get frequency from MIDI note
		public static float GetFrequencyFromMidiNote(int _note){
			return 440f * Mathf.Pow(2f,((float)_note-69f)/12f);
		}
		
	}
} //namespace TmLib
