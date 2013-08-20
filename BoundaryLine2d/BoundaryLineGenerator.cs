using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BoundaryLine2d
{
    public static class BoundaryLine2dCaculator
    {
        static Int16Double[][] AdjIndexToEdge = new Int16Double[4][]
        {
            new Int16Double[2] { new Int16Double(0, 0), new Int16Double(0, 1) },
            new Int16Double[2] { new Int16Double(0, 1), new Int16Double(1, 1) },
            new Int16Double[2] { new Int16Double(1, 1), new Int16Double(1, 0) },
            new Int16Double[2] { new Int16Double(1, 0), new Int16Double(0, 0) },
            };
        static Int16Double[] VertexIndexToPositionDelta = new Int16Double[4]
        {
            new Int16Double(-1, 0),
            new Int16Double(0, 1),
            new Int16Double(1, 0),
            new Int16Double(0, -1)
        };
        class PolyLine2dBuider
        {
            int width;
            int height;
            int[,] mapHash;
            PolyLine polyline;
            public PolyLine2dBuider(int width, int height)
            {
                this.height = height + 2;
                this.width = width + 2;
                mapHash = new int[this.width, this.height];
                for (int i = 0; i < this.width; i++)
                    for (int j = 0; j < this.height; j++)
                        mapHash[i, j] = -1;
                polyline = new PolyLine();
            }
            public void AddLine(int x1, int y1, int x2, int y2)
            {
                int index1;
                if (mapHash[x1, y1] == -1)
                {
                    index1 = polyline.Points.Count;
                    polyline.Points.Add(new FloatDouble(x1, y1));
                    mapHash[x1, y1 ] = index1;
                }
                else
                {
                    index1 = mapHash[x1 , y1];
                }
                int index2;
                if (mapHash[x2, y2] == -1)
                {
                    index2 = polyline.Points.Count;
                    polyline.Points.Add(new FloatDouble(x2, y2));
                    mapHash[x2, y2] = index2;
                }
                else
                {
                    index2 = mapHash[x2, y2];
                }
                Int16Double line = new Int16Double(index1, index2);
                polyline.Edges.Add(line);
            }
            public PolyLine GetLine()
            {
                return polyline;
            }
            public void Clear()
            {
                mapHash = null;
            }
        }
        class PolyLine
        {
            public PolyLine()
            {
                Points = new List<FloatDouble>();
                Edges = new List<Int16Double>();
            }
            public List<FloatDouble> Points;
            public List<Int16Double> Edges;
            public List<FloatDouble> GetDrawablePointList()
            {
                List<FloatDouble> result = new List<FloatDouble>();
                List<int>[] adjinfo = new List<int>[Points.Count];
                for (int i = 0; i < Points.Count; i++)
                {
                    adjinfo[i] = new List<int>(2);
                }
                for (int i = 0; i < Edges.Count; i++)
                {
                    adjinfo[Edges[i].X].Add(Edges[i].Y);
                    adjinfo[Edges[i].Y].Add(Edges[i].X);
                }
                for (int i = 0; i < Points.Count; i++)
                {
                    if (adjinfo[i].Count != 2)
                        throw new Exception();
                }
                bool[] visitedflags = new bool[Points.Count];
                for (int i = 0; i < Points.Count;i++ )
                    visitedflags[i] = false;
                Queue<int> queue=new Queue<int>();
                visitedflags[0] = true;
                visitedflags[adjinfo[0][0]]=true;
                result.Add(Points[0]);
                result.Add(Points[adjinfo[0][0]]);
                queue.Enqueue(adjinfo[0][0]);
                while(queue.Count!=0)
                {
                    int index=queue.Dequeue();
                    int adjindex0=adjinfo[index][0];
                    int adjindex1=adjinfo[index][1];
                    if(!visitedflags[adjindex0]&&visitedflags[adjindex1])
                    {
                        visitedflags[adjindex0]=true;
                        queue.Enqueue(adjindex0);
                        result.Add(Points[adjindex0]);
                    }
                    else if(visitedflags[adjindex0]&&!visitedflags[adjindex1])
                    {
                         visitedflags[adjindex1]=true;
                        queue.Enqueue(adjindex1);
                        result.Add(Points[adjindex1]);
                    }
                    //else { //throw new Exception(); };
                }
                return result;
            }

        }
        static void InitAdj4(Int16Double[] adj4, int x, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                adj4[i].X = x + VertexIndexToPositionDelta[i].X;
                adj4[i].Y = y + VertexIndexToPositionDelta[i].Y;
            }
        }
        class SquarSurface
        {
            BitMap2d bmp;
            int Width;
            int Height;
            PolyLine2dBuider pb;
            public SquarSurface(BitMap2d bmp)
            {
                this.bmp = bmp;
                Width = bmp.width;
                Height = bmp.height;
                pb = new PolyLine2dBuider(bmp.width, bmp.height);
            }
            public PolyLine GenerateLine()
            {
                Int16Double[] adj = new Int16Double[4];
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (bmp.GetPixel(i, j) != BitMap2d.WHITE)
                            continue;
                        InitAdj4(adj, i, j);
                        for (int k = 0; k < 4; k++)
                        {
                            Int16Double t = adj[k];
                            if (bmp.GetPixel(t.X, t.Y) != BitMap2d.WHITE)
                            {
                                Int16Double v1d = AdjIndexToEdge[k][0];
                                Int16Double v2d = AdjIndexToEdge[k][1];
                                int x1 = i + v1d.X;
                                int y1 = j + v1d.Y;
                                int x2 = i + v2d.X;
                                int y2 = j + v2d.Y;
                                pb.AddLine(x1, y1, x2, y2);
                            }
                        }
                    }
                }
                return pb.GetLine();
            }
        }
        public static List<Point3d> GetBoundaryLine2d(List<Int16Triple> regionPoints, bool smoothing,int iteration)
        {
            Box3Int box = GetBox3(regionPoints);
            int stx = 0, sty = 0;
            BitMap2d bmp_1 = GetMappedBitmap(regionPoints, box,ref stx,ref sty);
           // bmp_1.OutputBitMap("1.bmp");
            BitMap2d bmp_2 = GetSingleRegionBitmap(bmp_1);
           // bmp_2.OutputBitMap("2.bmp");
            bmp_1 = null;
            SquarSurface mc2 = new SquarSurface(bmp_2);
            PolyLine polyline = mc2.GenerateLine();
            if (smoothing&&iteration>=1)
            {
                for (int i = 0; i < iteration;i++)
                    SmoothingLine(polyline);
            }
           // PlyManager.Output2(polyline.Points,polyline.Edges,"3.ply");
            List<FloatDouble> points = polyline.GetDrawablePointList();
            List<Point3d> ret = new List<Point3d>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                ret.Add(new Point3d(points[i].X + stx, points[i].Y + sty,0));
            }
            return ret;
        }
        private static void SmoothingLine(PolyLine line)
        {
            List<FloatDouble> Points = line.Points;
            List<Int16Double> Edges = line.Edges;
            List<int>[] adjinfo = new List<int>[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                adjinfo[i] = new List<int>(2);
            }
            for (int i = 0; i < Edges.Count; i++)
            {
                adjinfo[Edges[i].X].Add(Edges[i].Y);
                adjinfo[Edges[i].Y].Add(Edges[i].X);
            }
            FloatDouble[] tempPos = new FloatDouble[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                FloatDouble ap0 = Points[adjinfo[i][0]];
                FloatDouble ap1 = Points[adjinfo[i][1]];
                tempPos[i].X = (ap0.X+ap1.X)/2.0f;
                tempPos[i].Y = (ap0.Y + ap1.Y) / 2.0f;
            }
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = tempPos[i];
            }
        }
        private static BitMap2d GetSingleRegionBitmap(BitMap2d b)
        {
            BitMap2d bmp = new BitMap2d(b.width, b.height, BitMap2d.WHITE);
            Queue<Int16Double> queue = new Queue<Int16Double>();
            queue.Enqueue(new Int16Double(0, 0));
            bmp.SetPixel(0, 0, BitMap2d.BLACK);
            Int16Double[] adj = new Int16Double[4];
            while (queue.Count != 0)
            {
                Int16Double pix = queue.Dequeue();
                InitAdj4(adj, pix.X, pix.Y);
                for (int i = 0; i < 4; i++)
                {
                    Int16Double t=adj[i];
                    if (t.X >= 0 && t.X < bmp.width && t.Y >= 0 && t.Y < bmp.height
                        && b.GetPixel(t.X, t.Y) == BitMap2d.BLACK && bmp.GetPixel(t.X, t.Y)==BitMap2d.WHITE)
                    {
                        bmp.SetPixel(t.X, t.Y, BitMap2d.BLACK);
                        queue.Enqueue(t);
                    }
                }
            }
            return bmp;
        }
        private static Box3Int GetBox3(List<Int16Triple> regionPoints)
        {
            Box3Int box = new Box3Int();
            for (int i = 0; i < regionPoints.Count; i++)
            {
                box.UpdataRange(regionPoints[i].X, regionPoints[i].Y, regionPoints[i].Z);
            }
            return box;
        }
        private static BitMap2d GetMappedBitmap(List<Int16Triple> regionPoints, Box3Int box, ref int stx, ref int sty)
        {
            BitMap2d bmp = new BitMap2d(box.GetXLength() + 2, box.GetYLength() + 2, BitMap2d.BLACK);
            stx = box.Min3[0] - 1;
            sty = box.Min3[1] - 1;
            for (int i = 0; i < regionPoints.Count; i++)
            {
                bmp.SetPixel(regionPoints[i].X - stx, regionPoints[i].Y - sty, BitMap2d.WHITE);
            }
            return bmp;
        }
    }
}
