using System;
using System.Collections.Generic;
using System.Text;

namespace SeededGrowSMC
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
}
