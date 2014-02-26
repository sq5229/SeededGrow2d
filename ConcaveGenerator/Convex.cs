using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ConcaveGenerator
{
    public struct Point2d:IComparable<Point2d>
    {
        public double X;
        public double Y;
        public int Index;
        public Point2d(double x, double y,int index)
        {
            this.X = x;
            this.Y = y;
            this.Index = index;
        }
        public Point2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Index = -1;
        }
        public static Point2d Minus(Point2d p0, Point2d p1)
        {
            return new Point2d(p0.X - p1.X, p0.Y - p1.Y);
        }
        public int CompareTo(Point2d other)
        {
            if (this.Y < other.Y)
                return -1;
            else if (this.Y < other.Y && this.X < other.X)
                return -1;
            else
                return 1;
        }
        public override string ToString()
        {
            return "("+Index+")";
        }
    }
    public class Convex
    {
        public static List<Point2d> GetConvexhull(List<Point2d> p)
        {
            List<Point2d> ret=new List<Point2d>();
            Point2d[] res = new Point2d[200];
            p.Sort();
            res[0] = p[0];
            res[1] = p[1];
            int top = 1, i = 0, n = p.Count ;
            for (i = 2; i < n; i++)
            {
                while (top!=0 && !ral(res[top], res[top - 1], p[i]))
                    top--;
                res[++top] = p[i];
            }
            int len = top;
            res[++top] = p[n - 2];
            for (i = n - 3; i >= 0; i--)
            {
                while (top != len && !ral(res[top], res[top - 1], p[i]))
                    top--;
                res[++top] = p[i];
            }
            for (i = 0; i < top; i++)
            {
                ret.Add(res[i]);
            }
            return ret;
        }
        static bool ral(Point2d p1, Point2d p2, Point2d p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) > (p3.X - p1.X) * (p2.Y - p1.Y);
        }


    }
}
