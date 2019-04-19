using UnityEngine;
using System.Collections.Generic;

namespace TmLib{
	public class TmMath {
        //-----------------------------------------------------------------------------
        //! 値を_divの符号内でループ
        //-----------------------------------------------------------------------------
        static public int SLoop(int _num, int _div)
        {
            return ((_num % _div) + _div) % _div;
        }
        //-----------------------------------------------------------------------------
        //! ブラウン運動乱数 (0-1)
        //-----------------------------------------------------------------------------
        static public float BrownRandom(float _old, float _moveRate, float _baseValue=0f, float _baseStickeyRate=0.2f) {
			float adjValue = (((_old - 0.5f) > _baseValue) ? -1f : 1f)*Mathf.Abs(_baseValue)*_baseStickeyRate;
			float ret = Mathf.PingPong(_old + (Random.value-0.5f+adjValue) * _moveRate,1f);
			return ret;
		}
		
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
		//! 衝突時刻取得:q1+qSpd*retがhit場所(retがnullの場合はhitしない） 
		//-----------------------------------------------------------------------
		static public float[] CollideTime(Vector3 q1, Vector3 qSpd, Vector3 p1, float ps){
			if (qSpd == Vector3.zero) {
				return new float[1]{(p1-q1).magnitude/ps};
			}
			float[] ret = null;
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
				ret = new float[1];
				ret[0] = (-c/b);
			}else if(aa>0.0f){
				float sqa = Mathf.Sqrt(aa);
				float t1 = Mathf.Min((-b+sqa) / (2*a),(-b-sqa) / (2*a)) ;
				float t2 = Mathf.Max((-b+sqa) / (2*a),(-b-sqa) / (2*a)) ;
				if(t2>0.0f){
					if(t1>0f){
						ret = new float[2];
						ret[0] = t1;
						ret[1] = t2;
					}else{
						ret = new float[1];
						ret[0] = t2;
					}
				}
			}
			return ret;
		}
		
		//-----------------------------------------------------------------------
		//! 重力g鉛直に投げ時間tで高さhに到達するための初速v0 
		//-----------------------------------------------------------------------
		static public float ParabolicSpeed(float _t, float _h, float _g){
			// h=v0*t+1/2*g*t^2; v0=(-0.5*g*t^2+h)/t
			return _h/_t-0.5f*_g*_t;
		}
		
		//-----------------------------------------------------------------------
		//! 重力g力vで鉛直に投げ高さhに到達する時間 (retがマイナスの場合は届かない）
		//-----------------------------------------------------------------------
		static public float[] ParabolicTime(float _v, float _h, float _g){
			float[] ret = null;
			float d = _v*_v-2f*_g*_h;
			if(d>=0){
				float dq = Mathf.Sqrt(d);
				float t1 = (-_v+Mathf.Min(-dq,dq))/_g;
				float t2 = (-_v+Mathf.Max(-dq,dq))/_g;
				if(t2>0f){
					if(t1>0f){
						ret = new float[2];
						ret[0] = t1;
						ret[1] = t2;
					}else{
						ret = new float[1];
						ret[0] = t2;
					}
				}
			}
			return ret;
		}
		
		//-----------------------------------------------------------------------
		//! 重力g力vで距離sに物体を投げるときの角度 (retがnullの場合は届かない）
		//! //http://www.sousakuba.com/Programming/algo_dandoukeisan2.html
		//! _g = Physics.gravity.y
		//-----------------------------------------------------------------------
		static public float[] ParabolicRad(float _v, float _s, float _g){
			float[] ret = null;
			float a = (_g*_s*_s)/(2*_v*_v);
			float d = (_s/a)*(_s/a)-4.0f;
			if(d>=0){
				float sd = Mathf.Sqrt(d);
				ret = new float[2]{Mathf.Atan((-_s/a-sd)/2.0f),Mathf.Atan((-_s/a+sd)/2.0f)};
			}
			return ret;
		}

		//-----------------------------------------------------------------------
		//! 重力g力vで距離distに物体を投げるときのVec (retがnullの場合は届かない）
		//! //http://www.sousakuba.com/Programming/algo_dandoukeisan3.html
		//! //http://www.kumamoto-nct.ac.jp/file/knct-kiyou-2013/pdf/no18.pdf
		//! _g = Physics.gravity.y
		//-----------------------------------------------------------------------
		static public Vector3[] ParabolicVec(float _v, Vector3 _dist, float _g){
			Vector3[] ret = null;
			Vector3 distXZ = new Vector3 (_dist.x, 0f, _dist.z);
			float dist_x = distXZ.magnitude;
			float dist_y = _dist.y;
			float a = (_g * dist_x * dist_x) /  ( 2f * _v * _v );
			float b = dist_x / a;
			float c = ( a - dist_y ) / a;			
			float ts = (b*b/4f) - c;
			if ( ts >= 0.0 ) {
				float rt = Mathf.Sqrt( -c + ( b * b ) / 4f);
				float[] agl = new float[2]{Mathf.Atan( ( -b / 2f ) - rt ),Mathf.Atan( ( -b / 2f ) + rt )};
				ret = new Vector3[2];
				for ( int i = 0; i < 2; i++ ){
					ret[i] = distXZ.normalized * _v * Mathf.Cos(agl[i]);
					ret[i].y = _v * Mathf.Sin(agl[i]);
				}
			}
			return ret;
		}

		//-----------------------------------------------------------------------
		//! 重力gで距離distに最小エネルギーで物体を投げるときのVec(ts==0:b*b=4*c)
		//! _g = Physics.gravity.y
		//-----------------------------------------------------------------------
		static public Vector3 ParabolicVec(Vector3 _dist, float _g){
			Vector3 ret = Vector3.zero;
			float xz2 = _dist.x * _dist.x + _dist.z * _dist.z;
			float y = _dist.y;
			float dd = _dist.magnitude; // Mathf.Sqrt (y * y + xz2);
			if (xz2 > 0.01f) {
				float e0 = (y - dd != 0f) ? (_g * xz2) / (y - dd) : -1f;
				float e1 = (y + dd != 0f) ? (_g * xz2) / (y + dd) : -1f;
				if ((e0 > 0f) || (e1 > 0f)) {
					Vector3 nmlVec = new Vector3 (_dist.x, Mathf.Sqrt (xz2)+y*1.1f, _dist.z).normalized;
					ret = (nmlVec * Mathf.Sqrt ((e0 > 0f) ? e0 : e1));
				}
			} else { // v = sqrt(2*g*s) | 0
				if (-_g * y > 0f) {
					ret = Vector3.up * Mathf.Sqrt (2f * -_g * y);
				}
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
        //! axis平面に射影したfrom-toの角度
        //! (transform.forward,dir,transform.up) でforward-dirのlocalxz方位角
        //-----------------------------------------------------------------------
        static public float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Mathf.Atan2(Vector3.Dot(axis, Vector3.Cross(from, to)), Vector3.Dot(from, to)) * Mathf.Rad2Deg;
        }
		
		/// <summary>
		/// Corrects the cam target.
		/// playerCollision(Cupsule), カメラ視線_tgtPos-_camPosから、
		/// Cupsuleの内側から始まる補正カメラtgtPosを返す
		/// </summary>
		/// <returns>The cam target.</returns>
		/// <param name="_tgtPos">world Tgt position.</param>
		/// <param name="_camPos">world Cam position.</param>
		/// <param name="_plPos">world Pl position.</param>
		/// <param name="_plHOfs">world Pl H ofs.</param>
		/// <param name="_plRad">Pl RAD.</param>
		/// <param name="_camRad">Cam RAD.</param>
		/*
		CapsuleCollider coll = GetComponent<CapsuleCollider> ();
		coll.height = (m_plHalfH + m_plRad) * 2.0f;
		coll.radius = m_plRad;
		tgtPos = transform.position + transform.rotation * m_targetOfs;
		camPos = tgtPos + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
		Vector3 plHOfs = transform.rotation * (Vector3.up * m_plHalfH);

		Vector3 corTgt;
		corTgt = CorrectCamTarget(tgtPos, camPos, transform.position, plHOfs, m_plRad, m_camRad);

		Vector3 corCam = corTgt + transform.rotation * Quaternion.AngleAxis (m_cameraYaw, Vector3.right) * Vector3.back * m_cameraDist;
		Debug.DrawLine (corTgt, corCam, Color.red);
		*/
		static public Vector3 CorrectCamTarget(Vector3 _tgtPos, Vector3 _camPos, Vector3 _plPos, Vector3 _plHOfs, float _plRad, float _camRad){
			Vector3 retTgtPos = _tgtPos;
			Vector3 _plPos0 = _plPos - _plHOfs;
			Vector3 _plPos1 = _plPos + _plHOfs;
			Vector3 p0, q0;
			TmMath.LineToLineDistance (out p0, out q0, _plPos0, _plPos1, _tgtPos, _camPos, true);
			q0 = TmMath.NearestPointOnLine (_tgtPos, _camPos, p0, true);
			float dist = (q0 - p0).magnitude;
			if (dist > (_plRad + _camRad)) {
				retTgtPos = p0 + (q0 - p0).normalized * (_plRad + _camRad);
			} else {
				Vector3 dir = (_tgtPos-_camPos).normalized;
				retTgtPos = q0 + dir * (dist - (_plRad + _camRad));
			}
			return retTgtPos;
		}

        //http://www.kurims.kyoto-u.ac.jp/~ooura/fftman/ftmn2_12.html#sec2_1_2
        /// <summary>
        /// 実数データの DFT :
        /// F[k]=Σ_j=0^n-1 a[j]*exp(-2*pi*i*j*k/n),0<=k<n を計算する.
        /// 出力 a[0...n/2], a[n/2+1...n-1] はそれぞれ F[0...n/2] の実部, 
        /// F[n/2+1...n-1] の虚部に相当する．
        /// </summary>
        /// <param name="a">入力(2^N)</param>
        /// <returns>出力</returns>
        static public float[] RFFT(float[] a)
        {
            int m, mh, mq, i, j, k, jr, ji, kr, ki;
            float theta, wr, wi, xr, xi;
            int n = a.Length;
            /* ---- scrambler ---- */
            i = 0;
            for (j = 1; j < n - 1; j++)
            {
                for (k = n >> 1; k > (i ^= k); k >>= 1) ;
                if (j < i)
                {
                    xr = a[j];
                    a[j] = a[i];
                    a[i] = xr;
                }
            }
            theta = -8 * Mathf.Atan(1.0f);  /* -2*pi */
            for (mh = 1; (m = mh << 1) <= n; mh = m)
            {
                mq = mh >> 1;
                theta *= 0.5f;
                /* ---- real to real butterflies (W == 1) ---- */
                for (jr = 0; jr < n; jr += m)
                {
                    kr = jr + mh;
                    xr = a[kr];
                    a[kr] = a[jr] - xr;
                    a[jr] += xr;
                }
                /* ---- complex to complex butterflies (W != 1) ---- */
                for (i = 1; i < mq; i++)
                {
                    wr = Mathf.Cos(theta * i);
                    wi = Mathf.Sin(theta * i);
                    for (j = 0; j < n; j += m)
                    {
                        jr = j + i;
                        ji = j + mh - i;
                        kr = j + mh + i;
                        ki = j + m - i;
                        xr = wr * a[kr] + wi * a[ki];
                        xi = wr * a[ki] - wi * a[kr];
                        a[kr] = -a[ji] + xi;
                        a[ki] = a[ji] + xi;
                        a[ji] = a[jr] - xr;
                        a[jr] = a[jr] + xr;
                    }
                }
                /* ---- real to complex butterflies are trivial ---- */
            }
            return a;
        }

    }
} //namespace TmLib
