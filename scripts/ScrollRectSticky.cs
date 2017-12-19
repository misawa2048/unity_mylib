using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

// Need [ScrollRectStickyEditor] for using events
// set GridLayoutGroup.cellSize to Canvas reference resoludion
public class ScrollRectSticky : ScrollRect
{
	[System.Serializable]
	public class SwipeEndEvt {
		public PointerEventData pointerEventData;
		public int targetPage;
		public GameObject targetObj;
		public Vector2 velocity;
		public bool pageChanged;
		public SwipeEndEvt(PointerEventData _pointerEventData, int _targetPage, GameObject _targetObj,Vector2 _velocity, bool _pageChanged){
			pointerEventData = _pointerEventData;
			targetPage = _targetPage;
			targetObj = _targetObj;
			velocity = _velocity;
			pageChanged = _pageChanged;
		}
	}

	[System.Serializable] public class StickyDragEvent : UnityEvent<PointerEventData>{}
	[System.Serializable] public class StickyDragEndEvent : UnityEvent<SwipeEndEvt>{}

	public StickyDragEvent beginDragEvent;
	public StickyDragEvent dragEvent;
	public StickyDragEndEvent endDragEvent;
	public StickyDragEndEvent pageChangeEvent;

	protected int m_sttPage;
	protected int m_tgtPage;
	public int targetPage { get { return m_tgtPage; } }
	protected bool m_dragging;
	protected float m_numF { get { return (float)(content.childCount - 1); } }
	protected Vector2 cellSize { get { return content.GetComponent<GridLayoutGroup>().cellSize; } }

	protected override void Awake()
	{
		base.Awake();
		if (viewport == null) {
			viewport = (transform.Find("Viewport") as RectTransform);
		}
		if (content == null) {
			content = (viewport.Find("Content") as RectTransform);
		}
		movementType = MovementType.Clamped;

		m_dragging = false;
		m_sttPage = m_tgtPage = 0;
	}
	protected override void Start(){
		base.Start ();
	}
	protected override void LateUpdate(){
		base.LateUpdate();
		if (!m_dragging) {
			stickToPage (m_tgtPage);
		}
	}

	public override void OnBeginDrag(PointerEventData _event)
	{
		base.OnBeginDrag(_event);
		m_dragging = true;
		m_sttPage = (int)(Mathf.Floor (0.5f + this.normalizedPosition.x * m_numF));
		if (beginDragEvent!=null) {
			beginDragEvent.Invoke (_event);
		}
	}

	public override void OnDrag(PointerEventData _event)
	{
		base.OnDrag(_event);
		m_dragging = true;
		this.normalizedPosition = getNormalisedPos(this.normalizedPosition,m_sttPage);
		if (dragEvent!=null) {
			dragEvent.Invoke (_event);
		}
	}

	public override void OnEndDrag(PointerEventData _event)
	{
		base.OnEndDrag(_event);
		m_dragging = false;
		float v0 = this.velocity.x;
		float s = -v0 * v0 / 2f * this.decelerationRate * Mathf.Sign(v0);
		s /= cellSize.x * m_numF; // normalize
		s = s *Time.deltaTime / scrollSensitivity;
		Vector2 nmlPos = this.normalizedPosition;
		nmlPos.x = Mathf.Clamp01 (nmlPos.x + s);
		m_tgtPage = getTargetPage (nmlPos, m_sttPage);

		GameObject tgtObj = content.GetChild (m_tgtPage).gameObject;
		bool pageChanged = (m_tgtPage != m_sttPage);
		if (endDragEvent!=null) {
			SwipeEndEvt evt = new SwipeEndEvt (_event,m_tgtPage,tgtObj,this.velocity,pageChanged);
			endDragEvent.Invoke (evt);
		}
		if (pageChanged && (pageChangeEvent!=null)) {
			SwipeEndEvt evt = new SwipeEndEvt (_event,m_tgtPage,tgtObj, this.velocity,pageChanged);
			pageChangeEvent.Invoke (evt);
		}
	}

	Vector2 getNormalisedPos(Vector2 _vec, int _sttPage){
		float min = Mathf.Max(0f,  ((float)_sttPage-1f) / m_numF);
		float max = Mathf.Min(m_numF,((float)_sttPage+1f) / m_numF);
		Vector2 nrmPos = _vec;
		nrmPos.x = Mathf.Clamp (nrmPos.x, min, max);
		return nrmPos;
	}

	int getTargetPage(Vector2 _vec, int _sttPage){
		int _nowPage = (int)(Mathf.Floor (0.5f + _vec.x * m_numF));
		if (Mathf.Abs (_nowPage - _sttPage) > 0) {
			_nowPage = _sttPage + (int)(Mathf.Sign ((float)(_nowPage - _sttPage)));
		}
		return _nowPage;
	}

	private void stickToPage (int _tgtPage){
		Vector3 pos = content.localPosition;
		Vector3 targetPos = content.localPosition;
		targetPos.x = -_tgtPage * cellSize.x;
		pos = Vector3.Lerp (pos, targetPos, 0.5f);
		content.localPosition = pos;
		this.StopMovement ();
		//			Debug.Log (_page + ":"+(nowX*100f)+":"+m_numF);
	}
}
