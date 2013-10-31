using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    public class Box3Float
    {
        public float[] Min3;
        public float[] Max3;
        public Box3Float()
        {
            Min3 = new float[3] { int.MaxValue, int.MaxValue, int.MaxValue };
            Max3 = new float[3] { int.MinValue, int.MinValue, int.MinValue };
        }
        public void UpdataRange(float x, float y, float z)
        {
            if (x < Min3[0])
                Min3[0] = x;
            if (y < Min3[1])
                Min3[1] = y;
            if (z < Min3[2])
                Min3[2] = z;

            if (x > Max3[0])
                Max3[0] = x;
            if (y > Max3[1])
                Max3[1] = y;
            if (z > Max3[2])
                Max3[2] = z;
        }
        public float GetXLength() { return Max3[0] - Min3[0] + 1; }
        public float GetYLength() { return Max3[1] - Min3[1] + 1; }
        public float GetZLength() { return Max3[2] - Min3[2] + 1; }
        public override string ToString()
        {
            return string.Format("[{0},{1}] [{2},{3}] [{4},{5}]", Min3[0], Max3[0], Min3[1], Max3[1], Min3[2], Max3[2]);
        }
    }
    public class MeshFiller
    {
        Mesh mesh;
        Box3Float range;
        public MeshFiller(Mesh m)
        {
            mesh = m;
            range = new Box3Float();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                range.UpdataRange(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            }
        }
        public BitMap3d FillImage()
        {
            int width = (int)(range.Max3[0]+2);
            int height = (int)(range.Max3[1]+2);
            int depth = (int)(range.Max3[2]+2);
            BitMap3d bmp = new BitMap3d(width,height,depth,0);
            FillShape fs = new FillShape(bmp);
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Triangle t = mesh.Faces[i];
                Point3d p0 = mesh.Vertices[t.P0Index];
                Point3d p1 = mesh.Vertices[t.P1Index];
                Point3d p2 = mesh.Vertices[t.P2Index];
                fs.DrawTriangle3d((int)p0.X, (int)p0.Y, (int)p0.Z, (int)p1.X, (int)p1.Y, (int)p1.Z, (int)p2.X, (int)p2.Y, (int)p2.Z);
            }

            return bmp;
        }
    }
}
