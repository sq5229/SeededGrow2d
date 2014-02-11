using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WingedEdge
{
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
    public class PlyManager
    {
        private static void AWriteV(StreamWriter sw, double v1, double v2, double v3, byte r, byte g, byte b)
        {
            int r1 = (int)r;
            int g1 = (int)g;
            int b1 = (int)b;
            sw.Write(string.Format("{0} {1} {2} {3} {4} {5}\n", v1.ToString("0.0"), v2.ToString("0.0"), v3.ToString("0.0"), r1, g1, b1));
        }
        private static void AWriteF(StreamWriter sw, int i1, int i2, int i3)
        {
            sw.Write(string.Format("{0} {1} {2} {3}\n", 3, i1, i2, i3));
        }
        public static void Output(Mesh mesh, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write("ply\n");
            sw.Write("format ascii 1.0\n");
            sw.Write("comment VCGLIB generated\n");
            sw.Write(string.Format("element vertex {0}\n", mesh.Vertices.Count));
            sw.Write("property float x\n");
            sw.Write("property float y\n");
            sw.Write("property float z\n");
            sw.Write("property uchar red\n");
            sw.Write("property uchar green\n");
            sw.Write("property uchar blue\n");
            sw.Write(string.Format("element face {0}\n", mesh.Faces.Count));
            sw.Write("property list int int vertex_indices\n");
            sw.Write("end_header\n");
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                AWriteV(sw, mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z, 255, 255, 255);
            }
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                AWriteF(sw, mesh.Faces[i].P0Index, mesh.Faces[i].P1Index, mesh.Faces[i].P2Index);
            }
            sw.Close();
            fs.Close();
        }
        public static void ReadMeshFile(Mesh mesh, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            StreamReader sw = new StreamReader(fs, Encoding.Default);
            int vn = 0;
            int fn = 0;
            string line = null;
            while (line != "end_header")
            {
                line = sw.ReadLine();
                if (line.Contains("element vertex "))
                {
                    vn = int.Parse(line.Replace("element vertex ", ""));
                }
                if (line.Contains("element face "))
                {
                    fn = int.Parse(line.Replace("element face ", ""));
                }
            }
            for (int i = 0; i < vn; i++)
            {
                line = sw.ReadLine();
                string[] r = line.Split(' ');
                Point3d p = new Point3d(float.Parse(r[0]), float.Parse(r[1]), float.Parse(r[2]));
                mesh.AddVertex(p);
            }
            for (int j = 0; j < fn; j++)
            {
                line = sw.ReadLine();
                string[] r = line.Split(' ');
                mesh.AddFace(new Triangle(int.Parse(r[1]), int.Parse(r[2]), int.Parse(r[3])));
            }
            sw.Close();
            fs.Close();
        }

    }
}
