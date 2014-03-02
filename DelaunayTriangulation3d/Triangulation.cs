using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelaunayTriangulation3d
{
    public class Delaunay
    {
        public List<PVector> vertices;     // 与えられた点列
        public List<Tetrahedron> tetras;   // 四面体リスト

        public List<Line> edges;

        public List<Line> surfaceEdges;
        public List<DTriangle> triangles;


        public Delaunay()
        {
            vertices = new List<PVector>();
            tetras = new List<Tetrahedron>();
            edges = new List<Line>();
            surfaceEdges = new List<Line>();
            triangles = new List<DTriangle>();
        }

        public void SetData(List<PVector> seq)
        {

            tetras.Clear();
            edges.Clear();

            // 1    : 点群を包含する四面体を求める
            //   1-1: 点群を包含する球を求める
            PVector vMax = new PVector(-999, -999, -999);
            PVector vMin = new PVector(999, 999, 999);
            foreach (PVector v in seq)
            {
                if (vMax.x < v.x) vMax.x = v.x;
                if (vMax.y < v.y) vMax.y = v.y;
                if (vMax.z < v.z) vMax.z = v.z;
                if (vMin.x > v.x) vMin.x = v.x;
                if (vMin.y > v.y) vMin.y = v.y;
                if (vMin.z > v.z) vMin.z = v.z;
            }

            PVector center = new PVector();     // 外接球の中心座標
            center.x = 0.5f * (vMax.x - vMin.x);
            center.y = 0.5f * (vMax.y - vMin.y);
            center.z = 0.5f * (vMax.z - vMin.z);
            float r = -1;                       // 半径
            foreach (PVector v in seq)
            {
                if (r < PVector.dist(center, v)) r = PVector.dist(center, v);
            }
            r += 0.1f;                          // ちょっとおまけ

            //   1-2: 球に外接する四面体を求める
            PVector v1 = new PVector();
            v1.x = center.x;
            v1.y = center.y + 3.0f * r;
            v1.z = center.z;

            PVector v2 = new PVector();
            v2.x = center.x - 2.0f * (float)Math.Sqrt(2) * r;
            v2.y = center.y - r;
            v2.z = center.z;

            PVector v3 = new PVector();
            v3.x = center.x + (float)Math.Sqrt(2) * r;
            v3.y = center.y - r;
            v3.z = center.z + (float)Math.Sqrt(6) * r;

            PVector v4 = new PVector();
            v4.x = center.x + (float)Math.Sqrt(2) * r;
            v4.y = center.y - r;
            v4.z = center.z - (float)Math.Sqrt(6) * r;

            PVector[] outer = { v1, v2, v3, v4 };
            tetras.Add(new Tetrahedron(v1, v2, v3, v4));

            // 幾何形状を動的に変化させるための一時リスト
            List<Tetrahedron> tmpTList = new List<Tetrahedron>();
            List<Tetrahedron> newTList = new List<Tetrahedron>();
            List<Tetrahedron> removeTList = new List<Tetrahedron>();
            foreach (PVector v in seq)
            {
                tmpTList.Clear();
                newTList.Clear();
                removeTList.Clear();
                foreach (Tetrahedron t in tetras)
                {
                    if ((t.o != null) && (t.r > PVector.dist(v, t.o)))
                    {
                        tmpTList.Add(t);
                    }
                }

                foreach (Tetrahedron t1 in tmpTList)
                {
                    // まずそれらを削除
                    tetras.Remove(t1);

                    v1 = t1.vertices[0];
                    v2 = t1.vertices[1];
                    v3 = t1.vertices[2];
                    v4 = t1.vertices[3];
                    newTList.Add(new Tetrahedron(v1, v2, v3, v));
                    newTList.Add(new Tetrahedron(v1, v2, v4, v));
                    newTList.Add(new Tetrahedron(v1, v3, v4, v));
                    newTList.Add(new Tetrahedron(v2, v3, v4, v));
                }

                bool[] isRedundancy = new bool[newTList.Count];
                for (int i = 0; i < isRedundancy.Length; i++) isRedundancy[i] = false;
                for (int i = 0; i < newTList.Count - 1; i++)
                {
                    for (int j = i + 1; j < newTList.Count; j++)
                    {
                        if (newTList[i].equals(newTList[j]))
                        {
                            isRedundancy[i] = isRedundancy[j] = true;
                        }
                    }
                }
                for (int i = 0; i < isRedundancy.Length; i++)
                {
                    if (!isRedundancy[i])
                    {
                        tetras.Add(newTList[i]);
                    }

                }

            }


            bool isOuter = false;
            for(int i=0;i<tetras.Count;i++)
            {
                Tetrahedron t4 = tetras[i];
                isOuter = false;
                foreach (PVector p1 in t4.vertices)
                {
                    foreach (PVector p2 in outer)
                    {
                        if (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z)
                        {
                            isOuter = true;
                        }
                    }
                }
                if (isOuter)
                {
                    tetras.Remove(t4);
                }
            }
            //for (Tetrahedron t4 in tetras)
            //{
                
            //}

            triangles.Clear();
            bool isSame = false;
            foreach (Tetrahedron t in tetras)
            {
                foreach (Line l1 in t.getLines())
                {
                    isSame = false;
                    foreach (Line l2 in edges)
                    {
                        if (l2.equals(l1))
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame)
                    {
                        edges.Add(l1);
                    }
                }
            }

            // ===
            // 面を求める

            List<DTriangle> triList = new List<DTriangle>();
            foreach (Tetrahedron t in tetras)
            {
                v1 = t.vertices[0];
                v2 = t.vertices[1];
                v3 = t.vertices[2];
                v4 = t.vertices[3];

                DTriangle tri1 = new DTriangle(v1, v2, v3);
                DTriangle tri2 = new DTriangle(v1, v3, v4);
                DTriangle tri3 = new DTriangle(v1, v4, v2);
                DTriangle tri4 = new DTriangle(v4, v3, v2);

                PVector n;
                // 面の向きを決める
                n = tri1.getNormal();
                if (n.dot(v1) > n.dot(v4)) tri1.turnBack();

                n = tri2.getNormal();
                if (n.dot(v1) > n.dot(v2)) tri2.turnBack();

                n = tri3.getNormal();
                if (n.dot(v1) > n.dot(v3)) tri3.turnBack();

                n = tri4.getNormal();
                if (n.dot(v2) > n.dot(v1)) tri4.turnBack();

                triList.Add(tri1);
                triList.Add(tri2);
                triList.Add(tri3);
                triList.Add(tri4);
            }
            bool[] isSameTriangle = new bool[triList.Count];
            for (int i = 0; i < triList.Count - 1; i++)
            {
                for (int j = i + 1; j < triList.Count; j++)
                {
                    if (triList[i].equals(triList[j])) isSameTriangle[i] = isSameTriangle[j] = true;
                }
            }
            for (int i = 0; i < isSameTriangle.Length; i++)
            {
                if (!isSameTriangle[i]) triangles.Add(triList[i]);
            }

            surfaceEdges.Clear();
            List<Line> surfaceEdgeList = new List<Line>();
            foreach (DTriangle tri in triangles)
            {
                surfaceEdgeList.AddRange(tri.getLines());
            }
            bool[] isRedundancy2 = new bool[surfaceEdgeList.Count];
            for (int i = 0; i < surfaceEdgeList.Count - 1; i++)
            {
                for (int j = i + 1; j < surfaceEdgeList.Count; j++)
                {
                    if (surfaceEdgeList[i].equals(surfaceEdgeList[j])) isRedundancy2[j] = true;
                }
            }

            for (int i = 0; i < isRedundancy2.Length; i++)
            {
                if (!isRedundancy2[i]) surfaceEdges.Add(surfaceEdgeList[i]);
            }

        }

    }
}
