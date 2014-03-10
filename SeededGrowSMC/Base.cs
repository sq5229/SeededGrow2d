using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SeededGrowSMC
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
    public class BitMap3d
    {
        public const byte WHITE = 255;
        public const byte BLACK = 0;
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public BitMap3d(int width, int height, int depth, byte v)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            data = new byte[width * height * depth];
            for (int i = 0; i < width * height * depth; i++)
                data[i] = v;
        }
        public BitMap3d(byte[] data, int width, int height, int depth)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        public void SetPixel(int x, int y, int z, byte v)
        {
            data[x + y * width + z * width * height] = v;
        }
        public byte GetPixel(int x, int y, int z)
        {
            return data[x + y * width + z * width * height];
        }
        public void ReadRaw(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, width * height * depth);
            fs.Close();
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
    }

}
