using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace FindMaxima
{
    public struct Int16Double
    {
        public int X;
        public int Y;
        public Int16Double(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public struct Int16DoubleWithValue:IComparable<Int16DoubleWithValue>
    {
        public int X;
        public int Y;
        public float V;
        public Int16DoubleWithValue(int x, int y, float value)
        {
            X = x;
            Y = y;
            V = value;
        }

        public int CompareTo(Int16DoubleWithValue other)
        {
            return -this.V.CompareTo(other.V);
        }
    }
    public class BitMap2d
    {
        public float[] data;
        public int width;
        public int height;
        public BitMap2d(int width, int height, float v)
        {
            this.width = width;
            this.height = height;
            data = new float[width * height];
            for (int i = 0; i < width * height; i++)
                data[i] = v;
        }
        public void SetPixel(int x, int y, byte v)
        {
            data[x + y * width] = v;
        }
        public float GetPixel(int x, int y)
        {
            return data[x + y * width];
        }
        public void ReadRaw(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader sr = new BinaryReader(fs);

            for (int i = 0; i < width * height; i++)
            {
                byte[] floatBytes = sr.ReadBytes(4);

                // swap the bytes

                byte temp = floatBytes[0];

                floatBytes[0] = floatBytes[3];

                floatBytes[3] = temp;

                temp = floatBytes[1];

                floatBytes[1] = floatBytes[2];

                floatBytes[2] = temp;

                // get the float from the byte array

                float value = BitConverter.ToSingle(floatBytes, 0);
                data[i] = value;
            }
            sr.Close();
            fs.Close();
            return;
        }
        public Bitmap MakeBmp()
        { 
            float min=float.MaxValue;
            float max=float.MinValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                     float r=this.GetPixel(i,j);
                    if(r>max)
                        max=r;
                    if(r<min)
                        min=r;
                }
            }
            float delta=max-min;
            Bitmap bmp = new Bitmap(this.width, this.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float r=this.GetPixel(i,j);
                    int b=(int)(255*(r-min)/delta);
                    Color c=Color.FromArgb((byte)b,(byte)b,(byte)b);
                    bmp.SetPixel(i, j, c);
                }
            }
            return bmp;
        }
    }
    public class FlagMap2d
    {
        public int action_set_count;
        public int action_get_count;
        public int width;
        public int height;
        byte[] flags;
        public FlagMap2d(int width, int height,byte v)
        {
            this.width = width;
            this.height = height;
            action_get_count = 0;
            action_set_count = 0;
            flags = new byte[width * height];
            for(int i=0;i<width*height;i++)
                flags[i] = v;
        }
        public void SetFlagOn(int x, int y, byte v)
        {
            flags[x + y * width] = v;
            action_set_count++;
        }
        public byte GetFlagOn(int x, int y)
        {
            action_get_count++;
            return flags[x + y * width];
        }
    }
}
