using System;
using System.Collections.Generic;
using System.Text;

namespace SeededBlockFill3d
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
            parm.image = new BitMap3d(301, 324, 56, BitMap3d.BLACK);
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
        public static void Test()
        {
            TestParms test = TestParms.CreateParmsForEngine();
            LargeSeededGrow lsg = new LargeSeededGrow(test.image.width, test.image.height, test.image.depth);
            lsg.SetBlockSize(50, 50, 50);
            //BitMap3d bmp = test.image;
            //LargeImage large = new LargeImage(bmp.width,bmp.height,bmp.depth);
            //large.CreateBlocks(50, 50, 50);

        }
    }
}
