using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\VTKproj\FillMeshTest.ply";
            Mesh mesh=new Mesh();
            PlyManager.ReadMeshFile(mesh, path);
            MeshVoxelizer mf = new MeshVoxelizer(mesh);
            List<Int16Triple> list = mf.GetVoxels();
            BitMap3d bmp = BitMap3d.CreateFromPointSet(list);
            CuberilleProcessor cb = new CuberilleProcessor(bmp);
            Mesh ret = cb.GeneratorSurface();
            PlyManager.Output(ret, "test.ply");
        }
    }
}
