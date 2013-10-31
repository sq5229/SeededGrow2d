using System;
using System.Collections.Generic;
using System.Text;

namespace SMC.SMC
{
    public class SMCTriangleNetHashTable
    {
        public int CurrentLayerIndex;

        int stx;
        int sty;
        int width;
        int height;
        List<int[,]> mapList;
        public SMCTriangleNetHashTable(int minx, int miny, int width, int height)
        {
            this.stx = minx - 1;
            this.sty = miny - 1;
            this.width = width + 2;
            this.height = height + 2;
            mapList = new List<int[,]>(2);
            mapList.Add(new int[this.width, this.height]);
            mapList.Add(new int[this.width, this.height]);
            SetDefaultValue(0);
            SetDefaultValue(1);
        }
        public void SetDefaultValue(int index0_1)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mapList[index0_1][i, j] = -1;
                }
            }
        }
        public void IncreaseIndex()
        {
            CurrentLayerIndex++;
            SetDefaultValue(0);
            int[,] temp = mapList[0];
            mapList[0] = mapList[1];
            mapList[1] = temp;
        }
        public void SetHashValue(int x, int y, int z, int value)
        {
            int index0_1 = z - CurrentLayerIndex;
            mapList[index0_1][x - stx, y - sty] = value;
        }
        public int GetHashValue(int x, int y, int z)
        {
            int index0_1 = z - CurrentLayerIndex;
            return mapList[index0_1][x - stx, y - sty];
        }
    }
    public class AdSMCProcessor
    {
        private struct OriginalTriangle
        {
            public Int16Triple P0;
            public Int16Triple P1;
            public Int16Triple P2;
            public OriginalTriangle(int p0x, int p0y, int p0z, int p1x, int p1y, int p1z, int p2x, int p2y, int p2z)
            {
                P0.X = p0x;
                P0.Y = p0y;
                P0.Z = p0z;
                P1.X = p1x;
                P1.Y = p1y;
                P1.Z = p1z;
                P2.X = p2x;
                P2.Y = p2y;
                P2.Z = p2z;
            }
        }
         BitMap3d bmp;
         int d;
         int h;
         int w;
         int wh;
         public AdSMCProcessor(BitMap3d bitmap)
         {
             this.bmp = bitmap;
         }
        public Mesh GenerateSurface()
         {
             d = bmp.depth;
             h = bmp.height;
             w = bmp.width;
             wh = w * h;
             Int16Triple[] temp = new Int16Triple[8];
             Mesh m = new Mesh();
             OriginalTriangle[] tempTriangles = new OriginalTriangle[4];
             SMCTriangleNetHashTable hash = new SMCTriangleNetHashTable(0,0,w,h);

             for (int k = 0; k <= d - 1; k++)
             {
                 for (int j = 0; j <= h - 1; j++)
                 {
                     for (int i = 0; i <= w - 1; i++)
                     {
                         byte value = GetConfig(temp, bmp, i, j , k );
                         if (value == 0 || value == 255)
                             continue;
                         int tcount = ExtractTriangles(temp, value, i, j , k, tempTriangles);
                         for (int tindex = 0; tindex < tcount; tindex++)
                         {
                             MergeTriangleIntoMesh(m, hash, tempTriangles[tindex]);
                         }
                     }
                 }
                 hash.IncreaseIndex();
             }
             return m;
         }

        private byte GetConfig(Int16Triple[] temp, BitMap3d flagsMap, int indexInWidth, int indexInHeight, int indexInDepth)
        {
            byte value = 0;
            for (int pi = 0; pi < 8; pi++)
            {
                temp[pi].X = indexInWidth + Cube.PointIndexToPointDelta[pi].X;
                temp[pi].Y = indexInHeight + Cube.PointIndexToPointDelta[pi].Y;
                temp[pi].Z = indexInDepth + Cube.PointIndexToPointDelta[pi].Z;
                if (temp[pi].X < w && temp[pi].X >= 0
                    && temp[pi].Y < h && temp[pi].Y >= 0
                    && temp[pi].Z < d && temp[pi].Z >= 0
                    && bmp.data[temp[pi].X + w * (temp[pi].Y) + wh * (temp[pi].Z)]==BitMap3d.WHITE)
                {
                    value |= Cube.PointIndexToFlag[pi];
                }
            }
            return value;
        }

        private int ExtractTriangles(Int16Triple[] temp, byte value, int indexInWidth, int indexInHeight, int indexInDepth, OriginalTriangle[] result)
        {
            int tcount = 0;
            if (SMCTable.TableFat[value, 0] != -1)
            {
                int index = 0;
                while (SMCTable.TableFat[value, index] != -1)
                {
                    Int16Triple t0 = temp[SMCTable.TableFat[value, index]];
                    Int16Triple t1 = temp[SMCTable.TableFat[value, index + 1]];
                    Int16Triple t2 = temp[SMCTable.TableFat[value, index + 2]];
                    result[tcount] = new OriginalTriangle(t0.X , t0.Y , t0.Z , t1.X , t1.Y , t1.Z , t2.X , t2.Y , t2.Z );
                    tcount++;
                    index += 3;
                }
            }
            return tcount;
        }

        private void MergeTriangleIntoMesh(Mesh mesh, SMCTriangleNetHashTable hashMap, OriginalTriangle ot)
        {
            int p0x = ot.P0.X;
            int p0y = ot.P0.Y;
            int p0z = ot.P0.Z;
            int p1x = ot.P1.X;
            int p1y = ot.P1.Y;
            int p1z = ot.P1.Z;
            int p2x = ot.P2.X;
            int p2y = ot.P2.Y;
            int p2z = ot.P2.Z;
            int p0i;
            int p1i;
            int p2i;
            int index = 0;
            index = hashMap.GetHashValue(p0x, p0y, p0z);
            if (index == -1)
            {
                p0i = mesh.AddVertex(new Point3d(p0x, p0y, p0z));
                hashMap.SetHashValue(p0x, p0y, p0z, p0i);
            }
            else
            {
                p0i = index;
            }

            index = hashMap.GetHashValue(p1x, p1y, p1z);
            if (index == -1)
            {
                p1i = mesh.AddVertex(new Point3d(p1x, p1y, p1z));
                hashMap.SetHashValue(p1x, p1y, p1z, p1i);
            }
            else
            {
                p1i = index;
            }

            index = hashMap.GetHashValue(p2x, p2y, p2z);
            if (index == -1)
            {
                p2i = mesh.AddVertex(new Point3d(p2x, p2y, p2z));
                hashMap.SetHashValue(p2x, p2y, p2z, p2i);
            }
            else
            {
                p2i = index;
            }

            Triangle t = new Triangle(p0i, p1i, p2i);
            mesh.AddFace(t);
        }
    }
}
