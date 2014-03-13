using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathSeeker.PathSeekMethod
{
    class AstarSet_Heap
    {
        private List<IntDouble> heapArray;
        private int[,] values;
        private int[,] setIndexMap;// stores the index to heapArray for each vertexIndex, -1 if not exist
        public AstarSet_Heap(int maxsize, int[,] values)
        {
            this.heapArray = new List<IntDouble>();
            this.values = values;
            this.setIndexMap = Consts.Init2dArray<int>(maxsize, -1);
        }
        public void Add(IntDouble pindex)
        {
            this.heapArray.Add(pindex);
            setIndexMap[pindex.X, pindex.Y] = heapArray.Count - 1;
            ShiftUp(heapArray.Count - 1);
        }
        public IntDouble ExtractMin()
        {
            if (GetCount() == 0)
                return new IntDouble(-1, -1);
            IntDouble pindex = heapArray[0];
            Swap(0, heapArray.Count - 1);
            heapArray.RemoveAt(heapArray.Count - 1);
            ShiftDown(0);
            setIndexMap[pindex.X, pindex.Y] = -1;
            return pindex;
        }
        public int GetCount()
        {
            return heapArray.Count;
        }
        public void UpdateKey(IntDouble pindex,int oldValue)
        {
            if (values[pindex.X, pindex.Y] > oldValue)
                ShiftDown(setIndexMap[pindex.X, pindex.Y]);
            else
                ShiftUp(setIndexMap[pindex.X, pindex.Y]);
        }
        public bool Exist(IntDouble pindex)
        {
            return setIndexMap[pindex.X, pindex.Y] != -1;
        }
        private int GetParent(int index)
        {
            return (index - 1) / 2;
        }
        private int GetLeftChild(int index)
        {
            return 2 * index + 1;
        }
        private int GetRightChild(int index)
        {
            return 2 * index + 2;
        }
        private bool IsLessThan(int index0, int index1)
        {
            return values[heapArray[index0].X, heapArray[index0].Y] < values[heapArray[index1].X, heapArray[index1].Y];
        }
        private void ShiftUp(int i)
        {
            if (i == 0)
                return;
            else
            {
                int parent = GetParent(i);
                if (IsLessThan(i, parent))
                {
                    Swap(i, parent);
                    ShiftUp(parent);
                }
            }
        }
        private void ShiftDown(int i)
        {
            if (i >= GetCount()) return;
            int min = i;
            int lc = GetLeftChild(i);
            int rc = GetRightChild(i);
            if (lc < GetCount() && IsLessThan(lc, min))
                min = lc;
            if (rc < GetCount() && IsLessThan(rc, min))
                min = rc;
            if (min != i)
            {
                Swap(i, min);
                ShiftDown(min);
            }
        }
        private void Swap(int i, int j)
        {
            IntDouble temp = heapArray[i];
            heapArray[i] = heapArray[j];
            heapArray[j] = temp;
            setIndexMap[heapArray[i].X, heapArray[i].Y] = i;//record new position
            setIndexMap[heapArray[j].X, heapArray[j].Y] = j;//record new position
        }
    }
    public class AStarProcessor
    {
        int[,] map;
        int size;
        List<IntDouble> tempList;
        IntDouble[] tempArray;
        int[,] gMap;
        int[,] fMap;
        IntDouble[,] prev;
        bool[,] flagMap_Close;
        bool[,] visited;
        AstarSet_Heap set_Open;
        IntDouble start;
        IntDouble end;
        List<IntDouble> resultPath;
        public AStarProcessor(int[,] map, int size)
        {
            this.map = map;
            this.size = size;
            this.tempList = new List<IntDouble>();
            this.tempArray = new IntDouble[8];
            this.resultPath = new List<IntDouble>();
            this.start = GetStartPos();
            this.end = GetEndPos();
            //this.InitEvaDistanceMap();
            this.prev = Consts.Init2dArray<IntDouble>(size, new IntDouble(-1, -1));
            this.flagMap_Close = Consts.Init2dArray<bool>(size, false);
            this.gMap = Consts.Init2dArray<int>(size, int.MaxValue);
            this.fMap = Consts.Init2dArray<int>(size, int.MaxValue);
            this.visited = Consts.Init2dArray<bool>(size, false);
            this.set_Open = new AstarSet_Heap(size * size, fMap);
        }
        public ResultCollection GetResult()
        {
            ResultCollection ret = new ResultCollection();
            this.FindShortestPath();
            resultPath = GetPath();
            ret.distanceMap = this.gMap;
            int[,] retMap = Consts.Init2dArray<int>(size, 0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (map[i, j] == 1)
                        retMap[i, j] = 1;
                    if (map[i, j] == 2)
                        retMap[i, j] = 2;
                    if (map[i, j] == 3)
                        retMap[i, j] = 3;
                    if (map[i, j] == 0 && visited[i, j])
                        retMap[i, j] = 5;
                }
            }
            for (int i = 0; i < resultPath.Count; i++)
            {
                IntDouble p = resultPath[i];
                if (retMap[p.X, p.Y] == 5)
                    retMap[p.X, p.Y] = 4;
            }
            ret.resultMap = retMap;
            ret.inputMap = map;
            int[,] parent = Consts.Init2dArray<int>(size, -1);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    IntDouble parentDir = prev[i, j];
                    if (parentDir.X != -1 && parentDir.Y != -1)
                    {
                        int index = Consts.OffsetToIndex[parentDir.X - i + 1, parentDir.Y - j + 1];
                        parent[i, j] = index;
                    }
                    else
                        parent[i, j] = -1;
                }
            }
            ret.parentMap = parent;
            ret.evaDistanceMap = new int[size,size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ret.evaDistanceMap[i,j] = GetH(new IntDouble(i, j));
                }
            }
            return ret;
        }
        private bool FindShortestPath()
        {
            set_Open.Add(start);
            gMap[start.X, start.Y] = 0;
            while (set_Open.GetCount() != 0)
            {
                IntDouble pindexnewlyfound = set_Open.ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
                flagMap_Close[pindexnewlyfound.X, pindexnewlyfound.Y] = true;//mark it
                if (pindexnewlyfound.X == end.X && pindexnewlyfound.Y == end.Y)
                    return true;
                UpdateMinDistance(pindexnewlyfound);// update its neighbour's s-distance
            }
            return false;
        }
        private int GetH(IntDouble p)
        {
            return Consts.GetEvaDistance_Euro(p.X, p.Y, end.X, end.Y);
        }
        private List<IntDouble> GetPath()
        {
            IntDouble cur = end;
            while (!(cur.X == start.X && cur.Y == start.Y))
            {
                resultPath.Add(cur);
                cur = prev[cur.X, cur.Y];
            }
            resultPath.Add(start);
            resultPath.Reverse();
            return resultPath;
        }
        private void UpdateMinDistance(IntDouble newlyfoundpIndex)
        {
            List<IntDouble> nlist = GetNeigboursList(newlyfoundpIndex.X, newlyfoundpIndex.Y);
            for (int i = 0; i < nlist.Count; i++)
            {
                IntDouble nindex = nlist[i];
                visited[nindex.X, nindex.Y] = true;
                if (flagMap_Close[nindex.X, nindex.Y])
                {
                    continue;
                }
                int gPassp = gMap[newlyfoundpIndex.X, newlyfoundpIndex.Y] + GetWeight(newlyfoundpIndex.X, newlyfoundpIndex.Y, nindex.X, nindex.Y);
                if (set_Open.Exist(nindex))
                {
                    if (gPassp < gMap[nindex.X, nindex.Y])
                    {
                        int oldvalue=fMap[nindex.X, nindex.Y];
                        gMap[nindex.X, nindex.Y] = gPassp;
                        fMap[nindex.X, nindex.Y] = gPassp + GetH(nindex);
                        set_Open.UpdateKey(nindex,oldvalue);
                        prev[nindex.X, nindex.Y] = newlyfoundpIndex;
                   }
                }
                else
                {
                    gMap[nindex.X, nindex.Y] = gPassp;
                    fMap[nindex.X, nindex.Y] = gPassp + GetH(nindex);
                    prev[nindex.X, nindex.Y] = newlyfoundpIndex;
                    set_Open.Add(nindex);
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
        private int GetWeight(int x, int y, int nx, int ny)
        {
            int dx = nx - x + 1;
            int dy = ny - y + 1;
            if (dx >= 0 && dx < 3 && dy >= 0 && dy < 3)
            {
                if (map[nx, ny] != 1)
                {
                    int w = Consts.OffsetToEurWeight[dx, dy];
                    return w;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception("ca");
            }
        }
        private List<IntDouble> GetNeigboursList(int x, int y)
        {
            tempList.Clear();
            InitAroundPos(tempArray, new IntDouble(x, y));
            for (int i = 0; i < 4; i++)
            {
                if (InMap(tempArray[i].X, tempArray[i].Y) && map[tempArray[i].X, tempArray[i].Y] != 1)
                {
                    tempList.Add(tempArray[i]);
                }
            }
            if (InMap(tempArray[4].X, tempArray[4].Y) && map[tempArray[4].X, tempArray[4].Y] != 1 && !(map[tempArray[0].X, tempArray[0].Y] == 1 && map[tempArray[2].X, tempArray[2].Y] == 1))
            {
                tempList.Add(tempArray[4]);
            }
            if (InMap(tempArray[5].X, tempArray[5].Y) && map[tempArray[5].X, tempArray[5].Y] != 1 && !(map[tempArray[1].X, tempArray[1].Y] == 1 && map[tempArray[2].X, tempArray[2].Y] == 1))
            {
                tempList.Add(tempArray[5]);
            }
            if (InMap(tempArray[6].X, tempArray[6].Y) && map[tempArray[6].X, tempArray[6].Y] != 1 && !(map[tempArray[0].X, tempArray[0].Y] == 1 && map[tempArray[3].X, tempArray[3].Y] == 1))
            {
                tempList.Add(tempArray[6]);
            }
            if (InMap(tempArray[7].X, tempArray[7].Y) && map[tempArray[7].X, tempArray[7].Y] != 1 && !(map[tempArray[1].X, tempArray[1].Y] == 1 && map[tempArray[3].X, tempArray[3].Y] == 1))
            {
                tempList.Add(tempArray[7]);
            }


            return tempList;
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
                aroundPArray[5].Y = p.Y - 1;
                aroundPArray[6].X = p.X - 1;
                aroundPArray[6].Y = p.Y + 1;
                aroundPArray[7].X = p.X + 1;
                aroundPArray[7].Y = p.Y + 1;
            }
        }
        private bool InMap(int x, int y)
        {
            return x >= 0 && x < size && y >= 0 && y < size;
        }

    }
}
