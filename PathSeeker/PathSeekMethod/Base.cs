using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathSeeker.PathSeekMethod
{
    public class Consts
    {
        public static string[] Directions = new string[8] { "X-", "X+", "Y-", "Y+", "X-Y-", "X+Y-", "X-Y+", "X+Y+" };
        public static IntDouble[] IndexToOffset = new IntDouble[8] {
                new IntDouble(-1, 0), new IntDouble(1, 0), new IntDouble(0, -1), new IntDouble(0, 1)
            , new IntDouble(-1,-1), new IntDouble(1,-1), new IntDouble(-1,1), new IntDouble(1,1)};
        public static int[,] OffsetToIndex = new int[3, 3] { { 4, 0, 6 }, { 2, -1, 3 }, { 5, 1, 7 } };
        public static int[,] OffsetToEurWeight = new int[3, 3] { { 14, 10, 14 }, { 10, 0, 10 }, { 14, 10, 14 } };
        public static int GetEvaDistance_Euro(int x1,int y1,int x2,int y2)
        {
            return (int)(10 * Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        }
        public static int GetEvaDistance_Manh(int x1, int y1, int x2, int y2)
        {
            return 10*(Math.Abs(x1 - x2) + Math.Abs(y1 - y2));
        }
        public static T[,] Init2dArray<T>(int size, T v)
        {
            T[,] ret = new T[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ret[i, j] = v;
                }
            }
            return ret;
        }
    }
    public class ResultCollection
    {
        public int[,] parentMap;
        public int[,] inputMap;
        public int[,] resultMap;
        public int[,] distanceMap;
        public int[,] evaDistanceMap;
    }
    public struct IntDouble
    {
        public int X;
        public int Y;
        public IntDouble(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public override string ToString()
        {
            return X+","+Y;
        }
    }
}
