using System;
using System.Collections.Generic;
using System.Text;

namespace ImageScaling
{
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

        public static BitMap2d CreateSampleCircle(int width)
        {
            if (width % 2 != 1) width += 1;
            BitMap2d bmp = new BitMap2d(width, width,0);
            int r = (width - 1) / 2;
            int mx=r;
            int my=r;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double dis2 = (i - mx) * (i - mx) + (j - my) * (j - my);
                    if (dis2 < r*r)
                    {
                        bmp.SetPixel(i, j, 255);
                    }
                }
            }
             return bmp;
        }

        public static BitMap2d CreateSampleCircle2(int width)
        {
            if (width % 2 != 1) width += 1;
            BitMap2d bmp = new BitMap2d(width, width, 0);
            int r = (width - 1) / 2;
            int mx = r;
            int my = r;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j < r)
                        continue;
                    double dis2 = (i - mx) * (i - mx) + (j - my) * (j - my);
                    if (dis2 < r * r)
                    {
                        bmp.SetPixel(i, j, 255);
                    }
                }
            }
            return bmp;
        }
    }
}
