using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterUtils{
    public class Wave4Line : MonoBehaviour
    {
        [SerializeField]
        MeshRenderer targetMr; // set Water MeshRendrer here (Tile)

        // Use this for initialization
        void Start()
        {
            Material srcMat = targetMr.materials[0];
            Material tgtMat = GetComponent<MeshRenderer>().materials[1];
            tgtMat.SetVector("_GAmplitude", srcMat.GetVector("_GAmplitude"));
            tgtMat.SetVector("_GFrequency", srcMat.GetVector("_GFrequency"));
            tgtMat.SetVector("_GSpeed", srcMat.GetVector("_GSpeed"));
            tgtMat.SetVector("_GDirectionAB", srcMat.GetVector("_GDirectionAB"));
            tgtMat.SetVector("_GDirectionCD", srcMat.GetVector("_GDirectionCD"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
