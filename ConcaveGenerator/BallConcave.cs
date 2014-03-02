using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcaveGenerator
{
public class BallConcave
{
    public MainWindow main;
    struct Point2dInfo : IComparable<Point2dInfo>
    {
        public Point2d Point;
        public int Index;
        public double DistanceTo;
        public Point2dInfo(Point2d p,int i,double dis)
        {
            this.Point = p;
            this.Index = i;
            this.DistanceTo = dis;
        }
        public int CompareTo(Point2dInfo other)
        {
            return DistanceTo.CompareTo(other.DistanceTo);
        }
        public override string ToString()
        {
            return Point+","+Index+","+DistanceTo;
        }
    }
    public BallConcave(List<Point2d> list)
    {
        this.points = list;
        points.Sort();
        flags = new bool[points.Count];
        for (int i = 0; i < flags.Length; i++)
            flags[i] = false;
        InitDistanceMap();
        InitNearestList();
    }
    private bool[] flags;
    private List<Point2d> points;
    private double[,] distanceMap;
    private List<int>[] rNeigbourList;
    private void InitNearestList()
    {
        rNeigbourList = new List<int>[points.Count];
        for (int i = 0; i < rNeigbourList.Length; i++)
        {
            rNeigbourList[i] = GetSortedNeighbours(i);
        }
    }
    private void InitDistanceMap()
    {
        distanceMap = new double[points.Count, points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                distanceMap[i, j] = GetDistance(points[i], points[j]);
            }
        }
    }
    public double GetRecomandedR()
    {
        double r=double.MinValue;
        for (int i = 0; i < points.Count; i++)
        {
            if (distanceMap[i, rNeigbourList[i][1]] > r)
                r = distanceMap[i, rNeigbourList[i][1]];
        }
        return r;
    }
    public double GetMinEdgeLength()
    {
        double min = double.MaxValue;
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if (i < j)
                {
                    if (distanceMap[i, j] < min)
                        min = distanceMap[i, j];
                }
            }
        }
        return min;
    }
    public List<Point2d> GetConcave_Ball(double radius)
    {
        List<Point2d> ret = new List<Point2d>();
        List<int>[] adjs = GetInRNeighbourList(2*radius);
        ret.Add(points[0]);
        //flags[0] = true;
        int i = 0, j = -1, prev = -1;
        while (true)
        {
            j = GetNextPoint_BallPivoting(prev, i, adjs[i],radius);
            if (j == -1)
                break;
            Point2d p = BallConcave.GetCircleCenter(points[i], points[j], radius);
            ret.Add(points[j]);
            flags[j] = true;
            prev = i;
            i = j;
        }
        return ret;
    }
    public List<Point2d> GetConcave_Edge(double radius)
    {
        List<Point2d> ret = new List<Point2d>();
        List<int>[] adjs = GetInRNeighbourList(2 * radius);
        ret.Add(points[0]);
        int i = 0, j = -1, prev = -1;
        while (true)
        {
            j = GetNextPoint_EdgePivoting(prev, i, adjs[i], radius);
            if (j == -1)
                break;
            //Point2d p = BallConcave.GetCircleCenter(points[i], points[j], radius);
            ret.Add(points[j]);
            flags[j] = true;
            prev = i;
            i = j;
        }
        return ret;
    }
    private bool CheckValid(List<int>[] adjs)
    {
        for (int i = 0; i < adjs.Length; i++)
        {
            if (adjs[i].Count < 2)
            {
                return false;
            }
        }
        return true;
    }
    public bool CompareAngel(Point2d a, Point2d b, Point2d m_origin, Point2d m_dreference)
    {

        Point2d da = new Point2d(a.X - m_origin.X, a.Y - m_origin.Y);
        Point2d db = new Point2d(b.X - m_origin.X, b.Y - m_origin.Y);
        double detb = GetCross(m_dreference, db);

        // nothing is less than zero degrees
        if (detb == 0 && db.X * m_dreference.X + db.Y * m_dreference.Y >= 0) return false;

        double deta = GetCross(m_dreference, da);

        // zero degrees is less than anything else
        if (deta == 0 && da.X * m_dreference.X + da.Y * m_dreference.Y >= 0) return true;

        if (deta * detb >= 0) {
            // both on same side of reference, compare to each other
            return GetCross(da, db) > 0;
        }

        // vectors "less than" zero degrees are actually large, near 2 pi
        return deta > 0;
    }
    public int GetNextPoint_EdgePivoting(int prev,int current,List<int> list,double radius)
    {
        if (list.Count == 2 && prev != -1)
        {
            return list[0] + list[1] - prev;
        }
        Point2d dp;
        if (prev == -1)
            dp = new Point2d(1, 0);
        else
            dp= Point2d.Minus(points[prev], points[current]);
        int min = -1;
        for (int j = 0; j < list.Count; j++)
        {
            if (!flags[list[j]])
            {
                if (min == -1)
                {
                    min = list[j];
                }
                else
                {
                    Point2d t = points[list[j]];
                    if (CompareAngel(points[min], t, points[current], dp) && GetDistance(t,points[current])<radius)
                    {
                        min = list[j];
                    }
                }
            }
        }
        //main.ShowMessage("seek P" + points[min].Index);
        return min;
    }
    public int GetNextPoint_BallPivoting(int prev, int current, List<int> list, double radius)
    {
        SortAdjListByAngel(list, prev, current);
        for (int j = 0; j < list.Count; j++)
        {
            if (flags[list[j]])
                continue;
            int adjIndex = list[j];
            Point2d xianp = points[adjIndex];
            Point2d rightCirleCenter = GetCircleCenter(points[current],xianp,radius);
            if (!HasPointsInCircle(list,rightCirleCenter,radius,adjIndex))
            {
                main.DrawCircleWithXian(rightCirleCenter, points[current], points[adjIndex], radius);
                return list[j];
            }
        }
        return -1;
    }
    private void SortAdjListByAngel(List<int> list, int prev, int current)
    {
        Point2d origin = points[current];
        Point2d df;
        if (prev != -1)
            df = new Point2d(points[prev].X - origin.X, points[prev].Y - origin.Y);
        else
            df = new Point2d(1, 0);
        int temp = 0;
        for (int i = list.Count; i > 0; i--)
        {
            for (int j = 0; j < i - 1; j++)
            {
                if (CompareAngel(points[list[j]], points[list[j + 1]], origin, df))
                {
                    temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }
    private bool HasPointsInCircle(List<int> adjPoints,Point2d center,double radius,int adjIndex)
    {
        for (int k = 0; k < adjPoints.Count; k++)
        {
            if (adjPoints[k] != adjIndex)
            {
                int index2 = adjPoints[k];
                if (IsInCircle(points[index2], center, radius))
                    return true;
            }
        }
        return false;
    }
    public static Point2d GetCircleCenter(Point2d a, Point2d b, double r)
    {
        double dx = b.X - a.X;
        double dy = b.Y - a.Y;
        double cx = 0.5 * (b.X + a.X);
        double cy = 0.5 * (b.Y + a.Y);
        if (r * r / (dx * dx + dy * dy) - 0.25 < 0)
        {
            return new Point2d(-1, -1);
        }
        double sqrt = Math.Sqrt(r * r / (dx * dx + dy * dy) - 0.25);
        return new Point2d(cx - dy * sqrt, cy + dx * sqrt);
    }
    public static bool IsInCircle(Point2d p,Point2d center,double r)
    {
        double dis2 = (p.X - center.X) * (p.X - center.X) + (p.Y - center.Y) * (p.Y - center.Y);
        return dis2 < r * r;
    }
    public List<int>[] GetInRNeighbourList(double radius)
    {
        List<int>[] adjs = new List<int>[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            adjs[i] = new List<int>();
        }
        for (int i = 0; i < points.Count; i++)
        {

            for (int j = 0; j < points.Count; j++)
            {
                if (i < j && distanceMap[i, j] < radius)
                {
                    adjs[i].Add(j);
                    adjs[j].Add(i);
                }
            }
        }
        return adjs;
    }
    private List<int> GetSortedNeighbours(int index)
    {
        List<Point2dInfo> infos = new List<Point2dInfo>(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            infos.Add(new Point2dInfo(points[i], i, distanceMap[index, i]));
        } 
        infos.Sort();
        List<int> adj = new List<int>();
        for (int i = 1; i < infos.Count; i++)
        {
                adj.Add(infos[i].Index);
        }
        return adj;
    }
    public static double GetDistance(Point2d p1, Point2d p2)
    {
        return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }
    public static double GetCross(Point2d a, Point2d b) 
    {
        return a.X * b.Y - a.Y * b.X;
    }
}

}
