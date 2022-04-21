using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreation
{
    public List<int[,]> mapList;
    public MapCreation()
    {
        mapList = new List<int[,]>();
        int[,] map1 = new int[,]{{0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0 },
            {0,1,1,0,1,0,0,1,1,1,1,0,0,1,1,0 },
            {0,0,1,0,1,1,0,1,0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,1,0,1,1,0,0,1,1,0,0,0 },
            {0,1,1,0,0,0,0,0,1,0,1,1,0,0,1,0 },
            {0,1,0,0,1,2,0,0,1,0,0,0,0,0,1,0 },
            {0,0,0,0,1,1,1,0,0,0,1,2,0,0,1,0 },
            {0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0 },
            {1,1,1,0,1,0,1,0,1,1,0,1,1,0,0,0 },
            {0,0,1,0,2,0,1,0,1,1,0,0,0,0,1,1 },
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            {0,1,0,1,1,1,1,0,0,2,0,0,1,1,1,0 },
            {0,1,0,0,0,0,0,0,1,1,0,0,0,0,1,0 },
            {0,1,1,1,0,0,1,1,1,0,0,1,1,0,1,0 },
            {0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0 }};
        mapList.Add(map1);
        int[,] map2 = new int[,]{{0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0},
            {0,1,0,1,0,0,0,0,0,1,0,0,0,1,1,0 },
            {0,1,0,0,0,2,1,0,0,0,0,1,0,0,0,0 },
            {0,1,0,0,1,1,1,1,0,1,1,1,0,0,1,1 },
            {0,0,0,0,1,0,0,0,0,0,0,1,0,0,1,0 },
            {0,0,1,1,1,0,1,0,1,1,0,1,0,0,1,0 },
            {1,0,0,0,1,0,1,0,0,1,0,2,0,0,0,1 },
            {1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0 },
            {1,1,1,0,0,0,0,1,0,0,0,1,0,0,1,0 },
            {0,1,0,0,1,0,0,0,0,1,0,1,0,0,1,0 },
            {0,0,0,0,1,1,0,1,1,1,2,1,0,0,0,1 },
            {0,0,0,1,2,0,0,0,0,1,0,0,0,0,0,0 },
            {0,1,0,1,0,0,0,0,0,1,0,0,0,0,1,0 },
            {0,1,1,1,0,0,1,0,1,0,0,1,1,0,1,0 },
            {0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0 }};
        mapList.Add(map2);
        int[,] map3 = new int[,] { {0,0,1,0,0,1,0,0,0,0,1,0,1,0,0,0 },
        {0,1,0,0,0,0,0,1,0,0,0,0,0,0,1,0 },
        {0,1,0,1,1,1,0,2,1,1,0,1,0,1,0,0 },
        {0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,1 },
        {1,0,0,1,0,1,0,1,0,1,0,0,1,0,1,1 },
        {0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        {0,1,0,0,1,0,1,0,1,0,1,0,1,0,0,0 },
        {0,0,2,0,0,0,0,0,0,0,1,0,2,1,1,0 },
        {0,0,1,1,0,0,1,0,1,0,0,0,0,1,0,0 },
        {1,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1 },
        {1,0,0,0,0,1,0,1,0,1,0,0,1,0,0,0 },
        {0,0,1,1,0,1,0,0,0,0,0,0,0,1,1,0 },
        {0,0,1,0,0,0,0,2,1,0,0,1,0,0,0,0 },
        {0,1,0,0,0,0,1,0,0,0,1,0,0,0,1,0 },
        {0,0,0,1,0,0,0,0,1,0,0,1,1,0,0,0 }};
        mapList.Add(map3);
        int[,] map4 = new int[,] { {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            {0,1,1,0,1,1,1,0,1,1,0,1,0,1,1,0 },
            {0,0,1,0,0,0,0,0,1,0,0,1,0,1,0,0 },
            {1,0,0,0,1,1,0,0,0,0,0,2,0,0,0,1 },
            {1,1,2,0,0,1,0,1,1,1,0,0,0,1,1,1 },
            {0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,0 },
            {0,0,0,1,0,0,0,0,0,0,0,1,1,0,1,1 },
            {0,0,0,1,0,0,1,1,1,1,0,0,1,0,0,0 },
            {1,1,0,1,1,0,0,0,0,0,0,0,1,0,0,0 },
            {0,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0 },
            {1,1,1,0,0,0,1,1,1,0,1,0,2,1,1,1 },
            {1,0,0,0,2,0,0,0,0,0,1,1,0,0,0,1 },
            {0,0,1,0,1,0,0,1,0,0,0,0,0,1,0,0 },
            {0,1,1,0,1,0,1,1,0,1,1,1,0,1,1,0 },
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }};
        mapList.Add(map4);
        int[,] map5 = new int[,]
        {{0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0 },
        {0,1,0,1,1,0,1,0,1,0,0,0,0,1,1,0 },
        {0,1,0,0,0,0,1,0,1,0,0,1,0,0,1,0 },
        {0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0 },
        {0,0,0,2,0,0,0,0,0,1,0,2,0,0,1,0 },
        {1,1,0,1,0,0,1,0,0,0,0,0,1,0,0,0 },
        {0,0,0,0,0,1,0,0,1,0,1,0,0,0,1,1 },
        {0,0,0,1,0,0,1,0,1,0,0,0,0,0,0,0 },
        {0,1,0,0,0,0,0,0,0,1,0,1,1,1,0,0 },
        {0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1 },
        {0,1,0,1,2,0,0,1,0,0,1,0,1,0,0,0 },
        {1,0,0,0,1,1,0,1,0,1,0,0,2,0,1,0 },
        {0,0,1,0,0,0,0,1,0,0,0,1,1,0,1,0 },
        {0,1,0,0,0,1,0,0,0,1,0,0,1,0,1,0 },
        {0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0 }};
        mapList.Add(map5);
    }
    public int[,] RandomMap()
    {
        #region
        /*int[,] map = new int[height, width];//1:墙 0:通路
        bool[,] isVisited = new bool[height, width];

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(!((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0) || (i == width - 1 && j == 0) || (i == width - 2 && j == 0) || (i == width - 1 && j == 1) || (i == 0 && j == height - 1) || (i == 0 && j == height - 2) || (i == 1 && j == height - 1) || (i == width-1 && j==height-1) || (i == width-1 && j == height-2) || (i == width-2 && j == height-1)))
                {
                    if ((i == 1 && j == 1) || (i == 1 && j == height - 2) || (i == width - 2 && j == 1) || (i == width - 2 && j == height - 2))
                    {
                        map[j, i] = 1;
                    }
                    else
                    {
                        if (Random.Range(0, 100) < 40)
                            map[j, i] = 1;
                    }
                    isVisited[j, i] = false;
                    
                }
            }
        }

        

        //如果一个空格周围存在三个以上墙,去掉一部分直到小于3
        List<int> adjacentWall = new List<int>();//相邻的墙,1:up 2:left 3:down 4:right
        for(int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j < height - 1; j++)
            {
                if (map[j, i] == 0)
                {
                    adjacentWall.Clear();
                    int roundBlank = 0;
                    int roundWall = 0;

                    if (map[j - 1, i] == 1)
                        adjacentWall.Add(1);
                    if (map[j + 1, i] == 1)
                        adjacentWall.Add(3);
                    if (map[j, i - 1] == 1)
                        adjacentWall.Add(2);
                    if (map[j, i + 1] == 1)
                        adjacentWall.Add(4);

                    for(int tx = i - 1; tx < i + 2; tx++)
                    {
                        for(int ty = j - 1; ty < j + 2; ty++)
                        {
                            if (map[ty, tx] == 0)
                                roundBlank++;
                            else
                                roundWall++;
                        }
                    }

                    //if (roundBlank > 6)
                    //{
                    //    map[j, i] = 1;
                    //    continue;
                    //}
                    if(roundWall > 6)
                    {
                        map[j, i] = 0;
                    }


                    while (adjacentWall.Count >= 3)
                    {
                        int index = Random.Range(0, adjacentWall.Count);
                        if (adjacentWall[index] == 1)
                        {
                            if ((i == 1 && j - 1 == 1) || (i == 1 && j - 1 == height - 2) || (i == width - 2 && j - 1 == 1) || (i == width - 2 && j - 1 == height - 2))
                                continue;
                            map[j - 1, i] = 0;
                        }
                            
                        if (adjacentWall[index] == 2)
                        {
                            if ((i - 1 == 1 && j == 1) || (i - 1 == 1 && j == height - 2) || (i - 1 == width - 2 && j == 1) || (i - 1 == width - 2 && j == height - 2))
                                continue;
                            map[j, i - 1] = 0;
                        }
                            
                        if (adjacentWall[index] == 3)
                        {
                            if ((i == 1 && j + 1 == 1) || (i == 1 && j + 1 == height - 2) || (i == width - 2 && j + 1 == 1) || (i == width - 2 && j + 1 == height - 2))
                                continue;
                            map[j + 1, i] = 0;
                        }
                            
                        if (adjacentWall[index] == 4)
                        {
                            if ((i + 1 == 1 && j == 1) || (i + 1 == 1 && j == height - 2) || (i + 1 == width - 2 && j == 1) || (i + 1 == width - 2 && j == height - 2))
                                continue;
                            map[j, i + 1] = 0;
                        }
                            
                        adjacentWall.RemoveAt(index);
                    }
                }
            }
        }

        //打通所有不连通的区域
        //先划分区域
        List<Coord> roadList = new List<Coord>();
        List<Area> areaList = new List<Area>();
        for(int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j < height - 1; j++)
            {
                if (map[j, i] == 0)
                    roadList.Add(new Coord(i, j));
            }
        }

        while (roadList.Count > 0)
        {
            if(roadList.Count == 1)
            {
                Area area = new Area();
                area.coordList = new List<Coord>();
                area.coordList.Add(roadList[0]);
                areaList.Add(area);
                break;
            }
            else
            {
                Area area = new Area();
                area.coordList = new List<Coord>();
                area.coordList.Add(roadList[0]);
                for (int i = 0; i < area.coordList.Count; i++)
                {
                    int x = area.coordList[i].x;
                    int y = area.coordList[i].y;
                    isVisited[y, x] = true;
                    if (x + 1 < width && map[y, x + 1] == 0 && isVisited[y, x + 1] == false)
                    {
                        area.coordList.Add(new Coord(x + 1, y));
                    }
                    if (x - 1 >= 0 && map[y, x - 1] == 0 && isVisited[y, x - 1] == false)
                    {
                        area.coordList.Add(new Coord(x - 1, y));
                    }
                    if (y + 1 < height && map[y + 1, x] == 0 && isVisited[y + 1, x] == false)
                    {
                        area.coordList.Add(new Coord(x, y + 1));
                    }
                    if (y - 1 > 0 && map[y - 1, x] == 0 && isVisited[y - 1, x] == false)
                    {
                        area.coordList.Add(new Coord(x, y - 1));
                    }
                }
                foreach (Coord m in area.coordList)
                {
                    foreach (Coord ma in roadList)
                    {
                        if (m.Equals(ma))
                        {
                            roadList.Remove(ma);
                            break;
                        }
                    }
                }
                areaList.Add(area);
            }
        }
        //再将区域打通
        while (areaList.Count > 1)
        {
            int tempdis = 99;
            Coord start = new Coord(0, 0), end = start;
            foreach (Coord m in areaList[0].coordList)
            {
                foreach (Coord ma in areaList[1].coordList)
                {
                    if (Mathf.Abs(m.x - ma.x) + Mathf.Abs(m.y - ma.y) < tempdis)
                    {
                        tempdis = Mathf.Abs(m.x - ma.x) + Mathf.Abs(m.y - ma.y);
                        start = m;
                        end = ma;
                    }
                }
            }
            if (start.x < end.x)
            {
                for (int x = start.x; x < end.x; x++)
                {
                    map[start.y, x] = 0;
                }
                if (start.y < end.y)
                {
                    for (int y = start.y; y < end.y; y++)
                    {
                        map[y, end.x] = 0;
                    }
                }
                else
                {
                    for (int y = start.y; y > end.y; y--)
                    {
                        map[y, end.x] = 0;
                    }
                }
            }
            else
            {
                for (int x = start.x; x > end.x; x--)
                {
                    map[start.y, x] = 0;
                }
                if (start.y < end.y)
                {
                    for (int y = start.y; y < end.y; y++)
                    {
                        map[y, end.x] = 0;
                    }
                }
                else
                {
                    for (int y = start.y; y > end.y; y--)
                    {
                        map[y, end.x] = 0;
                    }
                }
            }
            foreach (Coord m in areaList[1].coordList)
            {
                areaList[0].coordList.Add(m);
            }
            areaList.Remove(areaList[1]);
        }


        return map;*/
        #endregion
        int index = Random.Range(0, mapList.Count);
        return mapList[index];

    }

}
