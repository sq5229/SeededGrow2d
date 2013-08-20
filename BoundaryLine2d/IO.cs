using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BoundaryLine2d
{
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
        private static void AWriteE(StreamWriter sw, int i1, int i2)
        {
            sw.Write(string.Format("{0} {1}\n", i1, i2));
        }
        public static void Output2(List<Int16Double> points, List<Int16Double> edges, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write("ply\n");
            sw.Write("format ascii 1.0\n");
            sw.Write("comment VCGLIB generated\n");
            sw.Write(string.Format("element vertex {0}\n", points.Count));
            sw.Write("property float x\n");
            sw.Write("property float y\n");
            sw.Write("property float z\n");
            sw.Write("property uchar red\n");
            sw.Write("property uchar green\n");
            sw.Write("property uchar blue\n");
            sw.Write(string.Format("element edge {0}\n", edges.Count));
            sw.Write("property int vertex1\n");
            sw.Write("property int vertex2\n");
            sw.Write("end_header\n");
            for (int i = 0; i < points.Count; i++)
            {
                AWriteV(sw, points[i].X, points[i].Y, 0, 255, 255, 255);
            }
            for (int i = 0; i < edges.Count; i++)
            {
                AWriteE(sw, edges[i].X, edges[i].Y);
            }
            sw.Close();
            fs.Close();
        }
        public static void Output3(List<Point3d> points, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write("ply\n");
            sw.Write("format ascii 1.0\n");
            sw.Write("comment VCGLIB generated\n");
            sw.Write(string.Format("element vertex {0}\n", points.Count));
            sw.Write("property float x\n");
            sw.Write("property float y\n");
            sw.Write("property float z\n");
            sw.Write("property uchar red\n");
            sw.Write("property uchar green\n");
            sw.Write("property uchar blue\n");
            sw.Write(string.Format("element edge {0}\n", points.Count));
            sw.Write("property int vertex1\n");
            sw.Write("property int vertex2\n");
            sw.Write("end_header\n");
            for (int i = 0; i < points.Count; i++)
            {
                AWriteV(sw, points[i].X, points[i].Y, points[i].Z, 255, 255, 255);
            }
            for (int i = 0; i < points.Count-1; i++)
            {
                AWriteE(sw, i, i+1);
            }
            AWriteE(sw, points.Count - 1, 0);
            sw.Close();
            fs.Close();
        }
    }
}
