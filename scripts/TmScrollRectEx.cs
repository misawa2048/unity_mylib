using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

namespace TmLib
{
    public class TmScrollRectEx : ScrollRect
    {
        public UnityEvent OnExBeginDragEvent;
        public UnityEvent OnExEndDragEvent;

        //bool isInButton;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            OnExBeginDragEvent.Invoke();
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            OnExEndDragEvent.Invoke();
        }
    }
}
