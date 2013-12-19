using System;
using System.Collections.Generic;
using System.Text;

namespace ImageScaling
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMap2d bmp = BitMap2d.CreateSampleCircle(201);
            BitMap2d bmp2=Zoom.ZoomOut(bmp, 0.7f);
            bmp2.OutputBitMap("1.bmp");
            BitMap2d bmp3 = Zoom.ZoomOut2(bmp, 0.7f);
            bmp3.OutputBitMap("2.bmp");
        }
    }
}
