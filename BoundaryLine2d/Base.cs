using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryLine2d
{
    public class Box3Int
    {
        public int[] Min3;
        public int[] Max3;
        public Box3Int()
        {
            Min3 = new int[3] { int.MaxValue, int.MaxValue, int.MaxValue };
            Max3 = new int[3] { int.MinValue, int.MinValue, int.MinValue };
        }
        public void UpdataRange(int x, int y, int z)
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
        public int GetXLength() { return Max3[0] - Min3[0] + 1; }
        public int GetYLength() { return Max3[1] - Min3[1] + 1; }
        public int GetZLength() { return Max3[2] - Min3[2] + 1; }
        public override string ToString()
        {
            return string.Format("[{0},{1}] [{2},{3}] [{4},{5}]", Min3[0], Max3[0], Min3[1], Max3[1], Min3[2], Max3[2]);
        }
    }
    public struct Int16Triple
    {
        public int X;
        public int Y;
        public int Z;
        public Int16Triple(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
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
    public struct Int16Double
    {
        public int X;
        public int Y;
        public Int16Double(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
    public struct FloatDouble
    {
        public float X;
        public float Y;
        public FloatDouble(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
    public class BitMap2d
    {
        public const byte WHITE = 255;
        public const byte BLACK = 0;
        public byte[] data;
        public int width;
        public int height;
        public int action_set_count;
        public int action_get_count;
        public BitMap2d(int width, int height, byte v)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];
            action_set_count = 0;
            action_get_count = 0;
            for (int i = 0; i < width * height; i++)
                data[i] = v;
        }
        public BitMap2d(byte[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }
        public void SetPixel(int x, int y, byte v)
        {
            action_set_count++;
            data[x + y * width] = v;
        }
        public byte GetPixel(int x, int y)
        {
            action_get_count++;
            return data[x + y * width];
        }
        public void ResetVisitCount()
        {
            action_get_count = 0;
            action_set_count = 0;
        }


        public void ReadBitmap(string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    System.Drawing.Color c = bmp.GetPixel(i, j);
                    byte v = (c.R == 255) ? WHITE : BLACK;
                    this.SetPixel(i, j, v);
                }
            }
            bmp.Dispose();
        }
        public void OutputBitMap(string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(this.width, this.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte s = this.GetPixel(i, j);
                    if (s > 128)
                        bmp.SetPixel(i, j, System.Drawing.Color.White);
                    else
                        bmp.SetPixel(i, j, System.Drawing.Color.Black);
                }
            }
            bmp.Save(path);
            bmp.Dispose();
        }
    }
}
