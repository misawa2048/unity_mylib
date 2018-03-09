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
        public struct PlayerInfo{
            public int playerId;
            public Vector2 viewPos;
            public PlayerInfo(int _playerId, Vector2 _viewPos){
                playerId = _playerId;
                viewPos = _viewPos;
            }
        }

        [System.Serializable]
        public struct TouchInfo{
            public int playerId;
            public int touchId;
            public Vector2 viewPos;
            public TouchInfo(int _playerId, int _touchId, Vector2 _viewPos){
                playerId = _playerId;
                touchId = _touchId;
                viewPos = _viewPos;
            }
        }

        [SerializeField,Range(1,10)] int maxFootprintNum;
        [SerializeField] Camera m_targetCamera;
        [SerializeField] Rect innerRect = new Rect(0f,0f,1f,1f);
        [SerializeField, Range(0.005f, 0.2f)] float stepLength = 0.05f;
        [SerializeField, Range(0f, 1f)] float stepRandom = 0.1f;
        [SerializeField] GameObject dbgFootprintPrefab;
        [SerializeField, Range(1, 1000)] float m_dbgDispDistance = 10f;
        [SerializeField] List<PlayerInfo> m_playerInfoList;
        [SerializeField] List<TouchInfo> m_touchInfoList;
        private List<FootprintInfo> footprintInfoList;
        private int ringPtr;
        private Vector3 oldVPos;

        public PlayerInfo[] players { get { return m_playerInfoList.ToArray(); } }
        public TouchInfo[] touches { get { return m_touchInfoList.ToArray(); }}
        public float dbgDispDistance { get { return m_dbgDispDistance; }}
        public Camera targetCamera { get { return m_targetCamera; } }

        // Use this for initialization
        void Start()
        {
            footprintInfoList = new List<FootprintInfo>();
            m_playerInfoList = new List<PlayerInfo>();
            m_touchInfoList = new List<TouchInfo>();
            GameObject footPrintBase = new GameObject("FootprintBase");
            footPrintBase.transform.SetParent(m_targetCamera.transform);
            footPrintBase.transform.localPosition = Vector3.zero;
            footPrintBase.transform.localRotation = Quaternion.identity;
            for (int i = 0; i < maxFootprintNum; ++i){
                GameObject obj=null;
                if(dbgFootprintPrefab!=null){
                    obj = Instantiate(dbgFootprintPrefab,footPrintBase.transform);
                    obj.name = "footprint_" + i.ToString();
                    obj.transform.localScale *= m_dbgDispDistance;
                }
                footprintInfoList.Add(new FootprintInfo(false, i, new Vector2(), obj));
            }
            ringPtr = maxFootprintNum-1;
            oldVPos = Vector2.one * -1f;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_targetCamera!=null){
                Vector3 vPos = m_targetCamera.ScreenToViewportPoint(Input.mousePosition);
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
                            Vector3 wPos = m_targetCamera.ViewportToWorldPoint(vPos);
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

                m_touchInfoList.Clear();
                for (int i = 0; i < maxFootprintNum; ++i)
                {
                    if(footprintInfoList[i].enabled){
                        m_touchInfoList.Add(new TouchInfo(Mathf.FloorToInt(i / 2), i,footprintInfoList[i].viewPos));
                    }
                }
                m_playerInfoList.Clear();
                Vector2[] tmpViewPos = new Vector2[Mathf.FloorToInt((m_touchInfoList.Count+1) / 2)];
                int[] tmpViewPosCnt = new int[Mathf.FloorToInt((m_touchInfoList.Count+1) / 2)];
                for (int i = 0; i < m_touchInfoList.Count; ++i)
                {
                    int plId = Mathf.FloorToInt(i / 2);
                    if(tmpViewPos[plId]==null){
                        tmpViewPos[plId] = Vector2.zero;
                    }
                    tmpViewPos[plId]+=m_touchInfoList[i].viewPos;
                    tmpViewPosCnt[plId]++;
                }
                for (int i = 0; i < tmpViewPos.Length;++i){
                    m_playerInfoList.Add(new PlayerInfo(i,tmpViewPos[i]/(float)tmpViewPosCnt[i]));
                }
            }
        }
    }
}
