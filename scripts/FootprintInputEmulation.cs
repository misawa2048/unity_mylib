using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib{
    public class FootprintInputEmulation : MonoBehaviour
    {
        [System.Serializable]
        public class FootprintInfo{
            public bool enabled;
            public int id;
            public Vector2 viewPos;
            public GameObject debugObj;
            public FootprintInfo(bool _enabled, int _id,Vector2 _viewPos, GameObject _debugObj){
                enabled = _enabled;
                id = _id;
                viewPos = _viewPos;
                debugObj = _debugObj;
            }
        }

        [System.Serializable]
        public struct TouchInfo{
            public int id;
            public Vector2 viewPos;
            public TouchInfo(int _id, Vector2 _viewPos){
                id = _id;
                viewPos = _viewPos;
            }
        }

        [SerializeField,Range(1,10)] int maxFootprintNum;
        [SerializeField] Camera targetCamera;
        [SerializeField] Rect innerRect = new Rect(0f,0f,1f,1f);
        [SerializeField, Range(0.005f, 0.2f)] float stepLength = 0.05f;
        [SerializeField, Range(0f, 1f)] float stepRandom = 0.1f;
        [SerializeField] GameObject dbgFootprintPrefab;
        [SerializeField, Range(1, 1000)] float dbgDispDistance = 10f;
        [SerializeField] private List<TouchInfo> _touchInfoList;
        private List<FootprintInfo> footprintInfoList;
        private int ringPtr;
        private Vector3 oldVPos;

        public TouchInfo[] touches { get { return _touchInfoList.ToArray(); }}

        // Use this for initialization
        void Start()
        {
            footprintInfoList = new List<FootprintInfo>();
            _touchInfoList = new List<TouchInfo>();
            for (int i = 0; i < maxFootprintNum; ++i){
                GameObject obj=null;
                if(dbgFootprintPrefab!=null){
                    obj = Instantiate(dbgFootprintPrefab, transform);
                    obj.name = "footprint_" + i.ToString();
                    obj.transform.localScale *= dbgDispDistance;
                }
                footprintInfoList.Add(new FootprintInfo(false, i, new Vector2(), obj));
            }
            ringPtr = maxFootprintNum-1;
            oldVPos = Vector2.one * -1f;
        }

        // Update is called once per frame
        void Update()
        {
            if(targetCamera!=null){
                Vector3 vPos = targetCamera.ScreenToViewportPoint(Input.mousePosition);
                bool isMoved = false;
                if ((oldVPos - vPos).magnitude > stepLength){
                    oldVPos = vPos;
                    isMoved = true;
                }else if(Random.value<Time.deltaTime*stepRandom*100f){
                    isMoved = true;
                    if(Random.value < 0.7f){
                        vPos = Vector2.one * -1f;
                    }else{
                        vPos += Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector3.right * stepLength;
                    }
                }
                if(isMoved){
                    bool _enabled = true;
                    ringPtr = (ringPtr + 1) % maxFootprintNum;
                    int footId = ringPtr;
                    FootprintInfo info = footprintInfoList[footId];
                    if (innerRect.Contains(vPos)){
                        vPos.z = dbgDispDistance;
                        info.viewPos = new Vector2(vPos.x, vPos.y);
                        if (info.debugObj != null){
                            Vector3 wPos = targetCamera.ViewportToWorldPoint(vPos);
                            info.debugObj.transform.position = wPos;
                            info.debugObj.transform.rotation = targetCamera.transform.rotation;
                        }
                    }else{
                        _enabled = false;
                    }
                    info.enabled = _enabled;
                    if (info.debugObj != null)
                    {
                        info.debugObj.SetActive(_enabled);
                    }
                }

                _touchInfoList.Clear();
                for (int i = 0; i < maxFootprintNum; ++i)
                {
                    if(footprintInfoList[i].enabled){
                        _touchInfoList.Add(new TouchInfo(i,footprintInfoList[i].viewPos));
                    }
                }

            }
        }
    }
}
