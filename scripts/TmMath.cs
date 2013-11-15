using UnityEngine;

public class TmMath {
	//-----------------------------------------------------------------------------
	//! 点にもっとも近い直線上の点(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public Vector3 nearestPointOnLine(Vector3 p1, Vector3 p2, Vector3 p, bool isSegment=true){
		Vector3 d = p2 - p1;
		if (d.sqrMagnitude == 0.0f)    return p1;
		float t = (d.x * (p - p1).x + d.y * (p - p1).y + d.z * (p - p1).z) / d.sqrMagnitude;
		if(isSegment){
			if (t < 0.0f)    return p1;
			if (t > 1.0f)    return p2;
		}
		return new Vector3( (1.0f-t)*p1.x + t*p2.x, (1.0f-t)*p1.y + t*p2.y, (1.0f-t)*p1.z + t*p2.z);
	}
	static public Vector2 nearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
		Vector3 ret = nearestPointOnLine(new Vector3(p1.x,p1.y), new Vector3(p2.x,p2.y), new Vector3(p.x,p.y),isSegment);
		return new Vector2(ret.x,ret.y);
	}
	
	//-----------------------------------------------------------------------------
	//! 直線と点の距離(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public float lineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
		return ( p - nearestPointOnLine(p1,p2,p,isSegment) ).magnitude;
	}
	static public float lineToPointDistance(Vector3 p1, Vector3 p2, Vector3 p, bool isSegment=true){
		return ( p - nearestPointOnLine(p1,p2,p,isSegment) ).magnitude;
	}
	
	//-----------------------------------------------------------------------------
	//! 2直線の近傍点(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public float lineToLineDistance(out Vector3 p0, out Vector3 q0, Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2, bool isSegment=true){
		if((p2-p1).sqrMagnitude==0.0f){
			p0 = p1;
			q0 = nearestPointOnLine(q1, q2, p1, isSegment);
		}else if((q2-q1).sqrMagnitude==0.0f){
			p0 = nearestPointOnLine(p1, p2, q1, isSegment);
			q0 = q1;
		}else{
			Vector3 m = (p2-p1).normalized;
			Vector3 n = (q2-q1).normalized;
			Vector3 ab = q1-p1;
			float mn = Vector3.Dot(m,n);
			if(Mathf.Abs(mn)==1.0f){
				Vector3 tp1 = nearestPointOnLine(p1, p2, q1, true);
				Vector3 tp2 = nearestPointOnLine(p1, p2, q2, true);
				Vector3 tq1 = nearestPointOnLine(q1, q2, p1, true);
				Vector3 tq2 = nearestPointOnLine(q1, q2, p2, true);
				p0 = (tp1+tp2)*0.5f;
				q0 = (tq1+tq2)*0.5f;
			}else{
				float s = (Vector3.Dot(ab,m)-Vector3.Dot(ab,n)*mn)/(1.0f-mn*mn);
				float t = (Vector3.Dot(ab,m)*mn-Vector3.Dot(ab,n))/(1.0f-mn*mn);
				p0 = p1+m*s;
				q0 = q1+n*t;
				if(isSegment){
					p0 = nearestPointOnLine(p1, p2, p0, true);
					q0 = nearestPointOnLine(q1, q2, q0, true);
				}
			}
		}
		return (q0-p0).magnitude;
	}
	static public float lineToLineDistance(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2, bool isSegment=true){
		Vector3 p0,q0;
		return lineToLineDistance(out p0, out q0, p1, p2, q1, q2, isSegment);
	}
	
	//-----------------------------------------------------------------------------
	//! 線分の交差チェック : 交差したらtrue
	//-----------------------------------------------------------------------------
	static bool crossCheck(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
		if(((p1.x-p2.x)*(p3.y-p1.y)+(p1.y-p2.y)*(p1.x-p3.x))*((p1.x-p2.x)*(p4.y-p1.y)+(p1.y-p2.y)*(p1.x-p4.x))<0){
			if(((p3.x-p4.x)*(p1.y-p3.y)+(p3.y-p4.y)*(p3.x-p1.x))*((p3.x-p4.x)*(p2.y-p3.y)+(p3.y-p4.y)*(p3.x-p2.x))<0){
				return(true);
			}
		}
		return(false);
	}
	
	//-----------------------------------------------------------------------------
	//! 直線と直線の交点(isSegmentがtrueで線分判定) : falseなら交差しない 
	//-----------------------------------------------------------------------------
	static public bool intersection(out Vector2 ret, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, bool isSegment=true){
		bool result = false;
		ret = Vector2.zero;
		Vector2 ac = (p3 - p1);
		float bs = ( p2.x - p1.x )*( p4.y - p3.y ) - ( p2.y - p1.y )*( p4.x - p3.x );
		if( bs != 0 ){	// !平行 
			float h1 = ( ( p2.y - p1.y ) * ac.x - ( p2.x - p1.x ) * ac.y ) / bs;
			float h2 = ( ( p4.y - p3.y ) * ac.x - ( p4.x - p3.x ) * ac.y ) / bs;
			if( (!isSegment) || ((h1>=0)&&(h1<=1)&&(h2>=0)&&(h2<=1)) ){
				ret = p1 + h2 * ( p2 - p1 );
				result = true;
			}
		}
		return result;
	}
}
