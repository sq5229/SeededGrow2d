using System;
using System.Collections.Generic;
using System.Text;

namespace OctreeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test4();
        }
        public static void Test1()
        {
            int width = 400;
            BitMap3d image = BitMap3d.CreateSampleTedVolume(width);
            Mesh m = OctreeSurfaceGenerator.GenerateSurface(image);
            PlyManager.Output(m, string.Format("D://VTKproj//Tree_Ted_{0}.ply",width));
        }

        public static void Test2()
        {
            string str = "x2";
            BitMap3d image = BitMap3d.CreateSampleEngineVolume(str);
            Mesh m = OctreeSurfaceGenerator.GenerateSurface(image);
            PlyManager.Output(m, "D://VTKproj//Tree_Engine"+str+".ply");
        }

        public static void Test3()
        {
            BitMap3d image = BitMap3d.CreateSampleForFan();
            Mesh m = OctreeSurfaceGenerator.GenerateSurface(image);
            PlyManager.Output(m, "D://VTKproj//Tree_Fan.ply");
        }

        public static void Test4()
        {
            BitMap3d image = BitMap3d.CreateSampleForLobsterX2();
            Mesh m = OctreeSurfaceGenerator.GenerateSurface(image);
            PlyManager.Output(m, "Tree_Lobster_602.ply");
        }
    }
}
