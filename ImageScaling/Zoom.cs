using System;
using System.Collections.Generic;
using System.Text;

namespace ImageScaling
{
    public class Zoom
    {
        public static BitMap2d ZoomOut(BitMap2d image, float rate)
        {
            int w=(int)(image.width*rate)+1;
            int h=(int)(image.height*rate)+1;

            BitMap2d bmp = new BitMap2d(w, h, 0);

            for (int i = 0; i < image.width; i++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    if (image.GetPixel(i, j) == 255)
                    {
                        int ni = (int)(i * rate);
                        int nj = (int)(j * rate);
                        bmp.SetPixel(ni, nj, 255);
                    }
                }
            }
            return bmp;
        }
        public static BitMap2d ZoomOut2(BitMap2d image, float rate)
        {
            int w = (int)(image.width * rate)+1;
            int h = (int)(image.height * rate)+1;

            BitMap2d bmp = new BitMap2d(w, h, 255);

            for (int i = 0; i < image.width; i++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    if (image.GetPixel(i, j) == 0)
                    {
                        int ni = (int)(i * rate);
                        int nj = (int)(j * rate);
                        bmp.SetPixel(ni, nj, 0);
                    }
                }
            }
            return bmp;
        }
    }
}
