using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace TmLib 
{
    // use with TmUIEx.TmScrollRectEx/TmScrollRectExEditor
    public class TmScrollPlaneCtrl : MonoBehaviour
    {
        public enum SwipeDirecttion { Horizontal, Vertical };
        [System.Serializable] public enum SwipeDirection { none = 0, dec, inc };
        //[System.Serializable] public class ScrollCtrlInfo { public bool raycast = false; };

        [SerializeField] SwipeDirecttion m_dir = SwipeDirecttion.Horizontal;
        [SerializeField, ReadOnlyWhenPlaying] int m_selectedIdx = 0;
        [SerializeField,ReadOnly] CanvasScaler m_canvasScaler = null;
        [SerializeField, ReadOnly] ScrollRect m_scrollRect = null;
        [SerializeField, ReadOnly] Image m_dragImage = null;
        [SerializeField,Range(0.05f,0.5f)] float m_scrollThreshold = 0.1f;
        [SerializeField] bool[] m_ignoreRaycastArr=null;
        [SerializeField, ReadOnly] Transform[] m_pageTrArr;
        int m_nextIdx;
        bool m_isEasing;
        Vector2 m_targetPos;

        // Start is called before the first frame update
        void Start()
        {
            m_canvasScaler = GetComponentInParent<CanvasScaler>();
            m_scrollRect = GetComponent<ScrollRect>();
            m_dragImage = GetComponent<Image>();
            m_nextIdx = m_selectedIdx;
            m_isEasing = false;
            m_targetPos = Vector2.zero;
            m_scrollRect.movementType = ScrollRect.MovementType.Clamped;
            m_pageTrArr = new Transform[transform.childCount];
            for(int i=0;i< m_pageTrArr.Length; ++i)
            {
                m_pageTrArr[i] = transform.GetChild(i);
            }

        }

        // Update is called once per frame
        void Update()
        {
            m_scrollRect.horizontal = (m_dir == SwipeDirecttion.Horizontal);
            m_scrollRect.vertical = !(m_dir == SwipeDirecttion.Horizontal);

            RectTransform rt = transform as RectTransform;
            //Debug.Log((float)Screen.width/rt.lossyScale.x+","+(float)Screen.height/rt.lossyScale.y);
            //m_dragImage.raycastTarget = !m_isEasing;
            if (m_isEasing)
            {
                rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, m_targetPos, 0.2f);
                if ((rt.anchoredPosition - m_targetPos).magnitude < 5f)
                {
                    m_isEasing = false;
                    if(m_targetPos.magnitude>0f)
                        m_targetPos = Vector2.zero;
                    rt.anchoredPosition = m_targetPos;
                    m_selectedIdx = m_nextIdx;

                    setDragImageCondition(m_selectedIdx);

                    relocateBySelectedIdx(m_selectedIdx);
                }
            }
            else
            {
                relocateBySelectedIdx(m_selectedIdx);
            }
        }

        public void OnValueChanged(Vector2 _value)
        {
            RectTransform rt = transform as RectTransform;

            if(m_dir == SwipeDirecttion.Horizontal)
            {
                if (m_selectedIdx == 0 && rt.anchoredPosition.x > 0f)
                    rt.anchoredPosition = Vector2.zero;
                if (m_selectedIdx == (m_pageTrArr.Length - 1) && rt.anchoredPosition.x < 0f)
                    rt.anchoredPosition = Vector2.zero;
            }
            else
            {
                if (m_selectedIdx == 0 && rt.anchoredPosition.y < 0f)
                    rt.anchoredPosition = Vector2.zero;
                if (m_selectedIdx == (m_pageTrArr.Length - 1) && rt.anchoredPosition.y > 0f)
                    rt.anchoredPosition = Vector2.zero;
            }
            //Debug.Log("OnValueChanged"+ rt.anchoredPosition.ToString());
        }
        public void OnBeginDrag()
        {
            m_isEasing = false;
            m_scrollRect.movementType = ScrollRect.MovementType.Elastic;
            //Debug.Log("OnDragStart");
        }
        public void OnEndDrag()
        {
            m_isEasing = true;
            m_scrollRect.movementType = ScrollRect.MovementType.Clamped;

            RectTransform rt = transform as RectTransform;
            float rate= getScaleRateByDir(m_dir);
            if (m_dir == SwipeDirecttion.Horizontal)
            {
                if (rate > m_scrollThreshold)
                    OnSwipe(SwipeDirection.dec);
                else if (rate < -m_scrollThreshold)
                    OnSwipe(SwipeDirection.inc);
            }
            else
            {
                if (rate > m_scrollThreshold)
                    OnSwipe(SwipeDirection.inc);
                else if (rate < -m_scrollThreshold)
                    OnSwipe(SwipeDirection.dec);
            }
            //Debug.Log("OnDragEnd:Th="+ m_scrollThreshold+":rate="+rate);
        }

        public void OnSwipe(SwipeDirection _dir)
        {
            if (_dir == SwipeDirection.none)
                return;

            RectTransform rt = transform as RectTransform;
            float scale = getScaleByDir(m_dir);
            m_nextIdx = Mathf.Clamp(m_nextIdx + (_dir == SwipeDirection.dec ? -1 : 1), 0, m_pageTrArr.Length - 1);
            if (m_dir == SwipeDirecttion.Horizontal)
            {
                m_targetPos = new Vector2(scale * (_dir == SwipeDirection.dec ? 1f : -1f), 0f);
            }
            else
            {
                m_targetPos = new Vector2(0f, scale * (_dir == SwipeDirection.dec ? -1f : 1f));
            }
            m_isEasing = true;
            m_scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }
        public void OnSwipeDec()
        {
            OnSwipe(SwipeDirection.dec);
        }
        public void OnSwipeInc()
        {
            OnSwipe(SwipeDirection.inc);
        }
        public void OnSwipeLeft() { OnSwipeDec(); }
        public void OnSwipeRight() { OnSwipeInc(); }

        public void OnStartButton()
        {
            // load next scene
            int myIdx = SceneManager.GetActiveScene().buildIndex;
            if (SceneManager.sceneCountInBuildSettings > myIdx + 1)
            {
                SceneManager.LoadScene(myIdx + 1);
            }
        }

        void relocateBySelectedIdx(int _selIdx)
        {
            RectTransform rt = transform as RectTransform;
            float scale = getScaleByDir(m_dir);
            for (int i=0;i< m_pageTrArr.Length; ++i)
            {
                RectTransform childRt = m_pageTrArr[i] as RectTransform;
                float pos = (float)(i - _selIdx) * scale;
                if (m_dir == SwipeDirecttion.Horizontal)
                    childRt.anchoredPosition = new Vector2(pos,0f);
                else
                    childRt.anchoredPosition = new Vector2(0f, -pos);
            }
        }

        void setDragImageCondition(int _selecterIdx)
        {
            if (m_ignoreRaycastArr != null && m_ignoreRaycastArr.Length > _selecterIdx)
            {
                m_dragImage.raycastTarget = !m_ignoreRaycastArr[_selecterIdx];
            }
            else
            {
                m_dragImage.raycastTarget = true;
            }
        }

        float getScaleByDir(SwipeDirecttion _dir)
        {
            RectTransform rt = transform as RectTransform;
            return (_dir == SwipeDirecttion.Horizontal) ? (float)Screen.width / rt.lossyScale.x : (float)Screen.height / rt.lossyScale.y;
        }
        float getScaleRateByDir(SwipeDirecttion _dir)
        {
            RectTransform rt = transform as RectTransform;
            return ((_dir == SwipeDirecttion.Horizontal) ? rt.anchoredPosition.x : rt.anchoredPosition.y) / getScaleByDir(_dir);
        }
    }
}
