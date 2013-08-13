using System;
using System.Collections.Generic;
using System.Text;
using SMC.MC;

namespace SMC.SMC.Generator
{
class SMCTableGenerator
{
    public int[,] TableFat = new int[256, 6];// table to be filled
    public void InitTable()
    {
        for (int i = 0; i < 255; i++)
        {
            List<Triangle> tlist = GetTriangle((byte)i);//find triangles in cube config i
            int index = 0;
            for (int j = 0; j < tlist.Count; j++)
            {
                Triangle t = tlist[i];
                int index1 = t.P0Index;
                int index2 = t.P1Index;
                int index3 = t.P2Index;
                TableFat[i, index] = index1;
                TableFat[i, index + 1] = index2;
                TableFat[i, index + 2] = index3;
                index += 3;
            }//fill table with triangle data
        }
    }

    private bool IsDegenerated(Triangle originalTriangle)
    {
        return originalTriangle.P0Index == originalTriangle.P1Index || originalTriangle.P1Index == originalTriangle.P2Index || originalTriangle.P2Index == originalTriangle.P0Index;
    }//check if the triangle is degenerated

    private List<Triangle> GetTriangle(byte cubeConfig)
    {
        List<Triangle> list = new List<Triangle>();
        if (MCTable.TriTable[cubeConfig, 0] != -1)
        {
            int index = 0;
            while (MCTable.TriTable[cubeConfig, index] != -1)
            {
                int ei1 = MCTable.TriTable[cubeConfig, index];
                int ei2 = MCTable.TriTable[cubeConfig, index + 1];
                int ei3 = MCTable.TriTable[cubeConfig, index + 2];
                //find edge indices which is intersected

                int e0p0 = Cube.EdgeIndexToEdgeVertexIndex[ei1,0];
                int e0p1 = Cube.EdgeIndexToEdgeVertexIndex[ei1,1];

                int e1p0 = Cube.EdgeIndexToEdgeVertexIndex[ei2,0];
                int e1p1 = Cube.EdgeIndexToEdgeVertexIndex[ei2,1];

                int e2p0 = Cube.EdgeIndexToEdgeVertexIndex[ei3,0];
                int e2p1 = Cube.EdgeIndexToEdgeVertexIndex[ei3,1];
                //find out their vertices

                bool creatFatTable = true;//if need thin table just change this to false;
                int e0pm = GetNewIntersetVertexIndex(cubeConfig, e0p0, e0p1, creatFatTable);
                int e1pm = GetNewIntersetVertexIndex(cubeConfig, e1p0, e1p1, creatFatTable);
                int e2pm = GetNewIntersetVertexIndex(cubeConfig, e2p0, e2p1, creatFatTable);
                //get the vertex to build the triangle

                Triangle t = new Triangle(e0pm, e1pm, e2pm);

                if (!IsDegenerated(t))
                {
                        list.Add(t);
                }//check if the triangle is degenerated
                index += 3;
            }//for each triangle
        }
        return list;
    }//get triangles from mc table by cube config

    private int GetNewIntersetVertexIndex(byte cubeConfig,int v0,int v1,bool isfat)
    {
        if (IsInside(cubeConfig, v0) && !IsInside(cubeConfig, v1))
        {
            if (isfat)
                return v1;
            else
                return v0;
        }
        else if (!IsInside(cubeConfig, v0) && IsInside(cubeConfig, v1))
        {
            if (isfat)
                return v0;
            else
                return v1;
        }
        else
        {
            throw new Exception();
        }
    }//decide which vertex chosed to be the interseted one

    private bool IsInside(byte p, int ep0)
    {
        return (p & Cube.PointIndexToFlag[ep0]) != 0;
    }//check if the vertex is white

}//generator smc table from classic marching cubes tables
}
