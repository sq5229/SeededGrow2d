using System;
using System.Collections.Generic;
using System.Text;

namespace ContourLineRecon
{
    public class ContourLine:IComparable<ContourLine>
    {
        List<FloatDouble> points;
        float z;
        public ContourLine()
        {
            points = new List<FloatDouble>();
            z = 0;
        }
        public void AddPoint2d(short x, short y)
        {
            FloatDouble t = new FloatDouble(x, y);
            points.Add(t);
        }
        public int GetLinePointCount()
        {
            return points.Count;
        }
        public void SetZ(int z)
        {
            this.z = z;
        }
        public float GetZ()
        {
            return z;
        }
        public List<FloatDouble> GetPointList()
        {
            return points;
        }
        public void ReverseClock()
        {

        }
        public Box3Float GetBox()
        {
            Box3Float box = new Box3Float();
            for (int i = 0; i < points.Count; i++)
            {
                box.UpdataRange(points[i].X, points[i].Y, 0);
            }
            return box;
        }
        public int CompareTo(ContourLine other)
        {
            return this.GetZ().CompareTo(other.GetZ());
        }
    }
}
