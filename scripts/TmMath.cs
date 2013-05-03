using UnityEngine;

public class TmMath {
	//-----------------------------------------------------------------------------
	//! 点にもっとも近い直線上の点(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	Vector2 nearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
	    Vector2 d = p2 - p1;
	    if (d.sqrMagnitude == 0)    return p1;
	    float t = (d.x * (p - p1).x + d.y * (p - p1).y) / d.sqrMagnitude;
		if(isSegment){
		    if (t < 0)    return p1;
		    if (t > 1)    return p2;
		}
		Vector2 c = new Vector2( (1 - t) * p1.x + t * p2.x, (1 - t) * p1.y + t * p2.y);
	    return c;
	}
	
	//-----------------------------------------------------------------------------
	//! 直線と点の距離(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	float lineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
		return ( p - nearestPointOnLine(p1,p2,p,isSegment) ).magnitude;
	}
	
	//-----------------------------------------------------------------------------
	//! 線分の交差チェック   交差したらtrue
	//-----------------------------------------------------------------------------
	bool crossCheck(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)	{
		if(((p1.x-p2.x)*(p3.y-p1.y)+(p1.y-p2.y)*(p1.x-p3.x))*((p1.x-p2.x)*(p4.y-p1.y)+(p1.y-p2.y)*(p1.x-p4.x))<0){
			if(((p3.x-p4.x)*(p1.y-p3.y)+(p3.y-p4.y)*(p3.x-p1.x))*((p3.x-p4.x)*(p2.y-p3.y)+(p3.y-p4.y)*(p3.x-p2.x))<0){
				return(true);
			}
		}
		return(false);
	}

	//-----------------------------------------------------------------------------
	//! 直線と直線の交点(isSegmentがtrueで線分判定):nullなら交差しない
	//-----------------------------------------------------------------------------
	bool intersection(out Vector2 ret, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, bool isSegment=true){
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
