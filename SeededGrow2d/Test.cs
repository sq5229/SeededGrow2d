using System;
using System.Collections.Generic;
using System.Text;

namespace SeededGrow2d
{
    public class TestBMP2D
    {
        static Int16Double seed_1 = new Int16Double(387, 654);
        static Int16Double seed_2 = new Int16Double(682, 34);
        static Int16Double seed_3 = new Int16Double(686, 110);
        static Int16Double seed_4 = new Int16Double(50, 50);
        static Int16Double seed_5 = new Int16Double(1150, 1150);
        static BitMap2d GetSampleTest1()
        {
            BitMap2d bmp = new BitMap2d(1124, 1924, BitMap2d.BLACK);
            bmp.ReadBitmap(@"D:\VTKproj\FF2TEST\test1.bmp");
            return bmp;
        }
        static BitMap2d GetSampleTest2()
        {
            BitMap2d bmp = new BitMap2d(1694, 1257, BitMap2d.BLACK);
            bmp.ReadBitmap(@"D:\VTKproj\FF2TEST\test2.bmp");
            return bmp;
        }
        static BitMap2d GetSampleTest3()
        {
            BitMap2d bmp = new BitMap2d(1694, 1257, BitMap2d.BLACK);
            bmp.ReadBitmap(@"D:\VTKproj\FF2TEST\test3.bmp");
            return bmp;
        }
        static BitMap2d GetSampleTest4()
        {
            BitMap2d bmp = new BitMap2d(5000, 5000, BitMap2d.WHITE);
            return bmp;
        }
        static BitMap2d GetSampleTest5()
        {
            BitMap2d bmp = new BitMap2d(2300, 2300, BitMap2d.BLACK);
            int width = bmp.width;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double distence = Math.Sqrt((i - width / 2) * (i - width / 2) + (j - width / 2) * (j - width / 2));
                    if (distence < width / 2)
                    {
                        bmp.SetPixel(i, j, BitMap2d.WHITE);
                    }
                }
            }
            bmp.ResetVisitCount();
            return bmp;
        }
        static BitMap2d GetSampleTest6()
        {
            BitMap2d bmp = new BitMap2d(10000, 10000, BitMap2d.WHITE);
            return bmp;
        }
        static void TestFloodFill(BitMap2d bmp, Int16Double seed)
        {
            {
                FloodFill2d_T ff = new FloodFill2d_T();
                ff.ExcuteFloodFill_Stack(bmp, seed);
                ff.report.PrintInfo();
            }
            {
                FloodFill2d_T ff = new FloodFill2d_T();
                ff.ExcuteFloodFill_Queue(bmp, seed);
                ff.report.PrintInfo();
            }
        }
        static void TestScanlineFill(BitMap2d bmp, Int16Double seed)
        {
            {
                ScanlineFill2d_T ff = new ScanlineFill2d_T();
                ff.ExcuteScanlineFill_Stack(bmp, seed);
                ff.report.PrintInfo();
            }
            {
                ScanlineFill2d_T ff = new ScanlineFill2d_T();
                ff.ExcuteScanlineFill_Queue(bmp, seed);
                ff.report.PrintInfo();
            }
        }
        static void TestSpanFill(BitMap2d bmp, Int16Double seed)
        {
            {
                SpanFill2d_T ff2 = new SpanFill2d_T();
                ff2.ExcuteSpanFill_Stack(bmp, seed);
                ff2.report.PrintInfo();
            }
            {
                SpanFill2d_T ff2 = new SpanFill2d_T();
                ff2.ExcuteSpanFill_Queue(bmp, seed);
                ff2.report.PrintInfo();
                //ff2.report.OutPutMap(ff2.flagsMap, "result.bmp");
            }
        }
        static void Test1()
        {
            BitMap2d bmp = GetSampleTest1();
            TestFloodFill(bmp, seed_1);
            TestScanlineFill(bmp, seed_1);
            TestSpanFill(bmp, seed_1);
        }
        static void Test2()
        {
            BitMap2d bmp = GetSampleTest2();
            TestFloodFill(bmp, seed_2);
            TestScanlineFill(bmp, seed_2);
            TestSpanFill(bmp, seed_2);
        }
        static void Test3()
        {
            BitMap2d bmp = GetSampleTest3();
            TestFloodFill(bmp, seed_3);
            TestScanlineFill(bmp, seed_3);
            TestSpanFill(bmp, seed_3);
        }
        static void Test4()
        {
            BitMap2d bmp = GetSampleTest4();
            TestFloodFill(bmp, seed_4);
            TestScanlineFill(bmp, seed_4);
            TestSpanFill(bmp, seed_4);
        }
        static void Test5()
        {
            BitMap2d bmp = GetSampleTest5();
            TestFloodFill(bmp, seed_5);
            TestScanlineFill(bmp, seed_5);
            TestSpanFill(bmp, seed_5);
        }
        static void TestXY()
        {
            BitMap2d bmp = GetSampleTest6();
            {
                SpanYFill2d_T ff = new SpanYFill2d_T();
                ff.ExcuteSpanFill_S(bmp, seed_1);
                ff.report.PrintInfo();
            }
            {
                SpanFill2d_T ff = new SpanFill2d_T();
                ff.ExcuteSpanFill_Stack(bmp, seed_1);
                ff.report.PrintInfo();
            }
        }
        public static void Test()
        {
            TestXY();
        }
    }
}
