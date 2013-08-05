using UnityEngine;
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
		Vector3[] vertices = new Vector3[(_vertNum+1)];
		int[] triangles = new int[(((_vertNum+1))/3+1)*3];
		Vector2[] uv = new Vector2[(_vertNum+1)];
		Color[] colors = new Color[(_vertNum+1)];
		Vector3[] normals = new Vector3[(_vertNum+1)];

		int cnt = 0;
		for(int ii = 0; ii <= _vertNum; ++ii){
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
}
