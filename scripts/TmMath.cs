using UnityEngine;
using System.Collections.Generic;

public class TmMath {
	//-----------------------------------------------------------------------------
	//! 点にもっとも近い直線上の点(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public Vector3 NearestPointOnLine(Vector3 p1, Vector3 p2, Vector3 p, bool isSegment=true){
		Vector3 d = p2 - p1;
		if (d.sqrMagnitude == 0.0f)    return p1;
		float t = (d.x * (p - p1).x + d.y * (p - p1).y + d.z * (p - p1).z) / d.sqrMagnitude;
		if(isSegment){
			if (t < 0.0f)    return p1;
			if (t > 1.0f)    return p2;
		}
		return (1.0f-t)*p1 + t*p2;
	}
	static public Vector2 NearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
		Vector3 ret = NearestPointOnLine(new Vector3(p1.x,p1.y), new Vector3(p2.x,p2.y), new Vector3(p.x,p.y),isSegment);
		return new Vector2(ret.x,ret.y);
	}
	
	//-----------------------------------------------------------------------------
	//! 直線と点の距離(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public float LineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment=true){
		return ( p - NearestPointOnLine(p1,p2,p,isSegment) ).magnitude;
	}
	static public float LineToPointDistance(Vector3 p1, Vector3 p2, Vector3 p, bool isSegment=true){
		return ( p - NearestPointOnLine(p1,p2,p,isSegment) ).magnitude;
	}
	
	//-----------------------------------------------------------------------------
	//! 2直線の近傍点(isSegmentがtrueで線分判定)
	//-----------------------------------------------------------------------------
	static public float LineToLineDistance(out Vector3 p0, out Vector3 q0, Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2, bool isSegment=true){
		if((p2-p1).sqrMagnitude==0.0f){
			p0 = p1;
			q0 = NearestPointOnLine(q1, q2, p1, isSegment);
		}else if((q2-q1).sqrMagnitude==0.0f){
			p0 = NearestPointOnLine(p1, p2, q1, isSegment);
			q0 = q1;
		}else{
			Vector3 m = (p2-p1).normalized;
			Vector3 n = (q2-q1).normalized;
			Vector3 ab = q1-p1;
			float mn = Vector3.Dot(m,n);
			if(Mathf.Abs(mn)==1.0f){
				Vector3 tp1 = NearestPointOnLine(p1, p2, q1, true);
				Vector3 tp2 = NearestPointOnLine(p1, p2, q2, true);
				Vector3 tq1 = NearestPointOnLine(q1, q2, p1, true);
				Vector3 tq2 = NearestPointOnLine(q1, q2, p2, true);
				p0 = (tp1+tp2)*0.5f;
				q0 = (tq1+tq2)*0.5f;
			}else{
				float s = (Vector3.Dot(ab,m)-Vector3.Dot(ab,n)*mn)/(1.0f-mn*mn);
				float t = (Vector3.Dot(ab,m)*mn-Vector3.Dot(ab,n))/(1.0f-mn*mn);
				p0 = p1+m*s;
				q0 = q1+n*t;
				if(isSegment){
					p0 = NearestPointOnLine(p1, p2, p0, true);
					q0 = NearestPointOnLine(q1, q2, q0, true);
				}
			}
		}
		return (q0-p0).magnitude;
	}
	static public float LineToLineDistance(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2, bool isSegment=true){
		Vector3 p0,q0;
		return LineToLineDistance(out p0, out q0, p1, p2, q1, q2, isSegment);
	}
	
	//-----------------------------------------------------------------------------
	//! 線分の交差チェック : 交差したらtrue
	//-----------------------------------------------------------------------------
	static bool CrossCheck(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
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
	static public bool Intersection(out Vector2 ret, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, bool isSegment=true){
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
	
	//-----------------------------------------------------------------------------
	//! 円(p,r)と直線の交点数(isSegmentがtrueで線分判定) : -1:解なし(!直線)
	//-----------------------------------------------------------------------------
	static public int CircleLineIntersection(out Vector2[] ret, Vector2 p, float r, Vector2 p1, Vector2 p2, bool isSegment=true){
		float a = p2.y-p1.y;
		float b = p2.x-p1.x;
		int num = CircleLineIntersection(out ret, p, r, -a, b, a*p1.x-b*p1.y);
		if((num>0) && isSegment){
			List <Vector2> vecList = new List<Vector2>();
			foreach(Vector2 vec in ret){
				if(LineToPointDistance(p1,p2,vec,true)==0.0f){
					vecList.Add(vec);
				}
			}
			num = vecList.Count;
			ret = vecList.ToArray();
		}
		return num;
	}
	//-----------------------------------------------------------------------------
	//! 円(p,r)と直線(ax+by+c=0)の交点数 : -1:解なし(!直線)
	//-----------------------------------------------------------------------------
	static public int CircleLineIntersection(out Vector2[] ret, Vector2 p, float r, float a, float b, float c){
		int result = -1;
		float l = a*a+b*b;
		if(l!=0){
			float k = a*p.x+b*p.y+c;
			float d = l*r*r-k*k;
			if(d>0){
				result = 2;
				float ds = Mathf.Sqrt(d);
				float apl = a/l;
				float bpl = b/l;
				float xc = p.x-apl*k;
				float yc = p.y-bpl*k;
				float xd = bpl*ds;
				float yd = apl*ds;
				ret = new Vector2[2]{ new Vector2(xc-xd,yc+yd),new Vector2(xc+xd,yc-yd)};
			}else if(d==0){
				result = 1;
				ret = new Vector2[1]{new Vector2(p.x-a*k/l,p.y-b*k/l)};
			}else{
				result = 0;
				ret = new Vector2[0];
			}
		}else{
			ret = new Vector2[0];
		}
		return result;
	}
	//-----------------------------------------------------------------------------
	//! 円と円の交点 : 0なら交差しない -1なら同一円
	//-----------------------------------------------------------------------------
	static public int CircleIntersection(out Vector2[] ret, Vector2 p, float rp, Vector2 q, float rq){
		float c = 0.5f * ((rp-rq)*(rp+rq)-(p.x-q.x)*(p.x+q.x)-(p.y-q.y)*(p.y+q.y));
		return CircleLineIntersection(out ret, p, rp, p.x-q.x, p.y-q.y, c);
	}
	
	//-----------------------------------------------------------------------
	//! 弾丸衝突チェック 
	//-----------------------------------------------------------------------
	static public bool BulletHit(out Vector3 hit, Vector3 p, Vector3 pOld, float pRadius, Vector3 q, Vector3 qOld, float qRadius){
		bool ret = false;
		Vector3 p0,q0;
		float hitRad = pRadius+qRadius;
		float dist = LineToLineDistance(out p0, out q0, p, pOld, q, qOld, true);
		hit = (p0*pRadius+q0*qRadius)/hitRad;
		if(dist<=hitRad){
			Vector3 qp = qOld+(q-qOld)*((p0-pOld).magnitude/(p-pOld).magnitude); // sync time 
			Vector3 pq = pOld+(p-pOld)*((q0-qOld).magnitude/(q-qOld).magnitude); // sync time 
			if(((p0-qp).sqrMagnitude < hitRad*hitRad)||((pq-q0).sqrMagnitude < hitRad*hitRad)){
				ret = true;
			}
		}
		return ret;
	}
	
	//-----------------------------------------------------------------------
	//! 衝突時刻取得:q1+qSpd*retがhit場所(retがマイナスの場合はhitしない） 
	//-----------------------------------------------------------------------
	static public float CollideTime(Vector3 q1, Vector3 qSpd, Vector3 p1, float ps){
		float ret = float.MinValue;
		Vector3 p0,q0;
		float qs = qSpd.magnitude;
		Vector3 q2 = q1+qSpd;
		float d = LineToLineDistance(out p0,out q0,p1,p1,q1,q2,false);
		// p1から直線q1q2に下ろした垂線の交点 を通る時間 t0
		float t0 = Vector3.Dot(qSpd.normalized,p0-q1)/qs;
		float a = qs*qs - ps*ps;
		float b = -(2*t0*qs*qs);
		float c = (t0*qs)*(t0*qs) + d*d;
		float aa = b*b - 4 * a * c;
		if(a==0.0f){
			ret = (-c/b);
		}else if(aa>0.0f){
			float t1 = (-b+Mathf.Sqrt(aa)) / (2*a);
			float t2 = (-b-Mathf.Sqrt(aa)) / (2*a);
			ret = Mathf.Min(t1,t2);
			if(ret<0.0f){
				ret = Mathf.Max(t1,t2);
			}
		}
		return ret;
	}

	//-----------------------------------------------------------------------
	//! 重力g力vで距離sに物体を投げるときの角度 (retがマイナスの場合は届かない）
	//! //http://www.sousakuba.com/Programming/algo_dandoukeisan2.html
	//! _g = Physics.gravity.y
	//-----------------------------------------------------------------------
	static public float ParabolicRad(float _v, float _s, float _g){
		float ret = -0.1f;
		float a = (_g*_s*_s)/(2*_v*_v);
		float d = (_s/a)*(_s/a)-4.0f;
		if(d>=0){
			float sd = Mathf.Sqrt(d);
			ret = Mathf.Atan(Mathf.Max((-(_s/a)+sd),(-(_s/a)-sd))/2.0f);
		}
		return ret;
	}

	//-----------------------------------------------------------------------
	//! 2D版LookRotation
	//-----------------------------------------------------------------------
	static public Quaternion LookRotation2D(Vector2 _vec,Vector2 _up){
		float tmpAng = (Mathf.Atan2(_vec.y, _vec.x)-Mathf.Atan2(_up.y, _up.x)) * Mathf.Rad2Deg;
		Quaternion rot = Quaternion.AngleAxis(tmpAng, Vector3.forward);
		return rot;
	}

	//-----------------------------------------------------------------------
	//!   カメラ消失点オフセット
	//-----------------------------------------------------------------------
	static public bool SetVanishingPoint(Vector2 perspectiveOffset,Camera cam){
		if(cam!=null){
			cam.ResetProjectionMatrix();
			Matrix4x4 m = cam.projectionMatrix;
			float w = 2f*cam.nearClipPlane/m.m00;
			float h = 2f*cam.nearClipPlane/m.m11;
			float left = -w/2f - perspectiveOffset.x;
			float right = left+w;
			float bottom = -h/2f - perspectiveOffset.y;
			float  top = bottom+h;
			cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
		}
		return (cam!=null);
	}

	//-----------------------------------------------------------------------
	//! perspectiveCenterOffset
	//-----------------------------------------------------------------------
	static public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far){
		float x =  (2f * near)      / (right - left);
		float y =  (2f * near)      / (top - bottom);
		float a =  (right + left)   / (right - left);
		float b =  (top + bottom)   / (top - bottom);
		float c = -(far + near)     / (far - near);
		float d = -(2f * far * near)/ (far - near);
		float e = -1f;
		Matrix4x4 m = new Matrix4x4();
		m[0,0] =  x;  m[0,1] = 0f;  m[0,2] = a;   m[0,3] = 0f;
		m[1,0] = 0f;  m[1,1] =  y;  m[1,2] = b;   m[1,3] = 0f;
		m[2,0] = 0f;  m[2,1] = 0f;  m[2,2] = c;   m[2,3] =  d;
		m[3,0] = 0f;  m[3,1] = 0f;  m[3,2] = e;   m[3,3] = 0f;
		return m;
	}

}
