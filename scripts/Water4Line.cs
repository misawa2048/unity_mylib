using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water4Line : MonoBehaviour {
	[SerializeField]
	MeshFilter water4Tile;
	[SerializeField]
	int materialId;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		updateParam ();
		
	}

	void updateParam(){
		if (water4Tile != null) {
			Material water4Material = water4Tile.GetComponent<MeshRenderer> ().material;
			Material mat = GetComponent<MeshRenderer> ().materials [materialId];
			mat.SetFloat ("_SurfaceHeight", water4Tile.transform.position.y);
			mat.SetVector ("_GSteepness", water4Material.GetVector ("_GSteepness"));
			mat.SetVector ("_GAmplitude", water4Material.GetVector ("_GAmplitude"));
			mat.SetVector ("_GFrequency", water4Material.GetVector ("_GFrequency"));
			mat.SetVector ("_GSpeed", water4Material.GetVector ("_GSpeed"));
			mat.SetVector ("_GDirectionAB", water4Material.GetVector ("_GDirectionAB"));
			mat.SetVector ("_GDirectionCD", water4Material.GetVector ("_GDirectionCD"));
		}
	}
}
