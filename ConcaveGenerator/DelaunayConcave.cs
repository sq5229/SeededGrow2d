using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcaveGenerator
{
    public class DelaunayConcave
    {
        public struct dVertex
        {
            public long x;
            public long y;
            public long z;
        }
        public struct dTriangle
        {
            public long vv0;
            public long vv1;
            public long vv2;
        }
        public struct OrTriangle
        {
            public Point2d p0;
            public Point2d p1;
            public Point2d p2;
        }
        public struct Triangle
        {
            public int P0Index;
            public int P1Index;
            public int P2Index;
            public int Index;
            public Triangle(int p0index, int p1index, int p2index)
            {
                this.P0Index = p0index;
                this.P1Index = p1index;
                this.P2Index = p2index;
                this.Index = -1;
            }
            public Triangle(int p0index, int p1index, int p2index, int index)
            {
                this.P0Index = p0index;
                this.P1Index = p1index;
                this.P2Index = p2index;
                this.Index = index;
            }
        }
        public struct EdgeInfo
        {
            public int P0Index;
            public int P1Index;
            public List<int> AdjTriangle;
            public bool Flag;
            public double Length;
            public int GetEdgeType()
            {
                return AdjTriangle.Count;
            }
            public bool IsValid()
            {
                return P0Index != -1;
            }
            public EdgeInfo(int d)
            {
                P0Index = -1;
                P1Index = -1;
                Flag = false;
                AdjTriangle = new List<int>();
                Length = -1;
            }
        }
        public class DelaunayMesh2d
        {
            public List<Point2d> Points;
            public List<Triangle> Faces;
            public EdgeInfo[,] Edges;
            public DelaunayMesh2d()
            {
                Points = new List<Point2d>();
                Faces = new List<Triangle>();
            }
            public int AddVertex(Point2d p)
            {
                Points.Add(p);
                return Points.Count - 1;
            }
            public int AddFace(Triangle t)
            {
                Faces.Add(t);
                return Faces.Count - 1;
            }
            public void InitEdgesInfo()
            {
                Edges = new EdgeInfo[Points.Count, Points.Count];
                for (int i = 0; i < Points.Count; i++)
                {
                    for (int j = 0; j < Points.Count; j++)
                    {
                        Edges[i, j] = new EdgeInfo(0);
                    }
                }
                for (int i = 0; i < Faces.Count; i++)
                {
                    Triangle t = Faces[i];
                    SetEdge(t, i);
                }

            }
            private void SetEdge(Triangle t, int i)
            {
                if (t.P0Index < t.P1Index)
                {
                    Edges[t.P0Index, t.P1Index].P0Index = t.P0Index;
                    Edges[t.P0Index, t.P1Index].P1Index = t.P1Index;
                    Edges[t.P0Index, t.P1Index].AdjTriangle.Add(i);
                    Edges[t.P0Index, t.P1Index].Length = BallConcave.GetDistance(Points[t.P0Index], Points[t.P1Index]);
                }
                else
                {
                    Edges[t.P1Index, t.P0Index].P0Index = t.P1Index;
                    Edges[t.P1Index, t.P0Index].P1Index = t.P0Index;
                    Edges[t.P1Index, t.P0Index].AdjTriangle.Add(i);
                    Edges[t.P1Index, t.P0Index].Length = BallConcave.GetDistance(Points[t.P0Index], Points[t.P1Index]);
                }

                if (t.P1Index < t.P2Index)
                {
                    Edges[t.P1Index, t.P2Index].P0Index = t.P1Index;
                    Edges[t.P1Index, t.P2Index].P1Index = t.P2Index;
                    Edges[t.P1Index, t.P2Index].AdjTriangle.Add(i);
                    Edges[t.P1Index, t.P2Index].Length = BallConcave.GetDistance(Points[t.P1Index], Points[t.P2Index]);
                }
                else
                {
                    Edges[t.P2Index, t.P1Index].P0Index = t.P2Index;
                    Edges[t.P2Index, t.P1Index].P1Index = t.P1Index;
                    Edges[t.P2Index, t.P1Index].AdjTriangle.Add(i);
                    Edges[t.P2Index, t.P1Index].Length = BallConcave.GetDistance(Points[t.P1Index], Points[t.P2Index]);
                }

                if (t.P0Index < t.P2Index)
                {
                    Edges[t.P0Index, t.P2Index].P0Index = t.P0Index;
                    Edges[t.P0Index, t.P2Index].P1Index = t.P2Index;
                    Edges[t.P0Index, t.P2Index].AdjTriangle.Add(i);
                    Edges[t.P0Index, t.P2Index].Length = BallConcave.GetDistance(Points[t.P0Index], Points[t.P2Index]);
                }
                else
                {
                    Edges[t.P2Index, t.P0Index].P0Index = t.P2Index;
                    Edges[t.P2Index, t.P0Index].P1Index = t.P0Index;
                    Edges[t.P2Index, t.P0Index].AdjTriangle.Add(i);
                    Edges[t.P2Index, t.P0Index].Length = BallConcave.GetDistance(Points[t.P0Index], Points[t.P2Index]);
                }
            }
            public void ExecuteEdgeDecimation(double length)
            {
                Queue<EdgeInfo> queue = new Queue<EdgeInfo>();
                for (int i = 0; i < Points.Count; i++)
                {
                    for (int j = 0; j < Points.Count; j++)
                    {
                        if (i < j && Edges[i, j].IsValid())
                        {
                            if (Edges[i, j].GetEdgeType() == 0)
                            {
                                throw new Exception();
                            }
                            if (Edges[i, j].Length > length && Edges[i, j].GetEdgeType() == 1)
                            {
                                queue.Enqueue(Edges[i, j]);
                            }
                        }
                    }
                }
                EdgeInfo[] opp1Temp = new EdgeInfo[2];
                while (queue.Count != 0)
                {
                    EdgeInfo info = queue.Dequeue();
                    if (info.AdjTriangle.Count != 1)
                        throw new Exception();
                    int tindex = info.AdjTriangle[0];
                    Triangle t = Faces[tindex];
                    InitOppEdge(opp1Temp, t, info);
                    SetInvalid(info.P0Index, info.P1Index);
                    for (int i = 0; i < 2; i++)
                    {
                        EdgeInfo e = opp1Temp[i];
                        e.AdjTriangle.Remove(tindex);
                        if (e.GetEdgeType() == 0)
                        {
                            SetInvalid(e.P0Index, e.P1Index);
                        }
                        else if (e.GetEdgeType() == 1 && e.Length > length)
                        {
                            queue.Enqueue(e);
                        }
                    }
                }
            }
            public List<EdgeInfo> GetBoundaryEdges()
            {
                List<EdgeInfo> list = new List<EdgeInfo>();
                for (int i = 0; i < Points.Count; i++)
                {
                    for (int j = 0; j < Points.Count; j++)
                    {
                        if (i < j)
                        {
                            if (Edges[i, j].GetEdgeType() == 1)
                            {
                                list.Add(Edges[i, j]);
                            }
                        }
                    }
                }
                return list;
            }
            private void SetInvalid(int i, int j)
            {
                Edges[i, j].AdjTriangle.Clear();
                Edges[i, j].Flag = true;
                Edges[i, j].P0Index = -1;
                Edges[i, j].P1Index = -1;
            }
            private void InitOppEdge(EdgeInfo[] opp1Temp, Triangle t, EdgeInfo info)
            {
                int vindex = t.P0Index + t.P1Index + t.P2Index - info.P0Index - info.P1Index;
                if (vindex < info.P0Index)
                {
                    opp1Temp[0] = Edges[vindex, info.P0Index];
                }
                else
                {
                    opp1Temp[0] = Edges[info.P0Index, vindex];
                }

                if (vindex < info.P1Index)
                {
                    opp1Temp[1] = Edges[vindex, info.P1Index];
                }
                else
                {
                    opp1Temp[1] = Edges[info.P1Index, vindex];
                }
            }
        }
        public class DelaunayTriangulation
        {
            public const int MaxVertices = 500;
            public const int MaxTriangles = 1000;
            public dVertex[] Vertex = new dVertex[MaxVertices];
            public dTriangle[] Triangle = new dTriangle[MaxTriangles];
            private bool InCircle(long xp, long yp, long x1, long y1, long x2, long y2, long x3, long y3, double xc, double yc, double r)
            {
                double eps;
                double m1;
                double m2;
                double mx1;
                double mx2;
                double my1;
                double my2;
                double dx;
                double dy;
                double rsqr;
                double drsqr;
                eps = 0.000000001;
                if (Math.Abs(y1 - y2) < eps && Math.Abs(y2 - y3) < eps)
                {
                    return false;
                }
                if (Math.Abs(y2 - y1) < eps)
                {
                    m2 = (-(Convert.ToDouble(x3) - Convert.ToDouble(x2)) / (Convert.ToDouble(y3) - Convert.ToDouble(y2)));
                    mx2 = Convert.ToDouble((x2 + x3) / 2.0);
                    my2 = Convert.ToDouble((y2 + y3) / 2.0);
                    xc = Convert.ToDouble((x2 + x1) / 2.0);
                    yc = Convert.ToDouble(m2 * (xc - mx2) + my2);
                }
                else if (Math.Abs(y3 - y2) < eps)
                {
                    m1 = (-(Convert.ToDouble(x2) - Convert.ToDouble(x1)) / (Convert.ToDouble(y2) - Convert.ToDouble(y1)));
                    mx1 = Convert.ToDouble((x1 + x2) / 2.0);
                    my1 = Convert.ToDouble((y1 + y2) / 2.0);
                    xc = Convert.ToDouble((x3 + x2) / 2.0);
                    yc = Convert.ToDouble(m1 * (xc - mx1) + my1);
                }
                else
                {
                    m1 = (-(Convert.ToDouble(x2) - Convert.ToDouble(x1)) / (Convert.ToDouble(y2) - Convert.ToDouble(y1)));
                    m2 = (-(Convert.ToDouble(x3) - Convert.ToDouble(x2)) / (Convert.ToDouble(y3) - Convert.ToDouble(y2)));
                    mx1 = Convert.ToDouble((x1 + x2) / 2.0);
                    mx2 = Convert.ToDouble((x2 + x3) / 2.0);
                    my1 = Convert.ToDouble((y1 + y2) / 2.0);
                    my2 = Convert.ToDouble((y2 + y3) / 2.0);
                    xc = Convert.ToDouble((m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2));
                    yc = Convert.ToDouble(m1 * (xc - mx1) + my1);
                }
                dx = (Convert.ToDouble(x2) - Convert.ToDouble(xc));
                dy = (Convert.ToDouble(y2) - Convert.ToDouble(yc));
                rsqr = Convert.ToDouble(dx * dx + dy * dy);
                r = Convert.ToDouble(Math.Sqrt(rsqr));
                dx = Convert.ToDouble(xp - xc);
                dy = Convert.ToDouble(yp - yc);
                drsqr = Convert.ToDouble(dx * dx + dy * dy);
                if (drsqr <= rsqr)
                {
                    return true;
                }
                return false;
            }
            private int WhichSide(long xp, long yp, long x1, long y1, long x2, long y2)
            {
                double equation;
                equation = ((Convert.ToDouble(yp) - Convert.ToDouble(y1)) * (Convert.ToDouble(x2) - Convert.ToDouble(x1))) - ((Convert.ToDouble(y2) - Convert.ToDouble(y1)) * (Convert.ToDouble(xp) - Convert.ToDouble(x1)));
                if (equation > 0)
                {
                    return -1;
                    //WhichSide = -1;
                }
                else if (equation == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            public int Triangulate(int nvert)
            {
                bool[] Complete = new bool[MaxTriangles];
                long[,] Edges = new long[3, MaxTriangles * 3 + 1];
                long Nedge;
                long xmin;
                long xmax;
                long ymin;
                long ymax;
                long xmid;
                long ymid;
                double dx;
                double dy;
                double dmax;
                int i;
                int j;
                int k;
                int ntri;
                double xc = 0.0;
                double yc = 0.0;
                double r = 0.0;
                bool inc;
                xmin = Vertex[1].x;
                ymin = Vertex[1].y;
                xmax = xmin;
                ymax = ymin;
                for (i = 2; i <= nvert; i++)
                {
                    if (Vertex[i].x < xmin)
                    {
                        xmin = Vertex[i].x;
                    }
                    if (Vertex[i].x > xmax)
                    {
                        xmax = Vertex[i].x;
                    }
                    if (Vertex[i].y < ymin)
                    {
                        ymin = Vertex[i].y;
                    }
                    if (Vertex[i].y > ymax)
                    {
                        ymax = Vertex[i].y;
                    }
                }
                dx = Convert.ToDouble(xmax) - Convert.ToDouble(xmin);
                dy = Convert.ToDouble(ymax) - Convert.ToDouble(ymin);
                if (dx > dy)
                {
                    dmax = dx;
                }
                else
                {
                    dmax = dy;
                }
                xmid = (xmax + xmin) / 2;
                ymid = (ymax + ymin) / 2;
                Vertex[nvert + 1].x = Convert.ToInt64(xmid - 2 * dmax);
                Vertex[nvert + 1].y = Convert.ToInt64(ymid - dmax);
                Vertex[nvert + 2].x = xmid;
                Vertex[nvert + 2].y = Convert.ToInt64(ymid + 2 * dmax);
                Vertex[nvert + 3].x = Convert.ToInt64(xmid + 2 * dmax);
                Vertex[nvert + 3].y = Convert.ToInt64(ymid - dmax);
                Triangle[1].vv0 = nvert + 1;
                Triangle[1].vv1 = nvert + 2;
                Triangle[1].vv2 = nvert + 3;
                Complete[1] = false;
                ntri = 1;
                for (i = 1; i <= nvert; i++)
                {
                    Nedge = 0;
                    j = 0;
                    do
                    {
                        j = j + 1;
                        if (Complete[j] != true)
                        {
                            inc = InCircle(Vertex[i].x, Vertex[i].y, Vertex[Triangle[j].vv0].x, Vertex[Triangle[j].vv0].y, Vertex[Triangle[j].vv1].x, Vertex[Triangle[j].vv1].y, Vertex[Triangle[j].vv2].x, Vertex[Triangle[j].vv2].y, xc, yc, r);
                            if (inc)
                            {
                                Edges[1, Nedge + 1] = Triangle[j].vv0;
                                Edges[2, Nedge + 1] = Triangle[j].vv1;
                                Edges[1, Nedge + 2] = Triangle[j].vv1;
                                Edges[2, Nedge + 2] = Triangle[j].vv2;
                                Edges[1, Nedge + 3] = Triangle[j].vv2;
                                Edges[2, Nedge + 3] = Triangle[j].vv0;
                                Nedge = Nedge + 3;
                                Triangle[j].vv0 = Triangle[ntri].vv0;
                                Triangle[j].vv1 = Triangle[ntri].vv1;
                                Triangle[j].vv2 = Triangle[ntri].vv2;
                                Complete[j] = Complete[ntri];
                                j = j - 1;
                                ntri = ntri - 1;
                            }
                        }
                    }
                    while (j < ntri);
                    for (j = 1; j <= Nedge - 1; j++)
                    {
                        if (Edges[1, j] != 0 && Edges[2, j] != 0)
                        {
                            for (k = j + 1; k <= Nedge; k++)
                            {
                                if (Edges[1, k] != 0 && Edges[2, k] != 0)
                                {
                                    if (Edges[1, j] == Edges[2, k])
                                    {
                                        if (Edges[2, j] == Edges[1, k])
                                        {
                                            Edges[1, j] = 0;
                                            Edges[2, j] = 0;
                                            Edges[1, k] = 0;
                                            Edges[2, k] = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    for (j = 1; j <= Nedge; j++)
                    {
                        if (Edges[1, j] != 0 && Edges[2, j] != 0)
                        {
                            ntri = ntri + 1;
                            Triangle[ntri].vv0 = Edges[1, j];
                            Triangle[ntri].vv1 = Edges[2, j];
                            Triangle[ntri].vv2 = i;
                            Complete[ntri] = false;
                        }
                    }
                }
                i = 0;
                do
                {
                    i = i + 1;
                    if (Triangle[i].vv0 > nvert || Triangle[i].vv1 > nvert || Triangle[i].vv2 > nvert)
                    {
                        Triangle[i].vv0 = Triangle[ntri].vv0;
                        Triangle[i].vv1 = Triangle[ntri].vv1;
                        Triangle[i].vv2 = Triangle[ntri].vv2;
                        i = i - 1;
                        ntri = ntri - 1;
                    }
                }
                while (i < ntri);
                return ntri;
            }
            public static double Diameter(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
            {
                double x, y;
                double a = Ax;
                double b = Bx;
                double c = Cx;
                double m = Ay;
                double n = By;
                double k = Cy;
                double A = a * b * b + a * n * n + a * a * c - b * b * c + m * m * c - n * n * c - a * c * c - a * k * k - a * a * b + b * c * c - m * m * b + b * k * k;
                double B = a * n + m * c + k * b - n * c - a * k - b * m;
                y = A / B / 2;
                double AA = b * b * m + m * n * n + a * a * k - b * b * k + m * m * k - n * n * k - c * c * m - m * k * k - a * a * n + c * c * n - m * m * n + k * k * n;
                double BB = b * m + a * k + c * n - b * k - c * m - a * n;
                x = AA / BB / 2;
                return Math.Sqrt((Ax - x) * (Ax - x) + (Ay - y) * (Ay - y));
            }
            public static double MaxEdge(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
            {
                double len1 = Math.Sqrt((Ax - Bx) * (Ax - Bx) + (Ay - By) * (Ay - By));
                double len2 = Math.Sqrt((Cx - Bx) * (Cx - Bx) + (Cy - By) * (Cy - By));
                double len3 = Math.Sqrt((Ax - Cx) * (Ax - Cx) + (Ay - Cy) * (Ay - Cy));
                double len = len1 > len2 ? len1 : len2;
                return len > len3 ? len : len3;
            }
        }
        public static List<OrTriangle> GetTriangles(List<Point2d> plist)
        {
            DelaunayTriangulation del = new DelaunayTriangulation();
            for (int i = 1; i <= plist.Count; i++)
            {
                del.Vertex[i].x = (int)plist[i - 1].X;
                del.Vertex[i].y = (int)plist[i - 1].Y;
            }
            int ntri = del.Triangulate(plist.Count);
            List<OrTriangle> tlist = new List<OrTriangle>();
            for (int i = 1; i <= ntri; i++)
            {
                OrTriangle t = new OrTriangle();
                t.p0.X = del.Vertex[del.Triangle[i].vv0].x;
                t.p0.Y = del.Vertex[del.Triangle[i].vv0].y;
                t.p1.X = del.Vertex[del.Triangle[i].vv1].x;
                t.p1.Y = del.Vertex[del.Triangle[i].vv1].y;
                t.p2.X = del.Vertex[del.Triangle[i].vv2].x;
                t.p2.Y = del.Vertex[del.Triangle[i].vv2].y;
                tlist.Add(t);
            }
            return tlist;
        }
        public static DelaunayMesh2d GetMesh(List<Point2d> plist)
        {
            DelaunayMesh2d mesh = new DelaunayMesh2d();
            DelaunayTriangulation del = new DelaunayTriangulation();
            for (int i = 1; i <= plist.Count; i++)
            {
                del.Vertex[i].x = (int)plist[i - 1].X;
                del.Vertex[i].y = (int)plist[i - 1].Y;
            }
            int ntri = del.Triangulate(plist.Count);
            for (int i = 0; i < plist.Count; i++)
            {
                mesh.AddVertex(plist[i]);
            }
            for (int i = 1; i <= ntri; i++)
            {
                mesh.AddFace(new Triangle((int)del.Triangle[i].vv0 - 1, (int)del.Triangle[i].vv1 - 1, (int)del.Triangle[i].vv2 - 1));
            }
            for (int i = 0; i < ntri; i++)
            {
                Triangle t = mesh.Faces[i];
                t.Index = i;
                mesh.Faces[i] = t;
            }
            return mesh;
        }
    }
}
