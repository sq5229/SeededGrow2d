using System;
using System.Collections.Generic;
using System.Text;
using SeededBlockFill3d.Layer;
using System.Collections;

namespace SeededBlockFill3d
{
    public class TestParms
    {
        public BitMap3d image;
        public Int16Triple seed;
        public byte min;
        public byte max;
        public string path;

        public static TestParms CreateParmsForLobster()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(301, 324, 56);
            parm.path = "D://VTKproj//lobster.raw";
            //parm.image.ReadRaw("D://VTKproj//lobster.raw");
            parm.seed = new Int16Triple(124, 168, 27);
            parm.min = 37;
            parm.max = 255;
            return parm;
        }
        public static TestParms CreateParmsForPhantom()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(512, 512, 442);
            //parm.image.ReadRaw("D://VTKproj//colon_phantom8.raw");
            parm.path = "D://VTKproj//colon_phantom8.raw";
            parm.seed = new Int16Triple(256, 256, 227);
            parm.max = 255;
            parm.min = 42;
            return parm;
        }
        public static TestParms CreateParmsForEngine()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(256, 256, 128);
            //parm.image.ReadRaw("D://VTKproj//engine.raw");
            parm.path = "D://VTKproj//engine.raw";
            parm.seed = new Int16Triple(149, 44, 43);
            parm.min = 64;
            parm.max = 255;
            return parm;
        }
        public static TestParms CreateParmsForBackPack()
        {
            TestParms parm = new TestParms();
            parm.image = new BitMap3d(512, 512, 373);
            //parm.image.ReadRaw("D://VTKproj//backpack8.raw");
           parm.path = "D://VTKproj//backpack8.raw";
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

        public void LoadData()
        {
            if(image.data==null)
                this.image.ReadRaw(this.path);
        }
    }
    public class TestClass
    {
        public static void TestLayerFloodFill(TestParms test)
        {
            DataFiller_Simulation df = new DataFiller_Simulation();
            df.Initialize(test);
            LargeSeededGrowManager_FloodFill lsg = new LargeSeededGrowManager_FloodFill();
            lsg.SetScale(test.image.width, test.image.height, test.image.depth,20);
            LargeFloodFill_Threshold ff = new LargeFloodFill_Threshold();
            ff.SetThres(test.min, test.max);
            lsg.SetExecutor(ff);
            lsg.SetDataProvider(df);
            lsg.ExecuteSeededGrow(test.seed);
            Console.WriteLine("Layer Based FloodFill Result Count :"+lsg.resultCount);
            //df.Close();
        }
        public static void TestFloodFill(TestParms test)
        {
            LargeFloodFill_Threshold ff = new LargeFloodFill_Threshold();
            test.LoadData();
            ff.SetThres(test.min, test.max);
            ff.ExecuteSeededGrow(new FloodFillInput(
                test.image.data, test.image.width, test.image.height, test.image.depth,
                new FlagMap3d(test.image.width,test.image.height,test.image.depth), new List<Int16Triple>() { test.seed},false,false,true)
                );
            Console.WriteLine("FloodFill Result Count :" + ff.GetResult());
            return ;
        }
        public static void TestLayerSpanFill(TestParms test)
        {
            DataFiller df = new DataFiller_Simulation();
            df.Initialize(test);
            LargeSeededGrowManager_SpanFill lsg = new LargeSeededGrowManager_SpanFill();
            lsg.SetScale(test.image.width, test.image.height, test.image.depth, 20);
            LargeSpanFill_Threshold ff = new LargeSpanFill_Threshold();
            ff.SetThres(test.min, test.max);
            lsg.SetExecutor(ff);
            lsg.SetDataProvider(df);
            lsg.ExecuteSeededGrow(test.seed);
            Console.WriteLine("Layer Based SpanFill Result Count :" + lsg.resultCount);
            //df.Close();
            //IO.WriteXYZFile(lsg.resultSet, "Test1.xyz");
            //return lsg.resultSet;
        }
        public static void TestSpanFill(TestParms test)
        {
            test.LoadData();
            LargeSpanFill_Threshold ff = new LargeSpanFill_Threshold();
            ff.SetThres(test.min, test.max);
            ff.ExecuteSeededGrow(new SpanFillInput(
                test.image.data, test.image.width, test.image.height, test.image.depth,
                new FlagMap3d(test.image.width, test.image.height ,test.image.depth), new List<Range>(),test.seed, false, false, true)
                );
            Console.WriteLine("SpanFill Result Count :" + ff.GetResult());
        }
        internal static void Test()
        {

            TestParms test = TestParms.CreateParmsForLobster();
            Console.WriteLine("Test Lobster data:");
            TestLayerFloodFill(test);
            TestFloodFill(test);
            TestLayerSpanFill(test);
            TestSpanFill(test);
            Console.WriteLine("\nTest Engine data:");
            test = TestParms.CreateParmsForEngine();
            TestLayerFloodFill(test);
            TestFloodFill(test);
            TestLayerSpanFill(test);
            TestSpanFill(test);
        }
    }
}
