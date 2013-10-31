using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"table1.ply";
            Mesh mesh=new Mesh();
            PlyManager.ReadMeshFile(mesh, path);
            MeshFiller mf = new MeshFiller(mesh);
            BitMap3d bmp = mf.FillImage();
           // bmp.SaveRaw("ret1 " + bmp.width + "_" + bmp.height + "_" + bmp.depth + ".raw");
            FloodFill3d sp = new FloodFill3d();
            sp.ExcuteFloodFill(bmp, new Int16Triple(0, 0, 0));
            bmp.Reverse();
            bmp.SaveRaw("ret2"+bmp.width+"_"+bmp.height+"_"+bmp.depth+".raw"); 
            //CuberilleProcessor cb = new CuberilleProcessor(bmp);
            //Mesh ret = cb.GeneratorSurface();
            //PlyManager.Output(ret, "ret2.ply");
        }
    }
}
