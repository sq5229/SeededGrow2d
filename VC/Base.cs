using System;
using System.Collections.Generic;
using System.Text;

namespace VC
{
    public class Box3Float
    {
        public float[] Min3;
        public float[] Max3;
        public Box3Float()
        {
            Min3 = new float[3] { int.MaxValue, int.MaxValue, int.MaxValue };
            Max3 = new float[3] { int.MinValue, int.MinValue, int.MinValue };
        }
        public void UpdataRange(float x, float y, float z)
        {
            if (x < Min3[0])
                Min3[0] = x;
            if (y < Min3[1])
                Min3[1] = y;
            if (z < Min3[2])
                Min3[2] = z;

            if (x > Max3[0])
                Max3[0] = x;
            if (y > Max3[1])
                Max3[1] = y;
            if (z > Max3[2])
                Max3[2] = z;
        }
        public float GetXLength() { return Max3[0] - Min3[0] + 1; }
        public float GetYLength() { return Max3[1] - Min3[1] + 1; }
        public float GetZLength() { return Max3[2] - Min3[2] + 1; }
        public override string ToString()
        {
            return string.Format("[{0},{1}] [{2},{3}] [{4},{5}]", Min3[0], Max3[0], Min3[1], Max3[1], Min3[2], Max3[2]);
        }
    }
}
