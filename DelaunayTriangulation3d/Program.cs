using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelaunayTriangulation3d
{
    class Program
    {
        static void Main(string[] args)
        {
            Mesh m = new Mesh();
            PlyManager.ReadMeshFile(m, @"D:\VTKproj\sample2.ply");
            List<PVector> plist = new List<PVector>();
            for (int i = 0; i < m.Vertices.Count; i++)
            {
                PVector p = new PVector(m.Vertices[i].X, m.Vertices[i].Y, m.Vertices[i].Z);
                plist.Add(p);
            }
            Delaunay d = new Delaunay();
            d.SetData(plist);
            Mesh mr = new Mesh();
            for (int i = 0; i < d.triangles.Count; i++)
            {
                Point3d p0 = new Point3d(d.triangles[i].v1.x,d.triangles[i].v1.y,d.triangles[i].v1.z);
                Point3d p1 = new Point3d(d.triangles[i].v2.x, d.triangles[i].v2.y, d.triangles[i].v2.z);
                Point3d p2 = new Point3d(d.triangles[i].v3.x, d.triangles[i].v3.y, d.triangles[i].v3.z);
                int index1=mr.AddVertex(p0);
                int index2=mr.AddVertex(p1);
                int index3=mr.AddVertex(p2);
                mr.AddFace(new Triangle(index1, index2, index3));
            }
            PlyManager.Output(mr, "Test.ply");
        }
    }
}
