using System;
using System.Collections.Generic;
using System.Text;

namespace ContourLineRecon
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
        public Point3d GetCenter()
        {
            return new Point3d((Max3[0] + Min3[0]) / 2.0f, (Max3[1] + Min3[1]) / 2.0f, (Max3[2] + Min3[2]) / 2.0f);
        }
        public override string ToString()
        {
            return string.Format("[{0},{1}] [{2},{3}] [{4},{5}]", Min3[0], Max3[0], Min3[1], Max3[1], Min3[2], Max3[2]);
        }
    }
    public struct FloatDouble
    {
        public float X;
        public float Y;
        public FloatDouble(float x, float y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }
    }
    public struct Int16Double
    {
        public int X;
        public int Y;
        public Int16Double(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return "["+X+","+Y+"]";
        }
    }
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
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;
        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public struct Point3d
    {
        public float X;
        public float Y;
        public float Z;
        public Point3d(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString()
        {
            return "[" + X + "," + Y + ","+Z+"]";
        }
    }
    public struct Triangle
    {
        public int P0Index;
        public int P1Index;
        public int P2Index;
        public Triangle(int p0index, int p1index, int p2index)
        {
            P0Index = p0index;
            P1Index = p1index;
            P2Index = p2index;
        }
        public static Vector CaculateNormal(Point3d p0, Point3d p1, Point3d p2)
        {
            Vector Normal = new Vector();
            float v1x = p1.X - p0.X;
            float v1y = p1.Y - p0.Y;
            float v1z = p1.Z - p0.Z;
            float v2x = p2.X - p1.X;
            float v2y = p2.Y - p1.Y;
            float v2z = p2.Z - p1.Z;
            Normal.X = v1y * v2z - v1z * v2y;
            Normal.Y = v1z * v2x - v1x * v2z;
            Normal.Z = v1x * v2y - v1y * v2x;
            float len = (float)Math.Sqrt(Normal.X * Normal.X + Normal.Y * Normal.Y + Normal.Z * Normal.Z);
            if (len == 0)
                throw new Exception();
            else
            {
                Normal.X /= len;
                Normal.Y /= len;
                Normal.Z /= len;
            }
            return Normal;
        }
    }
    public class Mesh
    {
        public List<Point3d> Vertices = null;
        public List<Triangle> Faces = null;
        public Mesh()
        {
            Vertices = new List<Point3d>();
            Faces = new List<Triangle>();
        }
        public int AddVertex(Point3d toAdd)
        {
            int index = Vertices.Count;
            Vertices.Add(toAdd);
            return index;
        }
        public int AddFace(Triangle tri)
        {
            int index = Faces.Count;
            Faces.Add(tri);
            return index;
        }
        public void Clear()
        {
            Vertices.Clear();
            Faces.Clear();
        }

        public void Scale(float r1, float r2, float r3)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Point3d p = Vertices[i];
                p.X *= r1;
                p.Y *= r2;
                p.Z *= r3;
                Vertices[i] = p;
            }
        }

        public void Transform(float d1, float d2, float d3)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Point3d p = Vertices[i];
                p.X += d1;
                p.Y += d2;
                p.Z += d3;
                Vertices[i] = p;
            }
        }
    }
}
