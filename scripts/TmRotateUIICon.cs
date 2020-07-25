using UnityEngine;
using System.Collections;
//using System.IO;
//using System.Runtime.InteropServices;

namespace TmLib
{
    public class TmRotateUIICon : MonoBehaviour
    {
        [SerializeField] Transform[] rotateTrArr = null;
        [SerializeField] DeviceOrientation m_dbgOverride = DeviceOrientation.Unknown;
        private DeviceOrientation mOri = DeviceOrientation.Unknown;
        private Vector3 mRotVec;

        // Use this for initialization
        void Start()
        {
            mOri = Input.deviceOrientation;
            mRotVec = getRotVec(mOri);
            foreach (Transform tr in rotateTrArr)
            {
                tr.localRotation = Quaternion.Euler(mRotVec);
            }
        }

        // Update is called once per frame
        void Update()
        {
            DeviceOrientation ori = Input.deviceOrientation;
            if (m_dbgOverride != DeviceOrientation.Unknown)
            {
                ori = m_dbgOverride;
            }
            if (ori != mOri)
            {
                mOri = ori;
                mRotVec = getRotVec(mOri);
            }
            foreach (Transform tr in rotateTrArr)
            {
                //			tr.Rotate(Vector3.forward*Time.deltaTime*100.0f);
                tr.localRotation = Quaternion.Lerp(tr.localRotation, Quaternion.Euler(mRotVec), 0.2f);
            }
        }

        private Vector3 getRotVec(DeviceOrientation _ori)
        {
            Vector3 rotVec = Vector3.forward;
            if (_ori == DeviceOrientation.Portrait)
            {
                rotVec = Vector3.forward * 0.0f;
            }
            if (_ori == DeviceOrientation.PortraitUpsideDown)
            {
                rotVec = Vector3.forward * 180.0f;
            }
            if (_ori == DeviceOrientation.LandscapeLeft)
            {
                rotVec = Vector3.forward * 270.0f;
            }
            if (_ori == DeviceOrientation.LandscapeRight)
            {
                rotVec = Vector3.forward * 90.0f;
            }
            return rotVec;
        }
    }
}
