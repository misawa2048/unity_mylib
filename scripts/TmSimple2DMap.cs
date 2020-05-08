using UnityEngine;
using System.Collections;
using System;

namespace TmLib
{
    public class TmSimple2DMap
    {
        int m_width;
        int m_height;
        int m_randomFillPercent;
        int m_defSeed;
        int m_seed;
        int[,] m_map;
        public int GetData(int _x, int _y) { return m_map[_x, _y]; } 

        public TmSimple2DMap(int _w, int _h, int _percent=45, int _seed=0)
        {
            m_width = _w;
            m_height = _h;
            m_randomFillPercent = _percent;
            m_defSeed = _seed;
            if (_seed == 0)
            {
                m_defSeed = Time.time.ToString().GetHashCode();
            }
            m_map = new int[m_width, m_height];
        }

        public void GenerateMap(int _seed = 0, int _smoothCnt = 5, float _scale = 1f)
        {

            m_seed = (_seed == 0) ? m_defSeed : _seed;

            PerlinFillMap(_scale);
            SmoothMap(_smoothCnt);
        }


        void RandomFillMap(float _scale=1f)
        {
            System.Random pseudoRandom = new System.Random(m_seed);

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    if (x == 0 || x == m_width - 1 || y == 0 || y == m_height - 1)
                    {
                        m_map[x, y] = 1;
                    }
                    else
                    {
                        m_map[x, y] = (pseudoRandom.Next(0, 100) < m_randomFillPercent) ? 1 : 0;
                    }
                }
            }
        }

        void PerlinFillMap(float _scale = 1f)
        {
            System.Random pseudoRandom = new System.Random(m_seed);
            float ofsx = (float)(pseudoRandom.Next(0, 1000));
            float ofsy = (float)(pseudoRandom.Next(0, 1000));

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    if (x == 0 || x == m_width - 1 || y == 0 || y == m_height - 1)
                    {
                        m_map[x, y] = 1;
                    }
                    else
                    {
                        float fx = ofsx + ((float)x / (float)(m_width - 1)) * 5f;
                        float fy = ofsy + ((float)y / (float)(m_height - 1)) * 5f;
                        m_map[x, y] = (Mathf.PerlinNoise(fx* _scale, fy* _scale) *100f > (float)m_randomFillPercent) ? 1 : 0;
                    }
                }
            }
        }

        void VoronoiFillMap(float _cellDencity=3f)
        {
            System.Random pseudoRandom = new System.Random(m_seed);
            float rOfs = (float)(pseudoRandom.Next(0, 100000))/ 1000f +2f;
            Debug.Log("ofs:" + rOfs);

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    if (x == 0 || x == m_width - 1 || y == 0 || y == m_height - 1)
                    {
                        m_map[x, y] = 1;
                    }
                    else
                    {
                        float fx = ((float)x / (m_width-1));
                        float fy = ((float)y / (m_height-1));
                        float fRet = Unity_Voronoi_float(new Vector2(fx, fy), rOfs, _cellDencity).x;
                        m_map[x, y] = (fRet * 100f > (float)m_randomFillPercent) ? 1 : 0;
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
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > 4)
                        m_map[x, y] = 1;
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
                    if (neighbourX >= 0 && neighbourX < m_width && neighbourY >= 0 && neighbourY < m_height)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += m_map[neighbourX, neighbourY];
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
    }
}
