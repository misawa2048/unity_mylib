using UnityEngine;
using System.Collections;
using System;

namespace TmLib
{
    public class TmSimple2DMap
    {
        public enum CreateType
        {
            none=0,
            random,
            perlin,
            voronoi
        };

        Vector2Int m_size;
        public Vector2Int mapSize { get { return m_size; } }
        int m_randomFillPercent;
        int m_defSeed;
        int m_seed;
        int[,] m_map; // map原点は左下、TileMap原点は中央(0,0)
        public bool IsInnerMap(int _x, int _y) { return isInnerMap(_x, _y); }
        public int GetId(int _x, int _y) { return isInnerMap(_x,_y) ? m_map[_x, _y] : 0; }
        public int SetId(int _x, int _y, int _id) {
            int ret = GetId(_x,_y);
            if (isInnerMap(_x, _y))
            {
                m_map[_x, _y] = _id;
            }
            return ret;
        }
        int[] m_sectionSizeArr;
        public int[] sectionSizeArr { get { return m_sectionSizeArr; } }

        public TmSimple2DMap(int _w, int _h, int _percent=45, int _seed=0)
        {
            m_size.x = _w;
            m_size.y = _h;
            m_randomFillPercent = _percent;
            m_defSeed = _seed;
            if (_seed == 0)
            {
                m_defSeed = Time.time.ToString().GetHashCode();
            }
            m_map = new int[m_size.x, m_size.y];
        }

        public int GenerateMap(int _seed = 0, int _fillPercent=-1, int _smoothCnt = 5, float _scale = 1f, CreateType _type=CreateType.perlin)
        {
            if (_fillPercent <0)
            {
                _fillPercent = m_randomFillPercent;
            }

            m_seed = (_seed == 0) ? m_defSeed : _seed;

            switch (_type)
            {
                case CreateType.perlin: PerlinFillMap(_scale, _fillPercent); break;
                case CreateType.voronoi: VoronoiFillMap(_scale, _fillPercent); break;
                default: RandomFillMap(_scale, _fillPercent); break;
            }

            SmoothMap(_smoothCnt);
            int sectionNum = sectionMap(); // 0を含めた数(id+1)
            return sectionNum;
        }

        public int GetLargestSectionId(int _sectionNum)
        {
            m_sectionSizeArr = new int[_sectionNum];
            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    if (m_map[x, y] >= 0)
                    {
                        m_sectionSizeArr[m_map[x, y]]++;
                    }
                }
            }
            int max = 0;
            int maxId = 0;
            for (int i = 0; i < m_sectionSizeArr.Length; ++i)
            {
                if (m_sectionSizeArr[i] > max)
                {
                    maxId = i;
                    max = m_sectionSizeArr[i];
                }
                //Debug.Log("m_sectionSizeArr[" + i + "]=" + m_sectionSizeArr[i]);
            }
            return maxId;
        }

        void RandomFillMap(float _scale=1f, int _fillPercent = 45)
        {
            System.Random pseudoRandom = new System.Random(m_seed);

            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    if (x == 0 || x == m_size.x - 1 || y == 0 || y == m_size.y - 1)
                    {
                        m_map[x, y] = -1;
                    }
                    else
                    {
                        m_map[x, y] = (pseudoRandom.Next(0, 100) < _fillPercent) ? 1 : 0;
                    }
                }
            }
        }

        void PerlinFillMap(float _scale = 1f, int _fillPercent=45)
        {
            System.Random pseudoRandom = new System.Random(m_seed);
            float ofsx = (float)(pseudoRandom.Next(0, 1000));
            float ofsy = (float)(pseudoRandom.Next(0, 1000));

            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    if (x == 0 || x == m_size.x - 1 || y == 0 || y == m_size.y - 1)
                    {
                        m_map[x, y] = -1;
                    }
                    else
                    {
                        float fx = ofsx + ((float)x / (float)(m_size.x - 1)) * 5f;
                        float fy = ofsy + ((float)y / (float)(m_size.y - 1)) * 5f;
                        m_map[x, y] = (Mathf.PerlinNoise(fx* _scale, fy* _scale) *100f > (float)_fillPercent) ? -1 : 0;
                    }
                }
            }
        }

        void VoronoiFillMap(float _cellDencity=3f, int _fillPercent = 45)
        {
            System.Random pseudoRandom = new System.Random(m_seed);
            float rOfs = (float)(pseudoRandom.Next(0, 100000))/ 1000f +2f;
            Debug.Log("ofs:" + rOfs);

            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    if (x == 0 || x == m_size.x - 1 || y == 0 || y == m_size.y - 1)
                    {
                        m_map[x, y] = -1;
                    }
                    else
                    {
                        float fx = ((float)x / (m_size.x-1));
                        float fy = ((float)y / (m_size.y-1));
                        float fRet = Unity_Voronoi_float(new Vector2(fx, fy), rOfs, _cellDencity).x;
                        m_map[x, y] = (fRet * 100f > (float)_fillPercent) ? -1 : 0;
                    }
                }
            }
        }

        void SmoothMap(int _smoothCnt=5)
        {
            for(int i = 0; i < _smoothCnt; ++i)
            {
                SmoothMapSub();
            }
        }

        void SmoothMapSub()
        {
            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > 4)
                        m_map[x, y] = -1;
                    else if (neighbourWallTiles < 4)
                        m_map[x, y] = 0;

                }
            }
        }

        Vector2 Unity_Voronoi_RandomVector_float(Vector2 _UV, float _offset)
        {
            _UV.x = Mathf.Repeat(Mathf.Sin(_UV.x * 15.27f + _UV.y * 47.63f), 1f);
            _UV.y = Mathf.Repeat(Mathf.Sin(_UV.x * 99.41f + _UV.y * 89.98f), 1f);
            return new Vector2(Mathf.Sin(_UV.y * _offset) * 0.5f + 0.5f, Mathf.Cos(_UV.x * _offset) * 0.5f + 0.5f);
        }

        Vector3 Unity_Voronoi_float(Vector2 _UV, float _AngleOffset, float _CellDensity)
        {
            Vector2 g = new Vector2(Mathf.Floor(_UV.x * _CellDensity), Mathf.Floor(_UV.y * _CellDensity));
            Vector2 f = new Vector2(Mathf.Repeat(_UV.x * _CellDensity,1f), Mathf.Repeat(_UV.y * _CellDensity,1f));
            Vector3 res = new Vector3(8f, 0f, 0f);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 lattice = new Vector2(x, y);
                    Vector2 offset = Unity_Voronoi_RandomVector_float(lattice + g, _AngleOffset);
                    float d = ((lattice + offset) - f).magnitude;
                    if (d < res.x)
                    {
                        res.x = d;
                        res.y = offset.x;
                        res.z = offset.y;
                    }
                }
            }
            return res;
        }


        int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < m_size.x && neighbourY >= 0 && neighbourY < m_size.y)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += (m_map[neighbourX, neighbourY]<0)?1:0;
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }
            return wallCount;
        }


        // 繋がっていない空間を色分けする。0のところが1,2,,,nになる
        int sectionMap()
        {
            int colId = 0;

            while (true)
            {
                Vector2Int ret = find1stSpacePos();
                if (ret.x < 0)
                {
                    break;
                }
                else
                {
                    colId++;
                    int depth = setColId(ret.x, ret.y, colId, 0);
                    Debug.Log("depth:"+depth);
                }
            }
            return colId+1;
        }

        bool isInnerMap(int _x, int _y)
        {
            return (_x >= 0 && _x < m_size.x && _y >= 0 && _y < m_size.y);
        }
        bool isEmptyArea(int _x, int _y)
        {
            return (isInnerMap(_x, _y) && m_map[_x, _y] == 0);
        }

        int setColId(int _x,int _y, int _colId, int _depth)
        {
            int maxDepth = _depth;
            if (isEmptyArea(_x,_y))
            {
                m_map[_x, _y] = _colId;
                int xOfsMin = 0;
                int xOfsMax = 0;
                while (isEmptyArea(_x + xOfsMax + 1, _y))
                {
                    xOfsMax++;
                    m_map[_x + xOfsMax, _y] = _colId;
                }
                while (isEmptyArea(_x + xOfsMin - 1, _y))
                {
                    xOfsMin--;
                    m_map[_x + xOfsMin, _y] = _colId;
                }
                for(int iOfs= xOfsMin; iOfs <= xOfsMax; ++iOfs)
                {
                    if (isEmptyArea(_x + iOfs, _y + 1))
                    {
                        maxDepth = Mathf.Max(setColId(_x + iOfs, _y + 1, _colId, _depth + 1), maxDepth);
                    }
                    if (isEmptyArea(_x + iOfs, _y - 1))
                    {
                        maxDepth = Mathf.Max(setColId(_x + iOfs, _y - 1, _colId, _depth + 1), maxDepth);
                    }
                }
            }
            return maxDepth;
        }

        Vector2Int find1stSpacePos() {
            Vector2Int ret = new Vector2Int(-1,-1);
            for (int x = 0; x < m_size.x; x++)
            {
                for (int y = 0; y < m_size.y; y++)
                {
                    if(m_map[x, y] == 0)
                    {
                        ret = new Vector2Int(x, y);
                        return ret; //
                    }
                }
            }
            return ret;
        }

    }
}
