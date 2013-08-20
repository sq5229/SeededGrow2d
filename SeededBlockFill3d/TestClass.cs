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
        public static List<Int16Triple> Test1(TestParms test)
        {
            LargeSeededGrowManager lsg = new LargeSeededGrowManager();
            lsg.SetScale(test.image.width, test.image.height, test.image.depth,20);
            DataFiller df = new DataFiller();
            df.Initialize(test);
            LargeFloodFill_Threshold ff = new LargeFloodFill_Threshold();
            ff.SetThres(test.min, test.max);
            lsg.SetExecutor(ff);
            lsg.ExecuteSeededGrow(test.seed);
            Console.WriteLine(lsg.resultSet.Count);
            //IO.WriteXYZFile(lsg.resultSet, "Test1.xyz");
            return lsg.resultSet;
        }
        public static List<Int16Triple> Test2(TestParms test)
        {
            
            LargeFloodFill_Threshold ff = new LargeFloodFill_Threshold();
            ff.SetThres(test.min, test.max);
            ff.ExecuteSeededGrow(new SeededGrowInput(
                test.image.data, test.image.width, test.image.height, test.image.depth,
                new BitArray(test.image.width * test.image.height * test.image.depth, false), new List<Int16Triple>() { test.seed},false,false,true)
                );
            Console.WriteLine(ff.GetResult().Count);
            //IO.WriteXYZFile(ff.GetResult(), "Test2.xyz");
            return ff.GetResult();
        }

        internal static void Test()
        {
            TestParms test = TestParms.CreateParmsForBackPack();
            List<Int16Triple> list2 = Test2(test);
            List<Int16Triple> list1 = Test1(test);
            //list2.Sort();
            //list1.Sort();
            //List<Int16Triple> ss = new List<Int16Triple>();
            //int r1 = -1;

            //for (int i = 0; i < list2.Count-1; i++)
            //{
            //    if (list2[i].CompareTo(list2[i + 1]) == 0)
            //    {
            //        throw new Exception();
            //    }
            //}
            //for (int i = 0; i < list1.Count - 1; i++)
            //{
            //    if (list1[i].CompareTo(list1[i + 1]) == 0)
            //    {
            //        throw new Exception();
            //    }
            //}


            //for (int i = 0; i < list2.Count; i++)
            //{
            //    if (list1[i].CompareTo(list2[i]) != 0)
            //    {
            //        if (r1 == -1)
            //        {
            //            r1 = i;
            //        }
            //    }
            //}
            //for (int i = r1; i < r1 + list1.Count - list2.Count;i++ )
            //{
            //    ss.Add(list1[i]);
            //}
            //IO.WriteXYZFile(ss, "left.xyz");
        }
    }
}
