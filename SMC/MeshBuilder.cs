using System;
using System.Collections.Generic;
using System.Text;

namespace SMC
{
    class MeshBuilder_IntegerVertex
    {
        Mesh mesh;
        HashTable_Double2dArray<int> hashMap;
        int width;
        int height;
        int depth;
        public MeshBuilder_IntegerVertex(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            mesh = new Mesh();
            this.hashMap = new HashTable_Double2dArray<int>(width,height,depth);
        }
        public void AddTriangle(Int16Triple p0, Int16Triple p1,Int16Triple p2)
        {
            int p0i;
            int p1i;
            int p2i;
            int index = 0;
            bool hasValue;
            hasValue = hashMap.GetHashValue(p0.X, p0.Y, p0.Z, ref index);
            if (!hasValue)
            {
                p0i = mesh.AddVertex(new Point3d(p0.X, p0.Y, p0.Z));
                hashMap.SetHashValue(p0.X, p0.Y, p0.Z, p0i);
            }
            else
            {
                p0i = index;
            }

            hasValue = hashMap.GetHashValue(p1.X, p1.Y, p1.Z, ref index);
            if (!hasValue)
            {
                p1i = mesh.AddVertex(new Point3d(p1.X, p1.Y, p1.Z));
                hashMap.SetHashValue(p1.X, p1.Y, p1.Z, p1i);
            }
            else
            {
                p1i = index;
            }

            hasValue = hashMap.GetHashValue(p2.X, p2.Y, p2.Z, ref index);
            if (!hasValue)
            {
                p2i = mesh.AddVertex(new Point3d(p2.X, p2.Y, p2.Z));
                hashMap.SetHashValue(p2.X, p2.Y, p2.Z, p2i);
            }
            else
            {
                p2i = index;
            }
            Triangle t = new Triangle(p0i, p1i, p2i);
            mesh.AddFace(t);
        }
        public Mesh GetMesh()
        {
            return mesh;
        }
        public void Clear()
        {
            hashMap.Clear();
        }
    }
    class MeshBuilder_FloatVertex
{
    Mesh mesh;
    Dictionary<Point3d, int> hashMap;
    public MeshBuilder_FloatVertex(int width, int height, int depth)
    {
        mesh = new Mesh();
        this.hashMap = new Dictionary<Point3d, int>();
    }
    public void AddTriangle(Point3d p0,Point3d p1,Point3d p2)
    {
        int p0i;
        int p1i;
        int p2i;
        int index = 0;
        bool hasValue;
        hasValue = hashMap.ContainsKey(p0);
        if (!hasValue)
        {
            p0i = mesh.AddVertex(p0);
            hashMap.Add(p0,p0i);
        }
        else
        {
            index = hashMap[p0];
            p0i = index;
        }

        hasValue = hashMap.ContainsKey(p1);
        if (!hasValue)
        {
            p1i = mesh.AddVertex(p1);
            hashMap.Add(p1,p1i);
        }
        else
        {
            index = hashMap[p1];
            p1i = index;
        }

        hasValue = hashMap.ContainsKey(p2);
        if (!hasValue)
        {
            p2i = mesh.AddVertex(p2);
            hashMap.Add(p2, p2i);
        }
        else
        {
            index = hashMap[p2];
            p2i = index;
        }
        Triangle t = new Triangle(p0i, p1i, p2i);
        mesh.AddFace(t);
    }
    public Mesh GetMesh()
    {
        return mesh;
    }
    public void Clear()
    {
        hashMap.Clear();
    }
}
    class MeshBuilder_NonWelding
    {
        Mesh mesh;
        public MeshBuilder_NonWelding(int width, int height, int depth)
        {
            mesh = new Mesh();
        }
        public void AddTriangle(Point3d p0, Point3d p1, Point3d p2)
        {
            int p0i=mesh.AddVertex(p0);
            int p1i=mesh.AddVertex(p1);
            int p2i=mesh.AddVertex(p2);
            Triangle t = new Triangle(p0i, p1i, p2i);
            mesh.AddFace(t);
        }
        public Mesh GetMesh()
        {
            return mesh;
        }
        public void Clear()
        {
            
        }
    }
}
