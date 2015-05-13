using UnityEngine;
using System.Collections;
// 
// TmMaze mMaze = new TmMaze(width,height);
// GameObject mazeBase = mMaze.createWallObjs(wallPrefab);
namespace TmLib{
	public class TmMaze {
		enum Dir{ L, R, U, D };
		public class clusteInfo{
			public int id;
			public bool wallLV;
			public bool wallDH;
			public clusteInfo(int _id, bool _lv, bool _dh){
				id = _id;
				wallLV = _lv;
				wallDH = _dh;
			}
		};
		
		//------------------------------------------------------------------
		private const int OUT_OF_AREA_ID = -1;
		private const int START_PLAYER_POS_ID = -2;
		
		private clusteInfo[,] mInfo;
		public clusteInfo[,] info{ get{ return mInfo; } }
		public int width{ get; private set; }
		public int height{ get; private set; }
		public bool isError{ get; private set; }
		
		public TmMaze(int _width, int _height){
			width = _width;
			height = _height;
			mInfo = createClusterArray(_width,_height);
			//		setInfoAsCircle(ref mInfo);
			setWallInfo(ref mInfo);
			int baseId = getStartId(mInfo);
			isError = createMazeInfo(baseId,ref mInfo);
			if(!isError){
				Debug.Log("Create Error!");
			}
		}
		
		//------------------------------------------------------------------
		public GameObject createWallObjs(GameObject _prefab=null,float _wallWidthRate=0.1f, clusteInfo[,] _info=null){
			if(_info==null){ _info = mInfo; }
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			GameObject baseObj = new GameObject("wallBase");
			for(int yy = 0; yy < sy+1;++yy){
				for(int xx = 0; xx < sx+1;++xx){
					Vector3 defPos = new Vector3((float)xx+0.5f,(float)yy+0.5f,0.0f);
					defPos -= new Vector3(sx,sy)*0.5f;
					GameObject go;
					Vector3 pos;
					Vector3 scale;
					if(_info[xx,yy].wallLV){
						if(_prefab==null){
							go = GameObject.CreatePrimitive(PrimitiveType.Cube);
						}else{
							go = GameObject.Instantiate(_prefab) as GameObject;
						}
						pos = defPos;
						pos.x -= 0.5f;
						scale = go.transform.localScale;
						scale.x *= _wallWidthRate;
						scale.y *= (1.0f+_wallWidthRate*0.5f);
						go.transform.position = pos;
						go.transform.localScale = scale;
						go.transform.parent = baseObj.transform;
					}
					if(_info[xx,yy].wallDH){
						if(_prefab==null){
							go = GameObject.CreatePrimitive(PrimitiveType.Cube);
						}else{
							go = GameObject.Instantiate(_prefab) as GameObject;
						}
						pos = defPos;
						pos.y -= 0.5f;
						scale = go.transform.localScale;
						scale.x *= (1.0f+_wallWidthRate*0.5f);
						scale.y *= _wallWidthRate;
						go.transform.position = pos;
						go.transform.localScale = scale;
						go.transform.parent = baseObj.transform;
					}
				}
			}
			return baseObj;
		}
		
		//------------------------------------------------------------------
		private clusteInfo[,] createClusterArray(int _sx, int _sy){
			clusteInfo[,] tmpInfo = new clusteInfo[_sx+1,_sy+1];
			int id = 0;
			for(int xx = 0; xx < _sx+1;++xx){
				for(int yy = 0; yy < _sy+1;++yy){
					int myId = ((xx<_sx)&&(yy<_sy)) ? id : -1;
					tmpInfo[xx,yy] = new clusteInfo(myId,(yy<_sy),(xx<_sx));
					if(myId>=0){
						id++;
					}
				}
			}
			return tmpInfo;
		}
		
		private void setInfoAsCircle(ref clusteInfo[,] _info){
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			Vector2 centerPos = new Vector2(sx-1,sy-1)*0.5f;
			for(int xx = 0; xx < _info.GetLength(0)-1;++xx){
				for(int yy = 0; yy < _info.GetLength(1)-1;++yy){
					Vector2 pos = new Vector2(xx,yy);
					if((pos-centerPos).magnitude > (centerPos.magnitude/1.41f+0.5f)){
						_info[xx,yy].id = OUT_OF_AREA_ID;
					}
				}
			}
		}
		
		private void setWallInfo(ref clusteInfo[,] _info){
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			for(int xx = 0; xx <= sx;++xx){
				for(int yy = 0; yy <= sy;++yy){
					_info[xx,yy].wallLV = !((_info[xx,yy].id<0)&&((xx==0)||(_info[xx-1,yy].id<0)));
					_info[xx,yy].wallDH = !((_info[xx,yy].id<0)&&((yy<=0)||(_info[xx,yy-1].id<0)));
				}
			}
		}
		
		private int getStartId(clusteInfo[,] _info){
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			int ret = -1;
			for(int xx = 0; xx < sx;++xx){
				for(int yy = 0; yy < sy;++yy){
					if((ret<0)||((_info[xx,yy].id>=0)&&(ret>_info[xx,yy].id))){
						ret = _info[xx,yy].id;
					}
				}
			}
			return ret;
		}
		
		private bool createMazeInfo(int _baseId, ref clusteInfo[,] _info){
			bool isFinished= false;
			bool isTimeOut = false;
			int timeOutCnt=0;
			while(!isFinished){
				isFinished = createMazeSub(_baseId, ref _info);
				timeOutCnt++;
				if(timeOutCnt>10){
					isTimeOut = true;
					isFinished = true;
				}
			}
			if(isTimeOut){
				isFinished = false;
			}
			return isFinished;
		}
		
		private bool createMazeSub(int _baseId, ref clusteInfo[,] _info){
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			bool ret = true;
			if(_baseId<0) return ret;
			
			for(int xx = 0; xx < sx;++xx){
				for(int yy = 0; yy < sy;++yy){
					int id0 = _info[xx,yy].id;
					if(id0<0){
						continue;
					}
					if(id0!=_baseId){
						ret = false;
					}
					float val = Random.value;
					Dir dir = (val<0.25f) ? Dir.L : (val<0.5f) ? Dir.R : (val<0.75f) ? Dir.U : Dir.D;
					int id1 = getNeighbourId(_info,xx,yy,dir);
					if( (id1>=0)&&(id0 != id1) ){
						relate(ref _info,id0,id1);
						setWallState(ref _info, xx,yy,dir,false);
					}
				}
			}
			return ret;
		}
		
		private int getNeighbourId(clusteInfo[,] _info, int _x, int _y, Dir _dir){
			int ret = -1;
			_x += (_dir==Dir.R) ? 1 : (_dir==Dir.L) ? -1 : 0;
			_y += (_dir==Dir.U) ? 1 : (_dir==Dir.D) ? -1 : 0;
			if((_x>=0)&&(_x<_info.GetLength(0))&&(_y>=0)&&(_y<_info.GetLength(1))){
				ret = _info[_x,_y].id;
			}
			return ret;
		}
		
		private bool relate(ref clusteInfo[,] _info, int _id0, int _id1){
			int sx = _info.GetLength(0)-1; //meshSize.x
			int sy = _info.GetLength(1)-1; //meshSize.y
			bool ret = false;
			if(_id0!=_id1){
				ret = true;
				int max = Mathf.Max(_id0,_id1);
				int min = Mathf.Min(_id0,_id1);
				for(int xx = 0; xx < sx;++xx){
					for(int yy = 0; yy < sy;++yy){
						if(_info[xx,yy].id == max){
							_info[xx,yy].id = min;
						}else{
							ret = false;
						}
					}
				}
			}
			return ret;
		}
		
		private bool setWallState(ref clusteInfo[,] _info, int _x, int _y, Dir _dir, bool state){
			bool ret = false;
			_x += (_dir==Dir.R) ? 1 : 0;
			_y += (_dir==Dir.U) ? 1 : 0;
			if((_x<_info.GetLength(0))&&(_y<_info.GetLength(1))){
				if((_dir==Dir.R)||(_dir==Dir.L)){
					_info[_x,_y].wallLV = state;
					ret = true;
				}else{
					_info[_x,_y].wallDH = state;
					ret = true;
				}
			}
			return ret;
		}
	}
} //namespace TmLib
