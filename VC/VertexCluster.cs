using System;
using System.Collections.Generic;
using System.Text;

namespace VC
{
public class VertexCluster
{
    Mesh mesh;
    public VertexCluster(Mesh mesh)
    {
        this.mesh = mesh;
    }
    public void Clear()
    {
        mesh = null;
    }
    public void ExecuteSimplification(float unitLength)
    {
        if (unitLength <= 1)
        {
            return;
        }
        Mesh newMesh = new Mesh();//新建Mesh存放削减后的网格
        Box3Float box = mesh.GetBox3();//首先获取Mesh的空间范围 XYZ方向的最大最小值存在box中
        int resx = (int)((box.Max3[0] - box.Min3[0]) / unitLength) + 1;
        int resy = (int)((box.Max3[1] - box.Min3[1]) / unitLength) + 1;
        int resz = (int)((box.Max3[2] - box.Min3[2]) / unitLength) + 1;
        //在新的单位长度下，得出新空间划分出的每一维的长度

        HashTable_Double2dArray<int> hash = new HashTable_Double2dArray<int>(resx+1, resy+1, resz+1);
        //创建hash表，这里使用的是双二维数组哈希表

        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            Point3d p = mesh.Vertices[i];
            int xindex = (int)((p.X - box.Min3[0]) / unitLength);
            int yindex = (int)((p.Y - box.Min3[1]) / unitLength);
            int zindex = (int)((p.Z - box.Min3[2]) / unitLength);
            //计算点在新单位长度下的坐标
            int value = 0;
            bool hasValue = hash.GetHashValue(xindex, yindex, zindex, ref value);
            if (!hasValue)
            {
                hash.SetHashValue(xindex, yindex, zindex, newMesh.Vertices.Count);
                newMesh.AddVertex(new Point3d(xindex, yindex, zindex));
            }//新单位长度下肯定有原来不同的点映射到相同的位置，
            //总是添加第一次映射到这个位置的点入Mesh,之后的不再添加
        }
        for (int i = 0; i < mesh.Faces.Count; i++)
        {
            Triangle t = mesh.Faces[i];
            Point3d p0 = mesh.Vertices[t.P0Index];
            Point3d p1 = mesh.Vertices[t.P1Index];
            Point3d p2 = mesh.Vertices[t.P2Index];
            int xindex0 = (int)((p0.X - box.Min3[0]) / unitLength);
            int yindex0 = (int)((p0.Y - box.Min3[1]) / unitLength);
            int zindex0 = (int)((p0.Z - box.Min3[2]) / unitLength);
            int index0 = 0;
            hash.GetHashValue(xindex0, yindex0, zindex0, ref index0);

            int xindex1 = (int)((p1.X - box.Min3[0]) / unitLength);
            int yindex1 = (int)((p1.Y - box.Min3[1]) / unitLength);
            int zindex1 = (int)((p1.Z - box.Min3[2]) / unitLength);
            int index1 = 0;
            hash.GetHashValue(xindex1, yindex1, zindex1, ref index1);

            int xindex2 = (int)((p2.X - box.Min3[0]) / unitLength);
            int yindex2 = (int)((p2.Y - box.Min3[1]) / unitLength);
            int zindex2 = (int)((p2.Z - box.Min3[2]) / unitLength);
            int index2 = 0;
            hash.GetHashValue(xindex2, yindex2, zindex2, ref index2);

            if (!(index0 == index1 || index0 == index2 || index1 == index2))
            {
                newMesh.AddFace(new Triangle(index0, index1, index2));
            }
            //对于每个三角形，找出其三点对应的新位置，检查是否有两个点重合（退化），
            //添加不退化的三角形
        }
        for (int i = 0; i < newMesh.Vertices.Count; i++)
        {
            Point3d p = newMesh.Vertices[i];
            p.X = (float)(unitLength * p.X + box.Min3[0]);
            p.Y = (float)(unitLength * p.Y + box.Min3[1]);
            p.Z = (float)(unitLength * p.Z + box.Min3[2]);
        }//将新Mesh的坐标放大到和原来一样的尺度
        mesh.Clear();
        mesh.Vertices = newMesh.Vertices;
        mesh.Faces = newMesh.Faces;
        //替换旧Mesh的数据
    }
}
}
