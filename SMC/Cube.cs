using System;
using System.Collections.Generic;
using System.Text;

namespace SMC
{
public struct Int16Triple
{
    public int X;
    public int Y;
    public int Z;
    public Int16Triple(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
class Cube
{
    public static byte VULF = 1 << 0;
    public static byte VULB = 1 << 1;
    public static byte VLLB = 1 << 2;
    public static byte VLLF = 1 << 3;
    public static byte VURF = 1 << 4;
    public static byte VURB = 1 << 5;
    public static byte VLRB = 1 << 6;
    public static byte VLRF = 1 << 7;
    //以上为体素为实点的位标记
    public static Int16Triple[] PointIndexToPointDelta = new Int16Triple[8]
    {
        new Int16Triple(0, 1, 1 ),
        new Int16Triple(0, 1, 0 ),
        new Int16Triple(0, 0, 0 ),
        new Int16Triple(0, 0, 1 ),
        new Int16Triple(1, 1, 1 ),
        new Int16Triple(1, 1, 0 ),
        new Int16Triple(1, 0, 0 ),
        new Int16Triple(1, 0, 1 )
    };//体元内每个体素相对基准体素坐标的偏移
    public static byte[] PointIndexToFlag=new byte[8]
    {
        VULF,
        VULB,
        VLLB,
        VLLF,
        VURF,
        VURB,
        VLRB,
        VLRF
    };//每个体素对应的位标记
    public static int[,] EdgeIndexToEdgeVertexIndex = new int[12, 2]
    {
	    {0,1}, {1,2}, 
        {2,3},{3,0},
	    {4,5},{5,6}, 
        {6,7}, {7,4},
	    {0,4}, {1,5}, 
        {2,6}, {3,7}
    };//每个边对应的两顶点体素的索引
    public int CellIndexX;
    public int CellIndexY;
    public int CellIndexZ;
    public Cube(int cellIndexX, int cellIndexY, int cellIndexZ)
    {
        this.CellIndexX = cellIndexX;
        this.CellIndexY = cellIndexY;
        this.CellIndexZ = cellIndexZ;
        for (int i = 0; i < 8; i++)
        {
            cubeImageIndices[i].X = cellIndexX + PointIndexToPointDelta[i].X;
            cubeImageIndices[i].Y = cellIndexY + PointIndexToPointDelta[i].Y;
            cubeImageIndices[i].Z = cellIndexZ + PointIndexToPointDelta[i].Z;
        }
    }//使用基准体素坐标初始化Cube
    public Int16Triple[] cubeImageIndices = new Int16Triple[8];//用于存储8个体素的坐标
}
}
