using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CutFaceObjCtrl : MonoBehaviour
{
    [SerializeField] Transform m_targetPlaneTr=null;
    Material[] m_mat=null; //ターゲット(カード)のマテリアル
    public Material[] cardMatArr { get { return m_mat; } }

    private void Awake()
    {
        findMaterial();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (m_targetPlaneTr == null)
        {
            m_targetPlaneTr = transform.Find("CardFront");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_mat == null)
        {
            findMaterial();
        }
        if ((m_targetPlaneTr != null)&& (m_mat != null))
        {
            Plane plane = new Plane(m_targetPlaneTr.forward, m_targetPlaneTr.position);
            Vector4 vec4 = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
            foreach(Material mat in m_mat)
            {
                mat.SetVector("_Section", vec4);
            }
        }
    }

    void findMaterial()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            m_mat = mr.sharedMaterials;
        }
        else
        {
            if (transform.childCount > 0)
            {
                mr = transform.GetChild(0).GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    m_mat = mr.sharedMaterials;
                }
            }
        }
        if (m_mat == null)
        {
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                m_mat = smr.sharedMaterials;
            }
            else
            {
                if (transform.childCount > 0)
                {
                    smr = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                    if (mr != null)
                    {
                        m_mat = smr.sharedMaterials;
                    }
                }
            }
        }
    }
}
