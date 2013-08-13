using System;
using System.Collections.Generic;
using System.Text;

namespace HashTableTest
{
public class QuickSorter<T> where T : IComparable<T>
{
    public T[] A
    {
        get;
        set;
    }

    public void Sort()
    {
        QuickSort(A, 0, A.Length - 1);
    }
    public void QuickSort(T[] A, int st, int ed)
    {
        if (st < ed)
        {
            int pa = Partition(A, st, ed, (st + ed) / 2);
            QuickSort(A, st, pa - 1);
            QuickSort(A, pa + 1, ed);
        }
    }
    public static void Swap(T[] A, int index1, int index2)
    {
        T temp = A[index1];
        A[index1] = A[index2];
        A[index2] = temp;
    }
    private int Partition(T[] A, int st, int ed, int partionPos)//SELECT VALUE ON PAPOS AND MAKE THE ARRAY(ST TO ED) INTO TWO PARTS: BEFORE RET SMALLER THAN VALUE AFTER BIGGER
    {
        if (partionPos != st)
            Swap(A, partionPos, st);
        T value = A[st];
        int boundary = st;//BOUNDARY REFER TO THE LAST SMALLER INDEX

        for (int i = st + 1; i <= ed; i++)
        {
            if (A[i].CompareTo(value) < 0)
            {
                if (boundary + 1 != i)
                    Swap(A, i, boundary + 1);
                boundary++;
            }
        }
        if (st != boundary)
            Swap(A, boundary, st);
        return boundary;
    }
}
public struct OriginalTriangle
{
    public Point3d P0;
    public Point3d P1;
    public Point3d P2;
    public OriginalTriangle(int x0,int y0,int z0,int x1,int y1,int z1,int x2,int y2,int z2)
    {
        P0.X=x0;
        P0.Y=y0;
        P0.Z=z0;
        P1.X=x1;
        P1.Y=y1;
        P1.Z=z1;
        P2.X=x2;
        P2.Y=y2;
        P2.Z=z2;
    }
}
public struct Point3dWithIndex :IComparable<Point3dWithIndex>
{
    public Point3d P;
    public int Index;
    public Point3dWithIndex(Point3d p, int index)
    {
        P = p;
        Index = index;
    }

    public int CompareTo(Point3dWithIndex other)
    {
        if (P.X != other.P.X)
            return P.X.CompareTo(other.P.X);
        else
        {
            if (P.Y != other.P.Y)
                return P.Y.CompareTo(other.P.Y);
            else
            {
                if (P.Z != other.P.Z)
                    return P.Z.CompareTo(other.P.Z);
                else
                    return 0;
            }
        }
    }
}
    public class RemoveDup
    {
public static int RemoveDuplicates<T>(T[] data) where T:IComparable<T>
{
    QuickSorter<T> sorter = new QuickSorter<T>();
    sorter.A = data;
    sorter.Sort();
    int lastIndex = 0;
    for (int i = 0; i < data.Length; i++)
    {
        if (data[i].CompareTo(data[lastIndex])==0)
        {
            continue;
        }
        else
        {
            data[lastIndex + 1] = data[i];
            lastIndex++;
        }
    }
    return lastIndex + 1;
}//remove duplicates in an T array

public static Mesh WeldingVertices(List<OriginalTriangle> triangleSoup)
{
    Mesh mesh=new Mesh();
    Point3dWithIndex[] pointArray = new Point3dWithIndex[triangleSoup.Count * 3];
    int lindex=0;
    for (int i = 0; i < triangleSoup.Count; i++)
    {
        pointArray[lindex].P = triangleSoup[i].P0;
        pointArray[lindex+1].P = triangleSoup[i].P1;
        pointArray[lindex+2].P = triangleSoup[i].P2;
        pointArray[lindex].Index = lindex;
        pointArray[lindex + 1].Index = lindex + 1;
        pointArray[lindex + 2].Index = lindex + 2;
        Triangle t=new Triangle();
        t.P0Index=lindex;
        t.P1Index=lindex+1;
        t.P2Index=lindex+2;
        mesh.AddFace(t);
        lindex += 3;
    }
    QuickSorter<Point3dWithIndex> sorter = new QuickSorter<Point3dWithIndex>();
    sorter.A = pointArray;
    sorter.Sort();
    int[] tempArray = new int[pointArray.Length];
    for (int i = 0; i < pointArray.Length; i++)
    {
        tempArray[pointArray[i].Index] = i;
    }
    int lastIndex = 0;
    for (int i = 0; i < pointArray.Length; i++)
    {
        if (pointArray[i].CompareTo(pointArray[lastIndex]) == 0)
        {
            continue;
        }
        else
        {
            pointArray[lastIndex + 1] = pointArray[i];
            tempArray[pointArray[i].Index] = lastIndex + 1 ;
            lastIndex++;
        }
    }
    for (int i = 0; i < lastIndex; i++)
    {
        mesh.AddVertex(pointArray[i].P);
    }
    for (int i = 0; i < mesh.Faces.Count; i++)
    {
        Triangle t = mesh.Faces[i];
        t.P0Index = tempArray[mesh.Faces[i].P0Index];
        t.P1Index = tempArray[mesh.Faces[i].P1Index];
        t.P2Index = tempArray[mesh.Faces[i].P2Index];
        mesh.Faces[i] = t;
    }

    return mesh;
}

public static Mesh WeldingVertices_Hash(List<OriginalTriangle> triangleSoup)
{
    Mesh mesh = new Mesh();
    IHashTable<int> hash = new HashTable_General<int>();
    //IHashTable<int> hash = new HashTable_3dArray<int>();
    //IHashTable<int> hash = new HashTable_2dArray<int>();
    //IHashTable<int> hash = new HashTable_Double2dArray<int>();

    for (int i = 0; i < triangleSoup.Count; i++)
    {
        Triangle t = new Triangle();
        Point3d p0 = triangleSoup[i].P0;
        Point3d p1 = triangleSoup[i].P1;
        Point3d p2 = triangleSoup[i].P2;
        int temp = -1;
        int index0, index1, index2;
        if (hash.GetHashValue((int)p0.X, (int)p0.Y, (int)p0.Z, ref temp))
        {
            index0 = temp;
        }
        else
        {
            index0 = mesh.AddVertex(p0);
        }
        if (hash.GetHashValue((int)p1.X, (int)p1.Y, (int)p1.Z, ref temp))
        {
            index1 = temp;
        }
        else
        {
            index1 = mesh.AddVertex(p1);
        }
        if (hash.GetHashValue((int)p2.X, (int)p2.Y, (int)p2.Z, ref temp))
        {
            index2 = temp;
        }
        else
        {
            index2 = mesh.AddVertex(p2);
        }
        t.P0Index = index0;
        t.P1Index = index1;
        t.P2Index = index2;
        mesh.AddFace(t);
    }

    return mesh;
}
    }
}
