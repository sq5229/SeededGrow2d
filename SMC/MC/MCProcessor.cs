using System;
using System.Collections.Generic;
using System.Text;

namespace SMC.MC
{
class MCProcessor
{
    BitMap3d bmp;
    public MCProcessor(BitMap3d bitmap)
    {
        this.bmp = bitmap;
    }
    public virtual bool IsInside(int x, int y, int z)
    {
        if (x <= 0 || y <= 0 || z <= 0 || x > bmp.width || y > bmp.height || z > bmp.depth)
            return false;
        else
        {
            return bmp.GetPixel(x, y, z) == BitMap3d.WHITE;
        }
    }//judge if a voxel is inside the surface
    public Mesh GeneratorSurface()
    {
        MeshBuilder_FloatVertex builder = new MeshBuilder_FloatVertex(bmp.width + 2, bmp.height + 2, bmp.depth + 2);// this class can build mesh from independent triangles
        for (int k = 0; k < bmp.depth - 1; k++)
        {
            for (int j = 0; j < bmp.height - 1; j++)
            {
                for (int i = 0; i < bmp.width - 1; i++)
                {
                    Cube cell = new Cube(i, j, k);//builde Cube for Cell at i j k
                    byte config = GetConfig(ref cell);// get byte config for the cell
                    ExtractTriangles(ref cell, config, builder);// extract triangles from cell and push into
                }
            }
        }
        return builder.GetMesh();
    }
    private byte GetConfig(ref Cube cube)
    {
        byte value = 0;
        for (int i = 0; i < 8; i++)
        {
            if (IsInside(cube.cubeImageIndices[i].X, cube.cubeImageIndices[i].Y, cube.cubeImageIndices[i].Z))
            {
                value |= Cube.PointIndexToFlag[i];
            }
        }
        return value;
    }//get copnfig
    private void ExtractTriangles(ref Cube cube, byte value, MeshBuilder_FloatVertex builder)
    {
        if (MCTable.TriTable[value, 0] != -1)
        {
            int index = 0;
            while (MCTable.TriTable[value, index] != -1)
            {
                int e0index = MCTable.TriTable[value, index];
                int e1index = MCTable.TriTable[value, index + 1];
                int e2index = MCTable.TriTable[value, index + 2];

                Int16Triple e0p0 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e0index, 0]];
                Int16Triple e0p1 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e0index, 1]];

                Int16Triple e1p0 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e1index, 0]];
                Int16Triple e1p1 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e1index, 1]];

                Int16Triple e2p0 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e2index, 0]];
                Int16Triple e2p1 = cube.cubeImageIndices[Cube.EdgeIndexToEdgeVertexIndex[e2index, 1]];

                Point3d e0pm = GetIntersetedPoint(e0p0, e0p1);
                Point3d e1pm = GetIntersetedPoint(e1p0, e1p1);
                Point3d e2pm = GetIntersetedPoint(e2p0, e2p1);

                builder.AddTriangle(e0pm, e1pm, e2pm);

                index += 3;
            }
        }
    }//extract triangles and put them into mesh builder

    private Point3d GetIntersetedPoint(Int16Triple p0, Int16Triple p1)
    {
        return new Point3d((p0.X + p1.X) / 2.0f, (p0.Y + p1.Y) / 2.0f, (p0.Z + p1.Z) / 2.0f);
    }//findInterseted point
}
}
