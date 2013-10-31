using System;
using System.Collections.Generic;
using System.Text;
using SMC.SMC;
using SMC.MC;
using System.Diagnostics;

namespace SMC
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestSMC();
            BitMap3d bmp = BitMap3d.CreateSampleForLobster();
            AdSMCProcessor smc = new AdSMCProcessor(bmp);
            Mesh m = smc.GenerateSurface();
            PlyManager.Output(m, "A2.ply");
            Console.WriteLine("cmp");
            Console.Read();
        }

        private static void TestSMC()
        {
            TestParms parm = TestParms.CreateParmsForEngine();
            BitMap3d bmp = new BitMap3d(parm.image.data, parm.image.width, parm.image.height, parm.image.depth);
            for (int i = 0; i < bmp.data.Length; i++)
            {
                if (bmp.data[i] >= parm.min && bmp.data[i] <= parm.max)
                {
                    bmp.data[i] = BitMap3d.WHITE;
                }
                else
                {
                    bmp.data[i] = BitMap3d.BLACK;
                }
            }
            SMCProcessor smc = new SMCProcessor(bmp);
            Mesh m = smc.GenerateSurface();
            PlyManager.Output(m, @"D:\VTKproj\engine.ply");
        }

        private static void TestMC()
        {
            TestParms parm = TestParms.CreateParmsForPhantom();
            BitMap3d bmp = new BitMap3d(parm.image.data, parm.image.width, parm.image.height, parm.image.depth);
            for (int i = 0; i < bmp.data.Length; i++)
            {
                if (bmp.data[i] >= parm.min && bmp.data[i] <= parm.max)
                {
                    bmp.data[i] = BitMap3d.WHITE;
                }
                else
                {
                    bmp.data[i] = BitMap3d.BLACK;
                }
            }
            MCProcessor mc = new MCProcessor(bmp);
            Mesh m = mc.GeneratorSurface();
            PlyManager.Output(m, "test5.ply");
        }

        private static void Test()
        {
            BitMap3d bmp = new BitMap3d(301, 324, 56, BitMap3d.BLACK);
            bmp.ReadRaw("D://VTKproj//lobster.raw");
            for (int i = 0; i < bmp.data.Length; i++)
            {
                if (bmp.data[i] >= 37 && bmp.data[i] <= 255)
                {
                    bmp.data[i] = BitMap3d.WHITE;
                }
                else
                {
                    bmp.data[i] = BitMap3d.BLACK;
                }
            }
            MCProcessor mc = new MCProcessor(bmp);
            Mesh m = mc.GeneratorSurface();
            Console.WriteLine(m.Vertices.Count);
            Console.WriteLine(m.Faces.Count);
            PlyManager.Output(m, "test2.ply");
        }

        private static void Compare()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //TestMC();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();
            TestSMC();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
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
}
