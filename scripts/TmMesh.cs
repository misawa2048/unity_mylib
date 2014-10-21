using UnityEngine;
using System.Collections;

public class TmMesh{
	public static Mesh CreateLineGridXY(int _width, int _height){
		return CreateLineGridXY(_width, _height, new Color(0.5f,0.5f,0.5f,1.0f), false);
	}
	public static Mesh CreateLineGridXY(int _width, int _height, Color _vertCol, bool _isUnitPerGrid){
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
