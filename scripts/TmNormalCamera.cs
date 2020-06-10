using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib
{
    [RequireComponent(typeof(Camera))]
    public class TmNormalCamera : MonoBehaviour
    {
        [SerializeField] Shader m_screenNormalShader = null;
        Material m_mat;
        // Start is called before the first frame update
        void Start()
        {
            // _CameraDepthNormalsTextureを生成するようにする設定
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
            m_mat = new Material(m_screenNormalShader);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, m_mat);
        }
    }
}
