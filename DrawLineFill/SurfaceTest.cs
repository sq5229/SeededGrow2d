using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DrawLineFill
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
    public class HashTable_Double2dArray<T>
    {
        struct IndexAndValue<T1>
        {
            public int Index;
            public T1 Value;
            public IndexAndValue(int index, T1 value)
            {
                Index = index;
                Value = value;
            }
        }
        List<IndexAndValue<T>>[,] mapHashXY;
        List<IndexAndValue<T>>[,] mapHashXZ;
        int width;
        int height;
        int depth;
        int Count;
        public HashTable_Double2dArray(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            mapHashXY = new List<IndexAndValue<T>>[this.width, this.height];
            mapHashXZ = new List<IndexAndValue<T>>[this.width, this.depth];
        }
        static int FindK(List<IndexAndValue<T>> list, int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Index == index)
                    return i;
            }
            return -1;
        }
        public void SetHashValue(int x, int y, int z, T value)
        {
            if (mapHashXY[x, y] == null)
            {
                mapHashXY[x, y] = new List<IndexAndValue<T>>();
                mapHashXY[x, y].Add(new IndexAndValue<T>(z, value));
                Count++;
            }
            else
            {
                if (mapHashXZ[x, z] == null)
                {
                    mapHashXZ[x, z] = new List<IndexAndValue<T>>();
                    mapHashXZ[x, z].Add(new IndexAndValue<T>(y, value));
                    Count++;
                }
                else
                {
                    if (mapHashXY[x, y].Count > mapHashXZ[x, z].Count)
                    {
                        mapHashXZ[x, z].Add(new IndexAndValue<T>(y, value));
                        Count++;
                    }
                    else
                    {
                        mapHashXY[x, y].Add(new IndexAndValue<T>(z, value));
                        Count++;
                    }
                }
            }
        }
        public bool GetHashValue(int x, int y, int z, ref T value)
        {
            if (mapHashXY[x, y] != null)
            {
                int index = FindK(mapHashXY[x, y], z);
                if (index == -1)
                {
                    if (mapHashXZ[x, z] != null)
                    {
                        int index2 = FindK(mapHashXZ[x, z], y);
                        if (index2 == -1)
                            return false;
                        else
                        {
                            value = mapHashXZ[x, z][index2].Value;
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    value = mapHashXY[x, y][index].Value;
                    return true;
                }
            }
            else
                return false;
        }
        public void SetDefaultValue(T value)
        {
            return;
        }


        public void Clear()
        {
            return;
        }
    }
    public class MeshBuilder_IntegerVertex
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
            this.hashMap = new HashTable_Double2dArray<int>(width, height, depth);
        }
        public void AddTriangle(Int16Triple p0, Int16Triple p1, Int16Triple p2)
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
    public class CuberilleProcessor
    {
        public static Int16Triple[][] AdjIndexToVertexIndices = new Int16Triple[6][]
        {
            new Int16Triple[2] { new Int16Triple(0, 1, 6), new Int16Triple(0, 6, 7) },
            new Int16Triple[2] { new Int16Triple(3, 4, 5), new Int16Triple(3, 5, 2) },
            new Int16Triple[2] { new Int16Triple(1, 2, 5), new Int16Triple(1, 5, 6) },
            new Int16Triple[2] { new Int16Triple(0, 7, 4), new Int16Triple(0, 4, 3) },
            new Int16Triple[2] { new Int16Triple(0, 3, 2), new Int16Triple(0, 2, 1) },
            new Int16Triple[2] { new Int16Triple(4, 7, 6), new Int16Triple(4, 6, 5) },
        };
        public static Int16Triple[] VertexIndexToPositionDelta = new Int16Triple[8]
        {
                new Int16Triple(0, 1, 1),
                new Int16Triple(1, 1, 1),
                new Int16Triple(1, 0, 1),
                new Int16Triple(0, 0, 1),
                new Int16Triple(0, 0, 0),
                new Int16Triple(1, 0, 0),
                new Int16Triple(1, 1, 0),
                new Int16Triple(0, 1, 0),
        };
        BitMap3d bmp;
        public CuberilleProcessor(BitMap3d bitmap)
        {
            bmp = bitmap;
        }
        public Mesh GeneratorSurface()
        {
            int Width = bmp.width;
            int Height = bmp.height;
            int Depth = bmp.depth;
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            MeshBuilder_IntegerVertex mb = new MeshBuilder_IntegerVertex(bmp.width, bmp.height, bmp.depth);

            for (int k = 0; k <= Depth - 1; k++)
            {
                for (int j = 0; j <= Height - 1; j++)
                {
                    for (int i = 0; i <= Width - 1; i++)
                    {
                        if (IsInside(i, j, k))
                        {
                            Int16Triple p = new Int16Triple(i, j, k);
                            InitAdj6(adjPoints6, p);
                            for (int r = 0; r < adjPoints6.Length; r++)
                            {
                                Int16Triple t = adjPoints6[r];
                                if (!IsInside(t.X, t.Y, t.Z))
                                {
                                    ExtractSquare(r, p, mb);
                                }
                            }
                        }
                    }
                }
            }
            Mesh m = mb.GetMesh();
            for (int i = 0; i < m.Vertices.Count; i++)
            {
                Point3d p = m.Vertices[i];
                p.X -= 0.5f;
                p.Y -= 0.5f;
                p.Z -= 0.5f;
            }//若需要真实位置，则都得平移回去
            return m;
        }

        private void ExtractSquare(int r, Int16Triple p, MeshBuilder_IntegerVertex mb)
        {
            int p0x, p0y, p0z, p1x, p1y, p1z, p2x, p2y, p2z;//
            Int16Triple deltaA0 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].X];
            Int16Triple deltaA1 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].Y];
            Int16Triple deltaA2 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][0].Z];
            p0x = p.X + deltaA0.X;
            p0y = p.Y + deltaA0.Y;
            p0z = p.Z + deltaA0.Z;
            p1x = p.X + deltaA1.X;
            p1y = p.Y + deltaA1.Y;
            p1z = p.Z + deltaA1.Z;
            p2x = p.X + deltaA2.X;
            p2y = p.Y + deltaA2.Y;
            p2z = p.Z + deltaA2.Z;
            mb.AddTriangle(new Int16Triple(p0x, p0y, p0z), new Int16Triple(p1x, p1y, p1z), new Int16Triple(p2x, p2y, p2z));


            Int16Triple deltaB0 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].X];
            Int16Triple deltaB1 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].Y];
            Int16Triple deltaB2 = VertexIndexToPositionDelta[AdjIndexToVertexIndices[r][1].Z];

            p0x = p.X + deltaB0.X;
            p0y = p.Y + deltaB0.Y;
            p0z = p.Z + deltaB0.Z;
            p1x = p.X + deltaB1.X;
            p1y = p.Y + deltaB1.Y;
            p1z = p.Z + deltaB1.Z;
            p2x = p.X + deltaB2.X;
            p2y = p.Y + deltaB2.Y;
            p2z = p.Z + deltaB2.Z;
            mb.AddTriangle(new Int16Triple(p0x, p0y, p0z), new Int16Triple(p1x, p1y, p1z), new Int16Triple(p2x, p2y, p2z));
        }
        public virtual bool IsInside(int x, int y, int z)
        {
            if (x <= 0 || y <= 0 || z <= 0 || x > bmp.width || y > bmp.height || z > bmp.depth)
                return false;
            else
            {
                return bmp.GetPixel(x, y, z) == BitMap3d.WHITE;
            }
        }//judge if a voxel is inside the surface

        public static void InitAdj6(Int16Triple[] adjPoints6, Int16Triple p)
        {
            adjPoints6[0].X = p.X;
            adjPoints6[0].Y = p.Y + 1;
            adjPoints6[0].Z = p.Z;

            adjPoints6[1].X = p.X;
            adjPoints6[1].Y = p.Y - 1;
            adjPoints6[1].Z = p.Z;

            adjPoints6[2].X = p.X + 1;
            adjPoints6[2].Y = p.Y;
            adjPoints6[2].Z = p.Z;

            adjPoints6[3].X = p.X - 1;
            adjPoints6[3].Y = p.Y;
            adjPoints6[3].Z = p.Z;

            adjPoints6[4].X = p.X;
            adjPoints6[4].Y = p.Y;
            adjPoints6[4].Z = p.Z + 1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z - 1;
        }//initialize poistions of the 6-adjacency points
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
    }
}
