using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FindMaxima
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMap2d bmp = new BitMap2d(501,702,0);
            bmp.ReadRaw("Clipboard.raw");
            Bitmap image = bmp.MakeBmp();
            MaximunFinder mf = new MaximunFinder(bmp, 20);
            List<Int16DoubleWithValue> list = mf.FindMaxima();
            for (int i = 0; i < list.Count; i++)
            {
                SetB(image, list[i].X, list[i].Y);
            }
            image.Save("ret.bmp");
        }
        static void SetB(Bitmap image,int x, int y)
        {
            int r=2;
            for (int i = -r; i <= r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    int rx = x + i;
                    int ry = y + j;
                    if (rx >= 0 && rx < image.Width && ry >= 0 && ry < image.Height)
                    {
                        image.SetPixel(rx,ry, Color.FromArgb(255, 0, 0));
                    }
                }
            }
        }
    }
}
