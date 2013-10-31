using System;
using System.Collections.Generic;
using System.Text;

namespace DrawLineFill
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestLine();
            TestTriangle();
        }
        public static void TestLine()
        {
            FillShape psc = new FillShape(100, 100,100);
            psc.Draw3DLine(15, 23, 32, 93, 91, 91);
            OutputMap(psc.GetMap(), "TestLine.ply");
        }

        public static void TestTriangle()
        {
            FillShape psc = new FillShape(300, 300, 300);
            psc.DrawTriangle3d(120, 123, 232, 43, 121, 31, 45, 245, 111);
            OutputMap(psc.GetMap(), "TestTriangle.ply");
        }

        public static void OutputMap(BitMap3d bmp,string path)
        {
            CuberilleProcessor cp = new CuberilleProcessor(bmp);
            Mesh m=cp.GeneratorSurface();
            PlyManager.Output(m, path);
        }
    }
}
