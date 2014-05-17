using UnityEngine;
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
	// Graphic関係
	// ----------------
	public static Mesh CreateGridXY(int _width, int _height){
		return CreateGridXY(_width, _height, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateGridXY(int _width, int _height, Color _vertCol){
		Vector3[] vertices = new Vector3[(_width+1)*(_height+1)*2];
		int[] incides = new int[(_width+1)*(_height+1)*2];
		Vector2[] uv = new Vector2[(_width+1)*(_height+1)*2];
		Color[] colors = new Color[(_width+1)*(_height+1)*2];
		Vector3[] normals = new Vector3[(_width+1)*(_height+1)*2];
		Vector4[] tangents = new Vector4[(_width+1)*(_height+1)*2];
		
		int cnt = 0;
		for(int ix = 0; ix <= _width; ++ix){
			vertices[cnt*2+0] = new Vector3(((float)ix/(float)_width - 0.5f),-0.5f,0.0f);
			vertices[cnt*2+1] = new Vector3(((float)ix/(float)_width - 0.5f), 0.5f,0.0f);
			incides[cnt*2+0] = cnt*2+0;
			incides[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			normals[cnt*2+0] = normals[cnt*2+1] = new Vector3(0.0f,0.0f,-1.0f);
			tangents[cnt*2+0] = tangents[cnt*2+1] = new Vector4(1.0f,0.0f,0.0f,0.0f);
			cnt++;
		}
		for(int iy = 0; iy <= _height; ++iy){
			vertices[cnt*2+0] = new Vector3(-0.5f,((float)iy/(float)_height - 0.5f),0.0f);
			vertices[cnt*2+1] = new Vector3( 0.5f,((float)iy/(float)_height - 0.5f),0.0f);
			incides[cnt*2+0] = cnt*2+0;
			incides[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			normals[cnt*2+0] = normals[cnt*2+1] = new Vector3(0.0f,0.0f,-1.0f);
			tangents[cnt*2+0] = tangents[cnt*2+1] = new Vector4(1.0f,0.0f,0.0f,0.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.SetIndices(incides,MeshTopology.Lines,0);
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.tangents = tangents;
		//		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
	
	public static Mesh CreateLine(Vector3[] _vertices, bool _isRing){
		return CreateLine(_vertices, _isRing, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateLine(Vector3[] _vertices, bool _isRing, Color _color){
		int vertNum = _vertices.Length;
		int[] incides = new int[_isRing?(vertNum*2):(vertNum-1)*2];
		Vector2[] uv = new Vector2[vertNum];
		Color[] colors = new Color[vertNum];
		Vector3[] normals = new Vector3[vertNum];
		
		for(int ii = 0; ii < vertNum; ++ii){
			if(ii<(vertNum-1)){
				incides[ii*2+0] = ii;
				incides[ii*2+1] = ii+1;
			}
			uv[ii] = new Vector2(_vertices[ii].x+0.5f,_vertices[ii].y+0.5f);
			colors[ii] = _color;
			normals[ii] = new Vector3(0.0f,1.0f,0.0f);
		}
		if(_isRing){
			incides[(vertNum-1)*2+0] = (vertNum-1);
			incides[(vertNum-1)*2+1] = 0;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = _vertices;
		mesh.SetIndices(incides,MeshTopology.Lines,0);
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		//		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
	
	public static Mesh CreateLineCircle(int _vertNum){
		return CreateLineCircle(_vertNum, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateLineCircle(int _vertNum, Color _color){
		Vector3[] vertices = new Vector3[_vertNum];
		for(int ii = 0; ii < _vertNum; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			vertices[ii] = new Vector3(fx,fy,0.0f);
		}
		return CreateLine(vertices,true,_color);
	}
	
	public static Mesh CreateTileMesh(int _divX, int _divY){
		return CreateTileMesh(_divX, _divY, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateTileMesh(int _divX, int _divY, Color _vertCol){
		int vertNum = (_divX+1)*(_divY+1);
		int quadNum = _divX*_divY;
		int[] triangles = new int[quadNum*6];
		Vector3[] vertices = new Vector3[vertNum];
		Vector2[] uv = new Vector2[vertNum];
		Color[] colors = new Color[vertNum];
		Vector3[] normals = new Vector3[vertNum];
		Vector4[] tangents = new Vector4[vertNum];
		
		for(int yy = 0; yy < (_divY+1); ++yy){
			for(int xx = 0; xx < (_divX+1); ++xx){
				Vector2 uvPos = new Vector2((float)xx/(float)_divX,(float)yy/(float)_divY);
				vertices[yy*(_divX+1)+xx] = new Vector3(uvPos.x-0.5f,uvPos.y-0.5f,0.0f);
				uv[yy*(_divX+1)+xx] = uvPos;
				colors[yy*(_divX+1)+xx] = _vertCol;
				normals[yy*(_divX+1)+xx] = new Vector3(0.0f,0.0f,-1.0f);
				tangents[yy*(_divX+1)+xx] = new Vector4(1.0f,0.0f,0.0f);
				if((xx<_divX)&&(yy<_divY)){
					int[] sw={0,0,1,1,1,0,1,1,0,0,0,1};
					for(int ii = 0; ii < 6; ++ii){
						triangles[(yy*_divX+xx)*6+ii] = (yy+sw[ii*2+1])*(_divX+1)+(xx+sw[ii*2+0]);
					}
				}
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.tangents = tangents;
		//		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Triangles,0);
		return mesh;
	}
	
	public static Mesh CreatePoly(int _vertNum, float _starRate=0.0f){
		return CreatePoly(_vertNum, new Color(0.5f,0.5f,0.5f,1.0f),_starRate);
	}
	public static Mesh CreatePoly(int _vertNum, Color _color, float _starRate=0.0f){
		if(_starRate!=0.0f) _vertNum *= 2;
		Vector3[] verts = new Vector3[_vertNum+1];
		Vector2[] uvs = new Vector2[_vertNum+1];
		Vector3[] norms = new Vector3[_vertNum+1];
		Vector4[] tgts = new Vector4[_vertNum+1];
		Color[] cols = new Color[_vertNum+1];
		verts[0]= new Vector3(0.0f,0.0f,0.0f);
		uvs[0]= new Vector2(0.5f,0.5f);
		norms[0]= new Vector3(0.0f,0.0f,-1.0f);
		tgts[0]= new Vector4(1.0f,0.0f,0.0f,0.0f);
		cols[0] = _color;
		for(int ii=0; ii< _vertNum; ++ii){
			float rr = 0.5f;
			if((_starRate!=0.0f)&&((ii&1)==1)){
				rr *= (1.0f-_starRate);
			}
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*rr;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*rr;
			verts[ii+1]= new Vector3(fx,fy,0.0f);
			uvs[ii+1]= new Vector2(fx+0.5f,fy+0.5f);
			cols[ii+1] = _color;
			norms[ii+1]= new Vector3(0.0f,0.0f,-1.0f);
			tgts[ii+1]= new Vector4(fx,fy,0.0f,0.0f);
		}
		
		int[] tris = new int[_vertNum*3];
		for(int ii=0; ii< _vertNum; ++ii){
			tris[ii*3+0] = 0;
			tris[ii*3+1] = (ii<(_vertNum-1)) ? (ii+2) : 1;
			tris[ii*3+2] = ii+1;
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.colors = cols;
		mesh.normals = norms;
		mesh.tangents = tgts;
		//		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
	
	public static Mesh CreatePolyRing(int _vertNum, float _minRad, float _maxRad){
		return CreatePolyRing(_vertNum, _minRad, _maxRad, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreatePolyRing(int _vertNum, float _minRad, float _maxRad, Color _color){
		Vector3[] vertices = new Vector3[(_vertNum+1)*2];
		int[] triangles = new int[(_vertNum+1)*6];
		Vector2[] uv = new Vector2[(_vertNum+1)*2];
		Color[] colors = new Color[(_vertNum+1)*2];
		Vector3[] normals = new Vector3[(_vertNum+1)*2];
		Vector4[] tangents = new Vector4[(_vertNum+1)*2];
		
		int cnt = 0;
		for(int ii = 0; ii <= _vertNum; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_vertNum));
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_vertNum));
			vertices[cnt*2+0] = new Vector3(fx*_maxRad*0.5f,fy*_maxRad*0.5f,0.0f);
			vertices[cnt*2+1] = new Vector3(fx*_minRad*0.5f,fy*_minRad*0.5f,0.0f);
			triangles[cnt*6+0] = cnt*2+0;
			triangles[cnt*6+1] = cnt*2+1;
			triangles[cnt*6+2] = (ii<(_vertNum)) ? (cnt*2+2) : 0;
			triangles[cnt*6+3] = cnt*2+0;
			triangles[cnt*6+4] = (ii>0) ? (cnt*2-1) : 0;
			triangles[cnt*6+5] = cnt*2+1;
			uv[cnt*2+0] = new Vector2((float)ii/(float)_vertNum,0.0f);
			uv[cnt*2+1] = new Vector2((float)ii/(float)_vertNum,1.0f);
			colors[cnt*2+0] = colors[cnt*2+1] = _color;
			normals[cnt*2+0] = normals[cnt*2+1] = new Vector3(0.0f,0.0f,-1.0f);
			tangents[cnt*2+0] = tangents[cnt*2+1] = new Vector4(1.0f,0.0f,0.0f,0.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		//		mesh.triangles = triangles;
		mesh.SetIndices(triangles,MeshTopology.Triangles,0);
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
	
	public static Mesh SetMeshColor(Mesh _nowMesh, Color _col){
		if(_nowMesh!=null){
			Color[] cols = new Color[_nowMesh.vertexCount];
			for(int ii = 0; ii < _nowMesh.vertexCount; ++ii){
				cols[ii] = _col;
			}
			_nowMesh.colors = cols;
		}
		return _nowMesh;
	}
	
	// ----------------
	// GUI関係 
	// ----------------
	// カメラから、_onPlanePosが含まれる平面までの距離を取得 
	public static float GetDistanceFromCameratoPlane(Vector3 _onPlanePos){
		Plane plane = new Plane(Camera.main.transform.forward,_onPlanePos);
		return -plane.GetDistanceToPoint(Camera.main.transform.position);
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
	public static Rect ViewportToWorldRect(Rect _vierportRect, float _dist){
		Vector2 wScale = GetScaleOnGUI(_dist,new Vector2(_vierportRect.width,_vierportRect.height));
		Vector3 bottomLeftVec = new Vector3(_vierportRect.xMin,_vierportRect.yMin,_dist);
		bottomLeftVec = Camera.main.ViewportToWorldPoint(bottomLeftVec);
		return new Rect(bottomLeftVec.x,bottomLeftVec.y,wScale.x,wScale.y);
	}
	// (0,0)-(1,1)のRectをワールド系のBoundsにして返す 
	public static Bounds ViewportToWorldBounds(Rect _vierportRect, float _dist){
		Vector3 bottomLeftVec = new Vector3(_vierportRect.xMin,_vierportRect.yMin,_dist);
		bottomLeftVec = Camera.main.ViewportToWorldPoint(bottomLeftVec);
		Vector3 upperRightVec = new Vector3(_vierportRect.xMax,_vierportRect.yMax,_dist);
		upperRightVec = Camera.main.ViewportToWorldPoint(upperRightVec);
		Vector3 worldSizeVec = (upperRightVec - bottomLeftVec);
		float zoomRate = Mathf.Sqrt(worldSizeVec.sqrMagnitude/(_vierportRect.width*_vierportRect.width + _vierportRect.height*_vierportRect.height));
		upperRightVec = bottomLeftVec + (Camera.main.transform.right*_vierportRect.width + Camera.main.transform.up*_vierportRect.width)*zoomRate;
		Bounds retBounds = new Bounds(bottomLeftVec,upperRightVec);
		return retBounds;
	}
	
	public static Rect ScreenToWorldRect(Rect _screenRect, float _dist){
		Rect retRect = new Rect(_screenRect);
		retRect.x /= Screen.width;
		retRect.width /= Screen.width;
		retRect.y /= Screen.height;
		retRect.height /= Screen.height;
		return ViewportToWorldRect(retRect,_dist);
	}
	
	// ワールド系の(_worldRect,_dist)を左下(0,0)-(1,1)のRectにして返す 
	public static Rect WorldToViewportRect(Rect _worldSizeRect, float _dist){
		Rect retRect;
		float worldZ = _dist + Camera.main.transform.position.z;
		Vector3 worldCenter = new Vector3(_worldSizeRect.center.x,_worldSizeRect.center.y,worldZ);
		Vector3 worldBottomLeft = new Vector3(_worldSizeRect.xMin,_worldSizeRect.yMin,worldZ);
		Vector3 viewportCenter = Camera.main.WorldToViewportPoint(worldCenter);
		Vector3 viewportBottomLeft = Camera.main.WorldToViewportPoint(worldBottomLeft);
		Vector3 lengthVec = (viewportCenter - viewportBottomLeft) * 2.0f;
		retRect = new Rect(viewportBottomLeft.x,viewportBottomLeft.y,lengthVec.x,lengthVec.y);
		return retRect;
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
	public static float setAspect(Vector2 _size, Camera _cam=null) {
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
