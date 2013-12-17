using UnityEngine;
using System.Collections;

public class Tm9Slice : MonoBehaviour {
	private const int SLICE_NUM_X = 3;
	private const int SLICE_NUM_Y = 3;
	public Vector2 worldEdgeSize = Vector2.one * 0.25f;
	public Vector2 maxEdgeRate = Vector2.one * 0.5f;
	private Vector2 mEdgeRate;
	private Mesh mMesh;

	// Use this for initialization
	void Start () {
		mMesh = TmUtils.CreateTileMesh(SLICE_NUM_X,SLICE_NUM_Y,Color.white);
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = mMesh;
	}
	
	// Update is called once per frame
	void Update () {
		mEdgeRate.x = Mathf.Min(worldEdgeSize.x / transform.lossyScale.x , maxEdgeRate.x);
		mEdgeRate.y = Mathf.Min(worldEdgeSize.y / transform.lossyScale.y , maxEdgeRate.y);

		Vector3[] vertices = mMesh.vertices;
		for(int yy = 0; yy < (SLICE_NUM_Y+1); ++yy){
			for(int xx = 0; xx < (SLICE_NUM_X+1); ++xx){
				if(((xx==0)||(xx==SLICE_NUM_X))&&((yy==0)||(yy==SLICE_NUM_Y))) continue;
				if(xx==1){
					vertices[yy*(SLICE_NUM_X+1)+xx].x = mEdgeRate.x - 0.5f;
				}else if(xx==SLICE_NUM_X-1){
					vertices[yy*(SLICE_NUM_X+1)+xx].x = (1.0f-mEdgeRate.x) - 0.5f;
				}
				if(yy==1){
					vertices[yy*(SLICE_NUM_X+1)+xx].y = mEdgeRate.y - 0.5f;
				}else if(yy==SLICE_NUM_Y-1){
					vertices[yy*(SLICE_NUM_X+1)+xx].y = (1.0f-mEdgeRate.y) - 0.5f;
				}
			}
		}
		mMesh.vertices = vertices;
	}

}
