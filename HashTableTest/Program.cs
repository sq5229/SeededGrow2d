using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HashTableTest
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
    class Program
    {
        static void Main(string[] args)
        {
            TestParms test = TestParms.CreateParmsForEngine();
            List<Int16Triple> list = FindBoundaryPoints(test);
            TestVisit_1(list, test);
            TestVisit_2(list, test);
            TestVisit_3(list, test);
            TestVisit_4(list, test);
            Console.ReadLine();
        }
        static void TestVisit_1(List<Int16Triple> list,TestParms test)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HashTable_Double2dArray<bool> hash = new HashTable_Double2dArray<bool>(test.image.width,test.image.height,test.image.depth);
            for (int i = 0; i < list.Count; i++)
            {
                hash.SetHashValue(list[i].X, list[i].Y, list[i].Z, true);
            }
            bool temp=false;
            for (int i = 0; i < list.Count; i++)
            {
                bool has=hash.GetHashValue(list[i].X, list[i].Y, list[i].Z,ref temp);
                if (!has)
                    throw new Exception();
            }
            watch.Stop();
            Console.WriteLine("double2d : "+watch.ElapsedMilliseconds);
        }
        static void TestVisit_2(List<Int16Triple> list, TestParms test)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HashTable_2dArray<bool> hash = new HashTable_2dArray<bool>(test.image.width, test.image.height, test.image.depth);
            for (int i = 0; i < list.Count; i++)
            {
                hash.SetHashValue(list[i].X, list[i].Y, list[i].Z, true);
            }
            bool temp = false;
            for (int i = 0; i < list.Count; i++)
            {
                bool has = hash.GetHashValue(list[i].X, list[i].Y, list[i].Z, ref temp);
                if (!has)
                    throw new Exception();
            }
            watch.Stop();
            Console.WriteLine("2d : " + watch.ElapsedMilliseconds);
        }
        static void TestVisit_3(List<Int16Triple> list, TestParms test)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HashTable_3dArray<bool> hash = new HashTable_3dArray<bool>(test.image.width, test.image.height, test.image.depth);
            for (int i = 0; i < list.Count; i++)
            {
                hash.SetHashValue(list[i].X, list[i].Y, list[i].Z, true);
            }
            bool temp = false;
            for (int i = 0; i < list.Count; i++)
            {
                bool has = hash.GetHashValue(list[i].X, list[i].Y, list[i].Z, ref temp);
                if (!has)
                    throw new Exception();
            }
            watch.Stop();
            Console.WriteLine("3d : " + watch.ElapsedMilliseconds);
        }
        static void TestVisit_4(List<Int16Triple> list, TestParms test)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HashTable_Triple2dArray<bool> hash = new HashTable_Triple2dArray<bool>(test.image.width, test.image.height, test.image.depth);
            for (int i = 0; i < list.Count; i++)
            {
                hash.SetHashValue(list[i].X, list[i].Y, list[i].Z, true);
            }
            bool temp = false;
            for (int i = 0; i < list.Count; i++)
            {
                bool has = hash.GetHashValue(list[i].X, list[i].Y, list[i].Z, ref temp);
                if (!has)
                    throw new Exception();
            }
            watch.Stop();
            Console.WriteLine("triple2d : " + watch.ElapsedMilliseconds);
        }

        public static List<Int16Triple> FindBoundaryPoints(TestParms test)
        {
            for (int i = 0; i < test.image.data.Length; i++)
            {
                if (test.image.data[i] >= test.min && test.image.data[i] <= test.max)
                    test.image.data[i] = BitMap3d.WHITE;
                else
                    test.image.data[i] = BitMap3d.BLACK;
            }

            List<Int16Triple> list = new List<Int16Triple>();
            Int16Triple[] adjPoints6=new Int16Triple[6];
            for (int k = 0; k < test.image.depth; k++)
            {
                for (int j = 0; j < test.image.height; j++)
                {
                    for (int i = 0; i < test.image.width; i++)
                    {
                        if(test.image.GetPixel(i,j,k)==BitMap3d.WHITE)
                        {
                            Int16Triple p = new Int16Triple(i,j,k);
                            InitAdj6(adjPoints6, p);
                            for (int pi = 0; pi < 6; pi++)
                            {
                                Int16Triple t = adjPoints6[pi];
                                if (!InRange(test.image, t)||test.image.GetPixel(t.X,t.Y,t.Z)==BitMap3d.BLACK)
                                {
                                    list.Add(p);
                                }
                            }
                        }
                        
                    }
                }
            }
            return list;
        }
        private static bool InRange(BitMap3d bitMap3d, Int16Triple t)
        {
            return t.X >= 0 && t.X < bitMap3d.width && t.Y >= 0 && t.Y < bitMap3d.height && t.Z >= 0 && t.Z < bitMap3d.depth;
        }
        public static void InitAdj6(Int16Triple[] adjPoints6, Int16Triple p)
        {
            adjPoints6[0].X = p.X - 1;
            adjPoints6[0].Y = p.Y;
            adjPoints6[0].Z = p.Z;

            adjPoints6[1].X = p.X + 1;
            adjPoints6[1].Y = p.Y;
            adjPoints6[1].Z = p.Z;

            adjPoints6[2].X = p.X;
            adjPoints6[2].Y = p.Y - 1;
            adjPoints6[2].Z = p.Z;

            adjPoints6[3].X = p.X;
            adjPoints6[3].Y = p.Y + 1;
            adjPoints6[3].Z = p.Z;


            adjPoints6[4].X = p.X;
            adjPoints6[4].Y = p.Y;
            adjPoints6[4].Z = p.Z - 1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z + 1;
        }
    }
}
