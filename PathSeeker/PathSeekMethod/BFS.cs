using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathSeeker.PathSeekMethod
{
    public class BFS_Manhattan
    {
        int size;
        int[,] map;
        List<IntDouble> records_passedblocks;
        IntDouble[,] pathmap;
        int[,] distanceMap;
        List<IntDouble> path;
        IntDouble start;
        IntDouble end;
        public BFS_Manhattan(int[,]map,int size)
        {
            this.map = map;
            this.size = size;
            this.start = GetStartPos();
            this.end = GetEndPos();
            InitDistanceMap();
        }
        private void InitDistanceMap()
        {
            distanceMap = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    distanceMap[i, j] = int.MaxValue;
                }
            }
        }
        private IntDouble GetStartPos()
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (map[i, j] == 2)
                        return new IntDouble(i, j);
            throw new Exception();
        }
        private IntDouble GetEndPos()
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (map[i, j] == 3)
                        return new IntDouble(i, j);
            throw new Exception();
        }
        private void FindShortestPath()
        {
            bool[,] visited = new bool[size, size];
            pathmap = new IntDouble[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    pathmap[i, j].X = -1; pathmap[i, j].Y = -1;
                    visited[i, j] = false;
                }
            }
            records_passedblocks = new List<IntDouble>();
            Queue<IntDouble> queue = new Queue<IntDouble>();
            queue.Enqueue(start);
            visited[start.X, start.Y] = true;
            distanceMap[start.X, start.Y] = 0;
            IntDouble[] aroundPArray = new IntDouble[4];
            path = new List<IntDouble>();
            while (queue.Count != 0)
            {
                IntDouble p = queue.Dequeue();
                InitAroundPos(aroundPArray,p);
                for (int i = 0; i < aroundPArray.Length; i++)
                {
                    IntDouble t = aroundPArray[i];
                    if (t.X == end.X && t.Y == end.Y)
                    {
                        pathmap[t.X, t.Y].X = p.X;
                        pathmap[t.X, t.Y].Y = p.Y;
                        distanceMap[t.X, t.Y] = distanceMap[p.X, p.Y] + 10;
                        IntDouble cur = p;
                        path.Add(end);
                        while (!(cur.X == start.X && cur.Y == start.Y))
                        {
                            path.Add(cur);
                            cur = pathmap[cur.X, cur.Y];
                        }
                        path.Add(start);
                        path.Reverse();
                        return;
                    }
                    if (InMap(t.X, t.Y) && !visited[t.X, t.Y]&&map[t.X,t.Y]==0)
                    {
                        visited[t.X, t.Y] = true;
                        distanceMap[t.X, t.Y] = distanceMap[p.X, p.Y] + 10;
                        queue.Enqueue(t);
                        pathmap[t.X, t.Y].X = p.X;
                        pathmap[t.X, t.Y].Y = p.Y;
                        records_passedblocks.Add(t);
                    }
                   
                }
            }
        }
        private void InitAroundPos(IntDouble[] aroundPArray, IntDouble p)
        {
            aroundPArray[0].X = p.X - 1;
            aroundPArray[0].Y = p.Y;
            aroundPArray[1].X = p.X + 1;
            aroundPArray[1].Y = p.Y;
            aroundPArray[2].X = p.X;
            aroundPArray[2].Y = p.Y - 1;
            aroundPArray[3].X = p.X;
            aroundPArray[3].Y = p.Y + 1;
            if (aroundPArray.Length > 4)
            {
                aroundPArray[4].X = p.X - 1;
                aroundPArray[4].Y = p.Y - 1;
                aroundPArray[5].X = p.X + 1;
                aroundPArray[5].Y = p.Y + 1;
                aroundPArray[6].X = p.X + 1;
                aroundPArray[6].Y = p.Y - 1;
                aroundPArray[7].X = p.X - 1;
                aroundPArray[7].Y = p.Y + 1;
            }
        }
        private bool InMap(int x, int y)
        {
            return x >= 0 && x < size && y >= 0 && y < size;
        }
        public ResultCollection GetResult()
        {
            this.FindShortestPath();
            ResultCollection ret = new ResultCollection();
            ret.inputMap = map;
            int[,] retMap = new int[size, size];
            int[,] parentMap = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    retMap[i, j] = map[i, j];
                    parentMap[i, j] = -1;
                }
            }
            for (int i = 0; i < records_passedblocks.Count; i++)
            {
                IntDouble p = records_passedblocks[i];
                if (map[p.X, p.Y] == 0)
                    retMap[p.X, p.Y] = 5;
            }
            for (int i = 0; i < path.Count; i++)
            {
                IntDouble p = path[i];
                if (map[p.X, p.Y] == 0)
                    retMap[p.X, p.Y] = 4;
            }
            
            ret.resultMap = retMap;
            for (int i = 0; i < records_passedblocks.Count; i++)
            {
                IntDouble p = records_passedblocks[i];
                IntDouble parentPos = pathmap[p.X, p.Y];
                int index = Consts.OffsetToIndex[parentPos.X - p.X + 1, parentPos.Y - p.Y + 1];
                parentMap[p.X, p.Y] = index;
            }
            int index2 = Consts.OffsetToIndex[pathmap[end.X, end.Y].X - end.X + 1, pathmap[end.X, end.Y].Y - end.Y + 1];
            parentMap[end.X, end.Y] = index2;
            ret.parentMap = parentMap ;
            ret.distanceMap = distanceMap;
            return ret;
        }
    }
}
