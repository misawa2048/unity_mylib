#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreatePanel : MonoBehaviour
{
	const string DEF_NAME = "CustomPanelXZ";
	
	[MenuItem ("GameObject/Create Other/Other/"+DEF_NAME)]
	static void Create ()
	{
		GameObject newGameobject = new GameObject (DEF_NAME);
		
		MeshRenderer meshRenderer = newGameobject.AddComponent<MeshRenderer> ();
		meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		
		MeshFilter meshFilter = newGameobject.AddComponent<MeshFilter> ();
		meshFilter.mesh = CreateTileMesh(1,1,Color.gray);
		Mesh mesh = meshFilter.sharedMesh;
		mesh.name = DEF_NAME;
		
		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
		AssetDatabase.SaveAssets ();
	}
	
	public static Mesh CreateTileMesh(int _divX, int _divY, Color _vertCol){
		int vertNum = (_divX+1)*(_divY+1);
		int quadNum = _divX*_divY;
		int[] triangles = new int[quadNum*6];
		Vector3[] vertices = new Vector3[vertNum];
		Vector2[] uv = new Vector2[vertNum];
		Color[] colors = new Color[vertNum];
		Vector3[] normals = new Vector3[vertNum];
		
		for(int yy = 0; yy < (_divY+1); ++yy){
			for(int xx = 0; xx < (_divX+1); ++xx){
				Vector2 uvPos = new Vector2((float)xx/(float)_divX,(float)yy/(float)_divY);
				vertices[yy*(_divX+1)+xx] = new Vector3(uvPos.x-0.5f,uvPos.y-0.5f,0.0f);
				uv[yy*(_divX+1)+xx] = uvPos;
				colors[yy*(_divX+1)+xx] = _vertCol;
			}
		}
		for(int yy = 0; yy < _divY; ++yy){
			for(int xx = 0; xx < _divX; ++xx){
				triangles[(yy*_divX+xx)*6+0] = (yy+0)*(_divX+1)+(xx+0);
				triangles[(yy*_divX+xx)*6+1] = (yy+1)*(_divX+1)+(xx+1);
				triangles[(yy*_divX+xx)*6+2] = (yy+0)*(_divX+1)+(xx+1);
				triangles[(yy*_divX+xx)*6+3] = (yy+0)*(_divX+1)+(xx+0);
				triangles[(yy*_divX+xx)*6+4] = (yy+1)*(_divX+1)+(xx+0);
				triangles[(yy*_divX+xx)*6+5] = (yy+1)*(_divX+1)+(xx+1);
			}
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
	
}
#endif
