using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededGrowSMC
{
class Method
{
    public static void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
    {
        adjPoints6[0].X = p.X;
        adjPoints6[0].Y = p.Y + 1;
        adjPoints6[0].Z = p.Z;

        adjPoints6[1].X = p.X;
        adjPoints6[1].Y = p.Y - 1;
        adjPoints6[1].Z = p.Z;

        adjPoints6[2].X = p.X + 1;
        adjPoints6[2].Y = p.Y;
        adjPoints6[2].Z = p.Z;

        adjPoints6[3].X = p.X - 1;
        adjPoints6[3].Y = p.Y;
        adjPoints6[3].Z = p.Z;

        adjPoints6[4].X = p.X;
        adjPoints6[4].Y = p.Y;
        adjPoints6[4].Z = p.Z + 1;

        adjPoints6[5].X = p.X;
        adjPoints6[5].Y = p.Y;
        adjPoints6[5].Z = p.Z - 1;
    }
    public static Mesh ExcuteMethod(BitMap3d bmp, Int16Triple seed)
    {
        #region FloodFill
        int width=bmp.width;
        int height=bmp.height;
        int depth=bmp.depth;
        byte[] data=bmp.data;
        MeshBuilder_IntegerVertex mb = new MeshBuilder_IntegerVertex(bmp.width + 2, bmp.height + 2, bmp.depth + 2);
        HashTable_Double2dArray<bool> CellhashMap = new HashTable_Double2dArray<bool>(bmp.width + 2, bmp.height + 2, bmp.depth + 2);

        BitArray flagsMap = new BitArray(width * height * depth, false);
        Queue<Int16Triple> queue = new Queue<Int16Triple>();
        Int16Triple[] adjPoints6 = new Int16Triple[6];
        bool temp=false;
        int stindex = seed.Z * width * height + seed.Y * width + seed.X;
        flagsMap[stindex] = true;
        queue.Enqueue(seed);
        while (queue.Count != 0)
        {
            byte adjState = 0;
            Int16Triple p = queue.Dequeue();
            InitAdj6(ref adjPoints6, ref p);
            for (int adjIndex = 0; adjIndex < adjPoints6.Length; adjIndex++)
            {
                Int16Triple t = adjPoints6[adjIndex];
                if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                {
                    int indext = t.Z * width * height + t.Y * width + t.X;
                    if (!flagsMap[indext])
                    {
                        if (data[indext]==BitMap3d.WHITE)
                        {
                            flagsMap[indext] = true;
                            adjState |= Tables.Adj6IndexToFilledState[adjIndex];
                            queue.Enqueue(t);
                        }
                    }
                    else
                    {
                        adjState |= Tables.Adj6IndexToFilledState[adjIndex];
                    }
                }
            }
            if (adjState != 63)
            {
                int[] qIndex = Tables.AdjStateToQuadrantIndices[adjState];
                for (int j = 0; j < qIndex.Length; j++)
                {
                    Int16Triple delta = Tables.QuadrantIndexToCellIndices[qIndex[j]];
                    int cellX = delta.X + p.X ;
                    int cellY = delta.Y + p.Y;
                    int cellZ = delta.Z + p.Z;
                    if (cellX >= 0 && cellY >= 0 && cellZ >= 0)
                    {
                        bool innerIndex = CellhashMap.GetHashValue(cellX, cellY, cellZ, ref temp);
                        if (!innerIndex)
                        {
                            CellhashMap.SetHashValue(cellX, cellY, cellZ, true);
                        }
                    }
                }
            }
        }
        #endregion
        #region SMC
        List<Int16TripleWithTValue<bool>> cubesList = CellhashMap.GetAllKeyValues();
        Int16Triple[] tempSet = new Int16Triple[8];
        for (int i = 0; i < cubesList.Count; i++)
        {
            int indexInWidth = cubesList[i].X;
            int indexInHeight = cubesList[i].Y;
            int indexInDepth = cubesList[i].Z;
            byte value = 0;
            for (int pi = 0; pi < 8; pi++)
            {
                tempSet[pi].X = indexInWidth + Tables.PointIndexToPointDelta[pi].X;
                tempSet[pi].Y = indexInHeight + Tables.PointIndexToPointDelta[pi].Y;
                tempSet[pi].Z = indexInDepth + Tables.PointIndexToPointDelta[pi].Z;
                if (tempSet[pi].X>=0&&tempSet[pi].X<width&&tempSet[pi].Y>=0&&tempSet[pi].Y<height&&tempSet[pi].Z>=0&&tempSet[pi].Z<depth
                    &&flagsMap[tempSet[pi].X + tempSet[pi].Y * width + tempSet[pi].Z * height * width])
                {
                    value |= Tables.PointIndexToFlag[pi];
                }
            }
            if (Tables.TableFat[value, 0] != -1)
            {
                int index = 0;
                while (Tables.TableFat[value, index] != -1)
                {
                    Int16Triple t0 = tempSet[Tables.TableFat[value, index]];
                    Int16Triple t1 = tempSet[Tables.TableFat[value, index + 1]];
                    Int16Triple t2 = tempSet[Tables.TableFat[value, index + 2]];
                    mb.AddTriangle(t0, t1, t2);
                    index += 3;
                }
            }
        }

        return mb.GetMesh();
        #endregion
    }
}
    class Method2
    {
        public static void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
        {
            adjPoints6[0].X = p.X;
            adjPoints6[0].Y = p.Y + 1;
            adjPoints6[0].Z = p.Z;

            adjPoints6[1].X = p.X;
            adjPoints6[1].Y = p.Y - 1;
            adjPoints6[1].Z = p.Z;

            adjPoints6[2].X = p.X + 1;
            adjPoints6[2].Y = p.Y;
            adjPoints6[2].Z = p.Z;

            adjPoints6[3].X = p.X - 1;
            adjPoints6[3].Y = p.Y;
            adjPoints6[3].Z = p.Z;

            adjPoints6[4].X = p.X;
            adjPoints6[4].Y = p.Y;
            adjPoints6[4].Z = p.Z + 1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z - 1;
        }
        public static Mesh ExcuteMethod2(BitMap3d bmp, Int16Triple seed)
        {
            #region FloodFill
            int width = bmp.width;
            int height = bmp.height;
            int depth = bmp.depth;
            byte[] data = bmp.data;
            MeshBuilder_IntegerVertex mb = new MeshBuilder_IntegerVertex(bmp.width + 2, bmp.height + 2, bmp.depth + 2);

            BitArray flagsMap = new BitArray(width * height * depth, false);
            Queue<Int16Triple> queue = new Queue<Int16Triple>();
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            int stindex = seed.Z * width * height + seed.Y * width + seed.X;
            flagsMap[stindex] = true;
            queue.Enqueue(seed);
            while (queue.Count != 0)
            {
                Int16Triple p = queue.Dequeue();
                InitAdj6(ref adjPoints6, ref p);
                for (int adjIndex = 0; adjIndex < adjPoints6.Length; adjIndex++)
                {
                    Int16Triple t = adjPoints6[adjIndex];
                    if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                    {
                        int indext = t.Z * width * height + t.Y * width + t.X;
                        if (!flagsMap[indext])
                        {
                            if (data[indext] == BitMap3d.WHITE)
                            {
                                flagsMap[indext] = true;
                                queue.Enqueue(t);
                            }
                        }
                    }
                }
            }
            #endregion
            #region SMC
            Int16Triple[] tempSet = new Int16Triple[8];
            for (int k = 0; k < depth - 1; k++)
            {
                for (int j = 0; j < height - 1; j++) 
                {
                    for (int i = 0; i < width - 1; i++)
                    {
                        int indexInWidth = i;
                        int indexInHeight = j;
                        int indexInDepth = k;
                        byte value = 0;
                        for (int pi = 0; pi < 8; pi++)
                        {
                            tempSet[pi].X = indexInWidth + Tables.PointIndexToPointDelta[pi].X;
                            tempSet[pi].Y = indexInHeight + Tables.PointIndexToPointDelta[pi].Y;
                            tempSet[pi].Z = indexInDepth + Tables.PointIndexToPointDelta[pi].Z;
                            if (tempSet[pi].X >= 0 && tempSet[pi].X < width && tempSet[pi].Y >= 0 && tempSet[pi].Y < height && tempSet[pi].Z >= 0 && tempSet[pi].Z < depth
                                && flagsMap[tempSet[pi].X + tempSet[pi].Y * width + tempSet[pi].Z * height * width])
                            {
                                value |= Tables.PointIndexToFlag[pi];
                            }
                        }
                        if (value == 255 || value == 0)
                            continue;
                        if (Tables.TableFat[value, 0] != -1)
                        {
                            int index = 0;
                            while (Tables.TableFat[value, index] != -1)
                            {
                                Int16Triple t0 = tempSet[Tables.TableFat[value, index]];
                                Int16Triple t1 = tempSet[Tables.TableFat[value, index + 1]];
                                Int16Triple t2 = tempSet[Tables.TableFat[value, index + 2]];
                                mb.AddTriangle(t0, t1, t2);
                                index += 3;
                            }
                        }
                    }
                }
            }

            return mb.GetMesh();
            #endregion
        }
    }
}
