using System;
using System.Collections.Generic;
using System.Text;

namespace HashTableTest
{
    class MeshBuilder
{
    Mesh mesh;
    IHashTable<int> hashMap;
    int width;
    int height;
    int depth;
    public MeshBuilder(int width, int height, int depth,IHashTable<int> hashtable)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        mesh = new Mesh();
        this.hashMap = hashtable;
    }
    public void AddTriangle(int p0x, int p0y, int p0z, int p1x, int p1y, int p1z, int p2x, int p2y, int p2z)
    {
        int p0i;
        int p1i;
        int p2i;
        int index = 0;
        bool hasValue;
        hasValue = hashMap.GetHashValue(p0x, p0y, p0z, ref index);
        if (!hasValue)
        {
            p0i = mesh.AddVertex(new Point3d(p0x, p0y, p0z));
            hashMap.SetHashValue(p0x, p0y, p0z, p0i);
        }
        else
        {
            p0i = index;
        }

        hasValue = hashMap.GetHashValue(p1x, p1y, p1z, ref index);
        if (!hasValue)
        {
            p1i = mesh.AddVertex(new Point3d(p1x, p1y, p1z));
            hashMap.SetHashValue(p1x, p1y, p1z, p1i);
        }
        else
        {
            p1i = index;
        }

        hasValue = hashMap.GetHashValue(p2x, p2y, p2z, ref index);
        if (!hasValue)
        {
            p2i = mesh.AddVertex(new Point3d(p2x, p2y, p2z));
            hashMap.SetHashValue(p2x, p2y, p2z, p2i);
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
