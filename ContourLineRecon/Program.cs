using System;
using System.Collections.Generic;
using System.Text;

namespace ContourLineRecon
{
    class Program
    {
        static void Test1(ContourLine line1, ContourLine line2)
        {
            line1.AddPoint2d(1, 0);
            line1.AddPoint2d(0, 1);
            line1.AddPoint2d(0, 2);
            line1.AddPoint2d(1, 3);
            line1.AddPoint2d(2, 3);
            line1.AddPoint2d(3, 2);
            line1.AddPoint2d(3, 1);
            line1.AddPoint2d(2, 0);

            line2.AddPoint2d(4, 2);
            line2.AddPoint2d(2, 0);
            line2.AddPoint2d(0, 2);
            line2.AddPoint2d(2, 4);
        }
        static void Test2(ContourLine line1, ContourLine line2)
        {
            line1.AddPoint2d(1, 0);
            line1.AddPoint2d(2, 0);
            line1.AddPoint2d(4, 0);
            line1.AddPoint2d(8, 0);
            line1.AddPoint2d(9, 0);
            line1.AddPoint2d(12, 0);
            line1.AddPoint2d(15, 0);
            line1.AddPoint2d(16, 0);
            line1.AddPoint2d(22, 0);

            line2.AddPoint2d(1, 0);
            line2.AddPoint2d(3, 0);
            line2.AddPoint2d(4, 0);
            line2.AddPoint2d(6, 0);
            line2.AddPoint2d(9, 0);
            line2.AddPoint2d(13, 0);
            line2.AddPoint2d(16, 0);
            line2.AddPoint2d(19, 0);
        }
        static void Test3(ContourLine line1, ContourLine line2)
        {
            line1.AddPoint2d(1, 0);
            line1.AddPoint2d(2, 1);
            line1.AddPoint2d(3, 1);
            line1.AddPoint2d(4, 0);
            line1.AddPoint2d(5, 1);
            line1.AddPoint2d(6, 1);
            line1.AddPoint2d(7, 2);
            line1.AddPoint2d(6, 3);
            line1.AddPoint2d(7, 4);
            line1.AddPoint2d(7, 5);
            line1.AddPoint2d(6, 6);
            line1.AddPoint2d(5, 6);
            line1.AddPoint2d(4, 7);
            line1.AddPoint2d(3, 6);
            line1.AddPoint2d(3, 5);
            line1.AddPoint2d(2, 4);
            line1.AddPoint2d(1, 4);
            line1.AddPoint2d(0, 3);
            line1.AddPoint2d(0, 2);
            line1.AddPoint2d(0, 1);

            line2.AddPoint2d(6, 0);
            line2.AddPoint2d(8, 2);
            line2.AddPoint2d(8, 3);
            line2.AddPoint2d(9, 4);
            line2.AddPoint2d(9, 5);
            line2.AddPoint2d(8, 6);
            line2.AddPoint2d(7, 6);
            line2.AddPoint2d(6, 7);
            line2.AddPoint2d(5, 7);
            line2.AddPoint2d(4, 8);
            line2.AddPoint2d(3, 8);
            line2.AddPoint2d(2, 7);
            line2.AddPoint2d(3, 6);
            line2.AddPoint2d(1, 4);
            line2.AddPoint2d(1, 3);
            line2.AddPoint2d(3, 1);
            line2.AddPoint2d(5, 1);
        }
        static void Test4(ContourLine line0,ContourLine line1, ContourLine line2,ContourLine line3)
        {
            line0.AddPoint2d(3, 3);
            line0.AddPoint2d(6, 3);
            line0.AddPoint2d(6, 6);
            line0.AddPoint2d(3, 6);

            line1.AddPoint2d(1, 0);
            line1.AddPoint2d(2, 1);
            line1.AddPoint2d(3, 1);
            line1.AddPoint2d(4, 0);
            line1.AddPoint2d(5, 1);
            line1.AddPoint2d(6, 1);
            line1.AddPoint2d(7, 2);
            line1.AddPoint2d(6, 3);
            line1.AddPoint2d(7, 4);
            line1.AddPoint2d(7, 5);
            line1.AddPoint2d(6, 6);
            line1.AddPoint2d(5, 6);
            line1.AddPoint2d(4, 7);
            line1.AddPoint2d(3, 6);
            line1.AddPoint2d(3, 5);
            line1.AddPoint2d(2, 4);
            line1.AddPoint2d(1, 4);
            line1.AddPoint2d(0, 3);
            line1.AddPoint2d(0, 2);
            line1.AddPoint2d(0, 1);

            line2.AddPoint2d(6, 0);
            line2.AddPoint2d(8, 2);
            line2.AddPoint2d(8, 3);
            line2.AddPoint2d(9, 4);
            line2.AddPoint2d(9, 5);
            line2.AddPoint2d(8, 6);
            line2.AddPoint2d(7, 6);
            line2.AddPoint2d(6, 7);
            line2.AddPoint2d(5, 7);
            line2.AddPoint2d(4, 8);
            line2.AddPoint2d(3, 8);
            line2.AddPoint2d(2, 7);
            line2.AddPoint2d(3, 6);
            line2.AddPoint2d(1, 4);
            line2.AddPoint2d(1, 3);
            line2.AddPoint2d(3, 1);
            line2.AddPoint2d(5, 1);

            line3.AddPoint2d(7, 5);
            line3.AddPoint2d(7, 6);
            line3.AddPoint2d(8, 7);
            line3.AddPoint2d(8, 8);
            line3.AddPoint2d(7, 9);
            line3.AddPoint2d(6, 8);
            line3.AddPoint2d(5, 8);
            line3.AddPoint2d(5, 7);
            line3.AddPoint2d(4, 6);
            line3.AddPoint2d(3, 7);
            line3.AddPoint2d(4, 8);
            line3.AddPoint2d(3, 9);
            line3.AddPoint2d(2, 9);
            line3.AddPoint2d(0, 7);
            line3.AddPoint2d(0, 6);
            line3.AddPoint2d(1, 5);
            line3.AddPoint2d(1, 4);
            line3.AddPoint2d(2, 3);
            line3.AddPoint2d(2, 2);
            line3.AddPoint2d(3, 1);
            line3.AddPoint2d(4, 2);
            line3.AddPoint2d(3, 3);
            line3.AddPoint2d(4, 4);
            line3.AddPoint2d(5, 4);

        }
        static void Main(string[] args)
        {
            ContourLine line0 = new ContourLine();
            line0.SetZ(-3);
            ContourLine line1 = new ContourLine();
            line1.SetZ(0);
            ContourLine line2 = new ContourLine();
            line2.SetZ(3);
            ContourLine line3 = new ContourLine();
            line3.SetZ(8);
            
            Test4(line0,line1, line2,line3);
            //ContourStitcher cs = new ContourStitcher(line1, line3);
            //Mesh m = cs.DoStitching();
            List<ContourLine> list = new List<ContourLine>() { line0,line1,line2,line3};
            ContourLineSurfaceGenerator scg = new ContourLineSurfaceGenerator(list) ;
            Mesh m = scg.GenerateSurface();
            m.Scale(20, 20, 20);
            m.Transform(300, 300, 300);
            PlyManager.Output(m, "test2.ply");
        }
    }
}
