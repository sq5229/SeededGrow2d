using System;
using System.Collections.Generic;
using System.Text;

namespace CubicSurface
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
public class CuberilleProcessor
{
        public static Int16Triple[][] AdjIndexToVertexIndices = new Int16Triple[6][]
        {
            new Int16Triple[2] { new Int16Triple(0, 1, 6), new Int16Triple(0, 6, 7) },
            new Int16Triple[2] { new Int16Triple(3, 4, 5), new Int16Triple(3, 5, 2) },
            new Int16Triple[2] { new Int16Triple(1, 2, 5), new Int16Triple(1, 5, 6) },
            new Int16Triple[2] { new Int16Triple(0, 7, 4), new Int16Triple(0, 4, 3) },
            new Int16Triple[2] { new Int16Triple(0, 3, 2), new Int16Triple(0, 2, 1) },
            new Int16Triple[2] { new Int16Triple(4, 7, 6), new Int16Triple(4, 6, 5) },
        };
    public static Int16Triple[] VertexIndexToPositionDelta = new Int16Triple[8]
    {
            new Int16Triple(0, 1, 1),
            new Int16Triple(1, 1, 1),
            new Int16Triple(1, 0, 1),
            new Int16Triple(0, 0, 1),
            new Int16Triple(0, 0, 0),
            new Int16Triple(1, 0, 0),
            new Int16Triple(1, 1, 0),
            new Int16Triple(0, 1, 0),
    };
    BitMap3d bmp;
    public CuberilleProcessor(BitMap3d bitmap)
    {
        bmp = bitmap;
    }
    public Mesh GeneratorSurface()
    {
        int Width = bmp.width;
        int Height = bmp.height;
        int Depth = bmp.depth;
        Int16Triple[] adjPoints6 = new Int16Triple[6];
        MeshBuilder_IntegerVertex mb = new MeshBuilder_IntegerVertex(bmp.width, bmp.height, bmp.depth);
          
        for (int k = 0; k <= Depth - 1; k++)
        {
            for (int j = 0; j <= Height - 1; j++)
            {
                for (int i = 0; i <= Width - 1; i++)
                {
                    if (IsInside(i,j,k))
                    {
                        Int16Triple p = new Int16Triple(i, j, k);
                        InitAdj6(adjPoints6,p);
                        for (int r = 0; r < adjPoints6.Length; r++)
                        {
                            Int16Triple t = adjPoints6[r];
                            if (!IsInside(t.X,t.Y,t.Z))
                            {
                                ExtractSquare(r,p,mb);
                            }
                        }
                    }
                }
            }
        }
        Mesh m= mb.GetMesh();
        for (int i = 0; i < m.Vertices.Count; i++)
        {
            Point3d p = m.Vertices[i];
            p.X -= 0.5f;
            p.Y -= 0.5f;
            p.Z -= 0.5f;
        }//若需要真实位置，则都得平移回去
        return m;
    }

    private void ExtractSquare(int r,Int16Triple p, MeshBuilder_IntegerVertex mb)
    {
        int p0x, p0y, p0z, p1x, p1y, p1z, p2x, p2y, p2z;//
        Int16Triple deltaA0 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].X];
        Int16Triple deltaA1 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].Y];
        Int16Triple deltaA2 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].Z];
        p0x = p.X + deltaA0.X;
        p0y = p.Y + deltaA0.Y;
        p0z = p.Z + deltaA0.Z;
        p1x = p.X + deltaA1.X;
        p1y = p.Y + deltaA1.Y;
        p1z = p.Z + deltaA1.Z;
        p2x = p.X + deltaA2.X;
        p2y = p.Y + deltaA2.Y;
        p2z = p.Z + deltaA2.Z;
        mb.AddTriangle(new Int16Triple(p0x, p0y, p0z), new Int16Triple(p1x, p1y, p1z), new Int16Triple(p2x, p2y, p2z));


        Int16Triple deltaB0 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].X];
        Int16Triple deltaB1 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].Y];
        Int16Triple deltaB2 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].Z];

        p0x = p.X + deltaB0.X;
        p0y = p.Y + deltaB0.Y;
        p0z = p.Z + deltaB0.Z;
        p1x = p.X + deltaB1.X;
        p1y = p.Y + deltaB1.Y;
        p1z = p.Z + deltaB1.Z;
        p2x = p.X + deltaB2.X;
        p2y = p.Y + deltaB2.Y;
        p2z = p.Z + deltaB2.Z;
        mb.AddTriangle(new Int16Triple(p0x, p0y, p0z), new Int16Triple(p1x, p1y, p1z), new Int16Triple(p2x, p2y, p2z));
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

    public static void InitAdj6(Int16Triple[] adjPoints6,Int16Triple p)
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
    }//initialize poistions of the 6-adjacency points
}
}
