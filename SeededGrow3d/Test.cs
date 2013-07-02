using System;
using System.Collections.Generic;
using System.Text;
using SeededGrow3d.FloodFill;
using SeededGrow3d.ScanLineFill;
using SeededGrow3d.SpanFill;

namespace SeededGrow3d
{
    public class TestParms
    {
        public BitMap3d image;
        public Int16Triple seed;
        public byte min;
        public byte max;
       
        public static TestParms CreateParmsForLobster()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(301, 324, 56,BitMap3d.BLACK);
            parm.image.ReadRaw("D://VTKproj//lobster.raw");
            parm.seed = new Int16Triple(124, 168, 27);
            parm.min = 37;
            parm.max = 255;
            return parm;
        }
        public static TestParms CreateParmsForPhantom()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(512, 512, 442, BitMap3d.BLACK);
            parm.image.ReadRaw("D://VTKproj//colon_phantom8.raw");
            parm.seed = new Int16Triple(256, 256, 227);
            parm.max = 255;
            parm.min = 42;
            return parm;
        }
        public static TestParms CreateParmsForEngine()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(256, 256, 128, BitMap3d.BLACK);
            parm.image.ReadRaw("D://VTKproj//engine.raw");
            parm.seed = new Int16Triple(149, 44, 43);
            parm.min = 64;
            parm.max = 255;
            return parm;
        }
        public static TestParms CreateParmsForBackPack()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(512, 512, 373, BitMap3d.BLACK);
            parm.image.ReadRaw("D://VTKproj//backpack8.raw");
            parm.seed = new Int16Triple(53, 390, 160);
            parm.min = 46;
            parm.max = 255;
            return parm;
        }
        public static TestParms CreateParmsForCube()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(200, 200, 200, 240);
            parm.seed = new Int16Triple(50, 50, 50);
            parm.min = 0;
            parm.max = 255;
            return parm;
        }
    }

    public class TestClass
    {
        static void TestFloodFill(TestParms parm, Int16Triple seed)
        {
            {
                FloodFill3d_T ff = new FloodFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteFloodFill_Stack(parm.image, seed);
                ff.report.PrintInfo();
            }
            {
                FloodFill3d_T ff = new FloodFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteFloodFill_Queue(parm.image, seed);
                ff.report.PrintInfo();
            }
        }
        static void TestScanlineFill(TestParms parm, Int16Triple seed)
        {
            {
                ScanlineFill3d_T ff = new ScanlineFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteScanlineFill_Stack(parm.image, seed);
                ff.report.PrintInfo();
            }
            {
                ScanlineFill3d_T ff = new ScanlineFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteScanlineFill_Queue(parm.image, seed);
                ff.report.PrintInfo();
            }
        }
        static void TestSpanFill(TestParms parm, Int16Triple seed)
        {
            {
                SpanFill3d_T ff = new SpanFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteSpanFill_Stack(parm.image, seed);
                ff.report.PrintInfo();
            }
            {
                SpanFill3d_T ff = new SpanFill3d_T();
                ff.max = parm.max;
                ff.min = parm.min;
                ff.ExcuteSpanFill_Queue(parm.image, seed);
                ff.report.PrintInfo();
            }
        }
        public static void Test()
        {
            TestParms test = TestParms.CreateParmsForBackPack();
            TestFloodFill(test, test.seed);
            TestScanlineFill(test, test.seed);
            TestSpanFill(test, test.seed);
        }
    }
    
}
