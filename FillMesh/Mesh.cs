using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
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
    //Box3Float box = new Box3Float();
    //        for (int i = 0; i < mesh.Vertices.Count; i++)
    //        {
    //            box.UpdataRange(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
    //        }
    //        float stx = box.Min3[0]-1;
    //        float sty = box.Min3[1]-1;
    //        float stz = box.Min3[2]-1;
    //        for (int i = 0; i < mesh.Vertices.Count; i++)
    //        {
    //            Point3d p = mesh.Vertices[i];
    //            p.X -= stx;
    //            p.Y -= sty;
    //            p.Z -= stz;
    //            mesh.Vertices[i] = p;
    //        }
    //        PlyManager.Output(mesh, "table1.ply");
}
