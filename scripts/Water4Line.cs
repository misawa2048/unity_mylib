using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water4Line : MonoBehaviour {
	[SerializeField]
	MeshFilter water4Tile;
	[SerializeField]
	int materialId;
    [SerializeField]
    bool update = false;

	// Use this for initialization
	void Start () {
        updateParam();
    }

    // Update is called once per frame
    void Update () {
        if (update)
        {
            updateParam();
        }
    }

	void updateParam(){
		if (water4Tile != null) {
			Material water4Material = water4Tile.GetComponent<MeshRenderer> ().material;
			Material mat = GetComponent<MeshRenderer> ().materials [materialId];
            Vector4 ampScale = Vector4.one * water4Tile.transform.lossyScale.y; // (1f, water4Tile.transform.lossyScale.y, 1f, 1f);
			mat.SetFloat ("_SurfaceHeight", water4Tile.transform.position.y);
			mat.SetVector ("_GSteepness", water4Material.GetVector ("_GSteepness"));
			mat.SetVector ("_GAmplitude", Vector4.Scale(water4Material.GetVector ("_GAmplitude"), ampScale));
			mat.SetVector ("_GFrequency", water4Material.GetVector ("_GFrequency"));
			mat.SetVector ("_GSpeed", water4Material.GetVector ("_GSpeed"));
			mat.SetVector ("_GDirectionAB", water4Material.GetVector ("_GDirectionAB"));
			mat.SetVector ("_GDirectionCD", water4Material.GetVector ("_GDirectionCD"));
		}
	}
}
