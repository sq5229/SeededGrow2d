using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryLine2d
{
    class Program
    {
        static void Main(string[] args)
        {
            SeededGrow3d.TestParms test= SeededGrow3d.TestParms.CreateParmsForLobster();
            MyFF ff = new MyFF();
            ff.max = test.max;
            ff.min = test.min;
            ff.ExcuteFloodFill_Stack(test.image, test.seed);
            List<Point3d> list=BoundaryLine2d.BoundaryLine2dCaculator.GetBoundaryLine2d(ff.result,true,3);
            PlyManager.Output3(list, @"D:\VTKproj\test_line.ply");
        }
    }
}
