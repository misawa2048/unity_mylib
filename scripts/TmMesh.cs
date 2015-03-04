using UnityEngine;
using System.Collections;

public class TmMesh{
	public enum AxisType{ XY,XZ }
	public static Mesh CreateLineGrid(int _width, int _height, AxisType _type){
		return CreateLineGrid(_width, _height, _type, new Color(0.5f,0.5f,0.5f,1.0f), false);
	}
	public static Mesh CreateLineGrid(int _width, int _height, AxisType _type, Color _vertCol, bool _isUnitPerGrid){
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
			if(_isUnitPerGrid){
				vertices[cnt*2+0] = Vector3.Scale(vertices[cnt*2+0],new Vector3(_width,_height,0));
				vertices[cnt*2+1] = Vector3.Scale(vertices[cnt*2+1],new Vector3(_width,_height,0));
			}
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
			if(_isUnitPerGrid){
				vertices[cnt*2+0] = Vector3.Scale(vertices[cnt*2+0],new Vector3(_width,_height,0));
				vertices[cnt*2+1] = Vector3.Scale(vertices[cnt*2+1],new Vector3(_width,_height,0));
			}
			incides[cnt*2+0] = cnt*2+0;
			incides[cnt*2+1] = cnt*2+1;
			uv[cnt*2+0] = new Vector2(vertices[cnt*2+0].x+0.5f,vertices[cnt*2+0].y+0.5f);
			uv[cnt*2+1] = new Vector2(vertices[cnt*2+1].x+0.5f,vertices[cnt*2+1].y+0.5f);
			colors[cnt*2+0] = colors[cnt*2+1] = new Color(0.5f,0.5f,0.5f,1.0f);
			normals[cnt*2+0] = normals[cnt*2+1] = new Vector3(0.0f,0.0f,-1.0f);
			tangents[cnt*2+0] = tangents[cnt*2+1] = new Vector4(1.0f,0.0f,0.0f,0.0f);
			cnt++;
		}
		if (_type == AxisType.XZ) {
			for(int ii = 0; ii < vertices.Length; ++ii){
				vertices[ii] = new Vector3(vertices[ii].x,vertices[ii].z,vertices[ii].y);
			}
			for(int ii = 0; ii < normals.Length; ++ii){
				normals[ii] = new Vector3(0.0f,1.0f,0.0f);
			}
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
	
	public static Mesh CreateLineGridXY(int _width, int _height){
		return CreateLineGridXY(_width, _height, new Color(0.5f,0.5f,0.5f,1.0f), false);
	}
	public static Mesh CreateLineGridXY(int _width, int _height, Color _vertCol, bool _isUnitPerGrid){
		return CreateLineGrid(_width, _height, AxisType.XY, new Color(0.5f,0.5f,0.5f,1.0f), false);
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
		return CreateTileMesh(_divX, _divY, new Color(0.5f,0.5f,0.5f,1.0f),false);
	}
	public static Mesh CreateTileMesh(int _divX, int _divY, Color _vertCol, bool _isUnitPerGrid){
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
				if(_isUnitPerGrid){
					vertices[yy*(_divX+1)+xx] = Vector3.Scale(vertices[yy*(_divX+1)+xx],new Vector3(_divX,_divY,0));
				}
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

	public static Mesh CreateHeightMesh(float[,] _heightArr){
		return CreateHeightMesh(_heightArr, new Color(0.5f,0.5f,0.5f,1.0f),false);
	}
	public static Mesh CreateHeightMesh(float[,] _heightArr, Color _vertCol, bool _isUnitPerGrid){
		int _divX = _heightArr.GetLength(0)-1;
		int _divZ = _heightArr.GetLength(1)-1;
		
		int vertNum = (_divX+1)*(_divZ+1);
		int quadNum = _divX*_divZ;
		int[] triangles = new int[quadNum*6];
		Vector3[] vertices = new Vector3[vertNum];
		Vector2[] uv = new Vector2[vertNum];
		Color[] colors = new Color[vertNum];
		Vector3[] normals = new Vector3[vertNum];
		Vector4[] tangents = new Vector4[vertNum];
		
		for(int zz = 0; zz < (_divZ+1); ++zz){
			for(int xx = 0; xx < (_divX+1); ++xx){
				float height = _heightArr[xx,zz];
				Vector2 uvPos = new Vector2((float)xx/(float)_divX,(float)zz/(float)_divZ);
				vertices[zz*(_divX+1)+xx] = new Vector3(uvPos.x-0.5f,height,uvPos.y-0.5f);
				if(_isUnitPerGrid){
					vertices[zz*(_divX+1)+xx] = Vector3.Scale(vertices[zz*(_divX+1)+xx],new Vector3(_divX,1f,_divZ));
				}
				uv[zz*(_divX+1)+xx] = uvPos;
				colors[zz*(_divX+1)+xx] = _vertCol;
				normals[zz*(_divX+1)+xx] = new Vector3(0.0f,1.0f,0.0f);
				tangents[zz*(_divX+1)+xx] = new Vector4(1.0f,0.0f,0.0f);
				if((xx<_divX)&&(zz<_divZ)){
					int[] sw={0,0,1,1,1,0,1,1,0,0,0,1};
					for(int ii = 0; ii < 6; ++ii){
						triangles[(zz*_divX+xx)*6+ii] = (zz+sw[ii*2+1])*(_divX+1)+(xx+sw[ii*2+0]);
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
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Triangles,0);
		return mesh;
	}

	public static Mesh CreateTubeMesh(int _divX, int _divZ, float _bottomR=0.5f, float _topR=0.5f){
		return CreateTubeMesh(_divX, _divZ, _bottomR, _topR, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateTubeMesh(int _divX, int _divZ, float _bottomR, float _topR, Color _vertCol){
		int vertNum = (_divX+1)*(_divZ+1);
		int quadNum = _divX*_divZ;
		int[] triangles = new int[quadNum*6];
		Vector3[] vertices = new Vector3[vertNum];
		Vector2[] uv = new Vector2[vertNum];
		Color[] colors = new Color[vertNum];
		Vector3[] normals = new Vector3[vertNum];
		Vector4[] tangents = new Vector4[vertNum];
		
		for(int zz = 0; zz < (_divZ+1); ++zz){
			for(int xx = 0; xx < (_divX+1); ++xx){
				Vector2 uvPos = new Vector2((float)xx/(float)_divX,(float)zz/(float)_divZ);
				vertices[zz*(_divX+1)+xx] = new Vector3(uvPos.x-0.5f,0.0f,uvPos.y-0.5f);
				uv[zz*(_divX+1)+xx] = uvPos;
				colors[zz*(_divX+1)+xx] = _vertCol;
				normals[zz*(_divX+1)+xx] = new Vector3(0.0f,0.0f,-1.0f);
				tangents[zz*(_divX+1)+xx] = new Vector4(1.0f,0.0f,0.0f);
				{
					float p = vertices[zz*(_divX+1)+xx].x*2.0f;
					float r = (vertices[zz*(_divX+1)+xx].z+0.5f);
					r = _bottomR + (_topR-_bottomR)*r;
					vertices[zz*(_divX+1)+xx].x = Mathf.Cos (p * Mathf.PI)*r;
					vertices[zz*(_divX+1)+xx].y = Mathf.Sin (p * Mathf.PI)*r;
					normals[zz*(_divX+1)+xx] = vertices[zz*(_divX+1)+xx].normalized;
				}
				if((xx<_divX)&&(zz<_divZ)){
					int[] sw={0,0,1,1,1,0,1,1,0,0,0,1};
					for(int ii = 0; ii < 6; ++ii){
						triangles[(zz*_divX+xx)*6+ii] = (zz+sw[ii*2+1])*(_divX+1)+(xx+sw[ii*2+0]);
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
	
	public static Mesh CreatePoly(int _vertNum, float _ofsDeg=0.0f){
		return CreatePoly(_vertNum, new Color(0.5f,0.5f,0.5f,1.0f),_ofsDeg,0.0f);
	}
	public static Mesh CreatePoly(int _vertNum, Color _color, float _ofsDeg=0.0f, float _starRate=0.0f){
		if(_starRate!=0.0f) _vertNum *= 2;
		float ofsDegRate = _ofsDeg/360.0f;
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
			float fx = Mathf.Cos(Mathf.PI*2.0f * (((float)ii / (float)_vertNum)+ofsDegRate))*rr;
			float fy = Mathf.Sin(Mathf.PI*2.0f * (((float)ii / (float)_vertNum)+ofsDegRate))*rr;
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
	
	public static Mesh CreateTriStrip(Vector3[] _verts){
		return CreateTriStrip(_verts, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateTriStrip(Vector3[] _verts, Color _color){
		if (_verts.Length < 3) {
			return null;
		}
		int _vertNum = _verts.Length;
		Vector3[] verts = _verts;
		Vector2[] uvs = new Vector2[_vertNum];
		Vector3[] norms = new Vector3[_vertNum];
		Vector4[] tgts = new Vector4[_vertNum];
		Color[] cols = new Color[_vertNum];
		
		for(int ii=0; ii< _vertNum; ++ii){
			float tu = (float)(ii/2)/(float)((_vertNum/2)-1);
			float tv = (float)(ii&1);
			uvs[ii]= new Vector2(tu,tv);
			cols[ii] = _color;
			norms[ii]= new Vector3(0.0f,0.0f,-1.0f);
			tgts[ii]= new Vector4(1f,0f,0f,0f);
		}
		
		int[] tris = new int[(_vertNum-2)*3];
		for(int ii=0; ii< _vertNum-2; ++ii){
			if((ii&1)==0){
				tris[ii*3+0] = ii+0;
				tris[ii*3+1] = ii+1;
				tris[ii*3+2] = ii+2;
			}else{
				tris[ii*3+0] = ii+2;
				tris[ii*3+1] = ii+1;
				tris[ii*3+2] = ii+0;
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.colors = cols;
		mesh.normals = norms;
		mesh.tangents = tgts;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		return mesh;
	}
	
	public static Mesh CreateSphere(int _divH, int _divV){
		return CreateSphere(_divH, _divV, new Color(0.5f,0.5f,0.5f,1.0f));
	}
	public static Mesh CreateSphere(int _divH, int _divV, Color _color){
		Mesh mesh = new Mesh();
		
		float radius = 0.5f;
		// Longitude |||
		int nbLong = _divH;
		// Latitude ---
		int nbLat = _divV;
		
		#region Vertices
		Vector3[] vertices = new Vector3[(nbLong+1) * nbLat + 2];
		int _vertNum = vertices.Length;
		Color[] cols = new Color[_vertNum];
		float _pi = Mathf.PI;
		float _2pi = _pi * 2f;
		
		vertices[0] = Vector3.up * radius;
		for( int lat = 0; lat < nbLat; lat++ )
		{
			float a1 = _pi * (float)(lat+1) / (nbLat+1);
			float sin1 = Mathf.Sin(a1);
			float cos1 = Mathf.Cos(a1);
			
			for( int lon = 0; lon <= nbLong; lon++ )
			{
				float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
				float sin2 = Mathf.Sin(a2+_pi);
				float cos2 = Mathf.Cos(a2+_pi);
				
				vertices[ lon + lat * (nbLong + 1) + 1] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * radius;
			}
		}
		vertices[vertices.Length-1] = Vector3.up * -radius;
		#endregion
		
		#region Normales		
		Vector3[] normales = new Vector3[vertices.Length];
		for( int n = 0; n < vertices.Length; n++ ){
			normales[n] = vertices[n].normalized;
			cols[n] = _color;
		}
		#endregion
		
		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		uvs[0] = Vector2.up;
		uvs[uvs.Length-1] = Vector2.zero;
		for( int lat = 0; lat < nbLat; lat++ )
			for( int lon = 0; lon <= nbLong; lon++ )
				uvs[lon + lat * (nbLong + 1) + 1] = new Vector2( (float)lon / nbLong, 1f - (float)(lat+1) / (nbLat+1) );
		#endregion
		
		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[ nbIndexes ];
		
		//Top Cap
		int i = 0;
		for( int lon = 0; lon < nbLong; lon++ )
		{
			triangles[i++] = lon+2;
			triangles[i++] = lon+1;
			triangles[i++] = 0;
		}
		
		//Middle
		for( int lat = 0; lat < nbLat - 1; lat++ )
		{
			for( int lon = 0; lon < nbLong; lon++ )
			{
				int current = lon + lat * (nbLong + 1) + 1;
				int next = current + nbLong + 1;
				
				triangles[i++] = current;
				triangles[i++] = current + 1;
				triangles[i++] = next + 1;
				
				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = next;
			}
		}
		
		//Bottom Cap
		for( int lon = 0; lon < nbLong; lon++ )
		{
			triangles[i++] = vertices.Length - 1;
			triangles[i++] = vertices.Length - (lon+2) - 1;
			triangles[i++] = vertices.Length - (lon+1) - 1;
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.tangents = CalcTangents(mesh);;
		
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		return mesh;
	}
	
	// http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
	public static Vector4[] CalcTangents(Mesh _mesh){
		long vertexCount = _mesh.vertexCount;
		long triangleCount = _mesh.triangles.Length;
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		Vector4[] tangents = new Vector4[vertexCount];
		for(long a = 0; a< triangleCount; a+=3)
		{
			long i1 = _mesh.triangles[a+0];
			long i2 = _mesh.triangles[a+1];
			long i3 = _mesh.triangles[a+2];
			
			Vector3 v1 = _mesh.vertices[i1];
			Vector3 v2 = _mesh.vertices[i2];
			Vector3 v3 = _mesh.vertices[i3];
			
			Vector2 w1 = _mesh.uv[i1];
			Vector2 w2 = _mesh.uv[i2];
			Vector2 w3 = _mesh.uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = _mesh.normals[a];
			Vector3 t = tan1[a];
			
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		return tangents;
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
