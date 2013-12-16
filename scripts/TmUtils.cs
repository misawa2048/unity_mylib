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
		Vector3[] vertices = new Vector3[(_width+1)*(_height+1)*2];
		int[] triangles = new int[(((_width+1)*(_height+1)*2)/3+1)*3];
		Vector2[] uv = new Vector2[(_width+1)*(_height+1)*2];
		Color[] colors = new Color[(_width+1)*(_height+1)*2];
		Vector3[] normals = new Vector3[(_width+1)*(_height+1)*2];

		int cnt = 0;
		for(int ix = 0; ix <= _width; ++ix){
			vertices[cnt*2+0] = new Vector3(((float)ix/(float)_width - 0.5f),-0.5f,0.0f);
			vertices[cnt*2+1] = new Vector3(((float)ix/(float)_width - 0.5f), 0.5f,0.0f);
			triangles[cnt*2+0] = cnt*2+0;
			triangles[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		for(int iy = 0; iy <= _height; ++iy){
			vertices[cnt*2+0] = new Vector3(-0.5f,((float)iy/(float)_height - 0.5f),0.0f);
			vertices[cnt*2+1] = new Vector3( 0.5f,((float)iy/(float)_height - 0.5f),0.0f);
			triangles[cnt*2+0] = cnt*2+0;
			triangles[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Lines,0);
		return mesh;
	}
	
	public static Mesh CreateLineCircle(int _vertNum){
		Vector3[] vertices = new Vector3[(_vertNum)];
		int[] triangles = new int[(((_vertNum))/3+1)*3];
		Vector2[] uv = new Vector2[(_vertNum)];
		Color[] colors = new Color[(_vertNum)];
		Vector3[] normals = new Vector3[(_vertNum)];

		int cnt = 0;
		for(int ii = 0; ii < _vertNum; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_vertNum))*0.5f;
			vertices[cnt] = new Vector3(fx,fy,0.0f);
			triangles[cnt] = cnt;
			uv[cnt] = new Vector2(vertices[cnt].x+0.5f,vertices[cnt].y+0.5f);
			colors[cnt] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.LineStrip,0);
		return mesh;
	}
	
	public static Mesh CreatePolyRing(int _vertNum, float _minRad, float _maxRad){
		Vector3[] vertices = new Vector3[(_vertNum+1)*2];
		int[] triangles = new int[(_vertNum+1)*6];
		Vector2[] uv = new Vector2[(_vertNum+1)*2];
		Color[] colors = new Color[(_vertNum+1)*2];
		Vector3[] normals = new Vector3[(_vertNum+1)*2];

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
//			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
//			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			uv[cnt*2+0] = new Vector2((float)ii/(float)_vertNum,0.0f);
			uv[cnt*2+1] = new Vector2((float)ii/(float)_vertNum,1.0f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Triangles,0);
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
	// 現在の_basePosの距離を基準として、画面左下から_scaleRateの位置になるようなworld座標を取得 
	public static Vector3 GetPosOnGUI(Vector3 _basePos, Vector2 _scaleRate, Vector2 _screenPosRate, TextAnchor _ancor = TextAnchor.MiddleCenter){
		Vector3 retPos;
		Plane plane = new Plane(Camera.main.transform.forward,_basePos);
		float dist = plane.GetDistanceToPoint(Camera.main.transform.position);
		Vector3 scrPos = new Vector3(_screenPosRate.x,_screenPosRate.y,-dist);
		scrPos.x *= (float)Screen.width;
		scrPos.y *= (float)Screen.height;
		retPos = Camera.main.ScreenToWorldPoint(scrPos);
		
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
		d = Vector3.Scale(d,_scaleRate);
		retPos += Camera.main.transform.right * d.x;
		retPos += Camera.main.transform.up * d.y;

		return retPos;
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
	
	// (0,0)-(1,1)のRectをワールド系のRectにして返す 
	public static Rect ViewportToWorldRect(Rect _vierportRect, float _dist){
		Vector3 bottomLeftVec = new Vector3(_vierportRect.xMin,_vierportRect.yMin,_dist);
		bottomLeftVec = Camera.main.ViewportToWorldPoint(bottomLeftVec);
		Vector3 upperRightVec = new Vector3(_vierportRect.xMax,_vierportRect.yMax,_dist);
		upperRightVec = Camera.main.ViewportToWorldPoint(upperRightVec);
		Vector3 worldSizeVec = (upperRightVec - bottomLeftVec);
		Rect retRect = new Rect(bottomLeftVec.x,bottomLeftVec.y,worldSizeVec.x,worldSizeVec.y);
		return retRect;
	}
	public static Rect ScreenToWorldRect(Rect _worldRect, float _dist){
		Rect retRect = new Rect(_worldRect);
		retRect.x /= Screen.width;
		retRect.width /= Screen.width;
		retRect.y /= Screen.height;
		retRect.height /= Screen.height;
		return ViewportToWorldRect(retRect,_dist);
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
